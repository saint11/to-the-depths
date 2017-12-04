using Otter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LD40
{
    class CameraTarget: Entity
    {
        public static bool transition = false;
        public static float shake = 0;

        public CameraTarget(Room room)
            : base(room.X + Global.screenWidth / 2, room.Y + Global.screenHeight * 0.6f )
        {

        }

        public override void Update()
        {
            base.Update();
        }


        public override void Added()
        {
            Scene.CameraFocus = this;
        }

        public void MoveTo(float x, float y)
        {
            transition = true;
            foreach (var e in Scene.GetEntities<Enemy>())
            {
                if (e.state.CurrentState!= Enemy.States.Dead)
                    e.state.ChangeState(Enemy.States.Sleep );
            }
            Tweener.Tween(this, new { X = x + Global.screenWidth / 2, Y = y + Global.screenHeight / 2 }, 60).Ease(Ease.CubeInOut)
                .OnComplete(() => { transition = false; });
        }
    }
}
