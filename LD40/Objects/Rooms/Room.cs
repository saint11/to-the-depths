using Otter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace LD40
{
    public class Room : Entity
    {
        public Collider roomArea;

        internal int Width;
        internal int Height;
        private int index;
        private GridCollider grid;

        public Room(float x, float y, bool closedTop, int index)
            :base(x ,y)
        {
            int w = Global.screenWidth;
            int h = Global.screenHeight;

            Layer = 10000000;

            if (closedTop)
            {
                AddCollider(new BoxCollider(w , Global.tile, Global.Tags.Wall));
            }
            else
            {
                AddCollider(new BoxCollider(w / 2 - 25, Global.tile, Global.Tags.Wall));
                AddCollider(new BoxCollider(w / 2 - 25, Global.tile, Global.Tags.Wall)).SetPosition(w / 2 + 25, 0);
            }

            AddCollider(new BoxCollider(Global.tile, h, Global.Tags.Wall));

            AddCollider(new BoxCollider(w / 2 - 25, Global.tile, Global.Tags.Wall)).SetPosition(0, h - Global.tile);
            AddCollider(new BoxCollider(w / 2 - 25, Global.tile, Global.Tags.Wall)).SetPosition(w / 2 + 25, h - Global.tile);

            AddCollider(new BoxCollider(Global.tile, Global.screenHeight, Global.Tags.Wall)).SetPosition(w - Global.tile, 0);

            if (closedTop)
                AddGraphic(new Image(Global.imagePath + "/Rooms/roomEnd.png"));
            else
                AddGraphic(new Image(Global.imagePath + "/Rooms/room0.png"));

            Width = w;
            Height = h;
            this.index = index;
        }
        public override void Added()
        {
            var fileList = new List<String>();

            int count = 0;
            while (true)
            {
                var fileName = Global.path + "Rooms/Room_" + index + "_" + count + ".tmx";
                if (File.Exists(fileName))
                {
                    fileList.Add(fileName);
                    count++;
                }
                else
                {
                    if (count == 0)
                        throw new Exception(fileName + " does not exist?");
                    break;
                }
            }
            XmlDocument doc = new XmlDocument();
            doc.Load(Rand.ChooseElement(fileList));

            MakeTiles(doc);
            MakeStuff(doc);
        }

        private void MakeStuff(XmlDocument doc)
        {
            var objects = doc.SelectNodes("map/objectgroup/object");
            foreach (XmlNode o in objects)
            {
                if (Rand.Chance(o.SelectSingleNode("properties/property[@name='chance']").AttributeFloat("value")))
                {
                    if (o.AttributeString("name") == "box")
                        Scene.Add(new Props.Box(o.AttributeInt("x")+16, o.AttributeInt("y")+16, this));

                    else if (o.AttributeString("name") == "block1")
                        Scene.Add(new Props.Block1(o.AttributeInt("x") + 16, o.AttributeInt("y") + 48, this));

                    else if (o.AttributeString("name") == "Teleporter")
                        Scene.Add(new Teleporter(o.AttributeInt("x"), o.AttributeInt("y"), this));

                    else if (o.AttributeString("name") == "DeepOne")
                        Scene.Add(new DeepOne(o.AttributeInt("x")+16, o.AttributeInt("y") + 32, this));

                    else if (o.AttributeString("name") == "EyeOrb")
                        Scene.Add(new EyeOrb(o.AttributeInt("x")+16, o.AttributeInt("y") + 32, this));

                    else if (o.AttributeString("name") == "Cultist")
                        Scene.Add(new Cultist(o.AttributeInt("x")+16, o.AttributeInt("y") + 32, this));

                    else if (o.AttributeString("name") == "Shoggoth")
                        Scene.Add(new Shoggoth(o.AttributeInt("x") + 16, o.AttributeInt("y") + 32, this));

                    else if (o.AttributeString("name") == "Yog")
                        Scene.Add(new Yog(o.AttributeInt("x") + 16, o.AttributeInt("y") + 32, this));
                }
            }
        }

        public void MakeTiles(XmlDocument doc)
        {

            XmlNode graphicsData = doc.SelectSingleNode("map/layer[@name='Graphics']/data");
            XmlNode gridData = doc.SelectSingleNode("map/layer[@name='Grid']/data");
            
            var tileset = AddGraphic(new Tilemap(Global.imagePath + "Rooms/tiles.png",640,32));
            grid = AddCollider(new GridCollider(Global.screenWidth, Global.screenHeight, 32, 32, Global.Tags.Wall));

            grid.LoadCSV(gridData.InnerXml.Replace("\r", ""),"0","66");
            tileset.LoadCSV(graphicsData.InnerXml.Replace("\r",""),',', '\n',"",1);

        }

        public override void Render()
        {
            base.Render();

            if (Global.debug)
            {
                foreach (var c in Colliders)
                {
                    c.Render(Color.White);
                }
            }
            
        }

    }
}
