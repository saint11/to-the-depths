using Otter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LD40
{
    public class Ui:Entity
    {
        public static Ui instance;

        public bool showingMessage;


        public Text drinkMessageText;
        public Text genericMessage;

        Image screenFX;

        Image splash;
        
        public Ui()
            :base()
        {
            Layer = int.MinValue;
            instance = this;

            drinkMessageText = new Text("Press SPACE to drink the blood", Global.Font, 8);
            drinkMessageText.OutlineColor = Color.Black;
            drinkMessageText.OutlineThickness = 2;
            drinkMessageText.CenterOrigin();
            drinkMessageText.SetOrigin((int)drinkMessageText.OriginX, (int)drinkMessageText.OriginY);
            drinkMessageText.X = Global.screenWidth / 2;
            drinkMessageText.Y = Global.screenHeight - 64;
            drinkMessageText.Visible = false;

            genericMessage = new Text("", Global.Font, 8);
            genericMessage.X = Global.screenWidth / 2;
            genericMessage.Y = Global.screenHeight - 32;
            genericMessage.Alpha = 0;
            genericMessage.OutlineColor = Color.Black;
            genericMessage.OutlineThickness = 4;

            screenFX = new Image(Global.imagePath + "FX/Thirst.png");
            screenFX.CenterOrigin();

            splash = new Image(Global.imagePath + "UI/Splash.png");
            splash.Scroll = 0;
        }
        
        public override void UpdateFirst()
        {
            drinkMessageText.Visible = false;
        }

        internal void ShowMessage(string v)
        {
            if (!showingMessage)
            {
                showingMessage = true;
                genericMessage.String = v;
                genericMessage.CenterOrigin();
                genericMessage.SetOrigin((int)genericMessage.OriginX, (int)genericMessage.OriginY);

                Tweener.Tween(genericMessage, new { Alpha = 1 }, 30).OnComplete(()=>
                {
                    Tweener.Tween(genericMessage, new { Alpha = 0 }, 30, 400).OnComplete(() =>
                    {
                        showingMessage = false;
                    });
                });
            }
        }

        public override void Render()
        {
            Draw.SetTarget((Scene as GameScene).UISurface);

            Draw.Graphic(drinkMessageText);
            Draw.Graphic(genericMessage);

            Color color = Stats.instance.thirst>0.3f ? new Color("f6d6bd") : new Color("816271");

            var t = Math.Max(Stats.instance.thirst, 0);
            var sizeH = t * Global.screenWidth;
            var sizeV = t * Global.screenHeight;

            Draw.Rectangle(Global.screenWidth / 2 - sizeH / 2, 0, sizeH, 4, color);
            Draw.Rectangle(Global.screenWidth / 2 - sizeH / 2, Global.screenHeight - 4, sizeH, 4, color);

            Draw.Rectangle(0, Global.screenHeight / 2 - sizeV / 2, 4, sizeH / 2, color);
            Draw.Rectangle(Global.screenWidth - 4, Global.screenHeight / 2 - sizeV / 2, 4, sizeH / 2, color);

            for (int i = 0; i < Stats.instance.injuries; i++)
            {
                Draw.Graphic(new Image(Global.imagePath + "UI/Hurt.png"), 24 + i*13, 24);
            }
            Draw.ResetTarget();

            if (Stats.instance.drain > 0)
            {
                screenFX.Alpha = 0.3f * (1 - Stats.instance.thirst) + (float)Math.Sin(Timer * 0.01f) * 0.1f * (1 - Stats.instance.thirst);
                screenFX.Blend = BlendMode.Add;

                screenFX.ScaleX = 1.2f + (float)Math.Sin(Timer * 0.01f) * 0.11f;
                screenFX.ScaleY = 1.4f + (float)Math.Sin(Timer * 0.1f) * 0.11f;
                screenFX.Angle = Rand.Float(-1, 1);
                Draw.Graphic(screenFX, (float)Math.Sin(Timer * 0.05f) * 2 - 32 + Scene.CameraCenterX, (float)Math.Cos(Timer * 0.05f) * 3 + Scene.CameraCenterY);

                screenFX.Angle = Rand.Float(-1, 1);
                screenFX.ScaleX = 1.2f + (float)Math.Cos(Timer * 0.05f) * 0.11f;
                screenFX.ScaleY = 1.4f + (float)Math.Cos(Timer * 0.02f) * 0.11f;
                Draw.Graphic(screenFX, (float)Math.Sin(Timer * 0.05f) * 2 + Scene.CameraCenterX, (float)Math.Cos(Timer * 0.05f) * 3 + Scene.CameraCenterY);
            }

            Draw.Graphic(splash);

            if (Stats.instance.started)
            {
                if (splash.Alpha > 0)
                {
                    splash.Alpha -= 0.01f;
                }
                else
                    splash.Alpha = 0;
            }
        }
    }
}
