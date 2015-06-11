using Game_Engine_Team.Texture;
using System.Collections.Generic;

// Andrew Meckling
namespace Game_Engine_Team.Actors
{
    /// <summary>
    /// Enumerated type of all enemies.
    /// </summary>
    public enum EnemyType
    {
        FireDemon,
        Snake,
        Beholder,
        Bat,
        Reaper,
        Poison,
        Hawk,
        HawkJr,
        Sorcerer,
        Whispy,
        MegaTortoise,
        Mimic,
        Ember,
        Golem,
        Gargoyle,
        Shaman,
    }

    public partial class Enemy
    {

        private Enemy( SpriteType type )
            : this( null, Textures.Get( type ) )
        {
        }

        /// <summary>
        /// Singleton which provides access to an assortment of predefined 
        /// enemy objects.
        /// </summary>
        public class Database
        {
            /// <summary>
            /// The global instance of the singleton.
            /// </summary>
            private static Database instance;

            /// <summary>
            /// Intializes the database of enemy objects. Only call this after 
            /// Game_Engine_Team.Texture.Textures and 
            /// Game_Engine_Team.Projectiles have been initialized.
            /// </summary>
            public static void Initialize()
            {
                instance = new Enemy.Database();
            }

            /// <summary>
            /// Dictionary of specialized enemy objects.
            /// </summary>
            private Dictionary<EnemyType, Enemy> roster = new Dictionary<EnemyType, Enemy>();

