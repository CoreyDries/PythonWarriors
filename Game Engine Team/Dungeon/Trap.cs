using Controls;
using Game_Engine_Team.Actors;
using Game_Engine_Team.Texture;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game_Engine_Team
{

    public delegate void TrapActivate( Trap sender );


    public enum TrapType
    {
        Simple,
        Sleep,
        Fire,
        Wind,
    }


    public class Trap : IEntity
    {
        public double Damage { get; internal set; }

        internal TrapType MyType { get; set; }

        public NavigationType NavMode { get; protected set; }

        protected Dungeon Stage { get; set; }

        public Sprite Sprite { get; private set; }

        public Point Position { get; internal set; }

        public int X { get { return Position.X; } }
        public int Y { get { return Position.Y; } }


        public bool Expired { get; private set; }

        /// <summary>
        /// Gets or sets the cost of the enemy in xp.
        /// </summary>
        public int ExpCost { get; protected set; }

        /// <summary>
        /// Gets or sets the cost of the enemy in gold.
        /// </summary>
        public int GoldCost { get; protected set; }

        public event TrapActivate Trigger;


        protected Trap( Sprite sprite )
        {
            Sprite = sprite;
            NavMode = NavigationType.Ground;
        }

        public override string ToString()
        {
            return MyType.ToString() + "Trap";
        }

        internal TrapProxy GetProxy()
        {
            return new TrapProxy( MyType, Position );
        }


        internal virtual Trap Spawn()
        {
            var trap = (Trap) this.MemberwiseClone();
            trap.Sprite = this.Sprite.Spawn();
            return trap;
        }

        internal virtual void Activate( Actor victim )
        {
            victim.TakeDamage( Damage, this );

            if ( Trigger != null )
                Trigger( this );

            Expire();
        }

        public virtual void Update( GameTime gameTime )
        {
            if ( Sprite != null )
                Sprite.Update( gameTime );
        }

        public virtual void Draw( Canvas canvas )
        {
            if ( Sprite != null )
                canvas.Draw( Sprite, Position );
        }

        public void Expire()
        {
            Expired = true;
        }


        public class Database
        {

            private static Trap.Database instance;

            public static void Initialize()
            {
                instance = new Trap.Database();
            }

            private Dictionary<TrapType, Trap> traps = new Dictionary<TrapType,Trap>();

            private Database()
            {

                traps[ TrapType.Simple ] = new Trap( Textures.Get( SpriteType.CopperLarge ) );

                traps[ TrapType.Fire ] = new Trap( Textures.Get( EffectType.FireSmall ) )
                {
                    Damage = 100,
                    ExpCost = 500,
                    GoldCost = 700,
                };

                traps[ TrapType.Sleep ] = new SleepTrap( Textures.Get( EffectType.Sleep ) )
                {
                    Duration = 2,
                    ExpCost = 500,
                    GoldCost = 700,
                };

                traps[ TrapType.Wind ] = new WindTrap( Textures.Get( EffectType.WindSmall ) )
                {
                    ExpCost = 1000,
                    GoldCost = 700,
                };

            }

            public static Trap Get( TrapType type, Dungeon stage )
            {
                var trap = instance.traps[ type ].Spawn();
                trap.MyType = type;
                trap.Stage = stage;
                return trap;
            }

            public static Sprite GetSprite( TrapType type )
            {
                return instance.traps[ type ].Sprite.Spawn();
            }

            public static int GetExpCost( TrapType type )
            {
                return instance.traps[ type ].ExpCost;
            }

            public static int GetGoldCost( TrapType type )
            {
                return instance.traps[ type ].GoldCost;
            }

        }

    }


    public class WindTrap : Trap
    {
        private Direction direction;

        public WindTrap( Sprite sprite )
            : base( sprite )
        {
        }

        internal override Trap Spawn()
        {
            var trap = (WindTrap) base.Spawn();
            trap.direction = direction = (Game_Engine_Team.Direction) new Random().Next( 4 );
            return trap;
        }

        internal override void Activate( Actor victim )
        {
            Tile target = victim.AdjacentTile( direction );

            for ( int i = 1; i < 2 && target.IsTraversable( victim.NavMode ); ++i )
                target = target.AdjacentTile( direction );

            victim.TryPushTo( target );

            base.Activate( victim );
        }

        public string Direction
        {
            get {
                return direction.ToString();
            }
            set { 
                switch ( value.ToLower() )
                {
                    case "down":
                        direction = Game_Engine_Team.Direction.Down;
                        break;

                    case "left":
                        direction = Game_Engine_Team.Direction.Left;
                        break;

                    case "right":
                        direction = Game_Engine_Team.Direction.Right;
                        break;

                    case "up":
                        direction = Game_Engine_Team.Direction.Up;
                        break;
                }
            }
        }
    }

    public class SleepTrap : Trap
    {
        public int Duration { get; internal set; }

        public SleepTrap( Sprite sprite )
            : base( sprite )
        {
        }


        internal override void Activate( Actor victim )
        {
            victim.Sleep( Duration );

            base.Activate( victim );
        }

    }


    [Serializable]
    public class TrapProxy
    {
        /// <summary>
        /// The type of the enemy.
        /// </summary>
        private TrapType type;

        /// <summary>
        /// The pos of the enemy.
        /// </summary>
        private Point pos;

        /// <summary>
        /// Creates a proxy object for serializing enemies.
        /// <param name="type"></param>
        /// <param name="pos"></param>
        public TrapProxy( TrapType type, Point pos )
        {
            this.type = type;
            this.pos = pos;
        }

        /// <summary>
        /// Makes a new trap.
        /// </summary>
        /// <returns>A non-aliasing trap object.</returns>
        public Trap New( Dungeon stage )
        {
            var trap = Trap.Database.Get( type, stage );
            trap.Position = pos;
            return trap;
        }
    }


}
