using Game_Engine_Team.Equipment;
using Game_Engine_Team.Texture;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Audio;

namespace Game_Engine_Team.Actors
{
    public sealed class Rogue : Player
    {

        private ProjectileEmitter emitter;

        public override IPlayerProxy GetProxy()
        {
            Stats stats = this.stats;
            stats -= this.gear.GetStats() + BaseStats;
            return new PlayerProxy<Rogue>( stats - this.gear.GetStats(), this.gear );
        }


        /// <summary>
        /// Constructs an instance of a user-controlled player.
        /// </summary>
        /// <param name="stage">The Dungeon in which the Player exists.</param>
        /// <param name="sprite">The sprite drawn to represent the Player in the 
        /// game world.</param>
        public Rogue( Dungeon stage, Stats stats, EquipmentSet gear )
            : base( stage, Textures.Get( PlayerType.Rogue ), stats, gear )
        {
            BaseActionPoints = 2;
            AttackCost = 1;

            AttackRange = 4;

            emitter = Projectiles.Get( ProjectileType.Shuriken );
            emitter.Range = this.AttackRange;
            emitter.Damage = this.CalculateDamage();
        }

        protected override void MeleeAttack( Actor victim, Direction dir )
        {
            base.MeleeAttack( victim, dir );
            //victim.TakeDamage( CalculateDamage() * 0.7 );
            victim.Sleep( 2 );
            ((SoundEffect)Sounds.SoundDaemon.GetSound(SoundEffectTypes.WeaponAttack)).Play();
        }

        protected override void RangedAttack( Direction dir )
        {
            ((SoundEffect) Sounds.SoundDaemon.GetSound( SoundEffectTypes.WeaponAttack )).Play();
            Stage.AddParticle( this, "shuriken", emitter.Emit( this, this.Position, dir ) );
        }

    }
}
