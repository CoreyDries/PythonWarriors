using Game_Engine_Team.Actors;
using Game_Engine_Team.Equipment;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game_Engine_Team.Texture
{

    public enum WallType
    {
        Stone0,
        Stone1,
        Stone2,
        Stone3,
        Rock0,
        Rock1,
        Rock2,
        Rock3,
        Quarry0,
        Quarry1,
        Quarry2,
        Quarry3,
        ReinforcedQuarry0,
        ReinforcedQuarry1,
        ReinforcedQuarry2,
        ReinforcedQuarry3
    }

    public enum GroundType
    {
        Stone0,
        Stone1,
        Stone2,
        Wood0,
        Wood1,
        Wood2
    }

    public enum PitType
    {
        Empty0,
        Empty1,
        Empty2,
        Ice0,
        Ice1,
        Ice2,
        Water0,
        Water1,
        Water2,
        Acid0,
        Acid1,
        Acid2,
        RoughLava,
        RoughWater,
        RoughAcid
    }

    public enum PlayerType
    {
        Base,
        Rogue,
        Mage,
        Warrior,
    }

    public enum EffectType
    {
        Shuriken,
        Sleep,
        FireLarge,
        FireSmall,
        FireBall,
        Waypoint,
        WindSmall,
        WindLarge,
        OrbSmall,
        OrbLarge
    }

    public enum VectorType
    {
        RedBeam,
        BlueBeam
    }

    public enum SpriteType
    {
        Null,
        FireDemon,
        SnakeBrownLarge,
        Beholder,
        Bat,
        BlackReaper,

        GoldSmall,
        GoldMedium,
        GoldLarge,
        SilverSmall,
        SilverMedium,
        SilverLarge,
        CopperSmall,
        CopperMedium,
        CopperLarge,

        HealthPotion,
        ExitPoint,

        GUITopLeft,
        GUITop,
        GUITopRight,
        GUILeft,
        GUICentre,
        GUIRight,
        GUIBottomLeft,
        GUIBottom,
        GUIBottomRight,
        Dragon,
        Shade,
        Poison,
        Hawk,
        HawkJr,
        Sorcerer,
        Whispy,
        MegaTortoise,
        MimicBox,
        Ember,
        Golem,
        Gargoyle,
        Shaman,
    }

    /*public enum WeaponType
    {
        Assassin_Dagger, Demon_Edge, Iron_Sword, 
        Magic_Stick, Plastic_Knife, Shiv,
        Simple_Stick, Stick_Of_Truth, Toy_Sword
    }

    public enum HelmetType
    {
        Assassin_Mask, Bandana, Cooking_Pot,
        Demon_Horns, Dunce_Cap, Fake_Crown,
        Feathered_Hat, Iron_Cap, Wizards_Hat
    }

    public enum ShirtType
    {
        Assassin_Cloak, Demon_Breastplate, Elven_Cloak,
        Gandalfs_Robes, Iron_Chestplate, Jean_Jacket,
        Leather_Shirt, Magic_Clothes, T_Shirt
    }

    public enum PantsType
    {
        Assassin_Tights, Demon_Chausers, Hot_Pants,
        Iron_Leggings, Jeans, Leather_Tights,
        Mystic_Socks, Pants_That_Must_Not_Be_Named, PeterPan_Tights
    }*/

    public class Textures
    {
        private static Textures instance;

        public static SpriteFont Monospace { get; private set; }
        public static Texture2D Background { get; private set; }

        public static void Inititialize( ContentManager contentManager )
        {
            Monospace = contentManager.Load<SpriteFont>( "Fonts/MonoSpace" );
            Background = contentManager.Load<Texture2D>( "Backgrounds/MainMenu" );

            var loader = new AssetLoader( contentManager );
            instance = new Textures( loader );
        }

        private Textures( AssetLoader loader )
        {
            walls[ WallType.Stone0 ] = loader.GetWallTexture( 0, 1 );
            walls[ WallType.Stone1 ] = loader.GetWallTexture( 0, 2 );
            walls[ WallType.Stone2 ] = loader.GetWallTexture( 0, 3 );
            walls[ WallType.Stone3 ] = loader.GetWallTexture( 0, 4 );

            walls[ WallType.Rock0 ] = loader.GetWallTexture( 1, 5 );
            walls[ WallType.Rock1 ] = loader.GetWallTexture( 1, 6 );
            walls[ WallType.Rock2 ] = loader.GetWallTexture( 1, 7 );
            walls[ WallType.Rock3 ] = loader.GetWallTexture( 1, 8 );

            walls[ WallType.Quarry0 ] = loader.GetWallTexture( 0, 5 );
            walls[ WallType.Quarry1 ] = loader.GetWallTexture( 0, 6 );
            walls[ WallType.Quarry2 ] = loader.GetWallTexture( 0, 7 );
            walls[ WallType.Quarry3 ] = loader.GetWallTexture( 0, 8 );

            walls[ WallType.ReinforcedQuarry0 ] = loader.GetWallTexture( 2, 1 );
            walls[ WallType.ReinforcedQuarry1 ] = loader.GetWallTexture( 2, 2 );
            walls[ WallType.ReinforcedQuarry2 ] = loader.GetWallTexture( 2, 3 );
            walls[ WallType.ReinforcedQuarry3 ] = loader.GetWallTexture( 2, 4 );


            grounds[ GroundType.Stone0 ] = loader.GetFloorTexture( 0, 2, true );
            grounds[ GroundType.Stone1 ] = loader.GetFloorTexture( 0, 3, true );
            grounds[ GroundType.Stone2 ] = loader.GetFloorTexture( 0, 4, true );

            grounds[ GroundType.Wood0 ] = loader.GetFloorTexture( 1, 6, true );
            grounds[ GroundType.Wood1 ] = loader.GetFloorTexture( 1, 7, true );
            grounds[ GroundType.Wood2 ] = loader.GetFloorTexture( 1, 8, true );


            foreach ( PitType type in EnumUtil.GetValues<PitType>() )
            {
                pits[ type ] = loader.GetPitTexture( 1 + (int) type );
            }


            players[ PlayerType.Base ] = loader.GetPlayerSprite( "Template" );
            players[ PlayerType.Rogue ] = loader.GetPlayerSprite( "Rogue" );
            players[ PlayerType.Mage ] = loader.GetPlayerSprite( "Mage" );
            players[ PlayerType.Warrior ] = loader.GetPlayerSprite( "Warrior" );


            effects[ EffectType.Shuriken ] = loader.GetAnimatedTexture( 1, 3, "Sprites/Items/Ammo" );
            effects[ EffectType.Sleep ] = loader.GetAnimatedTexture( 5, 16, "Sprites/Objects/Effect", 2 );
            effects[ EffectType.FireLarge ] = loader.GetAnimatedTexture( 1, 21, "Sprites/Objects/Effect", 2 );
            effects[ EffectType.FireSmall ] = loader.GetAnimatedTexture( 0, 21, "Sprites/Objects/Effect", 2 );
            effects[ EffectType.OrbLarge ] = loader.GetAnimatedTexture( 7, 23, "Sprites/Objects/Effect", 2 );
            effects[ EffectType.OrbSmall ] = loader.GetAnimatedTexture( 6, 23, "Sprites/Objects/Effect", 2 );
            effects[ EffectType.FireBall ] = loader.GetAnimatedTexture( 1, 24, "Sprites/Objects/Effect", 2 );
            effects[ EffectType.Waypoint ] = loader.GetAnimatedTexture( 4, 23, "Sprites/Objects/Effect", 2 );

            effects[ EffectType.WindSmall ] = loader.GetAnimatedTexture( 4, 22, "Sprites/Objects/Effect", 2 );
            effects[ EffectType.WindLarge ] = loader.GetAnimatedTexture( 5, 22, "Sprites/Objects/Effect", 2 );

            effects[ EffectType.FireLarge ].Tint = new Color( 255, 255, 255, 225 );
            effects[ EffectType.FireSmall ].Tint = new Color( 255, 255, 255, 225 );

            effects[ EffectType.OrbLarge ].Tint = new Color( 255, 255, 255, 225 );
            effects[ EffectType.OrbSmall ].Tint = new Color( 255, 255, 255, 225 );


            vectors[ VectorType.RedBeam ] = loader.GetVectorSprite( 0, 15, "Sprites/Objects/Effect", 2 );
            vectors[ VectorType.BlueBeam ] = loader.GetVectorSprite( 0, 16, "Sprites/Objects/Effect", 2 );


            weapons[ WeaponType.Assassin_Dagger ] = loader.GetSprite( 6, 0, "Sprites/Items/LongWep" );
            weapons[ WeaponType.Demon_Edge ] = loader.GetSprite( 1, 1, "Sprites/Items/LongWep" );
            weapons[ WeaponType.Iron_Sword ] = loader.GetSprite( 2, 0, "Sprites/Items/LongWep" );
            weapons[ WeaponType.Magic_Stick ] = loader.GetSprite( 5, 0, "Sprites/Items/LongWep" );
            weapons[ WeaponType.Plastic_Knife ] = loader.GetSprite( 0, 0, "Sprites/Items/LongWep" );
            weapons[ WeaponType.Shiv ] = loader.GetSprite( 0, 1, "Sprites/Items/LongWep" );
            weapons[ WeaponType.Simple_Stick ] = loader.GetSprite( 2, 4, "Sprites/Items/LongWep" );
            weapons[ WeaponType.Stick_Of_Truth ] = loader.GetSprite( 4, 0, "Sprites/Items/LongWep" );
            weapons[ WeaponType.Toy_Sword ] = loader.GetSprite( 3, 1, "Sprites/Items/LongWep" );

            helmets[ HeadType.Assassin_Mask ] = loader.GetSprite( 0, 2, "Sprites/Items/Hat" );
            helmets[ HeadType.Bandana ] = loader.GetSprite( 0, 3, "Sprites/Items/Hat" );
            helmets[ HeadType.Cooking_Pot ] = loader.GetSprite( 0, 0, "Sprites/Items/Hat" );
            helmets[ HeadType.Demon_Horns ] = loader.GetSprite( 0, 1, "Sprites/Items/Hat" );
            helmets[ HeadType.Dunce_Cap ] = loader.GetSprite( 2, 2, "Sprites/Items/Hat" );
            helmets[ HeadType.Fake_Crown ] = loader.GetSprite( 1, 3, "Sprites/Items/Hat" );
            helmets[ HeadType.Feathered_Hat ] = loader.GetSprite( 2, 0, "Sprites/Items/Hat" );
            helmets[ HeadType.Iron_Cap ] = loader.GetSprite( 3, 0, "Sprites/Items/Hat" );
            helmets[ HeadType.Wizards_Hat ] = loader.GetSprite( 1, 2, "Sprites/Items/Hat" );

            shirts[ ChestType.Assassin_Cloak ] = loader.GetSprite( 6, 4, "Sprites/Items/Armor" );
            shirts[ ChestType.Demon_Breastplate ] = loader.GetSprite( 0, 1, "Sprites/Items/Armor" );
            shirts[ ChestType.Elven_Cloak ] = loader.GetSprite( 0, 4, "Sprites/Items/Armor" );
            shirts[ ChestType.Gandalfs_Robes ] = loader.GetSprite( 7, 4, "Sprites/Items/Armor" );
            shirts[ ChestType.Iron_Chestplate ] = loader.GetSprite( 2, 6, "Sprites/Items/Armor" );
            shirts[ ChestType.Jean_Jacket ] = loader.GetSprite( 3, 7, "Sprites/Items/Armor" );
            shirts[ ChestType.Leather_Shirt ] = loader.GetSprite( 0, 7, "Sprites/Items/Armor" );
            shirts[ ChestType.Magic_Clothes ] = loader.GetSprite( 2, 7, "Sprites/Items/Armor" );
            shirts[ ChestType.T_Shirt ] = loader.GetSprite( 7, 7, "Sprites/Items/Armor" );

            pants[ LegsType.Assassin_Tights ] = loader.GetSprite( 0, 1, "Sprites/Items/Boot" );
            pants[ LegsType.Demon_Chausers ] = loader.GetSprite( 1, 0, "Sprites/Items/Boot" );
            pants[ LegsType.Hot_Pants ] = loader.GetSprite( 0, 0, "Sprites/Items/Boot" );
            pants[ LegsType.Iron_Leggings ] = loader.GetSprite( 1, 1, "Sprites/Items/Boot" );
            pants[ LegsType.Jeans ] = loader.GetSprite( 6, 0, "Sprites/Items/Boot" );
            pants[ LegsType.Leather_Tights ] = loader.GetSprite( 5, 0, "Sprites/Items/Boot" );
            pants[ LegsType.Mystic_Socks ] = loader.GetSprite( 3, 0, "Sprites/Items/Boot" );
            pants[ LegsType.Pants_That_Must_Not_Be_Named ] = loader.GetSprite( 7, 0, "Sprites/Items/Boot" );
            pants[ LegsType.PeterPan_Tights ] = loader.GetSprite( 4, 0, "Sprites/Items/Boot" );


            // Enemies

            sprites[ SpriteType.Null ] = loader.GetSprite( 0, 0, "Sprites/Null" );
            sprites[ SpriteType.FireDemon ] = loader.GetAnimatedTexture( 0, 1, "Sprites/Characters/Demon", 2 );
            sprites[ SpriteType.SnakeBrownLarge ] = loader.GetAnimatedTexture( 2, 4, "Sprites/Characters/Reptile", 2 );
            sprites[ SpriteType.Beholder ] = loader.GetAnimatedTexture( 2, 5, "Sprites/Characters/Elemental", 2 );
            sprites[ SpriteType.Bat ] = loader.GetAnimatedTexture( 2, 11, "Sprites/Characters/Avian", 2 );
            sprites[ SpriteType.BlackReaper ] = loader.GetAnimatedTexture( 2, 5, "Sprites/Characters/Undead", 2 );

            //Additional enemy textures are by Jacob Lim
            sprites[ SpriteType.Poison ] = loader.GetAnimatedTexture( 2, 1, "Sprites/Characters/Slime", 2 );
            sprites[ SpriteType.Hawk ] = loader.GetAnimatedTexture( 7, 1, "Sprites/Characters/Avian", 2 );
            sprites[ SpriteType.HawkJr ] = loader.GetAnimatedTexture( 6, 1, "Sprites/Characters/Avian", 2 );
            sprites[ SpriteType.Sorcerer ] = loader.GetAnimatedTexture( 1, 7, "Sprites/Characters/Undead", 2 );
            sprites[ SpriteType.Whispy ] = loader.GetAnimatedTexture( 2, 4, "Sprites/Characters/Undead", 2 );
            sprites[ SpriteType.MegaTortoise ] = loader.GetAnimatedTexture( 0, 7, "Sprites/Characters/Misc", 2 );
            sprites[ SpriteType.MimicBox ] = loader.GetAnimatedTexture( 0, 8, "Sprites/Characters/Elemental", 2 );
            sprites[ SpriteType.Ember ] = loader.GetAnimatedTexture( 2, 4, "Sprites/Characters/Elemental", 2 );
            sprites[ SpriteType.Golem ] = loader.GetAnimatedTexture( 1, 1, "Sprites/Characters/Elemental", 2 );
            sprites[ SpriteType.Gargoyle ] = loader.GetAnimatedTexture( 1, 1, "Sprites/Characters/Demon", 2 );
            sprites[ SpriteType.Shaman ] = loader.GetAnimatedTexture( 3, 1, "Sprites/Characters/Misc", 2 );


            // Loot

            sprites[ SpriteType.GoldSmall ] = loader.GetSprite( 2, 1, "Sprites/Items/Money" );
            sprites[ SpriteType.GoldMedium ] = loader.GetSprite( 1, 1, "Sprites/Items/Money" );
            sprites[ SpriteType.GoldLarge ] = loader.GetSprite( 0, 1, "Sprites/Items/Money" );

            sprites[ SpriteType.SilverSmall ] = loader.GetSprite( 5, 1, "Sprites/Items/Money" );
            sprites[ SpriteType.SilverMedium ] = loader.GetSprite( 4, 1, "Sprites/Items/Money" );
            sprites[ SpriteType.SilverLarge ] = loader.GetSprite( 3, 1, "Sprites/Items/Money" );

            sprites[ SpriteType.CopperSmall ] = loader.GetSprite( 2, 0, "Sprites/Items/Money" );
            sprites[ SpriteType.CopperMedium ] = loader.GetSprite( 1, 0, "Sprites/Items/Money" );
            sprites[ SpriteType.CopperLarge ] = loader.GetSprite( 0, 0, "Sprites/Items/Money" );

            sprites[ SpriteType.HealthPotion ] = loader.GetSprite( 0, 0, "Sprites/Items/Potion" );

            sprites[ SpriteType.ExitPoint ] = loader.GetSprite( 2, 1, "Sprites/Objects/Tile" );
            

            sprites[ SpriteType.GUITopLeft ] = loader.GetSprite( 9, 7, "Sprites/GUI/GUI0" );
            sprites[ SpriteType.GUITop ] = loader.GetSprite( 10, 7, "Sprites/GUI/GUI0" );
            sprites[ SpriteType.GUITopRight ] = loader.GetSprite( 11, 7, "Sprites/GUI/GUI0" );
            sprites[ SpriteType.GUILeft ] = loader.GetSprite( 9, 8, "Sprites/GUI/GUI0" );
            sprites[ SpriteType.GUICentre ] = loader.GetSprite( 10, 8, "Sprites/GUI/GUI0" );
            sprites[ SpriteType.GUIRight ] = loader.GetSprite( 11, 8, "Sprites/GUI/GUI0" );
            sprites[ SpriteType.GUIBottomLeft ] = loader.GetSprite( 9, 9, "Sprites/GUI/GUI0" );
            sprites[ SpriteType.GUIBottom ] = loader.GetSprite( 10, 9, "Sprites/GUI/GUI0" );
            sprites[ SpriteType.GUIBottomRight ] = loader.GetSprite( 11, 9, "Sprites/GUI/GUI0" );
        }

        /// <summary>
        /// Alias for Game_Engine_Team.Actors.Enemy.Database.GetSprite().
        /// </summary>
        /// <param name="type">The enemy type whose sprite to return.</param>
        /// <returns>A non-aliasing sprite object.</returns>
        public static Sprite Get( EnemyType type )
        {
            return Enemy.Database.GetSprite( type );
        }

        public static Sprite Get( TrapType type )
        {
            return Trap.Database.GetSprite( type );
        }


        private Dictionary<WallType, WallTexture> walls = new Dictionary<WallType, WallTexture>();
        private Dictionary<GroundType, GroundTexture> grounds = new Dictionary<GroundType, GroundTexture>();
        private Dictionary<PitType, PitTexture> pits = new Dictionary<PitType, PitTexture>();
        private Dictionary<SpriteType, Sprite> sprites = new Dictionary<SpriteType, Sprite>();
        private Dictionary<PlayerType, PlayerSprite> players = new Dictionary<PlayerType, PlayerSprite>();
        private Dictionary<EffectType, AnimatedTexture> effects = new Dictionary<EffectType, AnimatedTexture>();
        private Dictionary<VectorType, VectorSprite> vectors = new Dictionary<VectorType, VectorSprite>();
        private Dictionary<WeaponType, Sprite> weapons = new Dictionary<WeaponType, Sprite>();
        private Dictionary<HeadType, Sprite> helmets = new Dictionary<HeadType, Sprite>();
        private Dictionary<ChestType, Sprite> shirts = new Dictionary<ChestType, Sprite>();
        private Dictionary<LegsType, Sprite> pants = new Dictionary<LegsType, Sprite>();


        public static WallTexture Get( WallType type )
        {
            return instance.walls[ type ].CloneSmart() as WallTexture;
        }

        public static WallType Find( WallTexture texture )
        {
            foreach ( var pair in instance.walls.ToArray() )
                if ( pair.Value == texture )
                    return pair.Key;

            throw new InvalidOperationException( "Texture not found." );
        }


        public static GroundTexture Get( GroundType type )
        {
            return instance.grounds[ type ].CloneSmart() as GroundTexture;
        }

        public static GroundType Find( GroundTexture texture )
        {
            foreach ( var pair in instance.grounds.ToArray() )
                if ( pair.Value == texture )
                    return pair.Key;

            throw new InvalidOperationException( "Texture not found." );
        }


        public static PitTexture Get( PitType type )
        {
            return instance.pits[ type ].CloneSmart() as PitTexture;
        }

        public static PitType Find( PitTexture texture )
        {
            foreach ( var pair in instance.pits.ToArray() )
                if ( pair.Value == texture )
                    return pair.Key;

            throw new InvalidOperationException( "Texture not found." );
        }


        public static Sprite Get( SpriteType type )
        {
            return instance.sprites[ type ].Spawn() as Sprite;
        }

        public static SpriteType Find( Sprite texture )
        {
            foreach ( var pair in instance.sprites.ToArray() )
                if ( pair.Value == texture )
                    return pair.Key;

            throw new InvalidOperationException( "Texture not found." );
        }


        public static PlayerSprite Get( PlayerType type )
        {
            return instance.players[ type ].Spawn() as PlayerSprite;
        }

        public static PlayerType Find( PlayerSprite texture )
        {
            foreach ( var pair in instance.players.ToArray() )
                if ( pair.Value == texture )
                    return pair.Key;

            throw new InvalidOperationException( "Texture not found." );
        }


        public static AnimatedTexture Get( EffectType type )
        {
            return instance.effects[ type ].Spawn() as AnimatedTexture;
        }

        public static EffectType Find( AnimatedTexture texture )
        {
            foreach ( var pair in instance.effects.ToArray() )
                if ( pair.Value == texture )
                    return pair.Key;

            throw new InvalidOperationException( "Texture not found." );
        }


        public static VectorSprite Get( VectorType type )
        {
            return instance.vectors[ type ].Spawn() as VectorSprite;
        }

        public static VectorType Find( VectorSprite texture )
        {
            foreach ( var pair in instance.vectors.ToArray() )
                if ( pair.Value == texture )
                    return pair.Key;

            throw new InvalidOperationException( "Texture not found." );
        }

        public static Sprite Get( WeaponType type )
        {
            return instance.weapons[ type ].Spawn() as Sprite;
        }

        public static Sprite Get( HeadType type )
        {
            return instance.helmets[ type ].Spawn() as Sprite;
        }

        public static Sprite Get( ChestType type )
        {
            return instance.shirts[ type ].Spawn() as Sprite;
        }

        public static Sprite Get( LegsType type )
        {
            return instance.pants[ type ].Spawn() as Sprite;
        }

        public static WeaponType FindWep( Sprite texture )
        {
            foreach ( var pair in instance.weapons.ToArray() )
                if ( pair.Value == texture )
                    return pair.Key;

            throw new InvalidOperationException( "Texture not found." );
        }

    }

    public static class EnumUtil
    {
        public static IEnumerable<T> GetValues<T>()
        {
            return Enum.GetValues( typeof( T ) ).Cast<T>();
        }

        public static int GetLength<T>()
        {
            return Enum.GetValues( typeof( T ) ).Length;
        }
    }
}
