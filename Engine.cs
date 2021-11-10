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
        public static Scene CurrentScene;
        public static Camera Camera;

        /// <summary>
        /// Called to begin the application
        /// </summary>
        public void Run()
        {
            //Call start for the entire application
            Start();

            float currentTime = 0;
            float lastTime = 0;
            float deltaTime = 0;

            //Loops until the application is told to close
            while (!_applicationShouldClose && !Raylib.WindowShouldClose())
            {
                //Get how much time has passed since the application started
                currentTime = _stopwatch.ElapsedMilliseconds / 1000.0f;

                //Set delta time to tbe the difference in time from the last time recorded to the current time
                deltaTime = currentTime - lastTime;

                //Update the application
                Update(deltaTime);
                //Draw all items
                Draw();

                //Set the last time recorded to be the current time
                lastTime = currentTime;
            }

            //Called when the application closes
            End();
        }


        /// <summary>
        /// Called when the application starts
        /// </summary>
        private void Start()
        {
            _stopwatch.Start();

            InitializeWindow();
            Scene.InitializeActors();
            Scene sceneOne = new Scene();
            sceneOne.AddActor(Scene.SceneOneActors);

            UIText text = new UIText(50, 50, "test", new Color(255, 255, 255, 255), 1000, 1000, 100, "Hello");

            sceneOne.AddUIElement(text);

            SetCurrentScene(sceneOne);
            _scenes[_currentSceneIndex].Start();
        }

        /// <summary>
        /// Called everytime the game loops
        /// </summary>
        private void Update(float deltaTime)
        {
            if (Raylib.IsKeyPressed(KeyboardKey.KEY_Q))
                Raylib.ToggleFullscreen();
            _scenes[_currentSceneIndex].Update(deltaTime);
            _scenes[_currentSceneIndex].UpdateUI(deltaTime);



            while (Console.KeyAvailable)
                Console.ReadKey(true);
        }

        /// <summary>
        /// Called every time the game loops to update visuals
        /// </summary>
        private void Draw()
        {
            Raylib.BeginDrawing();
            Raylib.BeginMode3D(Camera.Camera3D);

            Raylib.ClearBackground(new Color(100, 0, 0, 255));
            Raylib.DrawGrid(500, 20);

            //Adds all actor icons to buffer
            _scenes[_currentSceneIndex].Draw();
            
            Raylib.EndMode3D();

            _scenes[_currentSceneIndex].DrawUI();

            Raylib.EndDrawing();
        }

        /// <summary>
        /// Called when the application exits
        /// </summary>
        private void End()
        {
            _scenes[_currentSceneIndex].End();
            Raylib.CloseWindow();
        }

        /// <summary>
        /// Adds a scene to the engine's scene array
        /// </summary>
        /// <param name="scene">The scene that will be added to the scene array</param>
        /// <returns>The index where the new scene is located</returns>
        public int AddScene(Scene scene)
        {
            //Create a new temporary array
            Scene[] tempArray = new Scene[_scenes.Length + 1];

            //Copy all the values from the old array into the new array
            for (int i = 0; i < _scenes.Length; i++)
                tempArray[i] = _scenes[i];

            //Set the last index to be the new scene
            tempArray[_scenes.Length] = scene;

            //Set the old array to be the new array
            _scenes = tempArray;

            //Return the last index
            return _scenes.Length - 1;
        }

        public void SetCurrentScene(Scene scene)
        {
            _currentSceneIndex = AddScene(scene);
            CurrentScene = _scenes[_currentSceneIndex];
        }

        public void InitializeWindow()
        {
            int height = Raylib.GetMonitorHeight(1);
            int width = Raylib.GetMonitorWidth(1);
            //Create a window using rayLib
            Raylib.InitWindow(width, height, "Math For Games");
            Raylib.DisableCursor();
            Raylib.MaximizeWindow();
            Raylib.SetTargetFPS(60);
        }

        /// <summary>
        /// A function that can be used globally to end the application
        /// </summary>
        public static void CloseApplication()
        {
            _applicationShouldClose = true;
        }
    }
}