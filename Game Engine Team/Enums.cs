using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game_Engine_Team
{

    /// <summary>
    /// The slots that equipment can be equipped to.
    /// </summary>
    public enum EquipmentSlots { Body, Helmet, Legs, Weapon }

    /// <summary>
    /// The current weapons available to be created.
    /// </summary>
    public enum Weapons { Toy_Sword, Plastic_Knife, Simple_Stick, Iron_Sword, Shiv, Magic_Stick, Demon_Edge, Assassin_Dagger, Stick_Of_Truth, Null_Weapon }

    /// <summary>
    /// The available body armours to be created and worn.
    /// </summary>
    public enum Shirts { Jean_Jacket, Leather_Shirt, T_Shirt, Iron_Chestplate, Elven_Cloak, Magic_Clothes, Demon_Breastplate, Assassin_Cloak, Gandalfs_Robes, Null_Shirt }

    /// <summary>
    /// The available leg armours to be created.
    /// </summary>
    public enum Pants { Jeans, Leather_Tights, Hot_Pants, Iron_Leggings, PeterPan_Tights, Mystic_Socks, Demon_Chausers, Assassin_Tights, Pants_That_Must_Not_Be_Named, Null_Pants }

    /// <summary>
    /// The available helmets to be created an worn.
    /// </summary>
    public enum Helmets { Cooking_Pot, Bandana, Fake_Crown, Iron_Cap, Feathered_Hat, Dunce_Cap, Demon_Horns, Assassin_Mask, Wizards_Hat, Null_Helmet }

    public enum MusicTypes { Menus, Battle, Victory }

    public enum SoundEffectTypes { Falling, MonsterDeath, WeaponAttack, MagicAttack, PickUpCoin }
}
