using System;
using System.Collections.Generic;
using System.Text;
using MathLibrary;
using Raylib_cs;

namespace _3dEngine
{
    class Player : Actor
    {
        private float _speed;
        private Vector3 _velocity;
        
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

        public Player(float x, float y, float speed, string name = "Player", Shape shape = Shape.SPHERE)
            : base(x, y, speed, name, shape) 
        {
            _speed = speed;
        }

        public override void Update(float deltaTime)
        {
            int xDiretion = -Convert.ToInt32(Raylib.IsKeyDown(KeyboardKey.KEY_A))
                + Convert.ToInt32(Raylib.IsKeyDown(KeyboardKey.KEY_D));
            int zDiretion = -Convert.ToInt32(Raylib.IsKeyDown(KeyboardKey.KEY_W))
                + Convert.ToInt32(Raylib.IsKeyDown(KeyboardKey.KEY_S));

            Vector3 moveDirection = new Vector3(xDiretion, 0, zDiretion);
            Velocity = moveDirection.Normalized * Speed * deltaTime;
            if (Velocity.Magnitude > 0)
                Forward = Velocity.Normalized;

            base.Translate(Velocity.X, Velocity.Y, Velocity.Z);

            base.Update(deltaTime);
        }
    }
}
