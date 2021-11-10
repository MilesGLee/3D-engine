using System;
using System.Collections.Generic;
using System.Text;
using MathLibrary;
using Raylib_cs;

namespace _3dEngine
{
    public enum ActorTag
    {
        PLAYER,
        ENEMY,
        BULLET,
        GENERIC
    }

    public enum Shape
    {
        CUBE,
        SPHERE,
        NULL
    }

    class Actor
    {
        private string _name;
        private bool _started;
        /// <summary>
        /// The forward facing direction of the actor
        /// </summary>
        private ActorTag _tag;
        private Collider _collider;
        private Matrix4 _globalTransform = Matrix4.Identity;
        private Matrix4 _localTransform = Matrix4.Identity;
        private Matrix4 _translation = Matrix4.Identity;
        private Matrix4 _rotation = Matrix4.Identity;
        private Matrix4 _scale = Matrix4.Identity;
        private Actor[] _children = new Actor[0];
        private Actor _parent;
        private Shape _shape;
        private Color _color;


        //The collider attached to this actor
        public Collider Collider
        {
            get { return _collider; }
            set { _collider = value; }
        }

        public Color ShapeColor
        {
            get { return _color; }
        }

        public ActorTag Tag
        {
            get { return _tag; }
            set { _tag = value; }
        }

        public Vector3 Forward
        {
            get { return new Vector3(_rotation.M02, _rotation.M12, _rotation.M22); }
        }

        public Vector3 Right
        {
            get { return new Vector3(_rotation.M00, _rotation.M10, _rotation.M20); }
        }

        public Vector3 Upwards
        {
            get { return new Vector3(_rotation.M01, _rotation.M11, _rotation.M21); }
        }

        public float ScaleX
        {
            get { return new Vector3(_scale.M00, _scale.M10, _scale.M20).Magnitude; }
        }

        public float ScaleY
        {
            get { return new Vector3(_scale.M01, _scale.M11, _scale.M12).Magnitude; }
        }

        public float ScaleZ
        {
            get { return new Vector3(_scale.M02, _scale.M12, _scale.M22).Magnitude; }
        }

        /// <summary>
        /// True if the start function has been called for this actor
        /// </summary>
        public bool Started
        {
            get { return _started; }
        }

        public Vector3 LocalPosition
        {
            get { return new Vector3(_translation.M03, _translation.M13, _translation.M23); }
            set { SetTranslation(value.X, value.Y, value.Z); }
        }

        public Vector3 WorldPosition
        {
            //Return the global transform's T column
            get { return new Vector3(_globalTransform.M03, _globalTransform.M13, _globalTransform.M23); }
            set
            {
                //If the parent has a parent...
                if (Parent != null)
                {
                    //...convert the world cooridinates into local coordinates and translate the actor
                    Vector3 offset = value - Parent.WorldPosition;
                    SetTranslation(offset.X / Parent.ScaleX, offset.Y / Parent.ScaleY, offset.Z / Parent.ScaleZ);
                }
                //If this actor doesn't have a parent...
                else
                    //...set local position to be the given value
                    SetTranslation(value.X, value.Y, value.Z);
            }
        }

