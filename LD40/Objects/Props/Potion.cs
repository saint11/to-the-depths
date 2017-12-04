using Otter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LD40
{
    class Potion:Entity
    {
        bool collected = false;

        public Potion(float x, float y)
            :base(x, y)
        {
            var sprite = AddGraphic(new Image(Global.imagePath + "Props/Potion.png"));
            sprite.CenterOrigin();
            
            AddComponent(new YSort());
            AddCollider(new BoxCollider(16, 16));
            Collider.CenterOrigin();
        }

        public override void Update()
        {
            if (!collected)
            {
                if (Collider.Collide(X, Y, Global.Tags.Player) != null)
                {
                    collected = true;
                    RemoveSelf();
                    Stats.instance.injuries--;
                    if (Stats.instance.injuries < 0)
                        Stats.instance.injuries = 0;
                    Scene.Add(new FadeOut(0.2f, 0, 50, new Color("6fdfb6")));
                    Sfx.instance.pickup.Play();
                    
                }
            }
        }
    }
}
