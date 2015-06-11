using Game_Engine_Team.Equipment;
using Game_Engine_Team.Actors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game_Engine_Team.Texture;
using System.Runtime.Serialization;

namespace Game_Engine_Team
{
    public partial class Inventory
    {
        /// <summary>
        /// The dictionaries representing the quantity a user has
        /// for any specific enumerated type of gear or enemy
        /// </summary>
        private Dictionary<WeaponType, int> Weapons = new Dictionary<WeaponType, int>();
        private Dictionary<HeadType, int> Helmets = new Dictionary<HeadType, int>();
        private Dictionary<ChestType, int> Shirts = new Dictionary<ChestType, int>();
        private Dictionary<LegsType, int> Pants = new Dictionary<LegsType, int>();
        private Dictionary<EnemyType, int> Enemies = new Dictionary<EnemyType, int>();
        private Dictionary<TrapType, int> Traps = new Dictionary<TrapType, int>();

        public Inventory()
        {
        }

        public int this[ WeaponType type ]
        {
            get {
                return Weapons.ContainsKey( type ) ? Weapons[ type ] : 0;
            }
            set {
                Weapons[ type ] = value;
            }
        }

        public int this[ HeadType type ]
        {
            get {
                return Helmets.ContainsKey( type ) ? Helmets[ type ] : 0;
            }
            set {
                Helmets[ type ] = value;
            }
        }

        public int this[ ChestType type ]
        {
            get {
                return Shirts.ContainsKey( type ) ? Shirts[ type ] : 0;
            }
            set {
                Shirts[ type ] = value;
            }
        }

        public int this[ LegsType type ]
        {
            get {
                return Pants.ContainsKey( type ) ? Pants[ type ] : 0;
            }
            set {
                Pants[ type ] = value;
            }
        }

        public int this[ EnemyType type ]
        {
            get {
                return Enemies.ContainsKey( type ) ? Enemies[ type ] : 0;
            }
            set {
                Enemies[ type ] = value;
            }
        }

        public int this[ TrapType type ]
        {
            get {
                return Traps.ContainsKey( type ) ? Traps[ type ] : 0;
            }
            set {
                Traps[ type ] = value;
            }
        }
    }
}
