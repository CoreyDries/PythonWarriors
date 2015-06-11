using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game_Engine_Team.Equipment
{
    /// <summary>
    /// Provides a structure for storing stats and operators to combine them 
    /// (addition, subtraction, negation).
    /// </summary>
    [Serializable]
    public struct Stats
    {
        public int TotalExp;
        public int SpentExp;

        public int MaxHealth;
        public int Offense;
        public double Defence;
        public int Dexterity;

        public Stats( int exp )
        {
            MaxHealth = 0;
            Offense = 0;
            Defence = 0;
            Dexterity = 0;

            TotalExp = exp;
            SpentExp = 0;
        }

        public static Stats operator +( Stats lhs, Stats rhs )
        {
            lhs.MaxHealth += rhs.MaxHealth;
            lhs.Offense += rhs.Offense;
            lhs.Defence += rhs.Defence;
            lhs.Dexterity += rhs.Dexterity;

            return lhs;
        }

        public static Stats operator -( Stats lhs, Stats rhs )
        {
            lhs.MaxHealth -= rhs.MaxHealth;
            lhs.Offense -= rhs.Offense;
            lhs.Defence -= rhs.Defence;
            lhs.Dexterity -= rhs.Dexterity;

            return lhs;
        }

        public static Stats operator +( Stats rhs )
        {
            return rhs;
        }

        public static Stats operator -( Stats rhs )
        {
            Stats neg = rhs;
            neg.MaxHealth = -rhs.MaxHealth;
            neg.Offense = -rhs.Offense;
            neg.Defence = -rhs.Defence;
            neg.Dexterity = -rhs.Dexterity;
            
            return neg;
        }
    }

    /// <summary>
    /// Provides intentional interface for accumulating stats from multiple 
    /// sources, such as equipment or buffs.
    /// </summary>
    public class StatsAccumulation
    {

        private Stats accumulation;

        public int MaxHealth { get { return accumulation.MaxHealth; } }

        public int Offense { get { return accumulation.Offense; } }

        public double Defence { get { return accumulation.Defence; } }

        public int Dexterity { get { return accumulation.Dexterity; } }


        /// <summary>
        /// Adds to the accumulation.
        /// </summary>
        /// <param name="stats">The stats to add.</param>
        public void Add( Stats stats )
        {
            accumulation += stats;
        }

        /// <summary>
        /// Removes from the accumulation.
        /// </summary>
        /// <param name="stats">The stats to remove.</param>
        public void Remove( Stats stats )
        {
            accumulation -= stats;
        }

        public static implicit operator Stats( StatsAccumulation stats )
        {
            return stats.accumulation;
        }

    }
}
