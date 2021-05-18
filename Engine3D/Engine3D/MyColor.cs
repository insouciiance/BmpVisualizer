using System;
using System.Drawing;

namespace Engine3D
{
    public struct MyColor : IEquatable<MyColor>
    {
        public static MyColor Black = new MyColor(0, 0, 0);
        public static MyColor White = new MyColor(1, 1, 1);
        public static MyColor Red = new MyColor(1, 0, 0);
        public static MyColor Green = new MyColor(0, 1, 0);
        public static MyColor Blue = new MyColor(0, 0, 1);
        
        
        private float r;
        private float g;
        private float b;
        private float a;

        public float R
        {
            get => r;
            set => r = Math.Clamp(value,0,1);
        }

        public float G
        {
            get => g;
            set => g = Math.Clamp(value,0,1);
        }

        public float B
        {
            get => b;
            set => b = Math.Clamp(value,0,1);
        }

        public float A
        {
            get => a;
            set => a = Math.Clamp(value,0,1);
        }

        public MyColor(float r, float g, float b)
        {
            this.r = Math.Clamp(r,0,1);
            this.g = Math.Clamp(g,0,1);
            this.b = Math.Clamp(b,0,1);
            a = 1;
        }

        public MyColor(float baseColor, float a)
        {
            r = Math.Clamp(baseColor,0,1);
            g = r;
            b = r;
            this.a = a;
        }

        public MyColor(Color color)
        {
            r = color.R / 255f;
            g = color.G / 255f;
            b = color.B / 255f;
            a = color.A / 255f;
        }

        public void Multiply(MyColor otherColor)
        {
            r *= otherColor.r;
            g *= otherColor.g;
            b *= otherColor.b;
            a *= otherColor.a;
        }

        public void Blend(MyColor color, float amount)
        {
            r = ((color.R * amount) + R * (1 - amount));
            g = ((color.G * amount) + G * (1 - amount));
            b = ((color.B * amount) + B * (1 - amount));
        }
        
        public Color ToColor()
        {
            return Color.FromArgb((int) (a * 255),(int) (r * 255),(int) (g * 255),(int) (b * 255));
        }

        public bool Equals(MyColor other)
        {
            return r.Equals(other.r) && g.Equals(other.g) && b.Equals(other.b) && a.Equals(other.a);
        }

        public override bool Equals(object obj)
        {
            return obj is MyColor other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(r, g, b, a);
        }
    }
}