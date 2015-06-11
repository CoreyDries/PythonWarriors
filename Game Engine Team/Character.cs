using Game_Engine_Team.Actors;
using Game_Engine_Team.Equipment;
using Game_Engine_Team.Texture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game_Engine_Team
{
    /*public struct EquipmentSet
    {
        public Equipment.Weapon weap;
        public Equipment.Helmet helm;
        public Equipment.Shirt shirt;
        public Equipment.Leggings legs;

        public EquipmentSet( Equipment.Weapon newWeap, Equipment.Helmet newHelm,
            Equipment.Shirt newShirt, Equipment.Leggings newLegs )
        {
            weap = newWeap;
            helm = newHelm;
            shirt = newShirt;
            legs = newLegs;
        }

        public EquipmentSet()
        {
            weap = (Weapon)Equipment.Equipment.CreateEquipment( Weapons.Null_Weapon );
            helm = (Helmet)Equipment.Equipment.CreateEquipment( Helmets.Null_Helmet );
            shirt = (Shirt)Equipment.Equipment.CreateEquipment( Shirts.Null_Shirt );
            legs = (Leggings)Equipment.Equipment.CreateEquipment( Pants.Null_Pants );
        }
    }
    [Serializable]
    public class EquipmentProxy
    {
        public Weapons weapEnum { get; set; }
        public Helmets helmEnum { get; set; }
        public Shirts shirtEnum { get; set; }
        public Pants legsEnum { get; set; }

        public EquipmentProxy( Weapons newWeap, Helmets newHelm,
            Shirts newShirt, Pants newLegs )
        {
            weapEnum = newWeap;
            helmEnum = newHelm;
            shirtEnum = newShirt;
            legsEnum = newLegs;
        }

        public EquipmentProxy()
        {
            weapEnum = Weapons.Null_Weapon;
            helmEnum = Helmets.Null_Helmet;
            shirtEnum = Shirts.Null_Shirt;
            legsEnum = Pants.Null_Pants;
        }
    }*/

    /// <summary>
    /// Represents a full character including name, experience, de-serialized character object and a 
    /// de-serialized stage object
    /// 
    /// Created by Morgan Wynne
    /// </summary>
    public class Character
    {

        public Character( string name, IPlayerProxy proxy )
        {
            Name = name;

            playerProxy = proxy;
            this.SerializedDungeon = Dungeon.Default;
            InventoryItems = new Inventory();
        }

        /// <summary>
        /// Constructor made by Morgan to begin implementing getting inventory
        /// </summary>
        public Character( string name, IPlayerProxy proxy, string serializedDungeon, string serializedInventory )
        {
            Name = name;
            playerProxy = proxy;
            this.SerializedDungeon = serializedDungeon;

            InventoryItems = string.IsNullOrEmpty( serializedInventory )
                             ? new Inventory()
                             : Inventory.Deserialize( serializedInventory );

        }

        public string Name { get; private set; }

        public bool Save()
        {
            bool success = ServerCommunicationDaemon.Instance.UpdateCharacterByCharName(
                Name, playerProxy.Serialize(), User.Instance.AuthToken );

            success = success && ServerCommunicationDaemon.Instance.UpdateStageByCharName(
                Name, SerializedDungeon, User.Instance.AuthToken );

            success = success && ServerCommunicationDaemon.Instance.SaveItemByCharName(
                Name, InventoryItems.Serialize(), User.Instance.AuthToken );

            return success;
        }

        /// <summary>
        /// TODO add description
        /// </summary>
        public string SerializedDungeon { get; set; }


        public Inventory InventoryItems { get; private set; }

        public string SerializedInventory
        {
            get {
                return InventoryItems.Serialize();
            }
        }

        /// <summary>
        /// TODO add description
        /// </summary>
        public string SerializedPlayer
        {
            get {
                return playerProxy.Serialize();
            }
        }

        public int Experience
        {
            get {
                return Stats.TotalExp;
            }
            set {
                Stats tmp = Stats;
                tmp.TotalExp = value;
                Stats = tmp;
            }
        }

        private IPlayerProxy playerProxy;

        public Player GetPlayer( Dungeon stage )
        {
            return playerProxy.New( stage );
        }

        public Dungeon GetDungeon()
        {
            return Dungeon.Deserialize( this.SerializedDungeon ) ?? new Dungeon();
        }

        public PlayerSprite Sprite
        {
            get {
                return playerProxy.Sprite;
            }
        }

        public Stats Stats
        {
            get {
                return playerProxy.Stats;
            }
            set {
                playerProxy.Stats = value;
            }
        }

        public Stats BaseStats
        {
            get {
                return playerProxy.BaseStats;
            }
        }

        public EquipmentSet Gear
        {
            get {
                return playerProxy.Gear;
            }
            set {
                playerProxy.Gear = value;
            }
        }

        /*public EquipmentProxy proxySet
        {
            get
            {
                return playerProxy.Set;
            }
            set
            {
                playerProxy.Set = value;
            }
        }*/

        /*
        /// <summary>
        /// TODO add description
        /// </summary>
        public int Experience { get; set; }

        /// <summary>
        /// TODO add description
        /// Clarify this name, probably a bad decision
        /// </summary>
        public Actors.Player CharacterPlayer { get; set; }

        /// <summary>
        /// TODO add description
        /// </summary>
        public Dungeon CharacterDungeon { get; set; }
        */
    }
}
