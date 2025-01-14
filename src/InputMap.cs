using Godot;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

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
            Player = 1,
            Object = 2,
            Spike = 3
        }

        public static string Map => "TileMapLayer";
        public static string Player => "CharacterBody2D";

        public static int Length() => Enum.GetNames(typeof(ObjectType)).Length;

        public static string GetObjectName(int ObjectNumber) =>
            ((ObjectType?)ObjectNumber) switch
            {
                ObjectType.Map => "TileMapLayer",
                ObjectType.Player => "CharacterBody2D",
                _ => "Undefined"
            };
    }

    internal class CollisionSelecter
    {
        private enum CollisionLayersAndMasks // The numbers of enum is powed of 2
        {
            Map = 0,
            Player = 2,
            Object = 4,
        }

        public static int ToDigit(string[] LayersAndMasks)
        {
            int result = 0;
            if (LayersAndMasks.Length == 0)
                return 0;

            foreach (var Name in LayersAndMasks)
            {
                result += (int)Enum.Parse(typeof(CollisionLayersAndMasks), Name);
            }

            return result;
        }
    }
}
