using Game_Engine_Team.Texture;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game_Engine_Team
{
    public class TrailProjectile : Projectile
    {

        private int lifespan;

        public int HazardDamage { get; private set; }

        private Sprite hazardSprite;
        private Sprite hazardSpriteLast;

        public TrailProjectile( object owner, int x, int y, Direction dir,
                                int range, int lifespan, bool passThrough,
                                int damage, int hazardDamage,
                                Sprite sprite,
                                Sprite hazardSprite = null,
                                Sprite hazardSpriteLast = null,
                                ProjectileImpact hitEffect = null )
            : base( owner, x, y, dir, range, passThrough, damage, sprite, hitEffect )
        {
            this.lifespan = lifespan;

            HazardDamage = hazardDamage;

            this.hazardSprite = hazardSprite ?? sprite;
            this.hazardSpriteLast = hazardSpriteLast ?? this.hazardSprite;
        }

        private Hazard hazard = null;

        protected override void Move()
        {
            base.Move();

            hazard = new Hazard( this.X, this.Y, lifespan, HazardDamage, hazardSprite.Spawn(), hazardSpriteLast.Spawn() );
        }

        public override bool DeliverEffect( Dungeon stage )
        {
            bool ret = base.DeliverEffect( stage );

            if ( hazard != null && !this.Expired )
            {
                stage.AddHazard( hazard );
                hazard = null;
            }

            return ret;
        }

    }

    public class TrailProjectileEmitter : ProjectileEmitter
    {
        public int Lifespan;
        public Sprite HazardSprite;
        public Sprite HazardSpriteLast;

        public int HazardDamage;

        public TrailProjectileEmitter( int range, int lifespan, bool passThrough,
                                       int damage, int hazardDamage,
                                       Sprite sprite,
                                       Sprite hazardSprite = null,
                                       Sprite hazardSpriteLast = null,
                                       ProjectileImpact hitEffect = null )
            : base( range, passThrough, damage, sprite, hitEffect )
        {
            Lifespan = lifespan;
            HazardDamage = hazardDamage;

            HazardSprite = hazardSprite;
            HazardSpriteLast = hazardSpriteLast;
        }

        public override object Clone()
        {
            var emitter = this.MemberwiseClone() as TrailProjectileEmitter;
            emitter.HazardSprite = HazardSprite.Spawn();
            emitter.HazardSpriteLast = HazardSpriteLast.Spawn();
            return emitter;
        }

        protected override Projectile Emit( object owner, int x, int y, Direction dir, int? range )
        {
            return new TrailProjectile( owner, x, y, dir, range ?? Range, Lifespan,
                                        PassThrough, Damage, HazardDamage,
                                        Sprite, HazardSprite, HazardSpriteLast, HitEffect );
        }
    }
}
