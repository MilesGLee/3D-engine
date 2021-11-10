using System;
using System.Collections.Generic;
using System.Text;
using MathLibrary;
using Raylib_cs;

namespace _3dEngine
{
    class Entity : Actor
    {
        private float _speed;
        private Vector3 _velocity;
        private Vector3 _gravity = new Vector3(0, -0.1f, 0);
        private int _health;

        public float Speed
        {
            get { return _speed; }
            set { _speed = value; }
        }
        public Vector3 Velocity
        {
            get { return _velocity; }
            set { _velocity = value; }
        }

        public int Health
        {
            get { return _health; }
            set { _health = value; }
        }

        public Vector3 Gravity
        {
            get { return _gravity; }
            set { _gravity = value; }
        }

        public virtual void ApplyGravity()
        {
            if (!IsGrounded())
            {
                Velocity += Gravity;
            }
            else
            {
                Velocity = new Vector3(Velocity.X, 0, Velocity.Z);
            }
        }

        public Entity(float x, float y, float z, float speed, int health, Color color, string name = "Character", Shape shape = Shape.SPHERE)
             : base(x, y, z, shape, color, name)
        {
            _health = health;
            _speed = speed;
            Velocity = new Vector3(0, 0, 0);
        }
        public bool IsGrounded()
        {
            if (WorldPosition.Y <= 1)
                return true;
            else return false;
        }
    }
}