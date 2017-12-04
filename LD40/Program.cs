using Otter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LD40.Global;

namespace LD40
{
    class Program
    {
        static void Main(string[] args)
        {
            Game game = new Game("LD 48", Global.screenWidth, Global.screenHeight);
            game.SetWindowScale(1);
            //game.LockMouse = true;
            

            Global.player = game.AddSession("Player1");

            Global.player.Controller.AddAxis(Controls.Axis, Axis.CreateWASD());
            Global.player.Controller.AddButton(Controls.Action);
            
            
            Global.player.Controller.Button(Controls.Action).AddKey(Key.Space);

            SpriteData.Init();
            SpriteData.Load(Global.path + "Sprites.xml");

            Global.Font = new Otter.Font(Global.path + "Monocons.ttf");

            var sfx = new Sfx();

            game.FirstScene = new GameScene();

#if DEBUG
            game.OnUpdate += () =>
            {
                if (Input.Instance.KeyPressed(Key.Num1))
                {
                    Global.debug = !Global.debug;
                }
            };
#endif

            game.Start();

            game.OnEnd += () =>
            {
                Sfx.instance.gameMusic.Stop();
                Sfx.instance.tenseMusic.Stop();
                Sfx.instance.gameOverMusic.Stop();
            };
        }
    }
}
