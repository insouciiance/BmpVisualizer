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
    }
}
