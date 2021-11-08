using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Diagnostics;
using MathLibrary;
using Raylib_cs;

namespace _3dEngine
{
    class Engine
    {
        private static bool _applicationShouldClose = false;
        private static int _currentSceneIndex;
        private Scene[] _scenes = new Scene[0];
        private Stopwatch _stopwatch = new Stopwatch();
        public static Scene _currentScene;
        private Camera3D _camera = new Camera3D();
        private Player _player = new Player(0, 0, 5, "Player", Shape.SPHERE);
        private Actor _cameraActor = new Actor(new Vector3(0, 5, 2), 0);

        public void Run() 
        {
            Start();

            float currentTime = 0;
            float lastTime = 0;
            float deltaTime = 0;

            while (!_applicationShouldClose || Raylib.WindowShouldClose()) 
            {
                currentTime = _stopwatch.ElapsedMilliseconds / 1000.0f;

                deltaTime = currentTime - lastTime;

                Update(deltaTime);

                Draw();

                lastTime = currentTime;
            }

            End();
        }

        private void Start() 
        {
            Raylib.InitWindow(800, 450, "3d Engine");
            Raylib.SetTargetFPS(60);
            

            Scene scene = new Scene();
            _player.Translate(0, 1, 0);
            _player.SetScale(1, 1, 1);
            _player.LookAt(new Vector3(1, 0, 0));
            _player.SetColor(new Vector4(52, 5, 12, 255));

            _cameraActor.Parent = _player;
            _cameraActor.SetTranslate(0, 5, -10);

            scene.AddActor(_cameraActor);
            scene.AddActor(_player);

            _stopwatch.Start();

            _currentSceneIndex = AddScene(scene);
            _scenes[_currentSceneIndex].Start();
            _currentScene = _scenes[_currentSceneIndex];

            InitializeCamera();
        }

        private void InitializeCamera() 
        {
            //_camera.position = new System.Numerics.Vector3(0, 10, 10); //Camera Position
            _camera.target = new System.Numerics.Vector3(0, 0, 0); //Point the Camera is focused on
            _camera.target = new System.Numerics.Vector3(0, 0, 0); //Point the Camera is focused on
            _camera.up = new System.Numerics.Vector3(0, 1, 0); //Camera up vector
            _camera.fovy = 45; //Camera field of view
            _camera.projection = CameraProjection.CAMERA_PERSPECTIVE; //Camera mode type
        }

        private void Update(float deltaTime) 
        {
            _scenes[_currentSceneIndex].Update(deltaTime);
            _camera.position = new System.Numerics.Vector3(_cameraActor.WorldPosition.X, _cameraActor.WorldPosition.Y, _cameraActor.WorldPosition.Z);
            _camera.target = new System.Numerics.Vector3(_player.WorldPosition.X, _player.WorldPosition.Y, _player.WorldPosition.Z);
            while (Console.KeyAvailable)
                Console.ReadKey(true);
        }

        private void Draw() 
        {
            Raylib.BeginDrawing();
            Raylib.BeginMode3D(_camera);

            Raylib.ClearBackground(Color.GRAY);
            Raylib.DrawGrid(50, 1);

            _currentScene.Draw();

            Raylib.EndMode3D();
            Raylib.EndDrawing();
        }

        private void End()
        {
            _scenes[_currentSceneIndex].End();
            Raylib.CloseWindow();
        }

        public int AddScene(Scene scene)
        {
            Scene[] tempArray = new Scene[_scenes.Length + 1];

            for (int i = 0; i < _scenes.Length; i++)
            {
                tempArray[i] = _scenes[i];
            }

            tempArray[_scenes.Length] = scene;

            _scenes = tempArray;

            return _scenes.Length - 1;
        }

        public static ConsoleKey GetNewtKey()
        {
            if (!Console.KeyAvailable)
                return 0;
            return Console.ReadKey(true).Key;
        }

        public static void CloseApplication()
        {
            _applicationShouldClose = true;
        }
    }
}
