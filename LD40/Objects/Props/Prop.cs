using Otter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LD40
{
    public class Prop : Entity
    {
        bool breakable;

        public Prop (float x, float y, Room room, Rectangle collisionSize, string imagePath, bool breakable)
            :base(room.X + x + 0 , room.Y + y + 16)
        {
            AddGraphic(new Image(Global.imagePath + "Props/" + imagePath + ".png" ));
            Graphic.CenterOrigin();
            AddCollider(new BoxCollider(collisionSize.Width, collisionSize.Height, Global.Tags.Wall));

            this.breakable = breakable;
            Collider.SetPosition(-collisionSize.X, -collisionSize.Y); // This is wrong but it's easier

            AddComponent(new YSort());
        }

        public override void UpdateLast()
        {
            var others = Collider.CollideEntities(X, Y, Global.Tags.Attack);
            if (others!=null)
            {
                foreach (var o in others)
                {
                    var damage = o.GetComponent<Damage>();
                    if (damage!=null)
                    {
                        damage.hitSomething = true;
                        Scene.Add(new SimpleExplosion(X, Y - 8));
                        RemoveSelf();
                        OnBreak();
                    }
                }
            }
        }

        public virtual void OnBreak()
        {
            
        }

        public override void Render()
        {
            if (Global.debug)
            {
                Collider.Render(Color.Gray);
            }
        }
    }
    
}
