using Otter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LD40
{
    public class Player : Entity
    {
        internal static Player instance;

        public Sprite sprite;
        private BasicMovement movement;
        public StateMachine<States> state = new StateMachine<States>();
        
        //Hurt
        protected Damage lastDamage;

        public CircleCollider damageCollider;

        public Room currentRoom;

        public enum States
        {
            Normal,
            Hurt,
            Dead,
            Attack,
            Stop
        }
        
        public Player(Room room)
        {
            instance = this;

            SetPosition(room.X + Global.screenWidth/ 2, room.Y + Global.screenHeight/ 2 + 116);
            sprite = SpriteData.GetAnimation("player");
            AddGraphic(sprite);
            

            AddCollider(new BoxCollider(10, 16, Global.Tags.Player));
            Collider.SetOrigin(5,16);

            movement = new BasicMovement(200, 200, 20);
            movement.AddCollision(Global.Tags.Wall);
            movement.Collider = Collider;
                
            AddComponent(movement);
            AddComponent(new YSort());

            AddComponent(state);
            state.ChangeState(States.Stop);

            damageCollider = AddCollider(new CircleCollider(16, Global.Tags.Enemy));
            damageCollider.SetOrigin(16, 32);
        }
        
        public override void Update()
        {
            var gameScene = Scene as GameScene;
            Room closest = null;
            float closestDist = float.MaxValue;
            foreach (var r in gameScene.rooms)
            {
                float dist = new Vector2(X - r.X - r.Width/2, Y - r.Y - r.Height/2).LengthSquared();
                if (dist<closestDist)
                {
                    closestDist = dist;
                    closest = r;
                }
            }

            if (closest != currentRoom)
            {
                currentRoom = closest;
                Scene.GetEntity<CameraTarget>().MoveTo(currentRoom.X, currentRoom.Y);
            }
        }


        // NORMAL State ------------
        public void EnterNormal()
        {
            sprite.Color = Color.White;
            movement.Axis = Global.player.Controller.Axis(Global.Controls.Axis);
            sprite.Play("idle");
        }
        public void UpdateNormal()
        {
            //Check for movement
            if (Global.player.Controller.Axis(Global.Controls.Axis).X!=0 || Global.player.Controller.Axis(Global.Controls.Axis).Y != 0)
            {
                if (sprite.CurrentAnimation.Name!="walk")
                    sprite.Play("walk");
                sprite.Flip = Global.player.Controller.Axis(Global.Controls.Axis).X < 0;
            }
            else
            {
                sprite.Play("idle");
            }

            //Check for input
            if (Input.MouseButtonPressed(MouseButton.Left))
            {
                state.ChangeState(States.Attack);
            }

            if (Global.player.Controller.Button(Global.Controls.Action).Pressed) // DRINK THE BLOOOOOD
            {
                var others = Collider.CollideEntities<Enemy>(X, Y, Global.Tags.Enemy);
                foreach (var e in others)
                {
                    var corpse = e.GetComponent<Corpse>();
                    if (corpse!=null && !corpse.Drained)
                    {
                        corpse.Drained = true;
                        Stats.instance.thirst = 1;

                        if (Stats.instance.drain == 0) // First drink
                        {
                            Stats.instance.drain = 0.25f;
                            if (!Sfx.instance.gameMusic.IsPlaying)
                                Sfx.instance.gameMusic.Play();
                            if (!Sfx.instance.tenseMusic.IsPlaying)
                                Sfx.instance.tenseMusic.Play();
                        }
                        Stats.instance.drain += 0.06f;
                        Stats.instance.injuries--;
                        if (Stats.instance.injuries < 0)
                            Stats.instance.injuries = 0;

                        Scene.Add(new FadeOut(0.8f, 0, 30, Color.White));
                        Sfx.instance.drink.Play();

                    }
                }
            }
            CheckForDamage();
        }

        // Attack state -----------------
        float attackTimer;
        public void EnterAttack()
        {
            movement.Axis = null;
            movement.TargetSpeed.X = 0;
            movement.TargetSpeed.Y = 0;

            Vector2 offset = new Vector2(0, -16);
            Vector2 direction = new Vector2(X - Input.MouseX + offset.X - Scene.CameraX, Y - Input.MouseY + offset.Y - Scene.CameraY);
            direction.Normalize();
            movement.Speed.X = 1000 * -direction.X;
            movement.Speed.Y = 1000 * -direction.Y;

            movement.MoveXY((int)(direction.X * -1000), (int)(direction.Y * -1000), Collider);

            if (Rand.Chance(50))
                Sfx.instance.sword.Play();
            else
                Sfx.instance.sword2.Play();
            
            var slash = Scene.Add(new Slash(X + offset.X, Y + offset.Y, direction.X, direction.Y, Damage.Side.player));
            attackTimer = 30;

            sprite.Play("attack");
            sprite.Flip = direction.X > 0;
        }
        public void UpdateAttack()
        {
            movement.Speed.X *= 0.98f;
            movement.Speed.Y *= 0.98f;
            CheckForDamage();

            attackTimer -= Game.DeltaTime;
            if (attackTimer<=0)
            {
                state.ChangeState(States.Normal);
            }
        }

        // HURT State -------------
        float hurtTimer;
        float hurtImpulse;
        public void EnterHurt()
        {
            sprite.Play("hurt");
            Sfx.instance.hurt.Play();

            movement.Axis = null;
            movement.TargetSpeed.X = 0;
            movement.TargetSpeed.Y = 0;

            sprite.Color = Color.Red;
            hurtTimer = 50 + 10*Stats.instance.injuries;
            Stats.instance.injuries++;
            hurtImpulse = 5;

            sprite.Scale = 1.25f;
            Tweener.Tween(sprite, new { ScaleX = 1, ScaleY = 1 }, 60).Ease(Ease.ElasticOut);
        }

        public void UpdateHurt()
        {
            hurtTimer--;
            Vector2 direction = new Vector2(X - lastDamage.X, Y - lastDamage.Y).Normalized(hurtImpulse * 200);
            hurtImpulse *= 0.8f;
            movement.MoveXY((int)direction.X, (int)direction.Y, Collider);

            if (hurtTimer <= 0)
            {
                state.ChangeState(States.Normal);
            }
            CheckForDamage();
        }

        //Dead state
        public void EnterDead()
        {
            sprite.Play("death");
            Sfx.instance.death.Play();
            Sfx.instance.gameOverMusic.Play();
            Tweener.Tween(Sfx.instance.gameMusic, new { Volume = 0 }, 150);
            Tweener.Tween(Sfx.instance.tenseMusic, new { Volume = 0 }, 150);

            RemoveCollider(damageCollider);
            damageCollider = null;

            movement.Speed.X = 0;
            movement.Speed.Y = 0;
            movement.TargetSpeed.X = 0;
            movement.TargetSpeed.Y = 0;
            movement.Axis = null;

            Scene.Add(new FadeOut(0, 1f, 200, Color.Black, () => { Game.SwitchScene(new GameOverScreen()); }));
        }

        //STOP State
        public void EnterStop()
        {
            sprite.Play("idle");
            movement.Speed.X = 0;
            movement.Speed.Y = 0;
            movement.TargetSpeed.X = 0;
            movement.TargetSpeed.Y = 0;
            movement.Axis = null;
        }
        public void UpdateStop()
        {
            if (Global.player.Controller.Axis(Global.Controls.Axis).X != 0 ||
                Global.player.Controller.Axis(Global.Controls.Axis).Y != 0 ||
                Global.player.Controller.Button(Global.Controls.Action).Pressed ||
                Input.MouseButtonPressed(MouseButton.Left))
            {
                if (!Stats.instance.started)
                {
                    Stats.instance.started = true;
                    state.ChangeState(States.Normal);
                }
                
            }
        }

        public override void Render()
        {
            base.Render();

            if (Global.debug)
            {
                Collider.Render(Color.Gray);

                if (damageCollider != null)
                    damageCollider.Render(new Color(1, 0.5f, 1));
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
                    if (damage != null && damage.side == Damage.Side.enemy)
                    {
                        lastDamage = damage;
                        damage.hitSomething = true;
                        if (state.CurrentState == States.Hurt)
                            state.ChangeState(States.Dead);
                        else
                            state.ChangeState(States.Hurt);
                    }
                }
            }

            if (Stats.instance.thirst<0)
            {
                state.ChangeState(States.Dead);
                Ui.instance.ShowMessage("The thirst takes you");
            }
        }
    }
}
