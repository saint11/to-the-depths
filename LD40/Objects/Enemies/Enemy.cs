using Otter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LD40
{
    public class Enemy : Entity
    {
        protected Collider movementCollider;
        protected Collider damageCollider;
        protected BasicMovement movement;
        protected Sprite sprite;

        public StateMachine<States> state = new StateMachine<States>();
        protected Damage lastDamage;

        public int hp;

        public Room room;

        public enum States
        {
            Idle,
            Hurt,
            Dead,
            Attack,
            Charge,
            Sleep
        }

        public Enemy(float x, float y, Room room)
            :base(room.X + x, room.Y + y)
        {
            movementCollider = AddCollider(new BoxCollider(16, 16, Global.Tags.Enemy));
            movementCollider.SetOrigin(8,16);

            movement = new BasicMovement(300, 300, 50);
            movement.Collider = movementCollider;
            movement.AddCollision(Global.Tags.Wall);
            AddComponent(movement);

            
            AddComponent(state);
            AddComponent(new YSort());

            this.room = room;
        }

        public override void Update()
        {
            if (state.CurrentState != States.Dead && !CameraTarget.transition)
            {
                if (Player.instance.currentRoom != room || Player.instance.state.CurrentState == Player.States.Stop || !Stats.instance.started)
                {
                    state.ChangeState(States.Sleep);
                }
                else
                {
                    if (state.CurrentState == States.Sleep)
                    {
                        state.ChangeState(States.Idle);
                    }
                }
                CheckForDamage();
            }
        }

        private void CheckForDamage()
        {
            if (damageCollider != null)
            {
                var other = damageCollider.Collide(X, Y, Global.Tags.Attack);
                if (other != null)
                {
                    var damage = other.Entity.GetComponent<Damage>();
                    if (damage != null && damage.side == Damage.Side.player)
                    {
                        lastDamage = damage;
                        damage.hitSomething = true;
                        hp -= damage.value;

                        if (hp > 0)
                            state.ChangeState(States.Hurt);
                        else
                            state.ChangeState(States.Dead);
                    }

                }
            }
        }

        public override void Render()
        {
            if (Global.debug)
            {
                movementCollider.Render(new Color(0.5f,0.6f,1));
                if (damageCollider!=null)
                    damageCollider.Render(new Color(1, 0.5f, 1));
            }
        }


        // SLEEP state
        public virtual void EnterSleep()
        {
            sprite.Color = Color.White;
            movement.Speed.X = 0;
            movement.Speed.Y = 0;
        }
    }
}
