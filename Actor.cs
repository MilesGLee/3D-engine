using System;
using System.Collections.Generic;
using System.Text;
using MathLibrary;
using Raylib_cs;

namespace _3dEngine
{
    public enum Shape 
    {
        CUBE,
        SPHERE
    }

    class Actor
    {
        private string _name;
        private bool _started;
        private float _speed;
        private Vector3 _forward = new Vector3(0, 0, 1);
        private Matrix4 _localTransform = Matrix4.Identity;
        private Matrix4 _globalTransform = Matrix4.Identity;
        public Matrix4 _translation = Matrix4.Identity;
        public Matrix4 _rotation = Matrix4.Identity;
        public Matrix4 _scale = Matrix4.Identity;
        private Actor[] _children = new Actor[0];
        private Actor _parent;
        private Shape _shape;

        public bool Started 
        {
            get { return _started; }
        }

        public string Name 
        {
            get { return _name; }
        }

        public float Speed 
        {
            get { return _speed; }
        }

        public Vector3 LocalPosition 
        {
            get { return new Vector3(_localTransform.M03, _localTransform.M13, _localTransform.M23); }
            set { _localTransform.M03 = value.X; _localTransform.M13 = value.Y; _localTransform.M23 = value.Z; }
        }

        public Vector3 WorldPosition 
        {
            get { return new Vector3(_globalTransform.M03, _globalTransform.M13, _globalTransform.M23); }
            set 
            {
                if (Parent != null)
                {
                    float xOffset = (value.X - Parent.WorldPosition.X) / new Vector3(_globalTransform.M00, _globalTransform.M10, _globalTransform.M20).Magnitude;
                    float yOffset = (value.Y - Parent.WorldPosition.Y) / new Vector3(_globalTransform.M01, _globalTransform.M11, _globalTransform.M21).Magnitude;
                    float zOffset = (value.Z - Parent.WorldPosition.Z) / new Vector3(_globalTransform.M02, _globalTransform.M12, _globalTransform.M22).Magnitude;
                    SetTranslate(xOffset, yOffset, zOffset);
                }
                else
                    LocalPosition = value;
            }
        }

        public Matrix4 GlobalTransform
        {
            get { return _globalTransform; }
            set { _globalTransform = value; }
        }

        public Matrix4 LocalTransform
        {
            get { return _localTransform; }
            private set { _localTransform = value; }
        }

        public Actor Parent
        {
            get { return _parent; }
            set { _parent = value; }
        }

        public Actor[] Children
        {
            get { return _children; }
        }

        public Vector3 Size
        {
            get
            {
                float xScale = new Vector3(_scale.M00, _scale.M10, _scale.M20).Magnitude;
                float yScale = new Vector3(_scale.M01, _scale.M11, _scale.M21).Magnitude;
                float zScale = new Vector3(_scale.M02, _scale.M12, _scale.M22).Magnitude;
                return new Vector3(xScale, yScale, zScale);
            }
            set { SetScale(value.X, value.Y, value.Z); }
        }

        public Vector3 Forward
        {
            get { return new Vector3(_rotation.M00, _rotation.M10, _rotation.M20); }
            set
            {
                Vector3 point = value.Normalized + LocalPosition;
                LookAt(point);
            }
        }

        public Actor() { }

        public Actor(float x, float y, float speed, string name = "Actor", Shape shape = Shape.CUBE) :
            this(new Vector3 { X = x, Y = y}, speed, name, shape) { }

        public Actor(Vector3 position, float speed, string name = "Actor", Shape shape = Shape.CUBE)
        {
            LocalPosition = position;
            _name = name;
            _shape = shape;
        }

        public void UpdateTransforms()
        {
            _localTransform = _translation * _rotation * _scale;

            if (Parent != null)
                GlobalTransform = Parent.GlobalTransform * LocalTransform;
            else
                GlobalTransform = LocalTransform;
        }

        public void AddChild(Actor child)
        {
            Actor[] temArray = new Actor[_children.Length + 1];

            for (int i = 0; i < _children.Length; i++)
            {
                temArray[i] = _children[i];
            }

            temArray[_children.Length] = child;

            child.Parent = this;

            _children = temArray;
        }

