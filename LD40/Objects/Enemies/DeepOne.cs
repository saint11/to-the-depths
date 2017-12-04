using Otter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LD40
{
    public class DeepOne : Enemy
    {
        Player player;
        private float hurtTimer;
        private float hurtImpulse;

        public DeepOne(float x, float y, Room room)
            :base(x,y,room)
        {
            sprite = SpriteData.GetAnimation("deepOne");
            sprite.Play("idle");
            AddGraphic(sprite);

            damageCollider = AddCollider(new CircleCollider(16, Global.Tags.Enemy));
            damageCollider.SetOrigin(16, 32);

            hp = 15;
        }

        public override void Added()
        {
            base.Added();
            state.ChangeState(States.Idle);
        }


        // IDLE state
        public void EnterIdle()
        {
            if (player==null)
            {
                player = Scene.GetEntity<Player>();
            }
        }
        public void UpdateIdle()
        {
            Vector2 direction = -new Vector2(X - player.X, Y - player.Y);
            float distance = direction.Length;
            direction.Normalize(40);

            movement.MoveXY((int)direction.X, (int)direction.Y, movementCollider);
            sprite.Flip = player.X < X;

            if (distance < 40)
            {
                state.ChangeState(States.Charge);
            }
        }


        //Charge
        float chargeTimer;
        public void EnterCharge()
        {
            chargeTimer = 20;
            sprite.Color = Color.Red;

            attackOffset = new Vector2(0, -16);
            attackDirection = new Vector2(X - player.X + attackOffset.X, Y - player.Y + attackOffset.Y + 16);
            attackDirection.Normalize();
        }
        public void UpdateCharge()
        {
            chargeTimer -= Game.DeltaTime;
            if (chargeTimer<=0)
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
            sprite.Color = Color.White;
            movement.Axis = null;
            movement.TargetSpeed.X = 0;
            movement.TargetSpeed.Y = 0;

            movement.Speed.X = 1000 * -attackDirection.X;
            movement.Speed.Y = 1000 * -attackDirection.Y;

            var slash = Scene.Add(new Slash(X + attackOffset.X, Y + attackOffset.Y, attackDirection.X, attackDirection.Y, Damage.Side.enemy));
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
            hurtTimer --;
            Vector2 direction = new Vector2(X - lastDamage.X, Y - lastDamage.Y).Normalized(hurtImpulse * 100);
            hurtImpulse *= 0.8f;
            movement.MoveXY((int)direction.X, (int)direction.Y, movementCollider);

            if (hurtTimer<=0)
            {
                state.ChangeState(States.Charge);
            }
        }

        //Dead state
        public void EnterDead()
        {
            sprite.Play("dead");
            sprite.SetOrigin(32, 64);
            AddGraphic(sprite);

            RemoveCollider(damageCollider);
            damageCollider = null;

            AddComponent(new Corpse(movementCollider));
        }
    }
}
