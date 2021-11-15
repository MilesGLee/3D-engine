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
        //Variables used
        private static bool _applicationShouldClose = false;
        private static int _currentSceneIndex;
        private Scene[] _scenes = new Scene[0];
        private Stopwatch _stopwatch = new Stopwatch();
        public static Scene CurrentScene;
        public static Camera Camera;
        private UIText text;
        private Actor player;

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
            //starts the delta time pretty much
            _stopwatch.Start();

            //Sets up the window size
            InitializeWindow();
            //New scene created
            Scene scene = new Scene();

            //Creates the player actor and their collision actor
            player = new Player(0, 1, 0, 6, 3, new Color(0, 0, 100, 255), "Player", Shape.SPHERE);
            SphereCollider playerCol = new SphereCollider(1, player);
            player.Collider = playerCol;
            //The camera for the player
            Engine.Camera = new Camera(player);

            //The HUD text
            text = new UIText(50, 50, "test", new Color(255, 255, 255, 255), 1000, 1000, 100, "");

            //Both boxes spawned in to provide collision detection
            Actor Wall1 = new Actor(new Vector3(0, 2.5f, -5), Shape.CUBE, new Color(255, 255, 255, 255), "Wall");
            Wall1.SetScale(5, 5, 5);
            AABBCollider wallCol = new AABBCollider(5, 5, 5, Wall1);
            Wall1.Collider = wallCol;
            Actor Wall2 = new Actor(new Vector3(5, 2.5f, 0), Shape.CUBE, new Color(255, 255, 255, 255), "Wall");
            Wall2.SetScale(5, 5, 5);
            AABBCollider wall2Col = new AABBCollider(5, 5, 5, Wall2);
            Wall2.Collider = wall2Col;

            //adds all the actors to the scene
            scene.AddUIElement(text);
            scene.AddActor(player);
            scene.AddActor(Camera);
            scene.AddActor(Wall1);
            scene.AddActor(Wall2);

            //Starts the scene
            SetCurrentScene(scene);
            _scenes[_currentSceneIndex].Start();
        }

        /// <summary>
        /// Called everytime the game loops
        /// </summary>
        private void Update(float deltaTime)
        {
            //Toggle fullscreen
            if (Raylib.IsKeyPressed(KeyboardKey.KEY_Q))
                Raylib.ToggleFullscreen();
            //Update actors and UI elements
            _scenes[_currentSceneIndex].Update(deltaTime);
            _scenes[_currentSceneIndex].UpdateUI(deltaTime);

            //update the hud's text to display the players position
            text.Text = "" + (String.Format("{0:0}", player.WorldPosition.X)) + "/" + (String.Format("{0:0}", player.WorldPosition.Y)) + "/" + (String.Format("{0:0}", player.WorldPosition.Z));

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

            //Background and grid to be drawn.
            Raylib.ClearBackground(new Color(150, 150, 255, 255));
            Raylib.DrawGrid(25, 10);

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
            //Window setup
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