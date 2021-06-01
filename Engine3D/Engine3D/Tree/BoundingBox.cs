using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Engine3D.Tree
{
    internal class BoundingBox
    {
        public Vector3 Max { get; init; }
        public Vector3 Min { get; init; }

        public BoundingBox(Vector3 first, Vector3 second)
        {
            if (Vector3.Max(first, second) == first)
            {
                Max = first;
                Min = second;
            }
            else
            {
                Max = second;
                Min = first;
            }
        }

        public bool Intersects(Ray r)
        {
            Vector3 dirfrac = new (1.0f / r.Dir.X, 1.0f / r.Dir.Y, 1.0f / r.Dir.Z);

            // lb is the corner of AABB with minimal coordinates - left bottom, rt is maximal corner
            // r.org is origin of ray
            float t1 = (Min.X - r.Origin.X) * dirfrac.X;
            float t2 = (Max.X - r.Origin.X) * dirfrac.X;
            float t3 = (Min.Y - r.Origin.Y) * dirfrac.Y;
            float t4 = (Max.Y - r.Origin.Y) * dirfrac.Y;
            float t5 = (Min.Z - r.Origin.Z) * dirfrac.Z;
            float t6 = (Max.Z - r.Origin.Z) * dirfrac.Z;

            float tmin = Math.Max(Math.Max(Math.Min(t1, t2), Math.Min(t3, t4)), Math.Min(t5, t6));
            float tmax = Math.Min(Math.Min(Math.Max(t1, t2), Math.Max(t3, t4)), Math.Max(t5, t6));

            // if tmax < 0, ray (line) is intersecting AABB, but the whole AABB is behind us
            if (tmax < 0)
            {
                return false;
            }

            // if tmin > tmax, ray doesn't intersect AABB
            if (tmin > tmax)
            {
                return false;
            }

            return true;
        }
    }
}
