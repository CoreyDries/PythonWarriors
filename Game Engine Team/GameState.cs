using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game_Engine_Team
{
    public sealed class GameState : Python_Team.GameState
    {
        public double time { get; internal set; }
        public int turnNo { get; internal set; }

        public ReadOnlyCollection<Waypoint> waypoints { get; internal set; }
    }
}
