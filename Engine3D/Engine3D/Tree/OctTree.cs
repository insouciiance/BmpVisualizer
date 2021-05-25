using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic.CompilerServices;

namespace Engine3D.Tree
{
    internal class OctTree
    {
        private const float MinSize = .0005f;

        public BoundingBox Region { get; init; }
        public OctTree[] Children { get; init; }
        public List<Face> Faces { get; init; }

        public OctTree(BoundingBox maxRegion, IEnumerable<Face> faces)
        {
            Region = maxRegion;
            Faces = faces.ToList();
            Children = new OctTree[8];
            BuildTree();
        }

        private void BuildTree()
        {
            if (Faces.Count <= 10)
            {
                return;
            }

            Vector3 dimensions = Region.Max - Region.Min;

            if (dimensions.Length() <= MinSize)
            {
                return;
            }

            Vector3 half = dimensions / 2;
            Vector3 center = Region.Min + half;

            BoundingBox[] childRegions = new BoundingBox[8];
            childRegions[0] = new BoundingBox(Region.Min, center);
            childRegions[1] = new BoundingBox(new Vector3(center.X, Region.Min.Y, Region.Min.Z),
                new Vector3(Region.Max.X, center.Y, center.Z));
            childRegions[2] = new BoundingBox(new Vector3(center.X, Region.Min.Y, center.Z),
                new Vector3(Region.Max.X, center.Y, Region.Max.Z));
            childRegions[3] = new BoundingBox(new Vector3(Region.Min.X, Region.Min.Y, center.Z),
                new Vector3(center.X, center.Y, Region.Max.Z));
            childRegions[4] = new BoundingBox(new Vector3(Region.Min.X, center.Y, Region.Min.Z),
                new Vector3(center.X, Region.Max.Y, center.Z));
            childRegions[5] = new BoundingBox(new Vector3(center.X, center.Y, Region.Min.Z),
                new Vector3(Region.Max.X, Region.Max.Y, center.Z));
            childRegions[6] = new BoundingBox(center, Region.Max);
            childRegions[7] = new BoundingBox(new Vector3(Region.Min.X, center.Y, center.Z),
                new Vector3(center.X, Region.Max.Y, Region.Max.Z));

            List<Face>[] regionsElements = new List<Face>[8];
            for (int i = 0; i < 8; i++)
            {
                regionsElements[i] = new List<Face>();
            }

            List<Face> nextElements = new();

            foreach (Face face in Faces)
            {
                for (int i = 0; i < 8; i++)
                {
                    if (IsFaceInsideBox(face, childRegions[i]))
                    {
                        regionsElements[i].Add(face);
                        nextElements.Add(face);
                    }
                }
            }

            foreach (Face nextFace in nextElements)
            {
                Faces.Remove(nextFace);
            }

            for (int i = 0; i < 8; i++)
            {
                if (regionsElements[i].Count != 0)
                {
                    Children[i] = CreateNode(childRegions[i], regionsElements[i]);
                }
            }
        }

        public List<IntersectionRecord> GetIntersection(Ray ray)
        {
            bool allChildrenNull = true;

            foreach (OctTree child in Children)
            {
                if (child is not null)
                {
                    allChildrenNull = false;
                }
            }

            if (allChildrenNull && Faces.Count == 0) return null;

            List<IntersectionRecord> intersectedFaces = new();

            foreach (Face face in Faces)
            {
                IntersectionRecord ir;

                if ((ir = face.Intersect(ray)) != null)
                {
                    intersectedFaces.Add(ir);
                }
            }

            for (int i = 0; i < 8; i++)
            {
                if (Children[i] != null && Children[i].Region.Intersects(ray))
                {
                    List<IntersectionRecord> intersections = Children[i].GetIntersection(ray);
                    if (intersections != null)
                    {
                        foreach (IntersectionRecord ir in intersections)
                        {
                            intersectedFaces.Add(ir);
                        }
                    }
                }
            }

            return intersectedFaces;
        }

        private OctTree CreateNode(BoundingBox region, IEnumerable<Face> faces)
        {
            OctTree tree = new(region, faces);
            return tree;
        }

