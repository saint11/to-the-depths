using Otter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LD40
{
    public class SlowDoor : Entity
    {
        bool opening = false;
        bool gotHit = false;
        private float ySpeed;

        public SlowDoor(float x, float y, Room room)
            : base(room.X + x, room.Y + y)
        {
            AddGraphic(new Image(Global.imagePath + "Rooms/SlowDoor.png"));
            Graphic.SetOrigin(48, 0);
            AddCollider(new BoxCollider(64, 52, Global.Tags.Wall));
            Collider.SetOrigin(32, 0);

            AddComponent(new YSort());
        }

        public override void Update()
        {
            if (gotHit)
            {
                Y -= ySpeed;
                ySpeed += 0.001f;
                Graphic.X = Rand.Int(-1, 1);
                if (!opening)
                {
                    if (Player.instance.state.CurrentState == Player.States.Dead)
                    {
                        Player.instance.state.ChangeState(Player.States.Stop);
                    }
                    opening = true;
                    Sfx.instance.door.Play();
                    //RemoveCollider(Collider);
                }

                if (CameraTarget.transition)
                {
                    RemoveSelf();
                    if (Player.instance.state.CurrentState == Player.States.Dead)
                    {
                        Player.instance.state.ChangeState(Player.States.Normal);
                    }
                }
            }
            else
            {
                var dist = 100;
                if (new Vector2(X - Player.instance.X, Y - Player.instance.Y).LengthSquared() < dist * dist)
                {
                    //Ui.instance.ShowMessage("Only those with the strength may enter");
                }

                var others = Collider.CollideEntities(X, Y, Global.Tags.Attack);
                if (others != null)
                {
                    foreach (var o in others)
                    {
                        var damage = o.GetComponent<Damage>();
                        if (damage != null)
                        {
                            damage.hitSomething = true;
                            gotHit = true;
                        }
                    }
                }
            }
        }

    }
}