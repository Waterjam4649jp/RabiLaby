using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;


namespace RabiLaby.src.PlayerController
{
    public static class GetKeyName
    {
        private static readonly Dictionary<string, string> KeyMap = new Dictionary<string, string>
    {
        { "jump", "move_jump" },
        { "right", "move_right" },
        { "left", "move_left" },
    };

        public static string jump => KeyMap["jump"];
        public static string right => KeyMap["right"];
        public static string left => KeyMap["left"];
    }

    public static class PlayerMovement
    {
        public static Vector2 velocity { get; set; }
        private static float Gravity { get; }
        private static int WalkSpeed { get; }
        private static int JumpForce { get; }
        private static int TerminalVelocity { get; }
        private static bool isOnFloor { get; }
        private static bool isControlled { get; }

        public static void Apply() // as main
        {
            velocity
            .PlayerController(WalkSpeed, JumpForce, isOnFloor, isControlled)
            .ApplyGravity(Gravity)
            .ApplyTerminalVelocity(TerminalVelocity);
        }


    }

    public static class VelocityController
    {

        public static Vector2 velocity { get; set; }

        public static Vector2 PlayerController(this Vector2 velocity, float walkSpeed, int JumpForce, bool isOnFloor, bool isControlled)
        {
            if (isOnFloor)
            {
                if (Input.IsActionJustPressed("jump"))
                {
                    velocity.Y = -JumpForce;
                }
            }

            // When inputting right and left, or no key, player waits
            if (isControlled)
            {
                if (Input.IsActionPressed(GetKeyName.left) && Input.IsActionPressed(GetKeyName.right))
                {
                    velocity.X = 0;
                    return velocity;
                }
                else if (Input.IsActionPressed(GetKeyName.left))
                {
                    velocity.X = -walkSpeed;
                    return velocity;
                }
                else if (Input.IsActionPressed(GetKeyName.right))
                {
                    velocity.X = walkSpeed;
                    return velocity;
                }
            }

            return velocity;
        }

        public static Vector2 ApplyGravity(this Vector2 velocity, float Gravity)
        {
            velocity.Y += Gravity;
            return velocity;
        }

        public static Vector2 ApplyTerminalVelocity(this Vector2 velocity, int TerminalVelocity)
        {
            if (velocity.Y > TerminalVelocity)
            {
                velocity.Y = TerminalVelocity;
            }
            return velocity;
        }
    }


    /*
    public List<string> GetFloorType(string[] ContactableObjNames)
    {
        var FloorTypes = new List<string>(10);

        if (GetSlideCollisionCount() == 0)
        {
            return new List<string>() { "None" };
        }

        for (int i = 0; i < GetSlideCollisionCount(); i++)
        {
            KinematicCollision2D collision = GetSlideCollision(i);
            var ContactedObj = collision.GetCollider();

            foreach (string ObjName in ContactableObjNames)
            {
                if ((ContactedObj as Node).Name == ObjName && !FloorTypes.Contains(ObjName)) // avoid overlapping
                {
                    switch(ObjName)
                    {
                        case "TileMapLayer":
                            FloorTypes.Add((ContactedObj as Node).Name);
                            break;
                        case "CharacterBody2D":
                            FloorTypes.Add((ContactedObj as Node).GetParent().Name);
                            break;
                        default:
                            FloorTypes.Add(ObjName);
                            break;
                    }
                }
            }
        }

        if (FloorTypes != null && FloorTypes.Count > 0)
            return FloorTypes;
        else
            return new List<string>() { "Error" };
    }
    */
}