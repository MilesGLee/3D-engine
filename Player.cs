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
        private float rot = 0;
        
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

            if (Raylib.IsKeyDown(KeyboardKey.KEY_LEFT))
                rot = rot + 0.05f;
            if (Raylib.IsKeyDown(KeyboardKey.KEY_RIGHT))
                rot -= 0.05f;

            //if (rot > 360)
            //    rot = 0;
            //if (rot < 0)
            //    rot = 360;

            Console.WriteLine(rot);

            Rotate(0, rot, 0);

            //Vector3 moveDirection = new Vector3(xDiretion, 0, zDiretion);
            //Velocity = moveDirection.Normalized * Speed * deltaTime;

            Vector3 moveDirectionX = Right * -xDiretion;
            Vector3 moveDirectionZ = Forward * -zDiretion;
            Velocity = (moveDirectionZ + moveDirectionX).Normalized * Speed * deltaTime;

            //if (Velocity.Magnitude > 0)
            //    Forward = Velocity.Normalized;

            base.Translate(Velocity.X, Velocity.Y, Velocity.Z);

            base.Update(deltaTime);
        }
    }
}
