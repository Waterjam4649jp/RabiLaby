using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabiLaby.src
{
    internal class GetControllMap
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

    internal class GetAnimationMap
    {
        private static readonly Dictionary<string, string> KeyMap = new Dictionary<string, string>
        {
            { "jump", "jump" },
            { "walk", "walk" },
            { "wait", "wait" },
        };

        public static string jump => KeyMap["jump"];
        public static string right => KeyMap["walk"];
        public static string left => KeyMap["wait"];
    }
}
