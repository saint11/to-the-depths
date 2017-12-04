using Otter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LD40
{
    public class ForestEntrance : Entity
    {

        public ForestEntrance(float x, float y)
            :base(x,y)
        {
            AddComponent(new YSort(500));
            var im = AddGraphic(new Image(Global.imagePath + "/Rooms/ForestEntrance.png"));
            im.SetOrigin(320,160);
            im.Scroll = 1.5f;
            AddCollider(new BoxCollider(Global.screenWidth, Global.tile, Global.Tags.Wall));
            Collider.SetOrigin(Global.screenWidth/2,-50);
        }

        public override void Render()
        {
            if (Global.debug)
            {
                Collider.Render();
            }
            
            base.Render();
        }
    }
}
