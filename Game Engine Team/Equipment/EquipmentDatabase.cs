using Game_Engine_Team.Texture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game_Engine_Team.Equipment
{

    public enum HeadType
    {
        None = 0,
        Cooking_Pot,
        Bandana,
        Fake_Crown,
        Iron_Cap,
        Feathered_Hat,
        Dunce_Cap,
        Demon_Horns,
        Assassin_Mask,
        Wizards_Hat,
    }

    public enum ChestType
    {
        None = 0,
        Jean_Jacket,
        Leather_Shirt,
        T_Shirt,
        Iron_Chestplate,
        Elven_Cloak,
        Magic_Clothes,
        Demon_Breastplate,
        Assassin_Cloak,
        Gandalfs_Robes,
    }

    public enum LegsType
    {
        None = 0,
        Jeans,
        Leather_Tights,
        Hot_Pants,
        Iron_Leggings,
        PeterPan_Tights,
        Mystic_Socks,
        Demon_Chausers,
        Assassin_Tights,
        Pants_That_Must_Not_Be_Named,
    }

    public enum WeaponType
    {
        None = 0,
        Toy_Sword,
        Plastic_Knife,
        Simple_Stick,
        Iron_Sword,
        Shiv,
        Magic_Stick,
        Demon_Edge,
        Assassin_Dagger,
        Stick_Of_Truth,
    }


    public partial class Equipment
    {
        /// <summary>
        /// Singleton which provides access to an assortment of predefined 
        /// equipment objects.
        /// </summary>
        public class Database
        {
            /// <summary>
            /// The global instance of the singleton.
            /// </summary>
            private static Database instance;

            /// <summary>
            /// Intializes the database of equipment objects. Only call this after 
            /// Game_Engine_Team.Texture.Textures has been initialized.
            /// </summary>
            public static void Initialize()
            {
                instance = new Equipment.Database();
            }

            /// <summary>
            /// Dictionary of specialized enemy objects.
            /// </summary>
            private Dictionary<HeadType, Helmet> heads = new Dictionary<HeadType, Helmet>();
            private Dictionary<ChestType, Shirt> chests = new Dictionary<ChestType, Shirt>();
            private Dictionary<LegsType, Leggings> legs = new Dictionary<LegsType, Leggings>();
            private Dictionary<WeaponType, Weapon> weapons = new Dictionary<WeaponType, Weapon>();

            /// <summary>
            /// Creates and populates an instance of the equipment database.
            /// </summary>
            private Database()
            {
                heads[ HeadType.None ] = new Helmet() {
                    Sprite = new NullSprite(),
                };
                heads[ HeadType.Assassin_Mask ] = new Helmet() {
                    Dexterity = 100,
                    Defence = 45,
                    Name = "Assassin Mask",
                    Restrictions = PlayerType.Rogue,
                    Description = "If looks could kill, the wearer of this mask would need nothing else.",
                    Sprite = Textures.Get( HeadType.Assassin_Mask ),
                    ExpCost = 2400,
                    GoldCost = 2850
                };
                heads[ HeadType.Bandana ] = new Helmet() {
                    Dexterity = 20,
                    Defence = 15,
                    Name = "Bandana",
                    Restrictions = PlayerType.Rogue,
                    Description = "A simple bandanna noting the guild the theif belongs to.",
                    Sprite = Textures.Get( HeadType.Bandana ),
                    ExpCost = 300,
                    GoldCost = 350
                };
                heads[ HeadType.Cooking_Pot ] = new Helmet() {
                    Health = 200,
                    Defence = 20,
                    Name = "Cooking Pot",
                    Restrictions = PlayerType.Warrior,
                    Description = "Don't be fooled. This helmet doubles as an excellent cooking utensil.",
                    Sprite = Textures.Get( HeadType.Cooking_Pot ),
                    ExpCost = 375,
                    GoldCost = 450
                };
                heads[ HeadType.Demon_Horns ] = new Helmet() {
                    Health = 500,
                    Defence = 70,
                    Name = "Demon Horns",
                    Restrictions = PlayerType.Warrior,
                    Description = "The poor demon. He really liked these too.",
                    Sprite = Textures.Get( HeadType.Demon_Horns ),
                    ExpCost = 2400,
                    GoldCost = 2850
                };
                heads[ HeadType.Dunce_Cap ] = new Helmet() {
                    Defence = 30,
                    Damage = 45,
                    Name = "Dunce Cap",
                    Restrictions = PlayerType.Mage,
                    Description = "For when you feel like you could have played that last level a bit smarter.",
                    Sprite = Textures.Get( HeadType.Dunce_Cap ),
                    ExpCost = 1000,
                    GoldCost = 1200
                };
                heads[ HeadType.Fake_Crown ] = new Helmet() {
                    Defence = 15,
                    Damage = 10,
                    Name = "Fake Crown",
                    Restrictions = PlayerType.Mage,
                    Description = "So you got it at a toy shop. It still works. Kinda.",
                    Sprite = Textures.Get( HeadType.Fake_Crown ),
                    ExpCost = 250,
                    GoldCost = 300
                };
                heads[ HeadType.Feathered_Hat ] = new Helmet() {
                    Defence = 30,
                    Dexterity = 45,
                    Name = "Feathered Hat",
                    Restrictions = PlayerType.Rogue,
                    Description = "Stolen straight from Robin Hood's smug head.",
                    Sprite = Textures.Get( HeadType.Feathered_Hat ),
                    ExpCost = 1000,
                    GoldCost = 1200
                };
                heads[ HeadType.Iron_Cap ] = new Helmet() {
                    Defence = 50,
                    Health = 300,
                    Name = "Iron Cap",
                    Restrictions = PlayerType.Warrior,
                    Description = "Some consider this better than a pot. Even though you can't cook with it.",
                    Sprite = Textures.Get( HeadType.Iron_Cap ),
                    ExpCost = 1000,
                    GoldCost = 1255
                };
                heads[ HeadType.Wizards_Hat ] = new Helmet() {
                    Defence = 45,
                    Damage = 100,
                    Name = "Wizard's Hat",
                    Restrictions = PlayerType.Mage,
                    Description = "The wizard on the hat is spelt Wizzard. One z's more powerful than a normal wizard.",
                    Sprite = Textures.Get( HeadType.Wizards_Hat ),
                    ExpCost = 3000,
                    GoldCost = 3540
                };


                chests[ ChestType.None ] = new Shirt() {
                    Sprite = new NullSprite(),
                };
                chests[ ChestType.Assassin_Cloak ] = new Shirt() {
                    Dexterity = 100,
                    Defence = 45,
                    Name = "Assassin Cloak",
                    Restrictions = PlayerType.Rogue,
                    Description = "Though the cloak is supposed to be black, all the dried blood makes it appear brown.",
                    Sprite = Textures.Get( ChestType.Assassin_Cloak ),
                    ExpCost = 2400,
                    GoldCost = 2850
                };
                chests[ ChestType.Demon_Breastplate ] = new Shirt() {
                    Health = 500,
                    Defence = 70,
                    Name = "Demon Breastplate",
                    Restrictions = PlayerType.Warrior,
                    Description = "Taken straight from the demon itself. It's still warm... Eww",
                    Sprite = Textures.Get( ChestType.Demon_Breastplate ),
                    ExpCost = 2400,
                    GoldCost = 2850
                };
                chests[ ChestType.Elven_Cloak ] = new Shirt() {
                    Defence = 30,
                    Dexterity = 45,
                    Name = "Elven Cloak",
                    Restrictions = PlayerType.Rogue,
                    Description = "Stolen from so-called elves. At least, thats what you tell people.",
                    Sprite = Textures.Get( ChestType.Elven_Cloak ),
                    ExpCost = 1000,
                    GoldCost = 1250
                };
                chests[ ChestType.Gandalfs_Robes ] = new Shirt() {
                    Defence = 45,
                    Damage = 100,
                    Name = "Gandalf's Robes",
                    Restrictions = PlayerType.Rogue,
                    Description = "Once belonging to Gandalf The Grey. Or White. Whatever he was calling himself.",
                    Sprite = Textures.Get( ChestType.Gandalfs_Robes ),
                    ExpCost = 3000,
                    GoldCost = 3540
                };
                chests[ ChestType.Iron_Chestplate ] = new Shirt() {
                    Defence = 50,
                    Health = 300,
                    Name = "Iron Chestplate",
                    Restrictions = PlayerType.Warrior,
                    Description = "A chunk of solid iron in the rough shape of a chestplate.",
                    Sprite = Textures.Get( ChestType.Iron_Chestplate ),
                    ExpCost = 1000,
                    GoldCost = 1255
                };
                chests[ ChestType.Jean_Jacket ] = new Shirt() {
                    Health = 200,
                    Defence = 20,
                    Name = "Jean Jacket",
                    Restrictions = PlayerType.Warrior,
                    Description = "Left overs from the 90's. Still works well for absorbing Damage.",
                    Sprite = Textures.Get( ChestType.Jean_Jacket ),
                    ExpCost = 375,
                    GoldCost = 450
                };
                chests[ ChestType.Leather_Shirt ] = new Shirt() {
                    Dexterity = 20,
                    Defence = 15,
                    Name = "Leather Shirt",
                    Restrictions = PlayerType.Rogue,
                    Description = "Has a tag saying $9.99 at Kinks R' Us.",
                    Sprite = Textures.Get( ChestType.Leather_Shirt ),
                    ExpCost = 300,
                    GoldCost = 350
                };
                chests[ ChestType.Magic_Clothes ] = new Shirt() {
                    Defence = 30,
                    Damage = 45,
                    Name = "Magic Clothes",
                    Restrictions = PlayerType.Mage,
                    Description = "100% Guanranteed. Warning the magical qualities of these clothes are not 100% guaranteed.",
                    Sprite = Textures.Get( ChestType.Magic_Clothes ),
                    ExpCost = 1000,
                    GoldCost = 1250
                };
                chests[ ChestType.T_Shirt ] = new Shirt() {
                    Defence = 15,
                    Damage = 10,
                    Name = "T-Shirt",
                    Restrictions = PlayerType.Mage,
                    Description = "This T-Shirt looks a lot like a dress. Oh well, it still works.",
                    Sprite = Textures.Get( ChestType.T_Shirt ),
                    ExpCost = 250,
                    GoldCost = 300
                };


                legs[ LegsType.None ] = new Leggings() {
                    Sprite = new NullSprite(),
                };
                legs[ LegsType.Assassin_Tights ] = new Leggings() {
                    Dexterity = 100,
                    Defence = 45,
                    Name = "Assassin Tights",
                    Restrictions = PlayerType.Rogue,
                    Description = "These dark boots of assassination look remarkibly like boots.",
                    Sprite = Textures.Get( LegsType.Assassin_Tights ),
                    ExpCost = 2400,
                    GoldCost = 2850
                };
                legs[ LegsType.Demon_Chausers ] = new Leggings() {
                    Health = 500,
                    Defence = 70,
                    Name = "Demon Chausers",
                    Restrictions = PlayerType.Warrior,
                    Description = "The demon was unwilling to part with its legs, so you stole its feet instead.",
                    Sprite = Textures.Get( LegsType.Demon_Chausers ),
                    ExpCost = 2400,
                    GoldCost = 2850
                };
                legs[ LegsType.Hot_Pants ] = new Leggings() {
                    Defence = 15,
                    Damage = 10,
                    Name = "Hot Pants",
                    Restrictions = PlayerType.Mage,
                    Description = "These small pants are missing a leg hole, but that can be fixed easily enough.",
                    Sprite = Textures.Get( LegsType.Hot_Pants ),
                    ExpCost = 250,
                    GoldCost = 300
                };
                legs[ LegsType.Iron_Leggings ] = new Leggings() {
                    Defence = 50,
                    Health = 300,
                    Name = "Iron Leggings",
                    Restrictions = PlayerType.Warrior,
                    Description = "The camera slipped, so all you can see are the sweet boots of the Iron Leggings.",
                    Sprite = Textures.Get( LegsType.Iron_Leggings ),
                    ExpCost = 1200,
                    GoldCost = 1255
                };
                legs[ LegsType.Jeans ] = new Leggings() {
                    Health = 200,
                    Defence = 20,
                    Name = "Jeans",
                    Restrictions = PlayerType.Warrior,
                    Description = "Black Jeans to be specific. To be precise, black jean booties.",
                    Sprite = Textures.Get( LegsType.Jeans ),
                    ExpCost = 375,
                    GoldCost = 450
                };
                legs[ LegsType.Leather_Tights ] = new Leggings() {
                    Dexterity = 20,
                    Defence = 15,
                    Name = "Leather Tights",
                    Restrictions = PlayerType.Rogue,
                    Description = "These tights seem to come from the same store as the leather shirt.  Also. Boots.",
                    Sprite = Textures.Get( LegsType.Leather_Tights ),
                    ExpCost = 300,
                    GoldCost = 350
                };
                legs[ LegsType.Mystic_Socks ] = new Leggings() {
                    Defence = 30,
                    Damage = 45,
                    Name = "Mystic Socks",
                    Restrictions = PlayerType.Mage,
                    Description = "These green socks have their own set of powerful arcane whiskers.",
                    Sprite = Textures.Get( LegsType.Mystic_Socks ),
                    ExpCost = 1000,
                    GoldCost = 1250
                };
                legs[ LegsType.Pants_That_Must_Not_Be_Named ] = new Leggings() {
                    Defence = 45,
                    Damage = 100,
                    Name = "Pants That Must Not Be Named",
                    Restrictions = PlayerType.Mage,
                    Description = "These unamed pants resemble a shoe with buckle. Don't let their appearance fool you. No one dares name these pants.",
                    Sprite = Textures.Get( LegsType.Pants_That_Must_Not_Be_Named ),
                    ExpCost = 3000,
                    GoldCost = 3540
                };
                legs[ LegsType.PeterPan_Tights ] = new Leggings() {
                    Defence = 30,
                    Dexterity = 45,
                    Name = "PeterPan Tights",
                    Restrictions = PlayerType.Rogue,
                    Description = "Due to copyright issues, all that can be shown of these pants are their boots.",
                    Sprite = Textures.Get( LegsType.PeterPan_Tights ),
                    ExpCost = 1000,
                    GoldCost = 1250
                };


                weapons[ WeaponType.None ] = new Weapon() {
                    Sprite = new NullSprite(),
                };
                weapons[ WeaponType.Assassin_Dagger ] = new Weapon() {
                    Dexterity = 65,
                    Damage = 45,
                    Restrictions = PlayerType.Rogue,
                    Name = "Assassin Dagger",
                    Description = "Pronged for your pleasure.",
                    Sprite = Textures.Get( WeaponType.Assassin_Dagger ),
                    ExpCost = 2700,
                    GoldCost = 3200
                };
                weapons[ WeaponType.Demon_Edge ] = new Weapon() {
                    Damage = 30,
                    Health = 700,
                    Restrictions = PlayerType.Warrior,
                    Name = "Demon Edge",
                    Description = "An edge made of demon bone. Excellent for boosting one's health before death.",
                    Sprite = Textures.Get( WeaponType.Demon_Edge ),
                    ExpCost = 1200,
                    GoldCost = 1500
                };
                weapons[ WeaponType.Iron_Sword ] = new Weapon() {
                    Damage = 20,
                    Health = 200,
                    Restrictions = PlayerType.Warrior,
                    Name = "Iron Sword",
                    Description = "A simple sword made of iron. Can be found everywhere.",
                    Sprite = Textures.Get( WeaponType.Iron_Sword ),
                    ExpCost = 1050,
                    GoldCost = 1325
                };
                weapons[ WeaponType.Magic_Stick ] = new Weapon() {
                    Damage = 40,
                    Restrictions = PlayerType.Mage,
                    Name = "Magic Stick",
                    Description = "This stick is rumoured to be magical. You see no reason to believe otherwise.",
                    Sprite = Textures.Get( WeaponType.Magic_Stick ),
                    ExpCost = 1550,
                    GoldCost = 1900
                };
                weapons[ WeaponType.Plastic_Knife ] = new Weapon() {
                    Dexterity = 10,
                    Damage = 7,
                    Restrictions = PlayerType.Rogue,
                    Name = "Plastic Knife",
                    Description = "This knife is more stick than blade.",
                    Sprite = Textures.Get( WeaponType.Plastic_Knife ),
                    ExpCost = 350,
                    GoldCost = 400
                };
                weapons[ WeaponType.Shiv ] = new Weapon() {
                    Dexterity = 40,
                    Damage = 30,
                    Restrictions = PlayerType.Rogue,
                    Name = "Shiv",
                    Description = "This shive still has a longer handle than blade.",
                    Sprite = Textures.Get( WeaponType.Shiv ),
                    ExpCost = 2200,
                    GoldCost = 2600
                };
                weapons[ WeaponType.Simple_Stick ] = new Weapon() {
                    Damage = 15,
                    Restrictions = PlayerType.Mage,
                    Name = "Simple Stick",
                    Description = "It's a simple stick. Maybe it has magic? Probably not.",
                    Sprite = Textures.Get( WeaponType.Simple_Stick ),
                    ExpCost = 350,
                    GoldCost = 400
                };
                weapons[ WeaponType.Stick_Of_Truth ] = new Weapon() {
                    Damage = 100,
                    Restrictions = PlayerType.Mage,
                    Name = "Stick of Truth",
                    Description = "This stick is the truth of all. Still not magical though.",
                    Sprite = Textures.Get( WeaponType.Stick_Of_Truth ),
                    ExpCost = 3000,
                    GoldCost = 3500
                };
                weapons[ WeaponType.Toy_Sword ] = new Weapon() {
                    Damage = 7,
                    Health = 70,
                    Restrictions = PlayerType.Warrior,
                    Name = "Toy Sword",
                    Description = "It may squeak everytime you hasHit something, but it's still sword-shaped.",
                    Sprite = Textures.Get( WeaponType.Toy_Sword ),
                    ExpCost = 375,
                    GoldCost = 440
                };
            }

            internal static Helmet Get( HeadType type )
            {
                var helmet = (Helmet) instance.heads[ type ].Spawn();
                helmet.MyType = type;
                return helmet;
            }

            internal static Shirt Get( ChestType type )
            {
                var chest = (Shirt) instance.chests[ type ].Spawn();
                chest.MyType = type;
                return chest;
            }

            internal static Leggings Get( LegsType type )
            {
                var leg = (Leggings) instance.legs[ type ].Spawn();
                leg.MyType = type;
                return leg;
            }

            internal static Weapon Get( WeaponType type )
            {
                var weapon = (Weapon) instance.weapons[ type ].Spawn();
                weapon.MyType = type;
                return weapon;
            }


            internal static Sprite GetSprite( HeadType type )
            {
                return instance.heads[ type ].Sprite.Spawn();
            }

            internal static int GetExpCost( HeadType type )
            {
                return instance.heads[ type ].ExpCost;
            }

            internal static int GetGoldCost( HeadType type )
            {
                return instance.heads[ type ].GoldCost;
            }

        }

    }
}
