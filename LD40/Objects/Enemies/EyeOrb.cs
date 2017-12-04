using Otter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LD40
{
    public class EyeOrb : Enemy
    {
        Player player;
        private float hurtTimer;
        private float hurtImpulse;
        
        public EyeOrb(float x, float y, Room room)
            : base(x, y, room)
        {
            sprite = SpriteData.GetAnimation("eyeOrb");
            sprite.Play("idle");
            AddGraphic(sprite);

            damageCollider = AddCollider(new CircleCollider(16, Global.Tags.Enemy));
            damageCollider.SetOrigin(16, 32);

            hp = 5;
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
            attackAnywayTimer = 300;
        }
        public void UpdateIdle()
        {
            Vector2 direction = -new Vector2(X - player.X, Y - player.Y);
            attackAnywayTimer -= 1;

            float distance = direction.Length;
            direction.Normalize(70);
            
            sprite.Flip = player.X < X;

            var dir = Util.Rotate(direction, 40);
            if (distance < 200 && attackAnywayTimer>0)
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
            chargeTimer = 60;
            Tweener.Tween(sprite,new { ScaleX = 1.1f, ScaleY = 0.85f }, 60).Ease(Ease.ElasticOut);
            attackOffset = new Vector2(0, -16);
            attackDirection = new Vector2(X - player.X + attackOffset.X, Y - player.Y + attackOffset.Y + 16);
            attackDirection.Normalize();
        }
        public void UpdateCharge()
        {
            chargeTimer -= Game.DeltaTime;
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
            Tweener.Tween(sprite, new { ScaleX = 1, ScaleY = 1 }, 60).Ease(Ease.ElasticOut);
            sprite.Color = Color.White;
            movement.Axis = null;
            movement.TargetSpeed.X = 0;
            movement.TargetSpeed.Y = 0;

            movement.Speed.X = 1000 * attackDirection.X;
            movement.Speed.Y = 1000 * attackDirection.Y;

            var slash = Scene.Add(new Projectile(X + attackOffset.X, Y + attackOffset.Y, -attackDirection.X, -attackDirection.Y, Damage.Side.enemy));
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
            hurtImpulse = 5;
        }
        public void UpdateHurt()
        {
            hurtTimer--;
            Vector2 direction = new Vector2(X - lastDamage.X, Y - lastDamage.Y).Normalized(hurtImpulse * 100);
            hurtImpulse *= 0.8f;
            movement.MoveXY((int)direction.X, (int)direction.Y, movementCollider);

            if (hurtTimer <= 0)
            {
                state.ChangeState(States.Idle);
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
