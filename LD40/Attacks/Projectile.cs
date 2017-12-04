using Otter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LD40
{
    public class Projectile : Entity
    {
        private float dirX;
        private float dirY;
        private Damage.Side side;
        private Spritemap sprite;

        Damage damage;

        public Projectile(float x, float y, float dirX , float dirY, Damage.Side side)
            : base(x, y)
        {
            this.dirX = dirX;
            this.dirY = dirY;
            this.side = side;

            sprite = new Spritemap(Global.imagePath + "FX/projectile.png", 32, 32);
            sprite.Add(0, new Anim("0-5", "2"));
            sprite.Play(0);
            sprite.CenterOrigin();
            AddGraphic(sprite);

            AddCollider(new CircleCollider(4, Global.Tags.Attack));
            Collider.CenterOrigin();

            damage = AddComponent(new Damage(5, X, Y, side));

            AddComponent(new YSort());

            LifeSpan = 250;
        }
        public override void Added()
        {
            base.Added();
            Sfx.instance.shot.Play();
        }

        public override void Update()
        {
            X += dirX*2;
            Y += dirY*2;
            if (damage.hitSomething)
            {
                RemoveSelf();
            }
        }
    }
}
