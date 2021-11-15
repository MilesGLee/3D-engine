using System;
using System.Collections.Generic;
using System.Text;
using MathLibrary;
using Raylib_cs;

namespace _3dEngine
{
    class Player : Actor
    {
        //Movement and collision variables
        private float _speed;
        private Vector3 _velocity;
        private bool collidingWithWall;

        //Mouse Variables
        public float mouseXSensitivity = 2;
        public float mouseYSensitivity = 1;
        private Vector2 mouseOrigin = new Vector2(Raylib.GetMonitorWidth(1) / 2, Raylib.GetMonitorHeight(1) / 2);

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

        public Player(float x, float y, float z, float speed, int health, Color color, string name = "Player", Shape shape = Shape.CUBE)
            : base(x, y, z, shape, color, name)
        {
            Speed = speed;
            SetScale(1, 1, 1);
        }

        public override void Start()
        {
            base.Start();
        }

        public override void Update(float deltaTime)
        {
            //Write player position in console window
            Console.WriteLine($"{WorldPosition.X}/{WorldPosition.Y}/{WorldPosition.Z}");
            //Update rotation and movement.
            GetTranslationInput(deltaTime);
            GetRotationInput(deltaTime);
            base.Update(deltaTime);
            collidingWithWall = false;
        }

        public void GetRotationInput(float deltaTime)
        {
            //get mouse position onscreen and calculate the mousedelta.
            Vector2 mousePosition = new Vector2(Raylib.GetMouseX(), Raylib.GetMouseY());
            Vector2 mouseDelta = mousePosition - mouseOrigin;

            //Get the angle to rotate
            float angle = MathF.Atan2(mouseDelta.Y, mouseDelta.X);

            //if there is an angle to rotate too, rotate the base actor.
            if (mouseDelta.Magnitude > 0)
                base.Rotate((MathF.Sin(angle) * deltaTime) * mouseYSensitivity, (-MathF.Cos(angle) * deltaTime) * mouseXSensitivity, 0);
            //Move cursor back to middle of screen to keep it in check.
            Raylib.SetMousePosition(Raylib.GetMonitorWidth(1) / 2, Raylib.GetMonitorHeight(1) / 2);
        }

        public void GetTranslationInput(float deltaTime)
        {

            //Gets the forward and side inputs of the player
            int forwardDirection = Convert.ToInt32(Raylib.IsKeyDown(KeyboardKey.KEY_W))
                - Convert.ToInt32(Raylib.IsKeyDown(KeyboardKey.KEY_S));
            int sideDirection = Convert.ToInt32(Raylib.IsKeyDown(KeyboardKey.KEY_A))
                - Convert.ToInt32(Raylib.IsKeyDown(KeyboardKey.KEY_D));
            //if the player is not colliding with any obstacle. Calculate the velocity of their movement.
            if(!collidingWithWall)
                Velocity = ((forwardDirection * Forward) + (sideDirection * Right)).Normalized * Speed * deltaTime;
            //Move player.
            base.Translate(Velocity.X, 0, Velocity.Z);
        }

        public override void OnCollision(Actor actor)
        {
            //if the player collides with a wall, make sure they cant move into or through it.
            if (actor.Name == "Wall") 
            {
                collidingWithWall = true;
                Velocity = -Velocity;
            }
        }

        //Draw
        public override void Draw()
        {
            //Draw
            base.Draw(); //Draw
        }
    }
}