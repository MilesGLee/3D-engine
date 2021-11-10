using System;
using System.Collections.Generic;
using System.Text;
using MathLibrary;
using Raylib_cs;

namespace _3dEngine
{
    class Player : Entity
    {
        //Dodge Variables
        private float _dodgeTime;
        private bool _canDodge;
        private bool _dodgeCheck;

        //Mouse Variables
        public float mouseXSensitivity = 2;
        public float mouseYSensitivity = 1;
        private Vector2 mouseOrigin = new Vector2(Raylib.GetMonitorWidth(1) / 2, Raylib.GetMonitorHeight(1) / 2);

        public Player(float x, float y, float z, float speed, int health, Color color, string name = "Player", Shape shape = Shape.CUBE)
            : base(x, y, z, speed, health, color, name, shape)
        {
            Speed = speed;
            Tag = ActorTag.PLAYER;
            SetScale(1, 1, 1);
        }

        public override void Start()
        {
            SphereCollider playerCollider = new SphereCollider(1, this);
            base.Start();
        }

        public override void Update(float deltaTime)
        {
            GetTranslationInput(deltaTime);
            GetRotationInput(deltaTime);
            GetDodgeInput(deltaTime);

            base.Update(deltaTime);
        }

        public void GetRotationInput(float deltaTime)
        {
            Vector2 mousePosition = new Vector2(Raylib.GetMouseX(), Raylib.GetMouseY());
            Vector2 mouseDelta = mousePosition - mouseOrigin;

            float angle = MathF.Atan2(mouseDelta.Y, mouseDelta.X);

            if (mouseDelta.Magnitude > 0)
                base.Rotate((MathF.Sin(angle) * deltaTime) * mouseYSensitivity, (-MathF.Cos(angle) * deltaTime) * mouseXSensitivity, 0);
            Raylib.SetMousePosition(Raylib.GetMonitorWidth(1) / 2, Raylib.GetMonitorHeight(1) / 2);
        }

        public void GetTranslationInput(float deltaTime)
        {

            //Gets the forward and side inputs of the player
            int forwardDirection = Convert.ToInt32(Raylib.IsKeyDown(KeyboardKey.KEY_W))
                - Convert.ToInt32(Raylib.IsKeyDown(KeyboardKey.KEY_S));
            int sideDirection = Convert.ToInt32(Raylib.IsKeyDown(KeyboardKey.KEY_A))
                - Convert.ToInt32(Raylib.IsKeyDown(KeyboardKey.KEY_D));

            Velocity = ((forwardDirection * Forward) + (sideDirection * Right)).Normalized * Speed * deltaTime + new Vector3(0, Velocity.Y, 0);
            ApplyGravity();


            base.Translate(Velocity.X, Velocity.Y, Velocity.Z);
        }

        public void GetDodgeInput(float deltaTime)
        {
            if (_dodgeTime > 1)
            {
                _dodgeTime = 0;
                _canDodge = true;
            }

            int forwardDirection = Convert.ToInt32(Raylib.IsKeyDown(KeyboardKey.KEY_W))
                - Convert.ToInt32(Raylib.IsKeyDown(KeyboardKey.KEY_S));
            int sideDirection = Convert.ToInt32(Raylib.IsKeyDown(KeyboardKey.KEY_A))
                - Convert.ToInt32(Raylib.IsKeyDown(KeyboardKey.KEY_D));

            if (_canDodge && (forwardDirection != 0 || sideDirection != 0) && Raylib.IsKeyDown(KeyboardKey.KEY_LEFT_SHIFT))
            {
                _canDodge = false;
                _dodgeCheck = false;
            }

            if (!_canDodge)
            {
                if (_dodgeTime >= 0 && _dodgeTime <= 0.25f)
                {
                    SetColor(new Color(0, 0, 100, 100));
                    if (_dodgeCheck == false)
                    {
                        Speed = 25;
                        _dodgeCheck = true;
                    }
                }
                if (_dodgeTime >= 0.25f)
                {
                    Speed = 6;
                    SetColor(new Color(0, 0, 100, 255));
                    if (_dodgeCheck == true)
                        _dodgeCheck = false;
                }
                _dodgeTime += deltaTime;
            }
        }

        public void TakeDamage()
        {
            Health--;
        }

        public override void OnCollision(Actor actor)
        {
            Console.WriteLine("Collision");

        }

        public override void Draw()
        {
            base.Draw();
            //Collider.Draw();
        }
    }
}