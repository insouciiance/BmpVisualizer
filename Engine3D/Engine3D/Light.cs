using System.Numerics;

namespace Engine3D
{
    public class Light
    {
        public Vector3 Position { get; }
        public MyColor Color { get; }
        public float Intensity { get; }

        public Light(Vector3 position, MyColor color, float intensity)
        {
            Position = position;
            Color = color;
            Intensity = intensity;
        }
    }
}