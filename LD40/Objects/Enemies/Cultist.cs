using Otter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LD40
{
    public class Cultist : Enemy
    {
        Player player;
        private float hurtTimer;
        private float hurtImpulse;

        private LineCollider visionCollider;
        bool canSee=false;

        public Cultist(float x, float y, Room room)
            : base(x, y, room)
        {
            sprite = SpriteData.GetAnimation("cultist");
            sprite.Play("idle");
            AddGraphic(sprite);

            damageCollider = AddCollider(new CircleCollider(16, Global.Tags.Enemy));
            damageCollider.SetOrigin(16, 32);

            visionCollider = AddCollider(new LineCollider(0, -8, 0, -8, Global.Tags.Vision));
            AddCollider(visionCollider);
            hp = 10;
        }

        public override void Added()
        {
            base.Added();
            state.ChangeState(States.Sleep);
            player = Player.instance;
        }

        public override void Update()
        {
            base.Update();
            if (Player.instance.currentRoom == room)
            {
                Vector2 direction = -new Vector2(X - player.X, Y - player.Y + 8);
                visionCollider.X = direction.X;
                visionCollider.Y = direction.Y;

                canSee = visionCollider.CollideEntities(X, Y, Global.Tags.Wall).Count == 0;
            }
        }

        // IDLE state
        float spotTimer;
        public void EnterIdle()
        {
            if (canSee)
                spotTimer = 50;
            else
                spotTimer = 250;
        }
        public void UpdateIdle()
        {
            if(canSee)
            {
                spotTimer -= 1;
                if (spotTimer<=0)
                {
                    state.ChangeState(States.Charge);
                }
                if(sprite.CurrentAnimation.Name!= "spot")
                    sprite.Play("spot");
            }
            else
            {
                canSee = false;
                sprite.Play("idle");
            }
        }


        //Charge--------------------
        public void EnterCharge()
        {
            sprite.Play("run");
        }
        public void UpdateCharge()
        {
            Vector2 direction = -new Vector2(X - player.X, Y - player.Y);
            float distance = direction.Length;
            direction.Normalize(150);

            movement.MoveXY((int)direction.X, (int)direction.Y, movementCollider);
            sprite.Flip = player.X < X;

            if (distance < 55)
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
            sprite.Play("attack");
            attackOffset = new Vector2(0, -16);
            attackDirection = new Vector2(X - player.X + attackOffset.X, Y - player.Y + attackOffset.Y + 16);
            attackDirection.Normalize();

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
            sprite.Play("hurt");
            hurtTimer = 90;
            hurtImpulse = 5;

            sprite.Scale = 1.3f;
            Tweener.Tween(sprite, new { ScaleX = 1, ScaleY = 1 }, 60).Ease(Ease.ElasticOut);
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
            sprite.SetOrigin(32, 64);
            AddGraphic(sprite);

            RemoveCollider(damageCollider);
            damageCollider = null;

            AddComponent(new Corpse(movementCollider));
        }

        public override void Render()
        {
            base.Render();

            if (Global.debug && Player.instance.currentRoom==room)
            {
                visionCollider.Render(canSee ? Color.Red : Color.Gray);
            }

        }
    }
}
