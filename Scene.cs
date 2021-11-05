using System;
using System.Collections.Generic;
using System.Text;

namespace _3dEngine
{
    class Scene
    {
        public static Actor[] _actors;

        public Scene() 
        {
            _actors = new Actor[0];
        }

        public virtual void Start() 
        {
            for (int i = 0; i < _actors.Length; i++)
                _actors[i].Start();
        }

        public virtual void Update(float deltaTime) 
        {
            for (int i = 0; i < _actors.Length; i++)
            {
                if (!_actors[i].Started)
                    _actors[i].Start();

                _actors[i].Update(deltaTime);

                if (i >= _actors.Length)
                    i--;
                for (int j = 0; j < _actors.Length; j++)
                {
                    if (j >= _actors.Length)
                        j--;
                    //if (_actors[i].Collider != null && _actors[i].Collider.CheckCollision(_actors[j]))
                    //{
                    //    _actors[j].OnCollision(_actors[i]);

                    //}
                }
            }
        }

        public virtual void Draw()
        {
            for (int i = 0; i < _actors.Length; i++)
                _actors[i].Draw();
        }

        public virtual void End()
        {
            for (int i = 0; i < _actors.Length; i++)
                _actors[i].End();
        }

        public void AddActor(Actor actor)
        {
            Actor[] temArray = new Actor[_actors.Length + 1];

            for (int i = 0; i < _actors.Length; i++)
            {
                temArray[i] = _actors[i];
            }

            temArray[_actors.Length] = actor;

            _actors = temArray;

        }

        public virtual bool RemoveActor(Actor actor)
        {
            bool actorRemoved = false;

            Actor[] temArray = new Actor[_actors.Length - 1];

            int j = 0;

            for (int i = 0; i < _actors.Length; i++)
            {
                if (_actors[i] != actor)
                {
                    temArray[j] = _actors[i];
                    j++;
                }
                else
                    actorRemoved = true;

            }

            if (actorRemoved)
                _actors = temArray;

            return actorRemoved;
        }
    }
}
