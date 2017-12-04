using Otter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LD40
{
    public class Yog : Enemy
    {
        Player player;
        private float hurtTimer;
        private float hurtImpulse;

        public Yog(float x, float y, Room room)
            : base(x, y, room)
        {
            sprite = SpriteData.GetAnimation("yog");
            sprite.Play("idle");
            AddGraphic(sprite);

            damageCollider = AddCollider(new CircleCollider(20, Global.Tags.Enemy));
            damageCollider.SetOrigin(16, 40);

            hp = 40;
        }

        public override void Added()
        {
            base.Added();
            state.ChangeState(States.Idle);
        }


        // IDLE state
        float attackAnywayTimer;
        public void EnterIdle()
        {
            sprite.Color = Color.White;
            if (player == null)
            {
                player = Scene.GetEntity<Player>();
            }
            attackAnywayTimer = 100;
        }
        public void UpdateIdle()
        {
            Vector2 direction = -new Vector2(X - player.X, Y - player.Y);
            attackAnywayTimer -= 1;

            float distance = direction.Length;
            direction.Normalize(5);

            sprite.Flip = player.X < X;

            var dir = Util.Rotate(direction, 40);
            if (distance < 200 && attackAnywayTimer > 0)
            {
                movement.MoveXY(-(int)dir.X, -(int)dir.Y, movementCollider);
            }
            else
            {
                state.ChangeState(States.Charge);
            }
        }


        //Charge
        float chargeTimer;
        public void EnterCharge()
        {
            chargeTimer = 100;
            Tweener.Tween(sprite, new { ScaleX = 1.1f, ScaleY = 0.85f }, 100).Ease(Ease.ElasticInOut);

            attackOffset = new Vector2(0, -16);
            attackDirection = new Vector2(X - player.X + attackOffset.X, Y - player.Y + attackOffset.Y + 16);
            attackDirection.Normalize();

            Scene.Add(new Slash(X + attackOffset.X, Y + attackOffset.Y, attackDirection.X, attackDirection.Y, Damage.Side.enemy));
        }
        public void UpdateCharge()
        {
            chargeTimer -= 1;
            if (chargeTimer <= 0)
            {
                state.ChangeState(States.Attack);
            }
        }

        // ATTACK state -----------------
        float attackTimer;
        Vector2 attackDirection;
        Vector2 attackOffset;
        public void EnterAttack()
        {
            attackOffset = new Vector2(0, -16);
            attackDirection = new Vector2(X - player.X + attackOffset.X, Y - player.Y + attackOffset.Y + 16);
            attackDirection.Normalize();

            Tweener.Tween(sprite, new { ScaleX = 1, ScaleY = 1 }, 60).Ease(Ease.ElasticOut);
            sprite.Color = Color.White;
            movement.Axis = null;
            movement.TargetSpeed.X = 0;
            movement.TargetSpeed.Y = 0;

            movement.Speed.X = 1000 * attackDirection.X;
            movement.Speed.Y = 1000 * attackDirection.Y;

            Scene.Add(new Projectile(X + attackOffset.X, Y + attackOffset.Y, -attackDirection.X, -attackDirection.Y, Damage.Side.enemy));

            attackDirection = Util.Rotate(attackDirection, -30);
            Scene.Add(new Projectile(X + attackOffset.X, Y + attackOffset.Y, -attackDirection.X, -attackDirection.Y, Damage.Side.enemy));
            attackDirection = Util.Rotate(attackDirection, -30);
            Scene.Add(new Projectile(X + attackOffset.X, Y + attackOffset.Y, -attackDirection.X, -attackDirection.Y, Damage.Side.enemy));

            attackDirection = Util.Rotate(attackDirection, 60 + 30);
            Scene.Add(new Projectile(X + attackOffset.X, Y + attackOffset.Y, -attackDirection.X, -attackDirection.Y, Damage.Side.enemy));
            attackDirection = Util.Rotate(attackDirection, 30);
            Scene.Add(new Projectile(X + attackOffset.X, Y + attackOffset.Y, -attackDirection.X, -attackDirection.Y, Damage.Side.enemy));
            attackTimer = 30;
        }
        public void UpdateAttack()
        {
            movement.Speed.X *= 0.98f;
            movement.Speed.Y *= 0.98f;

            attackTimer -= Game.DeltaTime;
            if (attackTimer <= 0)
            {
                state.ChangeState(States.Idle);
            }
        }

        //HURT state
        public void EnterHurt()
        {
            hurtTimer = 50;
            hurtImpulse = 2;

            Tweener.Cancel();
            sprite.Scale = 1.15f;
            Tweener.Tween(sprite, new { ScaleX = 1, ScaleY = 1 }, 50).Ease(Ease.ElasticOut);
        }
        public void UpdateHurt()
        {
            hurtTimer--;
            Vector2 direction = new Vector2(X - lastDamage.X, Y - lastDamage.Y).Normalized(hurtImpulse * 100);
            hurtImpulse *= 0.8f;
            movement.MoveXY((int)direction.X, (int)direction.Y, movementCollider);

            if (hurtTimer <= 0)
            {
                state.ChangeState(States.Charge);
            }
        }

        //Dead state
        public void EnterDead()
        {
            sprite.Play("dead");

            RemoveCollider(damageCollider);
            damageCollider = null;

            AddComponent(new Corpse(movementCollider));
        }
    }
}
