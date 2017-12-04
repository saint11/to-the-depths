using Otter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LD40
{
    class GameOverScreen : Scene
    {
        bool inputBlock = true;
        public GameOverScreen()
        {
            AddGraphic(new Image(Global.imagePath + "Ui/GameOver.png"));
            Add(new FadeOut(1, 0, 50, Color.Black, () => { inputBlock = false; }));
        }

        public override void Update()
        {
            if (!inputBlock)
            {
                if (Input.MouseButtonPressed(MouseButton.Left) || Global.player.Controller.Button(Global.Controls.Action).Pressed)
                {
                    inputBlock = true;
                    Add(new FadeOut(0, 1, 100, Color.Black, () => { Game.SwitchScene(new GameScene()); }));
                }
            }
        }
    }
}
