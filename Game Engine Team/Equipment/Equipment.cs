using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Game_Engine_Team.Texture;
using Game_Engine_Team;
using Controls;
using Game_Engine_Team.Actors;

namespace Game_Engine_Team.Equipment
{
    /// <summary>
    /// The basic equipment that can be used by the various classes. Equipment can be either
    /// weapons or armour.
    /// 
    /// Author: Jonathan Gribble
    /// Sub-Author:
    /// 
    /// Created: Nov 15th
    /// Last Update: Nov 18th
    /// </summary>
    [Serializable]
    public abstract partial class Equipment
    {
        public Sprite Sprite { get; protected set; }

        public String Name { get; protected set; }

        public Stats Stats;

        /// <summary>
        /// The defence bonus the equipment provides.
        /// </summary>
        public double Defence
        {
            get {
                return Stats.Defence;
            }
            protected set {
                Stats.Defence = value;
            }
        }

        /// <summary>
        /// The damage bonus the equipment provides.
        /// </summary>
        public int Damage
        {
            get {
                return Stats.Offense;
            }
            protected set {
                Stats.Offense = value;
            }
        }

        /// <summary>
        /// The health bonus the equipment provides.
        /// </summary>
        public int Health
        {
            get {
                return Stats.MaxHealth;
            }
            protected set {
                Stats.MaxHealth = value;
            }
        }

        /// <summary>
        /// The dexterity bonus the equipment provides.
        /// </summary>
        public int Dexterity
        {
            get {
                return Stats.Dexterity;
            }
            protected set {
                Stats.Dexterity = value;
            }
        }

        /// <summary>
        /// A brief description of the equipment.
        /// </summary>
        public string Description { get; protected set; }

        /// <summary>
        /// The class(es) that are allowed to use the equipment.
        /// </summary>
        public PlayerType Restrictions { get; protected set; }

        /// <summary>
        /// The XP cost to equip the equipment.
        /// </summary>
        public int ExpCost { get; protected set; }

        public int GoldCost { get; protected set; }

        public virtual Equipment Spawn()
        {
            return (Equipment) this.MemberwiseClone();
        }

        //public Equipment( PlayerType restriction )
        //{
        //    Restrictions = restriction;
        //}
    }
}
