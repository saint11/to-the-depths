using Otter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LD40
{
    public class GameScene: Scene
    {
        public Surface UISurface;

        public List<Room> rooms;

        public GameScene()
        {
            UISurface = new Surface(Global.screenWidth, Global.screenHeight);
            UISurface.Scroll = 0;

            DungeonMaker.CreateDungeon(this);

            var s = new Stats(Player.instance);

            Add(new Ui());
            Add(new FadeOut(1, 0, 100, Color.Black));
            //Sfx.instance.gameMusic.Play();
            Sfx.instance.gameMusic.Volume = 0;
            Sfx.instance.tenseMusic.Volume = 0;
        }

        public override void Update()
        {
            Stats.instance.Update();
        }

        public override void Render()
        {
            base.Render();
            
            Draw.Graphic(UISurface);

            Draw.Circle(Input.MouseScreenX, Input.MouseScreenY, 3, Color.White);
        }
    }
}
