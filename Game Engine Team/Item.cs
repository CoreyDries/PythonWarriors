using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game_Engine_Team
{
    class Item
    {
        /// <summary>
        /// This class is an Item class intended to represent how Items are made in the database.
        /// ...It may not be necessary if Item Lists are implemented as a Dictionary.
        /// 
        /// Created by: Jacob Lim
        /// </summary>
        public Item(string Key, int Value)
        {
            this.Key = Key;
            this.Value = Value;
        }

        /// <summary>
        /// TODO add description
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// TODO add description
        /// </summary>
        public int Value { get; set; }
    }
}
