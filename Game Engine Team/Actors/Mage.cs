using Game_Engine_Team.Equipment;
using Game_Engine_Team.Texture;
using Microsoft.Xna.Framework;
using System;
using Microsoft.Xna.Framework.Audio;

namespace Game_Engine_Team.Actors
{
    public sealed class Mage : Player
    {

        private ProjectileEmitter emitter;

        public override IPlayerProxy GetProxy()
        {
            Stats stats = this.stats;
            stats -= this.gear.GetStats() + BaseStats;
            return new PlayerProxy<Mage>( stats - this.gear.GetStats(), this.gear );
        }

        /// <summary>
        /// Constructs an instance of a user-controlled player.
        /// </summary>
        /// <param name="stage">The Dungeon in which the Player exists.</param>
        /// <param name="sprite">The sprite drawn to represent the Player in the 
        /// game world.</param>
        public Mage( Dungeon stage, Stats stats, EquipmentSet gear )
            : base( stage, Textures.Get( PlayerType.Mage ), stats, gear )
        {
            BaseActionPoints = 2;

            AttackRange = 4;
            HasMeleeAttack = false;

            int damage = CalculateDamage();

            if ( true )
            {
                var e = (TrailProjectileEmitter) Projectiles.Get( ProjectileType.WallOfFire );
                e.Damage = damage;
                e.HazardDamage = Math.Max( damage / 2, 1 );
                e.Lifespan = 3;
                e.Range = this.AttackRange;
                emitter = e;
            }
            else
            {
                emitter = Projectiles.Get( ProjectileType.Whirlwind );
                emitter.Range = this.AttackRange;
                emitter.Damage = damage / 2;
            }
        }

        protected override void RangedAttack( Direction dir )
        {
            ((SoundEffect)Sounds.SoundDaemon.GetSound(SoundEffectTypes.MagicAttack)).Play();
            Stage.AddParticle( this, "wall of fire", emitter.Emit( this, this.Position, dir ) );
        }

    }
}
