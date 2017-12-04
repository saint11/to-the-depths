using Otter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LD40
{
    public class Slash:Entity
    {
        public Spritemap sprite;
        public Damage damage;

        public Slash(float x, float y, float dirX, float dirY, Damage.Side side)
            : base(x, y)
        {
            sprite = new Spritemap(Global.imagePath + "Player/Slash.png", 64, 64);
            sprite.Add(0, new Anim("0-3", "3"));
            sprite.Angle = MathHelper.ToDegrees((float)Math.Atan2(dirX, dirY)) + 90;
            sprite.FlippedY = dirX > 0;
            sprite.SetOrigin(0,32);

            AddGraphic(sprite);
            sprite.Play(0);
            sprite.Anims[0].NoRepeat();
            sprite.Anims[0].OnComplete += () => { RemoveSelf(); };

            AddComponents(damage = new Damage(5, x, y, side));

            var col = new LineCollider(-dirX*10, -dirY*10, -dirX*50,-dirY*50, Global.Tags.Attack);
            AddCollider(col);
            AddComponent(new YSort());
        }

        public override void UpdateLast()
        {
            if (damage.hitSomething)
            {
                RemoveCollider(Collider);
                damage.hitSomething = false;
                CameraTarget.shake = 30;
            }
        }

        public override void Render()
        {
            base.Render();

            if (Global.debug && Collider != null)
            {
                Collider.Render(Color.Red);
            }
        }
    }
}
