using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LD40
{
    class Stats
    {
        public static Stats instance;

        public float thirst = 0;
        public float drain = 0;
        public int injuries = 0;
        internal bool started = false;
        private Player player;


        public Stats(Player player)
        {
            instance = this;
            this.player = player;
            started = false;
        }

        public void Update()
        {
            if (player.state.CurrentState != Player.States.Dead)
            {
                thirst -= drain * 0.00075f;

                if (drain>0)
                {

                    Sfx.instance.gameMusic.Volume = thirst * thirst * 0.75f;
                    Sfx.instance.tenseMusic.Volume = (1 - thirst) * (1 - thirst) * 1f;
                }
                else
                {
                    Sfx.instance.tenseMusic.Volume = 0;
                    Sfx.instance.gameMusic.Volume = 0;
                }
            }
        }
    }
}
