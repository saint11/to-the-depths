using Otter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LD40
{
    class MacGuffin : Entity
    {
        bool collected = false;

        public MacGuffin(float x, float y, Room room)
            : base(room.X + x, room.Y + y)
        {
            var im = AddGraphic(new Image(Global.imagePath + "Props/Source.png"));
            im.CenterOrigin();
            
            AddComponent(new YSort());
        }

        public override void Update()
        {
            if (!collected)
            {
                if (new Vector2(X - Player.instance.X, Y - Player.instance.Y).LengthSquared() < 32 * 32)
                {
                    collected = true;
                    Scene.Add(new FadeOut(0, 1, 100, Color.Black, () => { Game.SwitchScene(new WinScreen()); }));
                    Player.instance.state.ChangeState(Player.States.Stop);
                }
            }
        }
    }
}
