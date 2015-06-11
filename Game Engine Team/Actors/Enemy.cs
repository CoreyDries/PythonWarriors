using Game_Engine_Team.Texture;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System;

// Andrew Meckling
namespace Game_Engine_Team.Actors
{

    /// <summary>
    /// Actor class which represents an in-game enemy.
    /// </summary>
    public partial class Enemy : Actor
    {
        // Stats

        /// <summary>
        /// The offense stat of the enemy. Used in calculating outgoing damage.
        /// </summary>
        public int Offense { get; protected set; }

        /// <summary>
        /// The defense stat of the enemy. Used in calculating incoming damage.
        /// </summary>
        public int Defense { get; protected set; }

        /// <summary>
        /// Gets or sets the radius at which the enemy will detect the player.
        /// </summary>
        public double SightRadius { get; protected set; }

        /// <summary>
        /// Gets or sets the extra sight radius the enemy has once they have 
        /// detected and are chasing the player.
        /// </summary>
        public double SightRadiusBonus { get; protected set; }

        /// <summary>
        /// Gets or sets the cost of the enemy in xp.
        /// </summary>
        public int ExpCost { get; protected set; }

        /// <summary>
        /// Gets or sets the cost of the enemy in gold.
        /// </summary>
        public int GoldCost { get; protected set; }

        // Utility

        /// <summary>
        /// The enumerated type representing the enemy (mainly used for 
        /// serialization.)
        /// </summary>
        internal EnemyType? MyType { get; private set; }

        /// <summary>
        /// Returns a suggested identifier for this enemy as to be used in the 
        /// Python script.
        /// </summary>
        /// <returns>A string representing the enemy.</returns>
        public override string ToString()
        {
            return MyType.HasValue ? MyType.ToString() : this.GetType().Name.ToLower();
        }


        // Construction

        /// <summary>
        /// Constructs an instance of a computer-controlled enemy.
        /// </summary>
        /// <param name="stage">The dungeon in which the enemy exists in.</param>
        /// <param name="sprite">The sprite drawn to represent the enemy in the 
        /// game world.</param>
        public Enemy( Dungeon stage, Sprite sprite )
            : base( stage, sprite )
        {
            MaxHealth = 250;
            Offense = 15;
            Defense = 0;

            ExpCost = 200;
            GoldCost = 200;

            SightRadius = 3;
            SightRadiusBonus = 2;
        }

        /// <summary>
        /// Spawns a fresh copy of the enemy with full health, action points 
        /// and other resources.
        /// </summary>
        /// <returns>A non-aliasing copy of the enemy.</returns>
        internal override Actor Spawn()
        {
            var actor = base.Spawn() as Enemy;

            if ( Emitter != null )
            {
                actor.Emitter = (ProjectileEmitter) this.Emitter.Clone();
                actor.Emitter.Range = this.AttackRange;
                actor.Emitter.Damage = this.CalculateDamage();
            }

            return actor;
        }

        /// <summary>
        /// Gets a proxy object used for serialization.
        /// </summary>
        /// <returns>A proxy object which represents the enemy.</returns>
        internal virtual EnemyProxy GetProxy()
        {
            return new EnemyProxy( this.MyType.Value, this.Position );
        }


        // Combat and Movement

        /// <summary>
        /// Attempts to move the enemy to the specified tile. This method will 
        /// only succeed if the enemy is able to be on the tile and it does not 
        /// already contain an actor. Will flip the enemy based on movement 
        /// direction (left/right).
        /// </summary>
        /// <param name="tile">The destination tile.</param>
        /// <returns>True if successful, false otherwise.</returns>
        internal override bool TryMoveTo( Tile tile )
        {
            Point pos = this.Position;
            bool moved = base.TryMoveTo( tile );

            if ( moved )
            {
                if ( pos.X < tile.X )
                    Sprite.Mirrored = true;

                else if ( pos.X > tile.X )
                    Sprite.Mirrored = false;
            }
            return moved;
        }

        /// <summary>
        /// Calculates the amount of base damage to deliver to a target 
        /// when attacking.
        /// </summary>
        /// <returns>An amount of damage.</returns>
        internal override int CalculateDamage()
        {
            return this.Offense;
        }

