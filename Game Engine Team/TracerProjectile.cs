using Game_Engine_Team.Texture;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game_Engine_Team
{
    public class TracerProjectile : Projectile
    {

        public new VectorSprite Sprite
        {
            get {
                return (VectorSprite) base.Sprite;
            }
        }

        private VectorSprite tracerSprite;

        public TracerProjectile( object owner, int x, int y, Direction dir,
                                 int range, bool passThrough, int damage,
                                 VectorSprite sprite,
                                 VectorSprite tracerSprite = null,
                                 ProjectileImpact hitEffect = null )
            : base( owner, x, y, dir, range, passThrough, damage, sprite, hitEffect )
        {
            this.tracerSprite = tracerSprite ?? sprite;

            Sprite.Direction = dir;
            this.tracerSprite.Direction = dir;

            firstTrace = new Particle( this.X, this.Y, tracerSprite.Spawn(), 0.25 );
            Point pos = tracerSprite.Direction.ToPoint();
            firstTrace.PixelOff.X = pos.X * Tile.WIDTH / 2;
            firstTrace.PixelOff.Y = pos.Y * Tile.HEIGHT / 2;
        }

        private Particle tracer = null;
        private Particle firstTrace = null;

        protected override void Move()
        {
            base.Move();

            tracer = new Particle( this.X, this.Y, tracerSprite.Spawn(), 0.25 );

            Point pos = tracerSprite.Direction.ToPoint();
            tracer.PixelOff.X = pos.X * Tile.WIDTH / 2;
            tracer.PixelOff.Y = pos.Y * Tile.HEIGHT / 2;
        }

        public override bool DeliverEffect( Dungeon stage )
        {
            bool ret = base.DeliverEffect( stage );

            if ( firstTrace != null && !this.Expired )
            {
                stage.AddParticle( this, "trace_", firstTrace );
                firstTrace = null;
            }

            if ( tracer != null && !this.Expired )
            {
                stage.AddParticle( this, "trace" + Distance, tracer );
                tracer = null;
            }

            return ret;
        }

    }

    public class TracerProjectileEmitter : ProjectileEmitter
    {
        public new VectorSprite Sprite
        {
            get {
                return (VectorSprite) base.Sprite;
            }
        }

        public VectorSprite TracerSprite;

        public TracerProjectileEmitter( int range, bool passThrough, int damage,
                                        VectorSprite sprite,
                                        VectorSprite tracerSprite = null,
                                        ProjectileImpact hitEffect = null )
            : base( range, passThrough, damage, sprite, hitEffect )
        {
            TracerSprite = tracerSprite ?? sprite;
        }

        public override object Clone()
        {
            var emitter = this.MemberwiseClone() as TracerProjectileEmitter;
            emitter.TracerSprite = (VectorSprite) TracerSprite.Spawn();
            return emitter;
        }

        protected override Projectile Emit( object owner, int x, int y, Direction dir, int? range )
        {
            return new TracerProjectile( owner, x, y, dir, range ?? Range, PassThrough,
                                         Damage, Sprite, TracerSprite, HitEffect );
        }
    }
}
