using System;
using System.Collections.Generic;
using System.Text;
using MathLibrary;
using Raylib_cs;

namespace _3dEngine
{
    class Camera : Actor
    {
        private Camera3D _camera3D;
        private Actor _targetActor;

        public Camera3D Camera3D
        {
            get { return _camera3D; }
            set { _camera3D = value; }
        }

        public Camera(Actor targetActor)
            : base()
        {
            _camera3D = new Camera3D();
            _targetActor = targetActor;
            _targetActor.AddChild(this);
        }

        public override void Start()
        {
            // Camera position
            _camera3D.up = new System.Numerics.Vector3(0, 1, 0); //Camera up vector (rotation towards target)
            _camera3D.fovy = 45; // Camera field of view
            _camera3D.projection = CameraProjection.CAMERA_PERSPECTIVE; //Camera mode type

            //Sets the camera in a good position to view the player.
            SetTranslation(0, 5, -13);
        }

        public override void Update(float deltaTime)
        {
            // Camera position
            _camera3D.position = new System.Numerics.Vector3(WorldPosition.X, WorldPosition.Y, WorldPosition.Z);
            // Point the camera is focused on
            _camera3D.target = new System.Numerics.Vector3(_targetActor.WorldPosition.X, _targetActor.WorldPosition.Y, _targetActor.WorldPosition.Z);

            base.Update(deltaTime);
        }
    }
}