        /// <summary>
        /// Deals damage to the enemy's Health. This value will be reduced 
        /// by the enemy's defense before actually subracting from its health.
        /// </summary>
        /// <param name="damage">The amount of incoming damage to deal to the enemy.</param>
        /// <param name="source">The source of the damage.</param>
        /// <returns>The amount of damage actually done to the enemy.</returns>
        internal override double TakeDamage( double damage, object source )
        {
            if ( source is Player )
                pursuit = (Player) source;

            if ( Defense >= 0 )
                damage *= 100.0 / (100 + Defense);
            else
                damage *= 2 - (100.0 / (100 - Defense));

            return base.TakeDamage( damage, source );
        }

        /// <summary>
        /// Causes the enemy to attack in the specified direction. Will flip 
        /// the enemy based on attack direction (left/right).
        /// </summary>
        /// <param name="dir">The direction the attack is directed in.</param>
        /// <returns>The actor which was attacked or null.</returns>
        protected override Actor Attack( Direction dir )
        {
            if ( dir == Direction.Right )
                Sprite.Mirrored = true;

            else if ( dir == Direction.Left )
                Sprite.Mirrored = false;

            return base.Attack( dir );
        }

        /// <summary>
        /// Performs a melee attack against an actor. Melee attacks take 
        /// priority over ranged attacks.
        /// </summary>
        /// <param name="victim">The actor to attack; this is guaranteed to be 
        /// non-null.</param>
        /// <param name="dir">The direction the attack was made in.</param>
        protected override void MeleeAttack( Actor victim, Direction dir )
        {
            base.MeleeAttack( victim, dir );
            victim.TakeDamage( CalculateDamage(), this );
        }

        /// <summary>
        /// Performs a ranged attack in a specifed direction. Melee attacks 
        /// take priority over ranged attacks.
        /// </summary>
        /// <param name="dir">The direction the attack is aimed in.</param>
        protected override void RangedAttack( Direction dir )
        {
            Stage.AddParticle( this, "ranged", Emitter.Emit( this, this.Position, dir, AttackRange ) );
        }

        /// <summary>
        /// Projectile Emitter used to generate projectiles for ranged attacks.
        /// </summary>
        internal ProjectileEmitter Emitter { get; set; }


        protected override void Die()
        {

            if ( new Random().Next( 100 ) >= 20 )
            {
                int gp = (int) Math.Max( GoldCost / 10, 1 );
                Stage.AddLoot( new GoldPiece( this.Position, gp ) );
            }
            else
            {
                Stage.AddLoot( new Potion( this.Position ) );
            }

            int xp = (int) Math.Max( ExpCost / 10, 1 );

            Stage.AddExp( xp );

            HeadsUpText( xp + "xp", Color.Magenta, 3 );

            Sounds.SoundDaemon.GetSound(SoundEffectTypes.MonsterDeath).Play();
            base.Die();
        }

        // Lifecycle

        /// <summary>
        /// Tells the enemy to take its turn.
        /// </summary>
        /// <param name="skip">A value indicating whether the actor should 
        /// skip their turn.</param>
        /// <returns>True if the enemy has more action points to spend and 
        /// should continue its turn during the next Update loop.</returns>
        internal override bool TakeTurn( bool skip = false )
        {
            if ( !base.TakeTurn( skip ) )
                return false;

            if ( EnRoute && !NextTile.IsPassable( NavMode ) )
                this.RecalculateCurrentPath();

            if ( !Sleeping )
                CheckSurroundings( this.SightRadius );
            EvaluateDestination();

            if ( Chasing )
            {
                Direction? dir = Position.DirectionTo( pursuit.Position );

                if ( dir != null )
                {
                    double dist = Stage.MeasureDistance( this.Position, pursuit.Position );

                    if ( (HasMeleeAttack && dist <= 1)
                         ||
                         (HasRangedAttack && dist <= this.AttackRange
                          && Stage.IsClearShot( this.Position, pursuit.Position )) )
                    {
                        this.Attack( dir.Value );
                        ActionPoints -= AttackCost;

                        return ActionPoints > 0;
                    }
                }
            }

            if ( EnRoute && this.TryMoveTo( NextTile ) )
                RemoveNextTile();

            ActionPoints -= 1;

            return ActionPoints > 0;
        }

        public override void Draw( Controls.Canvas canvas )
        {
            //foreach ( Tile tile in Stage.VisibleTilesFrom( this.Position, SightRadius ) )
            //{
            //    canvas.DrawRect( tile.GetRekt(), new Color( 255, 0, 0, 10 ), true );
            //}
            base.Draw( canvas );
        }

    }
}
