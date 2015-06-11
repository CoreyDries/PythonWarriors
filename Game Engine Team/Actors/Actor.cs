using Controls;
using Game_Engine_Team.Texture;
using Microsoft.Xna.Framework;
using System;
using System.Diagnostics;
using Microsoft.Xna.Framework.Audio;

// Andrew Meckling
namespace Game_Engine_Team.Actors
{
    // This will be removed eventually
    public delegate void ActorAttack( object sender, Point loc );

    /// <summary>
    /// Callback function for when something happens to an actor.
    /// </summary>
    /// <param name="sender">The actor object which the event was triggered by.</param>
    public delegate void ActorEvent( object sender );

    /// <summary>
    /// Types of navigation modes for actors.
    /// </summary>
    public enum NavigationType
    {
        All, Ground, Flying
    }

    /// <summary>
    /// Abstract class which represents a single creature in the game. Common 
    /// derived classes of this include the main player (Player), enemies 
    /// (Enemy) or even inanimate objects which block movement (barrels, 
    /// crates, etc.)
    /// </summary>
    [DebuggerDisplay( "{Position} -- {GetType().Name}" )]
    public abstract partial class Actor : IEntity
    {
        // Events

        /// <summary>
        /// Called when the Actor attacks. Passes the location of the attack.
        /// </summary>
        public event ActorAttack Attacked;

        /// <summary>
        /// Called when the Actor dies.
        /// </summary>
        public event ActorEvent Death;

        /// <summary>
        /// Called when the actor awakes from a sleeping state.
        /// </summary>
        public event ActorEvent WakeUp;


        // Core state

