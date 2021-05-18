using System;
using System.Drawing;

namespace Engine3D
{
    public struct MyColor : IEquatable<MyColor>
    {
        public static MyColor Black { get; } = new(0, 0, 0);
        public static MyColor White { get; } = new(1, 1, 1);
        public static MyColor Red { get; } = new(1, 0, 0);
        public static MyColor Green { get; } = new(0, 1, 0);
        public static MyColor Blue { get; } = new(0, 0, 1);

        private float _r;
        private float _g;
        private float _b;
        private float _a;

        public float R
        {
            get => _r;
            set => _r = Math.Clamp(value, 0, 1);
        }

        public float G
        {
            get => _g;
            set => _g = Math.Clamp(value, 0, 1);
        }

        public float B
        {
            get => _b;
            set => _b = Math.Clamp(value, 0, 1);
        }

        public float A
        {
            get => _a;
            set => _a = Math.Clamp(value, 0, 1);
        }

        public MyColor(float r, float g, float b)
        {
            this._r = Math.Clamp(r, 0, 1);
            this._g = Math.Clamp(g, 0, 1);
            this._b = Math.Clamp(b, 0, 1);
            _a = 1;
        }

        public MyColor(float baseColor, float a)
        {
            _r = baseColor;
            _g = baseColor;
            _b = baseColor;
            this._a = a;
        }

        public MyColor(Color color)
        {
            _r = color.R / 255f;
            _g = color.G / 255f;
            _b = color.B / 255f;
            _a = color.A / 255f;
        }

        public void Multiply(MyColor otherColor)
        {
            _r *= otherColor._r;
            _g *= otherColor._g;
            _b *= otherColor._b;
            _a *= otherColor._a;
        }

        public void Blend(MyColor color, float amount)
        {
            _r = color.R * amount + R * (1 - amount);
            _g = color.G * amount + G * (1 - amount);
            _b = color.B * amount + B * (1 - amount);
        }

        public Color ToColor()
        {
            return Color.FromArgb((int)(_a * 255), (int)(_r * 255), (int)(_g * 255), (int)(_b * 255));
        }

        public bool Equals(MyColor other)
        {
            return _r.Equals(other._r) && _g.Equals(other._g) && _b.Equals(other._b) && _a.Equals(other._a);
        }

        public override bool Equals(object obj)
        {
            return obj is MyColor other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_r, _g, _b, _a);
        }
    }
}