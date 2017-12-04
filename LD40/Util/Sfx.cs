using Otter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LD40
{
    public class Sfx
    {
        public static Sfx instance;


        public Sound sword;
        public Sound sword2;
        public Sound death;
        public Sound hurt;
        public Sound breaksound;
        public Sound breaksound2;
        public Sound drink;
        public Sound door;
        public Sound shot;
        public Sound teleport;
        public Sound pickup;
        public Music gameMusic;
        public Music tenseMusic;

        public Sound gameOverMusic;

        public Sfx()
        {
            instance = this;

            sword = new Sound(Global.path + "SFX/sword.wav");
            sword2 = new Sound(Global.path + "SFX/sword2.wav");

            death = new Sound(Global.path + "SFX/death.wav");
            hurt = new Sound(Global.path + "SFX/hurt.wav");
            breaksound = new Sound(Global.path + "SFX/break.wav");
            breaksound2 = new Sound(Global.path + "SFX/break2.wav");
            drink = new Sound(Global.path + "SFX/drink.wav");

            door = new Sound(Global.path + "SFX/door.wav");
            shot = new Sound(Global.path + "SFX/shot.wav");
            teleport = new Sound(Global.path + "SFX/shot.wav");
            pickup = new Sound(Global.path + "SFX/pickup.wav");

            gameOverMusic = new Sound(Global.path + "SFX/gameOver.ogg");
            gameMusic = new Music(Global.path + "SFX/game.ogg");
            tenseMusic = new Music(Global.path + "SFX/tense.ogg");
        }
    }
}