        /// <summary>
        /// The Dungeon object in which the actor currently exists in.
        /// </summary>
        protected Dungeon Stage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the type of tiles the actor can 
        /// traverse.
        /// </summary>
        public NavigationType NavMode { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether the actor should be removed 
        /// from the dungeon.
        /// </summary>
        public bool Expired { get; private set; }

        /// <summary>
        /// Indicates whether the actor is alive.
        /// </summary>
        public virtual bool IsAlive
        {
            get {
                return Health > 0;
            }
        }

        /// <summary>
        /// Gets a value indicating that the actor is waiting for something to 
        /// complete (such as an animation.)
        /// </summary>
        public virtual bool RequestsDelay
        {
            get {
                return Tweening;
            }
        }


        // Position and Sprite

        /// <summary>
        /// The x-component of the actor's position in the game world.
        /// </summary>
        public int X { get { return Position.X; } }
        /// <summary>
        /// The y-component of the actor's position in the game world.
        /// </summary>
        public int Y { get { return Position.Y; } }

        /// <summary>
        /// The position of the actor in the game world in screen-tile-coordinates.
        /// </summary>
        public Point Position { get; internal set; }

        /// <summary>
        /// The sprite which will be drawn to represent the actor visually in 
        /// the game world.
        /// </summary>
        internal Sprite Sprite { get; set; }


        // Stats

        /// <summary>
        /// Gets or sets the base amount of action points the actor has 
        /// avaliable to spend each turn.
        /// </summary>
        public int BaseActionPoints { get; protected set; }

        /// <summary>
        /// Gets or sets the current amount of action points the actor has to 
        /// spend this turn.
        /// </summary>
        public int ActionPoints { get; protected set; }

        /// <summary>
        /// Gets or sets the action point cost of the actor's attack.
        /// </summary>
        public int AttackCost { get; protected set; }

        /// <summary>
        /// Gets a value which indicates whether the actor has enough action 
        /// points to perform an attack.
        /// </summary>
        public bool CanAttack
        {
            get {
                return ActionPoints >= AttackCost;
            }
        }

        /// <summary>
        /// Backing field for the MaxHealth property.
        /// </summary>
        private int maxHealth;

        /// <summary>
        /// Gets or sets the maximum health of the actor.
        /// </summary>
        public int MaxHealth
        {
            get {
                return maxHealth;
            }
            protected set {
                health = maxHealth = value;
            }
        }

        /// <summary>
        /// Backing field for Health.
        /// </summary>
        private double health;

        /// <summary>
        /// The current remaining hit-points of the actor. Normally, when this 
        /// becomes 0 the actor will die.
        /// </summary>
        public double Health
        {
            get {
                return health;
            }
            private set
            {
                healthDiff += (value - health);
                health = Math.Min( value, MaxHealth );
            }
        }

        private double healthDiff = 0;

        // Construction

        /// <summary>
        /// Creates an instance of the actor class.
        /// </summary>
        /// <param name="stage">The Dungeon in which the Actor exists in.</param>
        /// <param name="sprite">The sprite drawn to represent the Actor in the 
        /// game world.</param>
        public Actor( Dungeon stage, Sprite sprite )
        {
            Stage = stage;
            Sprite = sprite;
            Position = new Point( -1, -1 );

            MaxHealth = 400;

            BaseActionPoints = 1;
            ActionPoints = BaseActionPoints;
            AttackCost = 2;

            AttackRange = 0;
            HasMeleeAttack = true;
            NavMode = NavigationType.Ground;
        }

        /// <summary>
        /// Spawns a fresh copy of the actor with full health, action points 
        /// and other resources.
        /// </summary>
        /// <returns>A non-aliasing copy of the actor.</returns>
        internal virtual Actor Spawn()
        {
            var actor = this.MemberwiseClone() as Actor;
            actor.Sprite = this.Sprite.Spawn();
            actor.Health = this.MaxHealth;
            actor.ActionPoints = this.BaseActionPoints;
            return actor;
        }

        /// <summary>
        /// Clones the actor then sets its position.
        /// </summary>
        /// <param name="x">The x-coordinate of the new actor.</param>
        /// <param name="y">The y-coordinate of the new actor.</param>
        /// <returns>A copy of the actor with the specified position.</returns>
        [Obsolete( "Use Actor.Spawn() instead" )]
        internal Actor PlaceCopy( int x, int y )
        {
            var actor = this.Spawn() as Actor;
            actor.Position = new Point( x, y );
            return actor;
        }


        // Lifecycle

        /// <summary>
        /// Tells the actor to take its turn.
        /// </summary>
        /// <param name="skip">A value indicating whether the actor should 
        /// skip their turn.</param>
        /// <returns>True if the actor has more action points to spend and 
        /// should continue its turn during the next Update loop.</returns>
        internal virtual bool TakeTurn( bool skip = false )
        {
            return !skip && ActionPoints > 0 && !PassesTurn();
        }

        /// <summary>
        /// Updates the sprite and internal clock of the actor.
        /// </summary>
        /// <param name="gameTime">Time passed since the last call to Update.</param>
        public virtual void Update( GameTime gameTime )
        {
            if (!CurrentTile.IsTraversable(NavMode))
            {
                if(new Random().Next(10) == 0) // 10% chance for Wilhelm Scream
                    ((SoundEffect)Sounds.SoundDaemon.GetSound(SoundEffectTypes.Falling)).Play();

                this.Die();
            }

            TweenMovement( gameTime );

            if ( !Expired && !IsAlive && !RequestsDelay )
            {
                this.Die();
                this.Sprite.Tint = new Color( 150, 225, 255, 128 );
            }

            if ( Sprite != null )
                Sprite.Update( gameTime );

            DisplayHealthDiff();
        }

        private void DisplayHealthDiff()
        {
            healthDiff = (Math.Round( healthDiff * 10 ) / 10);

            if ( healthDiff < 0 )
                HeadsUpText( "" + healthDiff, Color.Red );

            else if ( healthDiff > 0 )
                HeadsUpText( "+" + healthDiff, Color.Lime );

            healthDiff = 0;
        }

        /// <summary>
        /// Draws the actor to a canvas.
        /// </summary>
        /// <param name="canvas">The canvas to draw on.</param>
        public virtual void Draw( Canvas canvas )
        {
            if ( Sprite != null )
                canvas.Draw( Sprite, Position, pixelOff );

            DrawHealthBar( canvas );
        }

        /// <summary>
        /// Draws a health bar above the actor.
        /// </summary>
        /// <param name="canvas">The canvas to draw the health bar on.</param>
        protected virtual void DrawHealthBar( Canvas canvas )
        {
            const int barHeight = 4;

            int x = (Position.X * Tile.WIDTH) + pixelOff.X;
            int y = (Position.Y * Tile.HEIGHT) + pixelOff.Y - (barHeight + 1);

            Rectangle rect = new Rectangle( x, y, Tile.WIDTH, barHeight );
            canvas.DrawRect( rect, new Color( 70, 0, 0, 200 ), true );

            rect.Width = (int) Math.Round( (this.Health / this.MaxHealth) * Tile.WIDTH );
            canvas.DrawRect( rect, new Color( 255, 50, 50, 200 ), true );
        }


        // Combat

        /// <summary>
        /// Deals damage to the actor. The damage value may be modified by 
        /// this method before actually subracting from the health.
        /// </summary>
        /// <param name="damage">The amount of incoming damage to deal to 
        /// the actor.</param>
        /// <param name="source">The source of the damage.</param>
        /// <returns>The amount of damage actually done to the actor.</returns>
        internal virtual double TakeDamage( double damage, object source )
        {
            this.Health -= damage;
            return damage;
        }

        internal virtual double TakeHealing( double healing, object source )
        {
            this.Health += healing;
            return healing;
        }

        private object deathless = new object();

        internal void HeadsUpText( string text, Color color, double lifeTime = 1 )
        {
            var report = new TextParticle( this.X, this.Y, Textures.Monospace, text, color, lifeTime );

            report.PixelOff.Y -= Tile.HEIGHT / 2 + 5;
            report.PixelOff.X += Tile.WIDTH / 2;

            Stage.AddParticle( deathless, text, report );
        }


        /// <summary>
        /// Causes the actor to attack in the specified direction.
        /// </summary>
        /// <param name="dir">The direction the attack is directed in.</param>
        /// <returns>The actor which was attacked or null.</returns>
        protected virtual Actor Attack( Direction dir )
        {
            Actor targetActor = AdjacentTile( dir ).GetActor();

            if ( targetActor != null && HasMeleeAttack )
                this.MeleeAttack( targetActor, dir );

            else if ( HasRangedAttack )
                this.RangedAttack( dir );


            if ( Attacked != null )
                Attacked( this, this.Position.Add( dir ) );

            return targetActor;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the actor has a melee attack.
        /// </summary>
        public bool HasMeleeAttack { get; protected set; }

        /// <summary>
        /// Performs a melee attack against an actor. Melee attacks take 
        /// priority over ranged attacks.
        /// </summary>
        /// <param name="victim">The actor to attack; this is guaranteed to be 
        /// non-null.</param>
        /// <param name="dir">The direction the attack was made in.</param>
        protected virtual void MeleeAttack( Actor victim, Direction dir )
        {
            this.Nudge( dir );
        }

        /// <summary>
        /// Gets or sets the range of the actor's ranged attack. If this is 
        /// less than 1 the actor cannot use ranged attacks.
        /// </summary>
        public int AttackRange { get; protected set; }

        /// <summary>
        /// Gets a value indicating whether the actor has a ranged attack.
        /// </summary>
        public bool HasRangedAttack
        {
            get {
                return AttackRange > 0;
            }
        }

        /// <summary>
        /// Performs a ranged attack in a specifed direction. There is no need 
        /// to call the base implementation in your derived implementation. 
        /// Melee attacks take priority over ranged attacks.
        /// </summary>
        /// <param name="dir">The direction the attack is aimed in.</param>
        protected virtual void RangedAttack( Direction dir )
        {
            // Default implementation does nothing.
        }


        // State

        /// <summary>
        /// Called to kill the actor. By default this method removes it from 
        /// the game.
        /// </summary>
        protected virtual void Die()
        {
            if ( Health > 0 )
                Health = 0;
            Expired = true;
            Stage.RemoveParticles( this );

            if ( Death != null )
                Death( this );
        }

        /// <summary>
        /// The number of turns the actor will sleep through.
        /// </summary>
        private int sleepCount = 0;

        /// <summary>
        /// Gets a value indicating if the actor is sleeping.
        /// </summary>
        public bool Sleeping
        {
            get {
                return sleepCount > 0;
            }
        }

        /// <summary>
        /// Tells the actor to sleep for a number of turns. If this is less 
        /// than the current number of remaining sleep turns, the current value 
        /// takes precendence over the new value.
        /// </summary>
        /// <param name="nTurns">A number of turns to sleep through.</param>
        public void Sleep( int nTurns )
        {
            if ( !Sleeping && IsAlive )
            {
                var particle = new Particle( this.X, this.Y, Textures.Get( EffectType.Sleep ) );
                particle.PixelOff.Y -= Tile.HEIGHT / 2;

                Stage.AddParticle( this, "sleep", particle );
            }

            sleepCount = Math.Max( sleepCount, nTurns );
        }

        /// <summary>
        /// Wakes the actor if it was sleeping.
        /// </summary>
        public void Wake()
        {
            if ( Sleeping && WakeUp != null )
                WakeUp( this );

            Stage.RemoveParticle( this, "sleep" );
            sleepCount = 0;
        }


        /// <summary>
        /// Calculates whether the actor should pass their turn and updates 
        /// the state to reflect that the turn was passed.
        /// </summary>
        /// <returns>True if the actor should pass the current turn; false 
        /// otherwise.</returns>
        private bool PassesTurn()
        {
            if ( Sleeping )
            {
                --sleepCount;
                ActionPoints = 0;
                return true;
            }
            else
            {
                Wake();
            }
            return false;
        }


        // Utility

        /// <summary>
        /// Calculates the amount of base damage to deliver to a target 
        /// when attacking.
        /// </summary>
        /// <returns>An amount of damage.</returns>
        internal abstract int CalculateDamage();

        /// <summary>
        /// Resets the actors ActionPoints to its BaseActionPoints value.
        /// </summary>
        internal void ResetActions()
        {
            ActionPoints = BaseActionPoints;
        }

        /// <summary>
        /// Gets or sets the tile which the actor is on. Updates the position 
        /// of the actor.
        /// </summary>
        internal virtual Tile CurrentTile
        {
            get {
                return (Position.X >= 0 || Position.Y >= 0)
                       ? Stage[ Position ]
                       : null;
            }
            set {
                this.Position = value.Position;
            }
        }

        /// <summary>
        /// Gets the tile adjacent to the actor in a specified direction.
        /// </summary>
        /// <param name="dir">The direction to check.</param>
        /// <returns>A tile adjacent to the actor's CurrentTile.</returns>
        internal Tile AdjacentTile( Direction dir )
        {
            return Stage[ this.Position.Add( dir ) ];
        }

    }
}
