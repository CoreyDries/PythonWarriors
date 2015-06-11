using Game_Engine_Team.Texture;
using Game_Engine_Team.Equipment;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Game_Engine_Team.Actors
{

    /// <summary>
    /// Actor class which represents the player's in-game character.
    /// 
    /// Author: Andrew Meckling
    /// Sub-Author: Jonathan Gribble
    /// 
    /// Last Update: Nov 18th
    /// </summary>
    public abstract class Player : Actor
    {

        private Direction? moveDir;

        private Direction? attackDir;

        private bool requestEndTurn = false;

        protected StatsAccumulation stats = new StatsAccumulation();

        internal static Stats GetBaseStats<PlayerType>()
            where PlayerType : Player
        {
            return GetBaseStats( typeof( PlayerType ) );
        }

        internal static Stats GetBaseStats( Type playerClass )
        {
            if ( playerClass == typeof( Warrior ) )
                return new Stats() {
                    MaxHealth = 713,
                    Offense = 102,
                    Defence = 38,
                    Dexterity = 30
                };

            if ( playerClass == typeof( Mage ) )
                return new Stats() {
                    MaxHealth = 512,
                    Offense = 142,
                    Defence = 19,
                    Dexterity = 45
                };

            if ( playerClass == typeof( Rogue ) )
                return new Stats() {
                    MaxHealth = 610,
                    Offense = 121,
                    Defence = 29,
                    Dexterity = 60
                };

            return new Stats();
        }


        internal Stats BaseStats
        {
            get {
                return GetBaseStats( this.GetType() );
            }
        }

        protected EquipmentSet gear;

        public abstract IPlayerProxy GetProxy();

        /// <summary>
        /// The Experience stat of the Player.
        /// </summary>
        public uint Exp { get; internal set; }

        /// <summary>
        /// The offense stat of the Player. Used in calculating outgoing damage.
        /// </summary>
        public int Offense { get { return stats.Offense; } }

        /// <summary>
        /// The defense stat of the Player. Used in calculating incoming damage.
        /// </summary>
        public double Defense { get { return stats.Defence; } }

        /// <summary>
        /// The Dexterity stat of the Player. Used in calculating Trap outcomes.
        /// </summary>
        public int Dexterity { get { return stats.Dexterity; } }

        /// <summary>
        /// The class the character is.
        /// </summary>
        public PlayerType Class { get; private set; }

        /// <summary>
        /// The direction the player is facing.
        /// </summary>
        public Direction Facing
        {
            get {
                return this.Sprite.Direction;
            }
            protected set {
                this.Sprite.Direction = value;
            }
        }

        internal new PlayerSprite Sprite
        {
            get {
                return base.Sprite as PlayerSprite;
            }
        }

        /// <summary>
        /// Constructs an instance of a user-controlled player.
        /// </summary>
        /// <param name="stage">The Dungeon in which the Player exists.</param>
        /// <param name="sprite">The sprite drawn to represent the Player in the 
        /// game world.</param>
        public Player( Dungeon stage, PlayerSprite sprite, Stats stats, EquipmentSet gear )
            : base( stage, sprite )
        {
            this.stats.Add( stats );

            this.stats.Add( BaseStats );

            this.stats.Add( gear.GetStats() );
            
            this.gear = gear;

            MaxHealth = this.stats.MaxHealth;
        }

        /// <summary>
        /// Causes the Actor to attack at the specified location. Handles the 
        /// logic for determining if an Actor exists at the specified location.
        /// </summary>
        /// <param name="targetTile">The tile to deal damage to in the Dungeon.</param>
        /// <returns>The Actor which was attacked or null.</returns>
        protected override Actor Attack( Direction dir )
        {
            this.Facing = dir;
            return base.Attack( dir );
        }
        
        internal override bool TryMove( Direction dir )
        {
            if ( Facing == dir.Opposite() )
            {
                this.Facing = dir;
                return false;
            }
            this.Facing = dir;
            return base.TryMove( dir );
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
        /// Deals damage to the Characters's Health. This value will be reduced 
        /// by the Player's Defense before subtracting the damage from its 
        /// Health.
        /// </summary>
        /// <param name="damage">The amount of incoming damage to deal to the Player.</param>
        /// <param name="source">The source of the damage.</param>
        /// <returns>The amount of damage actually done to the Player.</returns>
        internal override double TakeDamage( double damage, object source )
        {
            if ( Defense >= 0 )
                damage *= 100.0 / (100 + Defense);
            else
                damage *= 2 - (100.0 / (100 - Defense));

            return base.TakeDamage( damage, source );
        }

        /// <summary>
        /// Called to kill the Player. This tells the Stage to end the game.
        /// </summary>
        protected override void Die()
        {
            Stage.EndGame( EndType.Loss );
            base.Die();
        }

        internal override Tile CurrentTile
        {
            get {
                return base.CurrentTile;
            }
            set {
                base.CurrentTile = value;

                Loot loot = Stage.FindEntity<Loot>( this.Position );

                if ( loot != null )
                    loot.PickUp( this );
            }
        }

        /// <summary>
        /// Tells the Player to take its turn. The Player will only take 
        /// a turn if the user supplied input to the Player, unless the skip 
        /// parameter is set to true.
        /// </summary>
        /// <param name="skip">A value indicating whether to skip the Player 
        /// to take their turn (this counts as skipping their turn).</param>
        /// <returns>True if the Player took/skipped its turn.</returns>
        internal override bool TakeTurn( bool skip = false )
        {
            if ( !base.TakeTurn( skip ) || requestEndTurn )
                return false;

            if ( moveDir != null )
            {
                if ( TryMove( moveDir.Value ) )
                    ActionPoints -= 1;

                moveDir = null;
            }

            if ( attackDir != null )
            {
                Attack( attackDir.Value );
                ActionPoints -= AttackCost;

                attackDir = null;
            }

            return ActionPoints > 0;
        }

        /// <summary>
        /// The current KeyboardState representing the input commands given 
        /// by the user to the Player.
        /// </summary>
        private KeyboardState savedKeyboard;

        /// <summary>
        /// Updates input commands from the user to the Player.
        /// </summary>
        /// <param name="gameTime">Time passed since the last call to Update.</param>
        public override void Update( GameTime gameTime )
        {
            base.Update( gameTime );

            this.Sprite.Walking = this.Tweening;

            if ( ActionPoints < 1 )
                this.Sprite.Normalize();

            KeyboardState liveKeyboard = Keyboard.GetState();
            
            requestEndTurn = false;
            if ( liveKeyboard != savedKeyboard && !Stage.RequestsDelay() )
            {
                requestEndTurn = CheckKey( ref liveKeyboard, Keys.Space );

                UpdateAttackRequest( liveKeyboard );
                UpdateMoveRequest( liveKeyboard );
            
                savedKeyboard = liveKeyboard;
            }
        }

        // Andrew Meckling -- Note:
        // It turns out that the KeyboardState struct is comprised of 8 uint 
        // bitfields; which is certainly an efficient way to store the needed 
        // information. However, this still results in a value type which is 
        // 32 capital-B-Bytes in size! (256 bits). This means we need to copy 
        // that struct each time we pass it as a parameter (which is multiple 
        // times per game loop). We can avoid this costful copying by passing 
        // by "ref KeyboardState" or "KeyboardState?", but then there is the 
        // difficulty of compiler optimization with pointer parameters. Seeing 
        // as that's how most things in C# are passed, I'm pretty sure the 
        // compiler is designed to handle this scenario. Also, depending on 
        // size, structs in C# might be reference types anyways. I'm not sure 
        // what the "correct" solution is in this scenario, but seeing as the 
        // Xna library developers decided it was worth it to make KeyboardState 
        // a struct, I'm going to go with the less verbose solution. Either 
        // way, I don't think any performance gains would be noticable.


        /// <summary>
        /// Checks that a given key was pressed during this game loop.
        /// </summary>
        /// <param name="liveKeyboard">The current state of the keyboard.</param>
        /// <param name="key">The key to check.</param>
        /// <returns>True if the key was pressed recently; false if the key 
        /// isn't pressed or is still pressed from a previous game loop.</returns>
        private bool CheckKey( ref KeyboardState liveKeyboard, Keys key )
        {
            return liveKeyboard.IsKeyDown( key ) && !savedKeyboard.IsKeyDown( key );
        }
        
        private void UpdateAttackRequest( KeyboardState keyState )
        {
            if ( CheckKey( ref keyState, Keys.Left ) )
                attackDir = Direction.Left;

            else if ( CheckKey( ref keyState, Keys.Right ) )
                attackDir = Direction.Right;

            else if ( CheckKey( ref keyState, Keys.Up ) )
                attackDir = Direction.Up;

            else if ( CheckKey( ref keyState, Keys.Down ) )
                attackDir = Direction.Down;
        }

        private void UpdateMoveRequest( KeyboardState keyState )
        {
            if ( CheckKey( ref keyState, Keys.A ) )
                moveDir = Direction.Left;

            else if ( CheckKey( ref keyState, Keys.D ) )
                moveDir = Direction.Right;

            else if ( CheckKey( ref keyState, Keys.W ) )
                moveDir = Direction.Up;

            else if ( CheckKey( ref keyState, Keys.S ) )
                moveDir = Direction.Down;
        }
    }
}
