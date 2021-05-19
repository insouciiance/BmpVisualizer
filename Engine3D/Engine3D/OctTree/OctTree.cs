using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Engine3D.OctTree
{
    internal class OctTree
    {
        private const int MinSize = 1;

        public BoundingBox Region { get; init; }
        public OctTree[] Children { get; init; }
        public Face[] Faces { get; init; }

        public OctTree(IEnumerable<Face> faces)
        {
            Faces = faces.ToArray();
        }

        private void BuildTree()
        {
            if (Faces.Length <= 1) return;

            Vector3 dimensions = Region.Max - Region.Min;

            if (dimensions.X <= MinSize && dimensions.Y <= MinSize && dimensions.Z <= MinSize) return;
            

        }
    }
}
