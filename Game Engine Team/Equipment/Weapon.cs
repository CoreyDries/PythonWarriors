using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game_Engine_Team.Actors;
using Game_Engine_Team.Texture;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game_Engine_Team.Equipment
{
    /// <summary>
    /// Weapons used by the characters.
    /// 
    /// Author: Jonathan Gribble
    /// Sub-Author: Corey Dries
    /// 
    /// Created: Nov 15th
    /// Last Update: Nov 24th
    /// </summary>
    public class Weapon : Equipment
    {
        public WeaponType MyType { get; set; }

    }
}
