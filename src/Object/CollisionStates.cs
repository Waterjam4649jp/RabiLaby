using Godot;
using System.Collections.Generic;

namespace RabiLaby.src.Object
{
    internal class CollisionStates
    {
        public static string GetFloorType(CharacterBody2D _body) // List <KinematicBody2D> collisions
        {
            if (_body.GetSlideCollisionCount() == 0)
            {
                return "None";
            }

            for (int i = 0; i < _body.GetSlideCollisionCount(); i++)
            {
                KinematicCollision2D collision = _body.GetSlideCollision(i);
                var contactedObj = collision.GetCollider() as Node;

                for (int j = 0; j < GetObjectMap.Length(); j++)
                {
                    if (contactedObj.Name == GetObjectMap.GetObjectName(j))
                    {
                        if (contactedObj.Name == GetObjectMap.Character
                            && (collision.GetCollider() as CharacterBody2D).Position.Y > _body.Position.Y)
                            return contactedObj.GetParent().Name;
                        if (contactedObj.Name == GetObjectMap.Map)
                            return contactedObj.Name;
                    }
                }
            }

            return "Error";
        }

        public static List<string> GetFloorTypeList(CharacterBody2D _body, int count, float y)
        {
            var FloorTypes = new List<string>(count);

            if (_body.GetSlideCollisionCount() == 0)
            {
                return new List<string>() { "None" };
            }

            for (int i = 0; i < _body.GetSlideCollisionCount(); i++)
            {
                KinematicCollision2D collision = _body.GetSlideCollision(i);
                var contactedObj = collision.GetCollider() as Node;

                for (int j = 0; j < GetObjectMap.Length(); i++)
                {
                    if (contactedObj.Name == GetObjectMap.GetObjectName(i))
                    {
                        if (contactedObj.Name == GetObjectMap.Character)
                            FloorTypes.Add(contactedObj.GetParent().Name);
                        if (contactedObj.Name == GetObjectMap.Map)
                            FloorTypes.Add(contactedObj.Name);
                    }
                }

                if (FloorTypes != null && FloorTypes.Count > 0)
                    return FloorTypes;
            }
            return new List<string>() { "Error" };
        }
    }
}