using Otter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LD40
{
    public class DungeonMaker
    {
        public static void CreateDungeon(GameScene scene)
        {
            const int dungeonSize = 20;

            scene.rooms = new List<Room>();

            for (int i = 0; i < dungeonSize; i++)
            {
                var room = scene.Add(new Room(0, Global.screenHeight * -i, i == dungeonSize - 1, i));

                scene.rooms.Add(room);

                if (i == dungeonSize - 1)
                {
                    // Last room
                    scene.Add(new MacGuffin(Global.screenWidth / 2, Global.screenHeight / 4, room));
                }
                if (i == dungeonSize - 2)
                {
                    // Almost Last room
                    scene.Add(new BattleDoor(Global.screenWidth / 2, 0, room));
                }
                else if (i==0)
                {
                    // First Room
                    scene.Add(new CameraTarget(room));
                    scene.Add(new ForestEntrance(Global.screenWidth / 2, room.Y + 235));

                    scene.Add(new Door(Global.screenWidth / 2, 0, room));
                }
                else
                {
                    // Other Doors
                    scene.Add(new SlowDoor(Global.screenWidth / 2, 0, room));

                }


                if (i==0)
                    scene.Add(new Player(room));
            }
            
        }
    }
}
