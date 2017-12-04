using Otter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LD40
{
    public class Teleporter : Entity
    {
        bool collected = false;
        Room room;

        Sprite sprite;

        public Teleporter(float x, float y, Room r)
            :base(x + r.X, y + r.Y)
        {
            sprite = AddGraphic(SpriteData.GetAnimation("teleporter"));
            sprite.Play("on");
            room = r;
            AddComponent(new YSort(-500));
            AddCollider(new BoxCollider(32, 32));
        }

        public override void Update()
        {
            if (!collected)
            {
                if (Collider.Collide(X,Y,Global.Tags.Player)!=null)
                {
                    collected = true;
                    foreach (var t in Scene.GetEntities<Teleporter>())
                    {
                        if (!t.collected && t.room == room)
                        {
                            Player.instance.SetPosition(t.X + 16, t.Y + 16);
                            t.collected = true;
                            Scene.Add(new FadeOut(1, 0, 50, new Color("6fdfb6")));
                            Sfx.instance.teleport.Play();
                            sprite.Play("off");
                            t.sprite.Play("off");
                        }
                    }
                }
            }
        }
    }
}
