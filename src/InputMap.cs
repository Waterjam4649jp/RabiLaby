using System;

namespace RabiLaby.src
{
    internal class GetControllMap
    {
        public static string Jump => "jump";
        public static string Right => "move_right";
        public static string Left => "move_left";
    }

    internal class GetAnimationMap
    {
        public static string Jump => "jump";
        public static string Walk => "walk";
        public static string Wait => "wait";
    }

    internal class GetObjectMap
    {
        public enum ObjectType
        {
            Map = 0,
            Character = 1,
            Object = 2,
            Spike = 3
        }

        public static string Map => "TileMapLayer";
        public static string Character => "CharacterBody2D";

        public static int Length() => Enum.GetNames(typeof(ObjectType)).Length;

        public static string GetObjectName(int ObjectNumber) =>
            ((ObjectType?)ObjectNumber) switch
            {
                ObjectType.Map => "TileMapLayer",
                ObjectType.Character => "CharacterBody2D",
                _ => "Undefined"
            };
    }
}
