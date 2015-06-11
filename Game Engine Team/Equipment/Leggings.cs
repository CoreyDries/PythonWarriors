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
    /// A Leggings equipment that provides defence bonuses to a character. Before this class is used
    /// the spritesheet for it needs to be loaded.
    /// 
    /// Author: Jonathan Gribble
    /// Sub-Author: Corey Dries
    /// 
    /// Created: Nov 15th
    /// Last Update: Nov 24th
    /// </summary>
    public class Leggings : Equipment
    {
        public LegsType MyType { get; set; }

    }

}

