using Otter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LD40.Props
{
    public class Box:Prop
    {
        public static float potionDrop = 0;

        public Box(float x, float y, Room r)
            : base(x, y, r, new Rectangle(18, 12, 36, 24), "boxes", true) {}

        public override void OnBreak()
        {
            if (Rand.Chance(5 + potionDrop))
            {
                Scene.Add(new Potion(X, Y));
                potionDrop = 0;
            }
            else
            {
                potionDrop += 2;
            }
        }
    }

    public class Block1 : Prop
    {
        public Block1(float x, float y, Room r)
            : base(x, y, r, new Rectangle(30, 10, 60, 30), "block00", true) { }
    }
}
