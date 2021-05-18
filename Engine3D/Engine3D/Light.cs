﻿using System.Numerics;

namespace Engine3D
{
    public class Light
    {
        private Vector3 _position;
        private MyColor _color;
        private float _intensity;

        public Vector3 Position => _position;

        public MyColor Color => _color;

        public float Intensity => _intensity;

        public Light(Vector3 position, MyColor color, float intensity)
        {
            _position = position;
            _color = color;
            _intensity = intensity;
        }
    }
}