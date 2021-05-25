using System.Numerics;

namespace Engine3D
{
    public struct Ray
    {
        public Vector3 Origin { get; set; }
        public Vector3 Dir { get; set; }

        public Ray(Vector3 origin, Vector3 dir)
        {
            this.Origin = origin;
            this.Dir = dir;
        }
    }
}