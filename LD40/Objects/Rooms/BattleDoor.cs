using Otter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LD40
{
    public class BattleDoor : Entity
    {
        bool opening = false;
        bool gotHit = false;
        private float ySpeed;

        Room room;

        public BattleDoor(float x, float y, Room room)
            : base(room.X + x, room.Y + y)
        {
            AddGraphic(new Image(Global.imagePath + "Rooms/BattleDoor.png"));
            Graphic.SetOrigin(48, 0);
            AddCollider(new BoxCollider(64, 52, Global.Tags.Wall));
            Collider.SetOrigin(32, 0);
            this.room = room;
            AddComponent(new YSort());
        }

        public override void Update()
        {
            if (gotHit)
            {
                Y -= ySpeed;
                ySpeed += 0.007f;
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
                bool fail = false;
                foreach (var e in Scene.GetEntities<Enemy>())
                {
                    if (e.room==room && e.state.CurrentState != Enemy.States.Dead)
                    {
                        fail = true;
                        break;
                    }
                }

                if (!fail)
                {
                    gotHit = true;
                }
            }
        }

    }
}