using System;
using System.Collections.Generic;
using System.Text;
using MathLibrary;
using Raylib_cs;

namespace _3dEngine
{
    class AABBCollider : Collider
    {
        private float _width;
        private float _height;
        private float _length;

        public AABBCollider(float width, float height, float length, Actor owner) : base(owner, ColliderType.AABB)
        {
            _width = width;
            _height = height;
            _length = length;
        }

        //Checking the collisisons for boxes. :)
        public override bool CheckCollisionAABB(AABBCollider other)
        {
            //If the object is overlapping itself, return false.
            if (other.Owner == Owner)
                return false;
            //Return true if there is an overlap between boxes.
            if (other.Left <= Right &&
                other.Top <= Bottom &&
                Left <= other.Right &&
                Top <= other.Bottom &&
                other.Front <= Back &&
                Front <= other.Back)
                return true;
            return false;
        }

        public override bool CheckCollisionSphere(SphereCollider other)
        {
            return other.CheckCollisionAABB(this);
        }

        //Width of the box collision
        public float Width
        {
            get { return _width; }
            set { _width = value; }
        }

        //Height of the box collision
        public float Height
        {
            get { return _height; }
            set { _height = value; }
        }

        public float Length
        {
            get { return _length; }
            set { _length = value; }
        }

        //The left face of the box
        public float Left
        {
            get
            {
                return Owner.WorldPosition.X - (Width / 2);
            }
        }

        //The right face of the box
        public float Right
        {
            get
            {
                return Owner.WorldPosition.X + (Width / 2);
            }
        }

        //The top face of the box
        public float Top
        {
            get
            {
                return Owner.WorldPosition.Y - (Height / 2);
            }
        }

        //The bottom face of the box
        public float Bottom
        {
            get
            {
                return Owner.WorldPosition.Y + (Height / 2);
            }
        }

        //The front face of the box
        public float Front
        {
            get
            {
                return Owner.WorldPosition.Z - (Length / 2);
            }
        }

        //The back face of the box
        public float Back
        {
            get
            {
                return Owner.WorldPosition.Z + (Length / 2);
            }
        }

        public override void Draw() 
        {
            Raylib.DrawCube(new System.Numerics.Vector3(base.Owner.WorldPosition.X, base.Owner.WorldPosition.Y, base.Owner.WorldPosition.Z), _width, _height, _length, new Color(255, 0, 255, 100));
        }
    }
}
