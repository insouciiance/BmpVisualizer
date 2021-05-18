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
        public List<Light> Lights;

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
                    var c = Trace(x, y);
                    _canvas.SetPixel(x, y, c.ToColor());
                }
            }
        }

        private MyColor Trace(int screenX, int screenY)
        {
            Vector2 uv = (new Vector2(screenX, screenY) - .5f * new Vector2(_canvas.Width, _canvas.Height)) /
                         _canvas.Height;
            MyColor color = MyColor.Black;

            Vector3 rayInterception = CenterOfScreen + uv.X * _localRight + -uv.Y * _localUp;
            Ray cameraTrace = new(_cameraOrigin, Vector3.Normalize(rayInterception - _cameraOrigin));

            // (float min, float max) distanceRange = GetDistanceRange(new Ray(_cameraOrigin, _lookDirection), Mesh);

            bool intercepted = RayFaceInterception(cameraTrace, Mesh, out float dist, out Vector3 faceNormal);

            if (intercepted)
            {
                Vector3 interceptionPoint = cameraTrace.dir * dist + cameraTrace.origin;
                foreach (var light in Lights)
                {
                    Ray lightTrace = new Ray(interceptionPoint, Vector3.Normalize(light.Position - interceptionPoint));
                    float dotProduct = Vector3.Dot(faceNormal, lightTrace.dir);
                    if (dotProduct > 0)
                    {
                        if (color.Equals(MyColor.Black))
                        {
                            color = MyColor.White;
                        }
                        float lightDistSqr = Vector3.DistanceSquared(interceptionPoint, light.Position);
                        float colorVal = dotProduct * light.Intensity / lightDistSqr;
                        var lColor = light.LightColor;
                        lColor.Multiply(new MyColor(colorVal, 1f));
                        color.Blend(lColor,.5f);
                    }
                    
                }

                
                /*int colorMaped = (int) (Math.Clamp(colorVal,0,1) * 255);
                color = Color.FromArgb(colorMaped,colorMaped,colorMaped);*/
                // float perpendicularDistance = Vector3.Dot(_lookDirection, r.dir * dist);
            }

            // color = Get255Color(intercepted ? perpendicularDistance : 0, distanceRange.min, distanceRange.max);

            return color;
        }

        public bool RayFaceInterception(Ray ray, Mesh mesh, out float distance, out Vector3 normal)
        {
            distance = float.PositiveInfinity;
            normal = Vector3.Zero;

            float tempDitance = -1;
            foreach (Face face in mesh.Faces)
            {
                //Vector3 faceNormal = GetPlaneNormal(face.Points[0], face.Points[1], face.Points[2]);

                tempDitance = CalculateDistanceToTriangle(ray.origin, ray.dir, face.Points[0], face.Points[1],
                    face.Points[2], out Vector3 faceNormal);

                if (tempDitance == -1) continue;
                if (tempDitance < distance)
                {
                    distance = tempDitance;
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
            // Вычисление вектора нормали к плоскости
            Vector3 pvec = Vector3.Cross(dir, e2);
            normal = pvec;
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

            return Color.FromArgb((int) (clampedValue * 255), (int) (clampedValue * 255), (int) (clampedValue * 255));
        }
    }
}