        public bool RemoveChild(Actor child)
        {
            bool actorRemoved = false;

            Actor[] temArray = new Actor[_children.Length - 1];

            int j = 0;

            for (int i = 0; i < _children.Length; i++)
            {
                if (_children[i] != child)
                {
                    temArray[j] = _children[i];
                    j++;
                }
                else
                    actorRemoved = true;

            }

            if (actorRemoved)
            {
                _children = temArray;
                child.Parent = null;
            }

            return actorRemoved;
        }

        public virtual void Start()
        {
            _started = true;
            if (Parent != null)
            {
                Parent.AddChild(this);
                _scale = ((_scale * Parent._scale));
            }
        }

        public virtual void Update(float deltaTime)
        {
            UpdateTransforms();

            Console.WriteLine(_name + ":" + LocalPosition.X + ":" + LocalPosition.Y + ":" + LocalPosition.Z);

        }

        public virtual void Draw() 
        {
            System.Numerics.Vector3 position = new System.Numerics.Vector3(WorldPosition.X, WorldPosition.Y, WorldPosition.Z);

            switch (_shape) 
            {
                case Shape.CUBE:
                    float sizeX = new Vector3(GlobalTransform.M00, GlobalTransform.M10, GlobalTransform.M20).Magnitude;
                    float sizeY = new Vector3(GlobalTransform.M01, GlobalTransform.M11, GlobalTransform.M21).Magnitude;
                    float sizeZ = new Vector3(GlobalTransform.M02, GlobalTransform.M12, GlobalTransform.M22).Magnitude;
                    Raylib.DrawCube(position, sizeX, sizeY, sizeZ, Color.BLUE);
                    break;
                case Shape.SPHERE:
                    sizeX = new Vector3(GlobalTransform.M00, GlobalTransform.M10, GlobalTransform.M20).Magnitude;
                    Raylib.DrawSphere(position, sizeX, Color.BLUE);
                    break;
            }
        }

        public void End()
        {

        }

        public virtual void OnCollision(Actor actor)
        {
            
        }

        public virtual bool CheckForCollision(Actor actor)
        {
            return false;
        }

        public void SetTranslate(float translationX, float translationY, float translationZ)
        {
            _translation = Matrix4.CreateTranslation(translationX, translationY, translationZ);
        }

        public void Translate(float translationX, float translationY, float translationZ)
        {
            _translation *= Matrix4.CreateTranslation(translationX, translationY, translationZ);
        }

        public void SetRotation(float radiansX, float radiansY, float radiansZ)
        {
            Matrix4 rotX = Matrix4.CreateRotationX(radiansX);
            Matrix4 rotY = Matrix4.CreateRotationY(radiansY);
            Matrix4 rotZ = Matrix4.CreateRotationZ(radiansZ);
            _rotation = rotX * rotY * rotZ;
        }
        public void Rotate(float radiansX, float radiansY, float radiansZ)
        {
            Matrix4 rotX = Matrix4.CreateRotationX(radiansX);
            Matrix4 rotY = Matrix4.CreateRotationY(radiansY);
            Matrix4 rotZ = Matrix4.CreateRotationZ(radiansZ);
            _rotation = rotX * rotY * rotZ;
        }

        public void SetScale(float x, float y, float z)
        {
            _scale = Matrix4.CreateScale(x, y, z);
        }
        public void Scale(float x, float y, float z)
        {
            _scale *= Matrix4.CreateScale(x, y, z);
        }

        public void LookAt(Vector3 position)
        {
            //Vector3 direction = (position - LocalPosition).Normalized;
            //float dotProduct = Vector3.DotProduct(direction, Forward);

            //if (dotProduct > 1)
            //    dotProduct = 1;

            //float angle = (float)Math.Acos(dotProduct);
            //Vector3 perpendicularDirection = new Vector3(direction.Y, -direction.X, direction.Z);
            //float perpendicularDotProduct = Vector3.DotProduct(perpendicularDirection, Forward);

            //if (perpendicularDotProduct != 0)
            //    angle *= -perpendicularDotProduct / Math.Abs(perpendicularDotProduct);

            //Rotate(angle);
        }
    }
}
