using Otter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LD40
{
    class Corpse : Component
    {
        public bool Drained = false;
        public Collider collider;

        public Corpse(Collider col)
        {
            collider = col;
        }
        public override void Update()
        {
            if (!Drained)
            {
                var other = collider.Collide(Entity.X, Entity.Y, Global.Tags.Player);
                if (other != null)
                {
                    Ui.instance.drinkMessageText.Visible = true;
                }
            }
        }

        public override void Added()
        {
            Sfx.instance.death.Play();
        }
    }
}
