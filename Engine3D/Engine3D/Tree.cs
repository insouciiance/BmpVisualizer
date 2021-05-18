using System.Collections.Generic;
using System.Net.Sockets;

namespace Engine3D
{
    public class Tree
    {
        public bool IsParent = false;
        public int Size = 0;
        private const int MaxSize = 10;
        public double xMax, xMin, yMax, yMin, zMax, zMin;
        public List<Face> Faces = new();
        public Tree Child1, Child2;

        public void Add(Face f)
        {
            Size++;
            if (Size == 1)
            {
                xMax = Max(f.Points[0].X, Max(f.Points[1].X, f.Points[2].X));
                xMin = Min(f.Points[0].X, Min(f.Points[1].X, f.Points[2].X));
                yMax = Max(f.Points[0].Y, Max(f.Points[1].Y, f.Points[2].Y));
                yMin = Min(f.Points[0].Y, Min(f.Points[1].Y, f.Points[2].Y));
                zMax = Max(f.Points[0].Z, Max(f.Points[1].Z, f.Points[2].Z));
                zMin = Min(f.Points[0].Z, Min(f.Points[1].Z, f.Points[2].Z));
                Faces.Add(f);
            }
            else
            {
                xMax = Max(Max(f.Points[0].X, Max(f.Points[1].X, f.Points[2].X)),xMax);
                xMin = Min(Min(f.Points[0].X, Min(f.Points[1].X, f.Points[2].X)), xMin);
                yMax = Max(Max(f.Points[0].Y, Max(f.Points[1].Y, f.Points[2].Y)), yMax);
                yMin = Min(Min(f.Points[0].Y, Min(f.Points[1].Y, f.Points[2].Y)), yMin);
                zMax = Max(Max(f.Points[0].Z, Max(f.Points[1].Z, f.Points[2].Z)), zMax);
                zMin = Min(Min(f.Points[0].Z, Min(f.Points[1].Z, f.Points[2].Z)), zMin);
                if (IsParent)
                {
                    if (OptimalInclude(Child1, Child2, f)) Child1.Add(f);
                    else Child2.Add(f);
                }
                else
                {
                    Faces.Add(f);
                    if (Size > MaxSize) Divide();
                }
            }
        }
        private static int CompareByCoordinateX(Face f1, Face f2)
        {
            if (Max(f1.Points[0].X, Max(f1.Points[1].X, f1.Points[2].X)) > Max(f2.Points[0].X, Max(f2.Points[1].X, f2.Points[2].X))) return 1;
            if (Min(f2.Points[0].X, Min(f2.Points[1].X, f2.Points[2].X)) < Min(f1.Points[0].X, Min(f1.Points[1].X, f1.Points[2].X))) return -1;
            if (Max(f1.Points[0].Y, Max(f1.Points[1].Y, f1.Points[2].Y)) > Max(f2.Points[0].Y, Max(f2.Points[1].Y, f2.Points[2].Y))) return 1;
            if (Min(f2.Points[0].Y, Min(f2.Points[1].Y, f2.Points[2].Y)) < Min(f1.Points[0].Y, Min(f1.Points[1].Y, f1.Points[2].Y))) return -1;
            if (Max(f1.Points[0].Z, Max(f1.Points[1].Z, f1.Points[2].Z)) > Max(f2.Points[0].Z, Max(f2.Points[1].Z, f2.Points[2].Z))) return 1;
            return -1;
        }

        private static int CompareByCoordinateY(Face f1, Face f2)
        {
            if (Max(f1.Points[0].Y, Max(f1.Points[1].Y, f1.Points[2].Y)) > Max(f2.Points[0].Y, Max(f2.Points[1].Y, f2.Points[2].Y))) return 1;
            if (Min(f2.Points[0].Y, Min(f2.Points[1].Y, f2.Points[2].Y)) < Min(f1.Points[0].Y, Min(f1.Points[1].Y, f1.Points[2].Y))) return -1;
            if (Max(f1.Points[0].X, Max(f1.Points[1].X, f1.Points[2].X)) > Max(f2.Points[0].X, Max(f2.Points[1].X, f2.Points[2].X))) return 1;
            if (Min(f2.Points[0].X, Min(f2.Points[1].X, f2.Points[2].X)) < Min(f1.Points[0].X, Min(f1.Points[1].X, f1.Points[2].X))) return -1;
            if (Max(f1.Points[0].Z, Max(f1.Points[1].Z, f1.Points[2].Z)) > Max(f2.Points[0].Z, Max(f2.Points[1].Z, f2.Points[2].Z))) return 1;
            return -1;
        }
        private static int CompareByCoordinateZ(Face f1, Face f2)
        {
            if (Max(f1.Points[0].Z, Max(f1.Points[1].Z, f1.Points[2].Z)) > Max(f2.Points[0].Z, Max(f2.Points[1].Z, f2.Points[2].Z))) return 1;
            if (Min(f2.Points[0].Z, Min(f2.Points[1].Z, f2.Points[2].Z)) < Min(f1.Points[0].Z, Min(f1.Points[1].Z, f1.Points[2].Z))) return -1;
            if (Max(f1.Points[0].X, Max(f1.Points[1].X, f1.Points[2].X)) > Max(f2.Points[0].X, Max(f2.Points[1].X, f2.Points[2].X))) return 1;
            if (Min(f2.Points[0].X, Min(f2.Points[1].X, f2.Points[2].X)) < Min(f1.Points[0].X, Min(f1.Points[1].X, f1.Points[2].X))) return -1;
            if (Max(f1.Points[0].Y, Max(f1.Points[1].Y, f1.Points[2].Y)) > Max(f2.Points[0].Y, Max(f2.Points[1].Y, f2.Points[2].Y))) return 1;
            return -1;
        }

