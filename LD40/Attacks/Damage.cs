using Otter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LD40
{
    public class Damage : Component
    {
        public enum Side
        {
            player,
            enemy
        }

        public float X, Y;
        public int value;
        public bool hitSomething = false;
        public Side side;

        public Damage (int value, float x, float y, Side side)
        {
            X = x;
            Y = y;
            this.value = value;
            this.side = side;
        }
    }
}
