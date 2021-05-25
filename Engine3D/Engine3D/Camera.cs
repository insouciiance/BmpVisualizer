using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata;
using Engine3D.Tree;

namespace Engine3D
{
    internal class Camera
    {
        private readonly Vector3 _cameraOrigin;
        private readonly Bitmap _canvas;

        private readonly Vector3 _localRight;
        private readonly Vector3 _localUp;
        private OctTree _tree;

        public Vector3 CenterOfScreen { get; }

        public Mesh Mesh { get; init; }

        public List<Light> Lights { get; set; }

        public Camera(Vector3 cameraOrigin, Vector3 lookAtPoint, float distanceToScreen, Bitmap canvas)
        {
            _cameraOrigin = cameraOrigin;
            _canvas = canvas;

            var lookDirection = Vector3.Normalize(lookAtPoint - _cameraOrigin);
            _localRight = Vector3.Cross(lookDirection, new Vector3(0, 0, 1));
            _localUp = Vector3.Cross(_localRight, lookDirection);
            CenterOfScreen = cameraOrigin + lookDirection * distanceToScreen;
        }

        public void Draw()
        {
            _tree = new(new BoundingBox(new Vector3(-30, -30, -30), new Vector3(30, 30, 30)), Mesh.Faces);

            for (int y = 0; y < _canvas.Height; y++)
            {
                Console.WriteLine($"{y * 100 /_canvas.Height}%");

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

            bool intercepted = RayFaceInterception(cameraTrace, out float dist, out Vector3 faceNormal);

            if (intercepted)
            {
                Vector3 interceptionPoint = cameraTrace.dir * dist + cameraTrace.origin;
                color = LightTrace(interceptionPoint, faceNormal);
            }

            return color;
        }

        private MyColor LightTrace(Vector3 point, Vector3 faceNormal)
        {
            MyColor? color = null;

            foreach (Light light in Lights)
            {
                Ray lightTrace = new(point, Vector3.Normalize(light.Position - point));
                float dotProduct = Vector3.Dot(faceNormal, lightTrace.dir);
                if (dotProduct > 0)
                {
                    float lightDistSqr = Vector3.DistanceSquared(point, light.Position);
                    float colorVal = dotProduct * light.Intensity / lightDistSqr;
                    MyColor lColor = light.Color;
                    lColor.Multiply(new MyColor(colorVal, 1f));

                    color ??= lColor;
                    color.Value.Blend(lColor, .5f);
                }
            }

            return color ?? MyColor.Black;
        }

        private bool RayFaceInterception(Ray ray, out float distance, out Vector3 normal)
        {
            distance = float.PositiveInfinity;
            normal = Vector3.Zero;

            foreach (IntersectionRecord intersectionRecord in _tree.GetIntersection(ray))
            {
                if (intersectionRecord.Distance < distance)
                {
                    distance = intersectionRecord.Distance;
                    normal = intersectionRecord.Normal;
                }
            }

            return !float.IsPositiveInfinity(distance);
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