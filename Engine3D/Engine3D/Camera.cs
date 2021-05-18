﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;

namespace Engine3D
{
    public class Camera
    {
        private readonly Vector3 _cameraOrigin;
        private readonly Bitmap _canvas;

        private readonly Vector3 _localRight;
        private readonly Vector3 _localUp;
        private readonly Vector3 _lookDirection;

        public Vector3 CenterOfScreen { get; }

        public Mesh Mesh { get; init; }
        public Vector3 Light { get; set; }

        public Camera(Vector3 cameraOrigin, Vector3 lookAtPoint, float distanceToScreen, Bitmap canvas)
        {
            _cameraOrigin = cameraOrigin;
            _canvas = canvas;

            _lookDirection = Vector3.Normalize(lookAtPoint - _cameraOrigin);
            _localRight = Vector3.Cross(_lookDirection, new Vector3(0, 0, 1));
            _localUp = Vector3.Cross(_localRight, _lookDirection);
            CenterOfScreen = cameraOrigin + _lookDirection * distanceToScreen;
        }

        public void Draw()
        {
            float[,] colorsMatrix = new float[_canvas.Height, _canvas.Width];

            for (int y = 0; y < _canvas.Height; y++)
            {
                for (int x = 0; x < _canvas.Width; x++)
                {
                    float colorVal = Trace(x, y);
                    colorsMatrix[y, x] = colorVal;
                }
            }

            ScaleValues(0, 1);

            for (int y = 0; y < _canvas.Height; y++)
            {
                for (int x = 0; x < _canvas.Width; x++)
                {
                    float pixelColorValue = colorsMatrix[y, x];
                    MyColor pixelColor = new MyColor(pixelColorValue, 1);
                    _canvas.SetPixel(x, y, pixelColor.ToColor());
                }
            }

            void ScaleValues(float lower, float upper)
            {
                float minColor = colorsMatrix[0, 0];
                float maxColor = colorsMatrix[0, 0];

                for (int y = 0; y < _canvas.Height; y++)
                {
                    for (int x = 0; x < _canvas.Width; x++)
                    {
                        if (colorsMatrix[y, x] < minColor)
                        {
                            minColor = colorsMatrix[y, x];
                        }

                        if (colorsMatrix[y, x] > maxColor)
                        {
                            maxColor = colorsMatrix[y, x];
                        }
                    }
                }

                for (int y = 0; y < _canvas.Height; y++)
                {
                    for (int x = 0; x < _canvas.Width; x++)
                    {
                        colorsMatrix[y, x] =
                            lower + (colorsMatrix[y, x] - minColor) / (maxColor - minColor) * (upper - lower);
                    }
                }
            }
        }

        private float Trace(int screenX, int screenY)
        {
            Vector2 uv = (new Vector2(screenX, screenY) - .5f * new Vector2(_canvas.Width, _canvas.Height)) /
                         _canvas.Height;
            float colorVal = 0;

            Vector3 rayInterception = CenterOfScreen + uv.X * _localRight + -uv.Y * _localUp;
            Ray cameraTrace = new(_cameraOrigin, Vector3.Normalize(rayInterception - _cameraOrigin));

            bool intercepted = RayFaceInterception(cameraTrace, Mesh, out float dist, out Vector3 faceNormal);

            if (intercepted)
            {
                Vector3 interceptionPoint = cameraTrace.dir * dist + cameraTrace.origin;
                colorVal = LightTrace(interceptionPoint, faceNormal);
            }
            return colorVal;
        }

        private float LightTrace(Vector3 point, Vector3 faceNormal)
        {
            Ray lightTrace = new (Light, Vector3.Normalize(point - Light));
            float dotProduct = Vector3.Dot(faceNormal, -lightTrace.dir);
            float colorVal = 0;
            
            if (dotProduct > 0)
            {
                float distanceSquared = Vector3.DistanceSquared(lightTrace.origin, point);
                colorVal = dotProduct / distanceSquared;
            }

            return colorVal;
        }

        private bool RayFaceInterception(Ray ray, Mesh mesh, out float distance, out Vector3 normal)
        {
            distance = float.PositiveInfinity;
            normal = Vector3.Zero;

            foreach (Face face in mesh.Faces)
            {
                var tempDistance = CalculateDistanceToTriangle(ray.origin, ray.dir, face.Points[0], face.Points[1],
                    face.Points[2], out Vector3 faceNormal);

                if (tempDistance == -1) continue;
                if (tempDistance < distance)
                {
                    distance = tempDistance;
                    normal = faceNormal;
                }
            }

            return !float.IsPositiveInfinity(distance);
        }

        // Möller–Trumbore intersection algorithm
        private float CalculateDistanceToTriangle(Vector3 orig, Vector3 dir, Vector3 v0, Vector3 v1, Vector3 v2,
            out Vector3 normal)
        {
            Vector3 e1 = v1 - v0;
            Vector3 e2 = v2 - v0;

            normal = Vector3.Normalize(Vector3.Cross(e1, e2));

            // Вычисление вектора нормали к плоскости
            Vector3 pvec = Vector3.Cross(dir, e2);

            float det = Vector3.Dot(e1, pvec);

            // Луч параллелен плоскости
            if (det < 1e-8 && det > -1e-8)
            {
                return -1;
            }

            float inv_det = 1 / det;
            Vector3 tvec = orig - v0;
            float u = Vector3.Dot(tvec, pvec) * inv_det;
            if (u < 0 || u > 1)
            {
                return -1;
            }

            Vector3 qvec = Vector3.Cross(tvec, e1);
            float v = Vector3.Dot(dir, qvec) * inv_det;
            if (v < 0 || u + v > 1)
            {
                return -1;
            }

            return Vector3.Dot(e2, qvec) * inv_det;
        }

        private (float, float) GetDistanceRange(Ray cameraNormal, Mesh m)
        {
            List<float> distances = new();

            foreach (Face f in m.Faces)
            {
                foreach (Vector3 point in f.Points)
                {
                    float distance = Vector3.Dot(point - cameraNormal.origin, cameraNormal.dir);
                    distances.Add(distance);
                }
            }

            return (distances.Min(), distances.Max());
        }
        private static Vector3 GetPlaneNormal(Vector3 p1, Vector3 p2, Vector3 p3)
        {
            Vector3 dir1 = p2 - p1;
            Vector3 dir2 = p3 - p1;
            return Vector3.Cross(dir1, dir2);
        }
    }
}