            /// <summary>
            /// Creates and populates an instance of the enemy database.
            /// </summary>
            private Database()
            {
                roster[ EnemyType.Snake ] = new Enemy( SpriteType.SnakeBrownLarge ) {
                    BaseActionPoints = 1,
                    MaxHealth = 250,
                    Offense = 15,
                    Defense = 0,
                    SightRadius = 2,
                    ExpCost = 100,
                    GoldCost = 200,
                };


                roster[ EnemyType.FireDemon ] = new Enemy( SpriteType.FireDemon ) {
                    Emitter = Projectiles.Get( ProjectileType.WallOfFire ),
                    BaseActionPoints = 2,
                    MaxHealth = 1000,
                    Offense = 40,
                    Defense = 10,
                    SightRadius = 3.5,
                    ExpCost = 400,
                    GoldCost = 480,
                    AttackRange = 3,
                    HasMeleeAttack = false,
                };


                roster[ EnemyType.Beholder ] = new Enemy( SpriteType.Beholder ) {
                    Emitter = Projectiles.Get( ProjectileType.RedBeam ),
                    BaseActionPoints = 2,
                    MaxHealth = 1800,
                    Offense = 15,
                    Defense = 15,
                    SightRadius = 3,
                    SightRadiusBonus = 3,
                    AttackRange = 4,
                    NavMode = NavigationType.Flying,
                    HasMeleeAttack = false,
                    ExpCost = 900,
                    GoldCost = 1400,
                };


                roster[ EnemyType.Bat ] = new Enemy( SpriteType.Bat ) {
                    Emitter = Projectiles.Get( ProjectileType.Whirlwind ),
                    BaseActionPoints = 1,
                    MaxHealth = 250,
                    Offense = 10,
                    Defense = 0,
                    SightRadius = 3,
                    ExpCost = 100,
                    GoldCost = 150,
                    AttackRange = 4,
                    NavMode = NavigationType.Flying,
                    HasMeleeAttack = true,
                };


                roster[ EnemyType.Reaper ] = new Enemy( SpriteType.BlackReaper )
                {
                    BaseActionPoints = 3,
                    MaxHealth = 450,
                    Offense = 20,
                    Defense = 0,
                    ExpCost = 400,
                    GoldCost = 600,
                    NavMode = NavigationType.Flying,
                    SightRadius = 4,
                };
                

                //The following enemies were made by Jacob Lim

                roster [EnemyType.Poison] = new Enemy( SpriteType.Poison ) {
                    BaseActionPoints = 2,
                    MaxHealth = 200,
                    Offense = 10,
                    Defense = 0,
                    SightRadius = 3,
                    ExpCost = 100,
                    GoldCost = 180,
                    NavMode = NavigationType.Ground,
                    HasMeleeAttack = true,
                };


                roster[EnemyType.Hawk] = new Enemy(SpriteType.Hawk)
                {
                    BaseActionPoints = 2,
                    MaxHealth = 250,
                    Offense = 15,
                    Defense = 0,
                    SightRadius = 3,
                    ExpCost = 100,
                    GoldCost = 180,
                    NavMode = NavigationType.Flying,
                    HasMeleeAttack = true,
                };

                roster[EnemyType.HawkJr] = new Enemy(SpriteType.HawkJr)
                {
                    BaseActionPoints = 2,
                    MaxHealth = 180,
                    Offense = 10,
                    Defense = 0,
                    SightRadius = 3,
                    ExpCost = 80,
                    GoldCost = 150,
                    NavMode = NavigationType.Flying,
                    HasMeleeAttack = true,
                };

                roster[EnemyType.Sorcerer] = new Enemy(SpriteType.Sorcerer)
                {
                    Emitter = Projectiles.Get(ProjectileType.MagicOrb),
                    BaseActionPoints = 1,
                    MaxHealth = 1400,
                    Offense = 50,
                    Defense = 30,
                    SightRadius = 5,
                    ExpCost = 1000,
                    GoldCost = 2000,
                    AttackCost = 1,
                    NavMode = NavigationType.Ground,
                    AttackRange = 5,
                    HasMeleeAttack = false,
                };

                roster[EnemyType.Whispy] = new Enemy(SpriteType.Whispy)
                {
                    Emitter = Projectiles.Get(ProjectileType.MagicOrb),
                    BaseActionPoints = 4,
                    MaxHealth = 150,
                    Offense = 10,
                    Defense = 0,
                    SightRadius = 2,
                    SightRadiusBonus = 5,
                    ExpCost = 400,
                    GoldCost = 500,
                    AttackCost = 2,
                    NavMode = NavigationType.Flying,
                    HasMeleeAttack = true,
                };

                roster[EnemyType.MegaTortoise] = new Enemy(SpriteType.MegaTortoise)
                {
                    BaseActionPoints = 1,
                    MaxHealth = 1000,
                    Offense = 15,
                    Defense = 60,
                    SightRadius = 3,
                    ExpCost = 800,
                    GoldCost = 1800,
                    AttackCost = 1,
                    NavMode = NavigationType.Flying,
                    HasMeleeAttack = true,
                };

                roster[EnemyType.Mimic] = new Enemy(SpriteType.MimicBox)
                {
                    BaseActionPoints = 2,
                    MaxHealth = 500,
                    Offense = 40,
                    Defense = 20,
                    SightRadius = 3,
                    ExpCost = 400,
                    GoldCost = 800,
                    AttackCost = 3,
                    NavMode = NavigationType.Ground,
                    HasMeleeAttack = true,
                };

                roster[EnemyType.Ember] = new Enemy(SpriteType.Ember)
                {
                    BaseActionPoints = 1,
                    MaxHealth = 400,
                    Offense = 25,
                    Defense = 0,
                    SightRadius = 3,
                    ExpCost = 400,
                    GoldCost = 800,
                    AttackCost = 1,
                    NavMode = NavigationType.Ground,
                    HasMeleeAttack = true,
                };

                roster[EnemyType.Golem] = new Enemy(SpriteType.Golem)
                {
                    BaseActionPoints = 2,
                    MaxHealth = 900,
                    Offense = 20,
                    Defense = 30,
                    SightRadius = 3,
                    ExpCost = 700,
                    GoldCost = 1000,
                    NavMode = NavigationType.Ground,
                    HasMeleeAttack = true,
                };

                roster[EnemyType.Gargoyle] = new Enemy(SpriteType.Gargoyle)
                {
                    BaseActionPoints = 2,
                    MaxHealth = 600,
                    Offense = 15,
                    Defense = 10,
                    SightRadius = 3,
                    ExpCost = 400,
                    GoldCost = 800,
                    NavMode = NavigationType.Flying,
                    HasMeleeAttack = true,
                };

                roster[EnemyType.Shaman] = new Enemy(SpriteType.Shaman)
                {
                    Emitter = Projectiles.Get(ProjectileType.BlueBeam),
                    BaseActionPoints = 2,
                    MaxHealth = 700,
                    Offense = 15,
                    Defense = 15,
                    SightRadius = 3,
                    ExpCost = 400,
                    GoldCost = 1000,
                    AttackRange = 4,
                    HasMeleeAttack = true,
                };
            }

            /// <summary>
            /// Gets the specified enemy from the database for use on the 
            /// specified dungeon.
            /// </summary>
            /// <param name="type">The enemy type to return.</param>
            /// <param name="stage">The dungeon object the enemy is returned to.</param>
            /// <returns>A non-aliasing enemy object.</returns>
            internal static Enemy Get( EnemyType type, Dungeon stage )
            {
                var enemy = (Enemy) instance.roster[ type ].Spawn();
                enemy.MyType = type;
                enemy.Stage = stage;
                return enemy;
            }

            /// <summary>
            /// Gets the sprite associated with the specified enemy from the 
            /// database.
            /// </summary>
            /// <param name="type">The enemy type whose sprite to return.</param>
            /// <returns>A non-aliasing sprite object.</returns>
            internal static Sprite GetSprite( EnemyType type )
            {
                return instance.roster[ type ].Sprite.Spawn();
            }

            internal static int GetExpCost( EnemyType type )
            {
                return instance.roster[ type ].ExpCost;
            }

            internal static int GetGoldCost( EnemyType type )
            {
                return instance.roster[ type ].GoldCost;
            }

        }

    }
}
