using Otter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LD40
{
    public class YSort : Component
    {
        int offset;
        public YSort (int offset=0)
        {
            this.offset = offset;
        }

        public override void UpdateLast()
        {
            Entity.Layer = -(int)Entity.Y - offset;
        }
    }
}
