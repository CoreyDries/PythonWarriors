using Game_Engine_Team.Equipment;
using Game_Engine_Team.Texture;
using IronPython.Runtime;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Audio;

namespace Game_Engine_Team.Actors
{
    public sealed class Warrior : Player
    {
        public override IPlayerProxy GetProxy()
        {
            Stats stats = this.stats;
            stats -= this.gear.GetStats() + BaseStats;
            return new PlayerProxy<Warrior>( stats, this.gear );
        }

        /// <summary>
        /// Constructs an instance of a user-controlled player.
        /// </summary>
        /// <param name="stage">The Dungeon in which the Player exists.</param>
        /// <param name="sprite">The sprite drawn to represent the Player in the 
        /// game world.</param>
        public Warrior( Dungeon stage, Stats stats, EquipmentSet gear )
            : base( stage, Textures.Get( PlayerType.Warrior ), stats, gear )
        {
            BaseActionPoints = 2;

            AttackRange = 3;
        }

        protected override void MeleeAttack( Actor victim, Direction dir )
        {
            base.MeleeAttack( victim, dir );
            victim.TakeDamage( CalculateDamage(), this );
            ((SoundEffect)Sounds.SoundDaemon.GetSound(SoundEffectTypes.WeaponAttack)).Play();
        }

        private Tile GetDashLanding( Point target )
        {
            Tile targetTile = this.CurrentTile; // The tile we want to land on.

            foreach ( var tile in Stage.TileRange( this.Position, target, 1, AttackRange ) )
            {
                if ( tile.IsTraversable( NavMode ) )
                    targetTile = tile;

                if ( !tile.IsPassable( NavigationType.Flying ) )
                    break;
            }

            return targetTile;
        }

        protected override void RangedAttack( Direction dir )
        {
            ((SoundEffect)Sounds.SoundDaemon.GetSound(SoundEffectTypes.WeaponAttack)).Play();
            
            Tile targetTile = GetDashLanding( this.Position.Add( dir ) );

            Actor victim = null;

            foreach ( Tile tile in Stage.TileRange( this.Position, targetTile.Position, 1, 1 ) )
            {
                victim = tile.GetActor();
                if ( victim != null )
                {
                    targetTile = victim.CurrentTile;
                    break;
                }
            }

            Tile bonusTile = targetTile.AdjacentTile( dir );
            Tile penultimateTile = targetTile.AdjacentTile( dir.Opposite() );

            if ( victim != null )
            {
                bool pushSuccess = victim.TryPushTo( bonusTile );
                bool nudgeSuccess = !pushSuccess && penultimateTile.IsPassable( NavMode );

                if ( nudgeSuccess )
                    victim.Nudge( dir );

                if ( pushSuccess || nudgeSuccess )
                {
                    victim.TakeDamage( CalculateDamage(), this );
                    victim.Sleep( 1 );
                }
                else
                {
                    victim = null;
                }
            }

            if ( targetTile == CurrentTile )
                this.Nudge( dir );

            if ( !this.TryMoveTo( targetTile ) )
                this.TryMoveTo( penultimateTile );

            if ( victim == null )
                this.Sleep( 1 );
        }

    }
}
