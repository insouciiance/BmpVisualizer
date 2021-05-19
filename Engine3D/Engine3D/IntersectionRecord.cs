using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Engine3D.OctTree
{
    internal class IntersectionRecord
    {
        public Vector3 IntersectionPoint { get; init; }
        public Vector3 Normal { get; init; }
        public Ray Ray { get; init; }
        public Face IntersectedFace { get; init; }
        public float Distance { get; init; }

        public IntersectionRecord()
        {
            IntersectionPoint = Vector3.Zero;
            Normal = Vector3.Zero;
            Ray = default;
            IntersectedFace = default;
            Distance = float.MaxValue;
        }

        public IntersectionRecord(Vector3 intersectionPoint, Vector3 normal, Ray ray, Face intersectedFace, float distance)
        {
            IntersectionPoint = intersectionPoint;
            Normal = normal;
            Ray = ray;
            IntersectedFace = intersectedFace;
            Distance = distance;
        }
    }
}
