using System.Numerics;

namespace Engine3D
{
    public struct Face
    {
        private Vector3 _point1;
        private Vector3 _point2;
        private Vector3 _point3;

        public Vector3 Point1 => _point1;
        public Vector3 Point2 => _point2;
        public Vector3 Point3 => _point3;

        public Face(Vector3 p1,Vector3 p2,Vector3 p3)
        {
            _point1 = p1;
            _point2 = p2;
            _point3 = p3;
        }
        
        public Face(params Vector3[] points)
        {
            _point1 = points.Length >= 1 ? points[0] : Vector3.Zero;
            _point2 = points.Length >= 2 ? points[1] : Vector3.Zero;
            _point3 = points.Length >= 3 ? points[2] : Vector3.Zero;
        }

    }
}