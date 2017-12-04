using Otter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LD40
{
    class Global
    {
        public static Session player;
        public static int screenWidth = 640;
        public static int screenHeight = 320;

        internal static int tile = 32;

#if DEBUG
        public static string path = "../../Content/";
#else
        public static string path =  "Content/";
#endif

        public static string imagePath = path + "Images/";

        public static bool debug = false;

        internal static Font Font;

        public enum Tags
        {
            Player,
            Enemy,
            Wall,
            Attack,
            Vision
        }

        public enum Controls
        {
            Axis,
            Action,
        }
    }
}