        public Matrix4 GlobalTransform
        {
            get { return _globalTransform; }
            private set { _globalTransform = value; }
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

        public Actor(float x, float y, float z, Shape shape, Color color, string name = "Actor", ActorTag tag = ActorTag.GENERIC) :
            this(new Vector3 { X = x, Y = y, Z = z }, shape, color, name, tag)
        {
        }

        public Actor(Vector3 position, Shape shape, Color color, string name = "Actor", ActorTag tag = ActorTag.GENERIC)
        {
            SetColor(color);
            LocalPosition = position;
            _name = name;
            Tag = tag;
            _shape = shape;
        }

        public Actor()
        {
            _shape = Shape.NULL;
        }

        public void UpdateTransforms()
        {
            //Updates the local transform by combining the seperate transforms
            _localTransform = _translation * _rotation * _scale;

            //If the actor has a parent...
            if (Parent != null)
                //...Set the actor's global transform to be the parent's global transform combined with the local transform
                GlobalTransform = Parent.GlobalTransform * LocalTransform;
            //If the parent doesn't have a parent, set the global transform to be equal to the local transform
            else GlobalTransform = LocalTransform;
        }

        /// <summary>
        /// Adds an actor the to actor's children array
        /// </summary>
        /// <param name="child">The actor that will be added as a child</param>
        public void AddChild(Actor child)
        {
            //Creates an array that is one element larger than our current children array
            Actor[] tempArray = new Actor[_children.Length + 1];

            //Copies the value of the children array into the temp array
            for (int i = 0; i < _children.Length; i++)
            {
                tempArray[i] = _children[i];
            }

            //Sets the temp array's last index to be equal to the actor that we are adding as a child
            tempArray[_children.Length] = child;
            //Set the children array to the temp array
            _children = tempArray;

            //Change the child's parent variable to store this actor
            child.Parent = this;
        }

        /// <summary>
        /// Removes an actor from the children array
        /// </summary>
        /// <param name="child">The actor that you are trying to remove</param>
        /// <returns>True if a child is removed</returns>
        public bool RemoveChild(Actor child)
        {
            //Creates a bool variable to store whether or not an actor was removed
            //Creates a new array that is one element smaller than our current children array
            bool removedActor = false;
            Actor[] tempArray = new Actor[_children.Length - 1];

            //Loops through the array, setting each of the children from children array into the temp array, except for the actor that is being removed
            int j = 0;
            for (int i = 0; i < _children.Length; i++)
            {
                if (_children[i] != child)
                {
                    tempArray[j] = _children[i];
                    j++;
                }
                else
                {
                    child.Parent = null;
                    removedActor = true;
                    j++;
                }
            }

            //Sets the children array equal to the temp array
            _children = tempArray;
            //Returns whether or not the actor was removed or not
            return removedActor;
        }

        public virtual void Start()
        {
            _started = true;
        }

        public virtual void Update(float deltaTime)
        {
            UpdateTransforms();
        }

        public virtual void Draw()
        {
            System.Numerics.Vector3 position = new System.Numerics.Vector3(WorldPosition.X, WorldPosition.Y, WorldPosition.Z);

            //System.Numerics.Vector3 endPos = new System.Numerics.Vector3(WorldPosition.X + Forward.X * 50, WorldPosition.Y + Forward.Y * 50, WorldPosition.Z + Forward.Z * 50);

            switch (_shape)

            {
                case Shape.CUBE:
                    Raylib.DrawCube(position, ScaleX, ScaleY, ScaleZ, ShapeColor);
                    break;
                case Shape.SPHERE:
                    Raylib.DrawSphere(position, ScaleX, ShapeColor);
                    //Raylib.DrawLine3D(position, endPos, Color.RED);
                    break;
                case Shape.NULL:
                    break;
            }
        }

        public virtual void End()
        { }

        public void DestroySelf()
        {
            Engine.CurrentScene.RemoveActor(this);
        }

        public virtual void OnCollision(Actor actor)
        {

        }

        /// <summary>
        /// Checks if this actor collided with another actor
        /// </summary>
        /// <param name="other"The actor to check for a collision against></param>
        /// <returns>True if a collision has occured</returns>
        public virtual bool CheckForCollision(Actor other)
        {
            //Return false if either actor doesn't have a collider
            if (Collider == null || other.Collider == null)
                return false;

            return Collider.CheckCollision(other);
        }

        /// <summary>
        /// Sets the position of the actor
        /// </summary>
        /// <param name="translationX">The new x position</param>
        /// <param name="translationY">The new y position</param>
        public void SetTranslation(float translationX, float translationY, float translationZ)
        {
            _translation = Matrix4.CreateTranslation(translationX, translationY, translationZ);
        }

        /// <summary>
        /// Applies the given values to the current translation
        /// </summary>
        /// <param name="translationX">The amount to move the x</param>
        /// <param name="translationY">The amount to move the y</param>
        public void Translate(float translationX, float translationY, float translationZ)
        {
            _translation *= Matrix4.CreateTranslation(translationX, translationY, translationZ);
        }

        /// <summary>
        /// Set the rotation of the actor.
        /// </summary>
        /// <param name="radians">The angle of the rotation in radians.</param>
        public void SetRotation(float radiansX, float radiansY, float radiansZ)
        {
            Matrix4 rotationX = Matrix4.CreateRotationX(radiansX);
            Matrix4 rotationY = Matrix4.CreateRotationY(radiansY);
            Matrix4 rotationZ = Matrix4.CreateRotationZ(radiansZ);

            _rotation = rotationX * rotationY * rotationZ;
        }

        /// <summary>
        /// Adds a rotation to the current transform's rotation.
        /// </summary>
        /// <param name="radians"></param>
        public void Rotate(float radiansX, float radiansY, float radiansZ)
        {

            Matrix4 rotationX = Matrix4.CreateRotationX(radiansX);
            Matrix4 rotationY = Matrix4.CreateRotationY(radiansY);
            Matrix4 rotationZ = Matrix4.CreateRotationZ(radiansZ);

            _rotation *= rotationX * rotationY * rotationZ;
        }

        /// <summary>
        /// Changes the scale of the actor
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void SetScale(float x, float y, float z)
        {
            _scale = Matrix4.CreateScale(x, y, z);
        }

        /// <summary>
        /// Scales the actor by the given amount.
        /// </summary>
        /// <param name="x">The value to scale on the x axis.</param>
        /// <param name="y">The value to scale on the y axis</param>
        public void Scale(float x, float y, float z)
        {
            _scale *= Matrix4.CreateScale(x, y, z);
        }

        /// <summary>
        /// Changes the actor's forward position to face the given position
        /// </summary>
        /// <param name="position">The position the actor will face</param>
        public void LookAt(Vector3 position)
        {
            //Find the direction that the actor should look in
            Vector3 direction = (position - LocalPosition).Normalized;

            //If the direction has a length of zero...
            if (direction.Magnitude == 0)
                //...set it tobe the default forward value
                direction = new Vector3(0, 0, 1);

            //Create a vector that points directly upwards
            Vector3 alignAxis = new Vector3(0, 1, 0);

            //Creates two new vectors that will be the new x and y axis
            Vector3 newYAxis = new Vector3(0, 1, 0);
            Vector3 newXAxis = new Vector3(1, 0, 0);

            //If the direciton vector is parallel to the alignAxis vector...
            if (Math.Abs(direction.Y) > 0 && direction.X == 0 && direction.Z == 0)
            {
                //Set the alignAxis vector to point directly to the right
                alignAxis = new Vector3(1, 0, 0);

                //Get the cross product of the direction and the right to find the new Y axis
                newYAxis = Vector3.CrossProduct(direction, alignAxis);
                newYAxis.Normalize(); //Normalize the new Y axis to prevent the matrix from being scaled

                //Get the cross product of the new y axis and the direction to find the new x axis
                newXAxis = Vector3.CrossProduct(newYAxis, direction);
                newXAxis.Normalize(); //Also normalize the new X Axis
            }
            else
            {
                //Get the cross product of upwards and the direction to find the new X axis
                newXAxis = Vector3.CrossProduct(alignAxis, direction);
                //Normalize the new X axis to prevent the matrix from being scaled
                newXAxis.Normalize();

                //Get the cross product of the direction and the new X axis to find the new Y axis
                newYAxis = Vector3.CrossProduct(direction, newXAxis);
                newYAxis.Normalize(); //Also normalize this axis
            }

            //Create a new matrix with the new axis
            _rotation = new Matrix4(
                newXAxis.X, newYAxis.X, direction.X, 0,
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