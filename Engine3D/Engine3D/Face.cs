using System.Numerics;
using Engine3D.Tree;

namespace Engine3D
{
    internal readonly struct Face
    {
        public Vector3[] Points { get; }

        public Face(Vector3 p1, Vector3 p2, Vector3 p3)
        {
            Points = new Vector3[3];
            Points[0] = p1;
            Points[1] = p2;
            Points[2] = p3;
        }

        public Face(params Vector3[] points)
        {
            Points = new Vector3[3];
            Points[0] = points.Length >= 1 ? points[0] : Vector3.Zero;
            Points[1] = points.Length >= 2 ? points[1] : Vector3.Zero;
            Points[2] = points.Length >= 3 ? points[2] : Vector3.Zero;
        }

        public IntersectionRecord Intersect(Ray ray)
        {
            // Möller–Trumbore intersection algorithm

            Vector3 orig = ray.Origin;
            Vector3 dir = ray.Dir;
            Vector3 v0 = Points[0];
            Vector3 v1 = Points[1];
            Vector3 v2 = Points[2];

            Vector3 e1 = v1 - v0;
            Vector3 e2 = v2 - v0;

            var normal = Vector3.Normalize(Vector3.Cross(e1, e2));

            // Вычисление вектора нормали к плоскости
            Vector3 pvec = Vector3.Cross(dir, e2);

            float det = Vector3.Dot(e1, pvec);

            // Луч параллелен плоскости
            if (det < 1e-8 && det > -1e-8)
            {
                return null;
            }

            float inv_det = 1 / det;
            Vector3 tvec = orig - v0;
            float u = Vector3.Dot(tvec, pvec) * inv_det;
            if (u < 0 || u > 1)
            {
                return null;
            }

            Vector3 qvec = Vector3.Cross(tvec, e1);
            float v = Vector3.Dot(dir, qvec) * inv_det;
            if (v < 0 || u + v > 1)
            {
                return null;
            }

            var distance = Vector3.Dot(e2, qvec) * inv_det;
            var intersectionPoint = ray.Origin + ray.Dir * distance;

            return new IntersectionRecord(intersectionPoint, normal, ray, this, distance);
        }
    }
}