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
        public bool _started;
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
        private Color _color;

        public Color ShapeColor 
        {
            get { return _color; }
        }

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
            get { return new Vector3(_rotation.M02, _rotation.M12, _rotation.M22); }
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
            System.Numerics.Vector3 endPos = new System.Numerics.Vector3(WorldPosition.X + Forward.X * 50, WorldPosition.Y + Forward.Y * 50, WorldPosition.Z + Forward.Z * 50);
            switch (_shape) 
            {
                case Shape.CUBE:
                    float sizeX = new Vector3(GlobalTransform.M00, GlobalTransform.M10, GlobalTransform.M20).Magnitude;
                    float sizeY = new Vector3(GlobalTransform.M01, GlobalTransform.M11, GlobalTransform.M21).Magnitude;
                    float sizeZ = new Vector3(GlobalTransform.M02, GlobalTransform.M12, GlobalTransform.M22).Magnitude;
                    Raylib.DrawCube(position, sizeX, sizeY, sizeZ, ShapeColor);
                    break;
                case Shape.SPHERE:
                    sizeX = new Vector3(GlobalTransform.M00, GlobalTransform.M10, GlobalTransform.M20).Magnitude;
                    Raylib.DrawSphere(position, sizeX, ShapeColor);
                    break;
            }
            Raylib.DrawLine3D(position, endPos, Color.RED);
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
            //Get the direction for the actor to look in
            Vector3 direction = (position - WorldPosition).Normalized;

            //If the direction has a length of zero...
            if (direction.Magnitude == 0)
                ///... Set it to be the default forward
                direction = new Vector3(0, 0, 1);
            //Create a vector that points directly upwards
            Vector3 alignAxis = new Vector3(0, 1, 0);
            //Creates two new vectors that will be the new x and y axis
            Vector3 newYAxis = new Vector3(0, 1, 0);
            Vector3 newXAxis = new Vector3(1, 0, 0);

            //if the direction vector is parallel to the align axis vector...
            if (Math.Abs(direction.Y) > 0 && direction.X == 0 && direction.Z == 0)
            {
                //... set the align axis vector to point to the right
                alignAxis = new Vector3(1, 0, 0);


                //Get the cross product of the direction and the right to find the y axis
                newYAxis = Vector3.CrossProduct(direction, alignAxis);
                //normalize the new y axis to prevent the matrix from being scaled
                newYAxis.Normalize();

                //Get the cross product of the new y axis and the direction to find the new x axis
                newXAxis = Vector3.CrossProduct(newYAxis, direction);
                //Normalize new x axis to prevent the matrix from being scaled
                newXAxis.Normalize();
            }
            //if the direction vector is not parallel
            else 
            {
                //get the cross product of the align axis and direction vector
                newXAxis = Vector3.CrossProduct(alignAxis, direction);
                //normalize the new x axis to prevent the matrix from being scaled
                newXAxis.Normalize();
                //get the cross product of the direction and new x axis vectors
                newYAxis = Vector3.CrossProduct(direction, newXAxis);
                //normalize the new y axis to prevent the matrix from being scaled
                newYAxis.Normalize();
            }
            //create new matrix with the new axis
            _rotation = new Matrix4(newXAxis.X, newYAxis.X, direction.X, 0,
                                    newXAxis.Y, newYAxis.Y, direction.Y, 0,
                                    newXAxis.Z, newYAxis.Z, direction.Z, 0,
                                    0, 0, 0, 1);
        }

        public void SetColor(Color color) 
        {
            _color = color;
        }

        public void SetColor(Vector4 colorValue) 
        {
            _color = new Color((int)colorValue.X, (int)colorValue.Y, (int)colorValue.Z, (int)colorValue.W);
        }
    }
}
