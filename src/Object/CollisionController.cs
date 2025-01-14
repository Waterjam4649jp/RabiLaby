using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabiLaby.src.Object
{
    internal class CollisionController
    {
        public static void MountOnAliceOrHat((string pre, string post) floorTypes)
        {
            // When collision is disabled against Alice or Hat, and steps onto their top, pre Floor will be "None"
            // Also, this void is used for Alice, Hat, Lily, (or something?)
            if (floorTypes == ("None", "Alice") || floorTypes == ("None", "Hat"))
            {
                GD.Print("はーいおｋでーす");
            }
        }
    }
}
