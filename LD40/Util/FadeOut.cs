using Otter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LD40
{

    public class FadeOut : Entity
    {
        public Color color = Color.Black;

        public FadeOut(float from, float to, float time, Color color, Action onComplete = null)
        {
            Layer = -1000;
            var im = AddGraphic(Image.CreateRectangle(Global.screenWidth, Global.screenHeight, color));
            im.Alpha = from;
            Tween(im, new { Alpha = to }, time).OnComplete(onComplete);
            LifeSpan = time+2;
        }
        public override void Added()
        {
            if (Scene is GameScene)
            {
                Surface = (Scene as GameScene).UISurface;
            }

        }
    }
}