        private static bool IsFaceInsideBox(Face f, BoundingBox box)
        {
            Vector3 c = (box.Min + box.Max) * 0.5f;
            float e0 = (box.Max.X - box.Min.X) * 0.5f;
            float e1 = (box.Max.Y - box.Min.Y) * 0.5f;
            float e2 = (box.Max.Z - box.Min.Z) * 0.5f;
            
            Vector3 v0 = f.Points[0] - c;
            Vector3 v1 = f.Points[1] - c;
            Vector3 v2 = f.Points[2] - c;

            Vector3 f0 = f.Points[1] - f.Points[0];
            Vector3 f1 = f.Points[2] - f.Points[1];
            Vector3 f2 = f.Points[0] - f.Points[2];
            
            Vector3 a00;
            a00.X = 0;
            a00.Y = -f0.Z;
            a00.Z = f0.Y;

            float p0 = Vector3.Dot(v0, a00);
            float p1 = Vector3.Dot(v1, a00);
            float p2 = Vector3.Dot(v2, a00);
            float r = e1 * Math.Abs(f0.Z) + e2 * Math.Abs(f0.Y);
            if (Math.Max(-Math.Max(p2,Math.Max(p0, p1)), Math.Min(Math.Min(p0, p1), p2)) > r)
            {
                return false;
            }

            Vector3 a01;
            a01.X = 0;
            a01.Y = -f1.Z;
            a01.Z = f1.Y;

            p0 = Vector3.Dot(v0, a01);
            p1 = Vector3.Dot(v1, a01);
            p2 = Vector3.Dot(v2, a01);
            r = e1 * Math.Abs(f1.Z) + e2 * Math.Abs(f1.Y);
            if (Math.Max(-Math.Max(p0,Math.Max(p1, p2)), Math.Min(Math.Min(p0, p1), p2)) > r)
            {
                return false;
            }
            
            
            Vector3 a02;
            a02.X = 0;
            a02.Y = -f2.Z;
            a02.Z = f2.Y;

            p0 = Vector3.Dot(v0, a02);
            p1 = Vector3.Dot(v1, a02);
            p2 = Vector3.Dot(v2, a02);
            r = e1 * Math.Abs(f2.Z) + e2 * Math.Abs(f2.Y);
            if (Math.Max(-Math.Max(p0,Math.Max(p1, p2)), Math.Min(Math.Min(p0, p1), p2)) > r)
            {
                return false;
            }
            
            Vector3 a10;
            a10.X = f0.Z;
            a10.Y = 0;
            a10.Z = -f0.X;

            p0 = Vector3.Dot(v0, a10);
            p1 = Vector3.Dot(v1, a10);
            p2 = Vector3.Dot(v2, a10);
            r = e0 * Math.Abs(f0.Z) + e2 * Math.Abs(f0.X);
            if (Math.Max(-Math.Max(p0,Math.Max(p1, p2)), Math.Min(Math.Min(p0, p1), p2)) > r)
            {
                return false;
            }
            
            Vector3 a11;
            a11.X = f1.Z;
            a11.Y = 0;
            a11.Z = -f1.X;

            p0 = Vector3.Dot(v0, a11);
            p1 = Vector3.Dot(v1, a11);
            p2 = Vector3.Dot(v2, a11);
            r = e0 * Math.Abs(f1.Z) + e2 * Math.Abs(f1.X);
            if (Math.Max(-Math.Max(p0,Math.Max(p1, p2)), Math.Min(Math.Min(p0, p1), p2)) > r)
            {
                return false;
            }
            
            Vector3 a12;
            a12.X = f2.Z;
            a12.Y = 0;
            a12.Z = -f2.X;

            p0 = Vector3.Dot(v0, a12);
            p1 = Vector3.Dot(v1, a12);
            p2 = Vector3.Dot(v2, a12);
            r = e0 * Math.Abs(f2.Z) + e2 * Math.Abs(f2.X);
            if (Math.Max(-Math.Max(p0,Math.Max(p1, p2)), Math.Min(Math.Min(p0, p1), p2)) > r)
            {
                return false;
            }
            
            Vector3 a20;
            a20.X = -f0.Y;
            a20.Y = f0.X;
            a20.Z = 0;

            p0 = Vector3.Dot(v0, a20);
            p1 = Vector3.Dot(v1, a20);
            p2 = Vector3.Dot(v2, a20);
            r = e0 * Math.Abs(f0.Y) + e1 * Math.Abs(f0.X);
            if (Math.Max(-Math.Max(p0,Math.Max(p1, p2)), Math.Min(Math.Min(p0, p1), p2)) > r)
            {
                return false;
            }
            
            Vector3 a21;
            a21.X = -f1.Y;
            a21.Y = f1.X;
            a21.Z = 0;

            p0 = Vector3.Dot(v0, a21);
            p1 = Vector3.Dot(v1, a21);
            p2 = Vector3.Dot(v2, a21);
            r = e0 * Math.Abs(f1.Y) + e1 * Math.Abs(f1.X);
            if (Math.Max(-Math.Max(p0,Math.Max(p1, p2)), Math.Min(Math.Min(p0, p1), p2)) > r)
            {
                return false;
            }
            
            Vector3 a22;
            a22.X = -f2.Y;
            a22.Y = f2.X;
            a22.Z = 0;

            p0 = Vector3.Dot(v0, a22);
            p1 = Vector3.Dot(v1, a22);
            p2 = Vector3.Dot(v2, a22);
            r = e0 * Math.Abs(f2.Y) + e1 * Math.Abs(f2.X);
            if (Math.Max(-Math.Max(p0,Math.Max(p1, p2)), Math.Min(Math.Min(p0, p1), p2)) > r)
            {
                return false;
            }
            
            
            if (Math.Max(v0.X, Math.Max(v1.X, v2.X)) < -e0 || Math.Min(v0.X, Math.Min(v1.X, v2.X)) > e0)
            {
                return false;
            }
            
            if (Math.Max(v0.Y, Math.Max(v1.Y, v2.Y)) < -e1 || Math.Min(v0.Y, Math.Min(v1.Y, v2.Y)) > e1)
            {
                return false;
            }
            
            if (Math.Max(v0.Z, Math.Max(v1.Z, v2.Z)) < -e2 || Math.Min(v0.Z, Math.Min(v1.Z, v2.Z)) > e2)
            {
                return false;
            }

            Vector3 plane_normal = Vector3.Cross(f0, f1);
            float plane_distance = Vector3.Dot(plane_normal, f.Points[0]);

            r = e0 * Math.Abs(plane_normal.X) + e1 * Math.Abs(plane_normal.Y) +
                e2 * Math.Abs(plane_normal.Z);

            if (plane_distance - Math.Sqrt(Math.Pow(c.X,2) + Math.Pow(c.Y,2) + Math.Pow(c.Z,2)) > r) return false;

            return true;
            // Vector3.Min(box.Min, f.Points[0]) == box.Min &&
            // Vector3.Max(box.Max, f.Points[0]) == box.Max ||
            // Vector3.Min(box.Min, f.Points[2]) == box.Min &&
            // Vector3.Max(box.Max, f.Points[2]) == box.Max ||
            // Vector3.Max(box.Max, f.Points[1]) == box.Max &&
            // Vector3.Min(box.Min, f.Points[1]) == box.Min;
            // Vector3.Min(box.Min, (f.Points[0] + f.Points[1]) / 2) == box.Min &&
            // Vector3.Max(box.Max, (f.Points[0] + f.Points[1]) / 2) == box.Max ||
            // Vector3.Min(box.Min, (f.Points[1] + f.Points[2]) / 2) == box.Min &&
            // Vector3.Max(box.Max, (f.Points[1] + f.Points[2]) / 2) == box.Max ||
            // Vector3.Min(box.Min, (f.Points[0] + f.Points[2]) / 2) == box.Min &&
            // Vector3.Max(box.Max, (f.Points[0] + f.Points[2]) / 2) == box.Max ||
            // Vector3.Min(box.Min, (f.Points[2]+(f.Points[0] + f.Points[1]) / 2)/2) == box.Min &&
            // Vector3.Max(box.Max, (f.Points[2]+(f.Points[0] + f.Points[1]) / 2)/2) == box.Max ||
            // Vector3.Min(box.Min, (f.Points[0]+(f.Points[1] + f.Points[2]) / 2)/2) == box.Min &&
            // Vector3.Max(box.Max, (f.Points[0]+(f.Points[1] + f.Points[2]) / 2)/2) == box.Max ||
            // Vector3.Min(box.Min, (f.Points[1]+(f.Points[0] + f.Points[2]) / 2)/2) == box.Min &&
            // Vector3.Max(box.Max, (f.Points[1]+(f.Points[0] + f.Points[2]) / 2)/2) == box.Max;


            // || !(Vector3.Min(box.Min, f.Points[0]) == f.Points[0] &&
            // Vector3.Min(box.Min, f.Points[1]) == f.Points[1] &&
            // Vector3.Min(box.Min, f.Points[2]) == f.Points[2] ||
            // Vector3.Max(box.Max, f.Points[0]) == f.Points[0] &&
            // Vector3.Max(box.Max, f.Points[1]) == f.Points[1] &&
            // Vector3.Max(box.Max, f.Points[2]) == f.Points[2])
        }
    }
}
