using System;
using System.Collections.Generic;
using System.Text;
using MathLibrary;
using Raylib_cs;

namespace _3dEngine
{
    class SphereCollider : Collider
    {
        private float _collisionRadius;

        public float CollisionRadius
        {
            get { return _collisionRadius; }
            set { _collisionRadius = value; }
        }
        public SphereCollider(float collisionRadius, Actor owner) : base(owner, ColliderType.SPHERE)
        {
            _collisionRadius = collisionRadius;
        }

        //Checking for a collision between Spheres.
        public override bool CheckCollisionSphere(SphereCollider other)
        {
            if (other.Owner == Owner) //If the colliding Sphere is not the owner of this Sphere.
                return false;
            float distance = Vector3.Distance(other.Owner.WorldPosition, Owner.WorldPosition); //Distance between both Spheres
            float combinedRadii = other.CollisionRadius + CollisionRadius; //The combined radii of the Spheres

            return distance <= combinedRadii; //if the distance is less than the combined radii, a collision occured.
        }

        public override bool CheckCollisionAABB(AABBCollider other)
        {
            //return false if colliding with itself
            if (other.Owner == Owner)
                return false;

            //Get and clamp the direction from the collider to the aabb
            Vector3 direction = Owner.WorldPosition - other.Owner.WorldPosition;
            direction.X = Math.Clamp(direction.X, -other.Width / 2, other.Width / 2);
            direction.Y = Math.Clamp(direction.Y, -other.Height / 2, other.Height / 2);
            direction.Z = Math.Clamp(direction.Z, -other.Length / 2, other.Length / 2);

            //find the closest point between the AABB and the collider
            Vector3 closestPoint = other.Owner.WorldPosition + direction;
            float distanceFromClosestPoint = Vector3.Distance(Owner.WorldPosition, closestPoint);

            //Return true if the colliders are colliding.
            return distanceFromClosestPoint <= CollisionRadius;
        }

        public override void Draw()
        {
            Raylib.DrawSphere(new System.Numerics.Vector3(base.Owner.WorldPosition.X, base.Owner.WorldPosition.Y, base.Owner.WorldPosition.Z), _collisionRadius, new Color(255, 0, 255, 100));
        }
    }
}
