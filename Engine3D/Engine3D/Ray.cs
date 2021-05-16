using System.Numerics;

namespace Engine3D
{
    public struct Ray
    {
        public Vector3 origin;
        public Vector3 dir;

        public Ray(Vector3 origin, Vector3 dir)
        {
            this.origin = origin;
            this.dir = dir;
        }
        
    }
}