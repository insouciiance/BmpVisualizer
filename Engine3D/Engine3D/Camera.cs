using System;
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
            for (int y = 0; y < _canvas.Height; y++)
            {
                for (int x = 0; x < _canvas.Width; x++)
                {
                    Color c = Trace(x, y);
                    _canvas.SetPixel(x, y, c);
                }
            }
        }

        private Color Trace(int screenX, int screenY)
        {
            Vector2 uv = (new Vector2(screenX, screenY) - .5f * new Vector2(_canvas.Width, _canvas.Height)) / _canvas.Height;

            Vector3 rayInterception = CenterOfScreen + uv.X * _localRight + -uv.Y * _localUp;
            Ray r = new(_cameraOrigin, Vector3.Normalize(rayInterception - _cameraOrigin));

            (float min, float max) distanceRange = GetDistanceRange(new Ray(_cameraOrigin, _lookDirection), Mesh);
            bool intercepted = RayFaceInterception(r, Mesh, out float t);

            float perpendicularDistance = Vector3.Dot(_lookDirection, r.dir * t);

            Color color = Get255Color(intercepted ? perpendicularDistance : 0, distanceRange.min, distanceRange.max);
            return color;
        }
        public bool RayFaceInterception(Ray ray, Mesh mesh, out float t)
        {
            t = -1;

            List<float> distances = new();

            foreach (Face face in mesh.Faces)
            {
                Vector3 faceNormal = GetPlaneNormal(face.Points[0], face.Points[1], face.Points[2]);
                faceNormal = Vector3.Normalize(faceNormal);

                t = CalculateDistanceToTriangle(ray.origin, ray.dir, face.Points[0], face.Points[1], face.Points[2]);

                if (t == -1) continue;

                // compute the intersection point using equation 1
                Vector3 P = ray.origin + t * ray.dir;

                // Step 2: inside-outside test
                Vector3 C; // vector perpendicular to triangle's plane 
                // Console.WriteLine("d1");
                // edge 0
                Vector3 edge0 = face.Points[1] - face.Points[0];
                Vector3 vp0 = P - face.Points[0];
                C = Vector3.Cross(edge0, vp0);
                if (Vector3.Dot(faceNormal, C) < 0) continue; // P is on the right side 
                // Console.WriteLine("d");
                // edge 1
                Vector3 edge1 = face.Points[2] - face.Points[1];
                Vector3 vp1 = P - face.Points[1];
                C = Vector3.Cross(edge1, vp1);
                if (Vector3.Dot(faceNormal, C) < 0) continue; // P is on the right side 
                // Console.WriteLine("d");
                // edge 2
                Vector3 edge2 = face.Points[0] - face.Points[2];
                Vector3 vp2 = P - face.Points[2];
                C = Vector3.Cross(edge2, vp2);
                if (Vector3.Dot(faceNormal, C) < 0) continue; // P is on the right side; 

                distances.Add(t); // this ray hits the triangle   
            }

            if (distances.Any())
            {
                t = distances.Min();
                return true;
            }

            return false;
        }
        // Möller–Trumbore intersection algorithm
        private float CalculateDistanceToTriangle(Vector3 orig, Vector3 dir, Vector3 v0, Vector3 v1, Vector3 v2)
        {
            Vector3 e1 = v1 - v0;
            Vector3 e2 = v2 - v0;
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
        private float DrawDebugSphere(Ray ray, Vector3 center, float radius)
        {
            float t = Vector3.Dot(center - ray.origin, ray.dir);
            Vector3 point = ray.origin + ray.dir * t;

            float y = (center - point).Length();
            if (y < radius)
            {
                return 1;
            }

            return 0;
        }
        private Color Get255Color(float value, float min, float max)
        {
            float clampedValue;

            if (value == 0)
            {
                clampedValue = 0;
            }
            else
            {
                if (min != max)
                {
                    clampedValue = 1 - (value - min) / (max - min);
                }
                else
                {
                    clampedValue = 1 - value / min;
                }
            }

            return Color.FromArgb((int)(clampedValue * 255), (int)(clampedValue * 255), (int)(clampedValue * 255));
        }
    }
}