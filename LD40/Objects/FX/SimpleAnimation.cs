using Otter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LD40
{
    public class SimpleAnimation : Entity
    {
        public SimpleAnimation(float x, float y, int w, int h, int frames, string path, float speed)
            :base(x, y)
        {
            var sprite = new Spritemap(Global.imagePath + "FX/" + path + ".png", w, h);
            sprite.CenterOrigin();
            sprite.Add(0, "0-" + frames, speed);
            AddGraphic(sprite);
            sprite.Anims[0].OnComplete += () => { RemoveSelf(); };
            sprite.Anims[0].NoRepeat();
            sprite.Play(0);

            AddComponent(new YSort());
        }
    }

    public class SimpleExplosion : SimpleAnimation
    {
        public SimpleExplosion(float x, float y)
            : base(x, y, 48, 48, 8, "SmExplosion", 3) {}
        public override void Added()
        {
            if (Rand.Chance(50))
                Sfx.instance.breaksound.Play();
            else
                Sfx.instance.breaksound2.Play();
        }
    }
}
