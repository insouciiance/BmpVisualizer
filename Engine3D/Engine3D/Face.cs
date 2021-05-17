using System.Numerics;

namespace Engine3D
{
    public struct Face
    {
        private readonly Vector3[] _points;
        public Vector3[] Points => _points;

        public Face(Vector3 p1,Vector3 p2,Vector3 p3)
        {
            _points = new Vector3[3];
            _points[0] = p1;
            _points[1] = p2;
            _points[2] = p3;
        }
        
        public Face(params Vector3[] points)
        {
            _points = new Vector3[3];
            _points[0] = points.Length >= 1 ? points[0] : Vector3.Zero;
            _points[1] = points.Length >= 2 ? points[1] : Vector3.Zero;
            _points[2] = points.Length >= 3 ? points[2] : Vector3.Zero;
        }

    }
}