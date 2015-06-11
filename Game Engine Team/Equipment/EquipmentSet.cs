using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game_Engine_Team.Equipment
{

    [Serializable]
    public struct EquipmentSet
    {
        public HeadType Head;

        public ChestType Chest;

        public LegsType Legs;

        public WeaponType Weapon;


        public Stats GetStats()
        {
            StatsAccumulation stats = new StatsAccumulation();

            stats.Add( Equipment.Database.Get( Head ).Stats );
            stats.Add( Equipment.Database.Get( Chest ).Stats );
            stats.Add( Equipment.Database.Get( Legs ).Stats );
            stats.Add( Equipment.Database.Get( Weapon ).Stats );

            return (Stats) stats;
        }
    }

}