        private void Divide()
        {
            IsParent = true;
            Child1 = new Tree();
            Child2 = new Tree();
            if (xMax-xMin>=yMax-yMin && xMax-xMin>=zMax-zMin) Faces.Sort(CompareByCoordinateX);
            else if (yMax - yMin >= xMax - xMin && yMax - yMin >= zMax - zMin) Faces.Sort(CompareByCoordinateY);
            else Faces.Sort(CompareByCoordinateZ);
            Child1.Add(Faces[0]);
            Child2.Add(Faces[Size-1]);
            Faces.RemoveAt(Size-1);
            Faces.RemoveAt(0);
            foreach (Face face in Faces)
            {
                if (OptimalInclude(Child1, Child2, face)) Child1.Add(face);
                else Child2.Add(face);
            }
        }
        public static bool OptimalInclude(Tree n1, Tree n2, Face f)
        {
            //imagine that we add a face to the first node
            double newXMax1 = Max(n1.xMax,  Max(f.Points[0].X, Max(f.Points[1].X, f.Points[2].X)));
            double newXMin1 = Min(n1.xMax, Min(f.Points[0].X, Min(f.Points[1].X, f.Points[2].X)));
            double newYMax1 = Max(n1.yMax, Max(f.Points[0].Y, Max(f.Points[1].Y, f.Points[2].Y)));
            double newYMin1 = Min(n1.yMax, Min(f.Points[0].Y, Min(f.Points[1].Y, f.Points[2].Y)));
            double newZMax1 = Max(n1.zMax, Max(f.Points[0].Z, Max(f.Points[1].Z, f.Points[2].Z)));
            double newZMin1 = Min(n1.zMax, Min(f.Points[0].Z, Min(f.Points[1].Z, f.Points[2].Z)));
            double v1 = (newXMax1 - newXMin1) * (newYMax1 - newYMin1) * (newZMax1 - newZMin1) + (n2.xMax - n2.xMin) * (n2.yMax - n2.yMin) * (n2.zMax - n2.zMin);
            //imagine that we add a face to the second node
            double newXMax2 = Max(n2.xMax, Max(f.Points[0].X, Max(f.Points[1].X, f.Points[2].X)));
            double newXMin2 = Min(n2.xMax, Min(f.Points[0].X, Min(f.Points[1].X, f.Points[2].X)));
            double newYMax2 = Max(n2.yMax, Max(f.Points[0].Y, Max(f.Points[1].Y, f.Points[2].Y)));
            double newYMin2 = Min(n2.yMax, Min(f.Points[0].Y, Min(f.Points[1].Y, f.Points[2].Y)));
            double newZMax2 = Max(n2.zMax, Max(f.Points[0].Z, Max(f.Points[1].Z, f.Points[2].Z)));
            double newZMin2 = Min(n2.zMax, Min(f.Points[0].Z, Min(f.Points[1].Z, f.Points[2].Z)));
            double v2 = (n1.xMax - n1.xMin) * (n1.yMax - n1.yMin) * (n1.zMax - n1.zMin) + (newXMax2 - newXMin2) * (newYMax2 - newYMin2) * (newZMax2 - newZMin2);
            //in which case the volume will be smaller?
            return v1 == v2 ? n1.Size < n2.Size : v1 < v2;
        }

        private static double Max(double d1, double d2)
        {
            return d1 > d2 ? d1 : d2;
        }
        private static double Min(double d1, double d2)
        {
            return d1 < d2 ? d1 : d2;
        }
        

    }
}