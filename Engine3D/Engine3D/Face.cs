using System.Numerics;

namespace Engine3D
{
    public readonly struct Face
    {
        public Vector3[] Points { get; }

        public Face(Vector3 p1,Vector3 p2,Vector3 p3)
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

    }
}