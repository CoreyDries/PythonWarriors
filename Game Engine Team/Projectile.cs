using Game_Engine_Team.Actors;
using Game_Engine_Team.Texture;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Game_Engine_Team
{

    public struct ProjectileImpactEventArgs
    {
        public Actor Victim;
        public Direction Direction;
        public int Distance;
        public int Range;

        public ProjectileImpactEventArgs( Actor victim, Direction dir, int distance, int range )
        {
            Victim = victim;
            Direction = dir;
            Distance = distance;
            Range = range;
        }
    }

    public delegate void ProjectileImpact( ProjectileImpactEventArgs e );

    public class Projectile : Particle
    {

        public object Owner { get; protected set; }

        public Direction Direction { get; private set; }

        public int Range { get; private set; }

        public bool PassThrough { get; protected set; }
        
        public int Damage { get; protected set; }

        private ProjectileImpact HitEffect;

        public int Distance { get; private set; }


        public Projectile( object owner, int x, int y, Direction dir,
                           int range, bool passThrough, int damage,
                           Sprite sprite, ProjectileImpact hitEffect = null )
            : base( x, y, sprite, true )
        {
            Owner = owner;
            Direction = dir;
            Range = range;
            PassThrough = passThrough;
            Damage = damage;
            HitEffect = hitEffect ?? (e => {});
        }

        protected override bool TakeTurn()
        {
            if ( !base.TakeTurn() )
                return false;

            Move();
            return true;
        }

        public override void Update( GameTime gameTime )
        {
            base.Update( gameTime );

            if ( Distance < Range )
            {
                Point vec = Direction.ToPoint();
                PixelOff.X = (int) ((vec.X * Tile.WIDTH) * (waitTime / TURN_LENGTH));
                PixelOff.Y = (int) ((vec.Y * Tile.HEIGHT) * (waitTime / TURN_LENGTH));
            }
            else
            {
                PixelOff.X = 0;
                PixelOff.Y = 0;
            }
        }

        protected virtual void Move()
        {
            Position = Position.Add( Direction );

            //Position.X += direction.X;//pos.X = Dungeon.WrapX( pos.X + direction.X );
            //Position.Y += direction.Y;//pos.Y = Dungeon.WrapY( pos.Y + direction.Y );

            ++Distance;
            hasHit = false;
        }

        private bool hasHit = false;

        public override bool DeliverEffect( Dungeon stage )
        {
            Tile tile = stage[ Position ];

            if ( tile.IsObstruction() || Distance > Range )
            {
                this.Expire();
            }
            else if ( Distance > 0 && hasHit == false )
            {
                Actor targetActor = tile.GetActor();

                if ( targetActor != null
                     && HitEffect != null )
                {
                    HitEffect( new ProjectileImpactEventArgs( targetActor, Direction, Distance, Range ) );
                    targetActor.TakeDamage( Damage, Owner );
                    hasHit = true;

                    if ( !PassThrough )
                        this.Expire();

                    return true;
                }
            }

            return base.DeliverEffect( stage );
        }
    }

    public enum ProjectileType
    {
        Shuriken, RedBeam, WallOfFire, Whirlwind,
        MagicOrb,
        BlueBeam
    }

    public class Projectiles
    {

        private static Projectiles instance;

        private Dictionary<ProjectileType, ProjectileEmitter> emitters = new Dictionary<ProjectileType, ProjectileEmitter>();

        public static void Initialize()
        {
            instance = new Projectiles();
        }

        private Projectiles()
        {
            emitters[ ProjectileType.Shuriken ] =
                new ProjectileEmitter( 1, false, 1,
                    Textures.Get( EffectType.Shuriken ) );

            emitters[ ProjectileType.RedBeam ] =
                new TracerProjectileEmitter( 1, false, 1,
                    Textures.Get(VectorType.RedBeam));

            emitters[ProjectileType.BlueBeam] =
                new TracerProjectileEmitter(1, false, 1,
                    Textures.Get(VectorType.BlueBeam));

            emitters[ ProjectileType.WallOfFire ] =
                new TrailProjectileEmitter( 1, 3, true, 1, 2,
                    Textures.Get( EffectType.FireLarge ),
                    Textures.Get( EffectType.FireLarge ),
                    Textures.Get(EffectType.FireSmall));

            emitters[ProjectileType.MagicOrb] =
                new ProjectileEmitter(1, false, 1,
                    Textures.Get(EffectType.OrbLarge));

            emitters[ ProjectileType.Whirlwind ] =
                new ProjectileEmitter( 1, true, 1, Textures.Get( EffectType.WindLarge ),
                    Whirlwind );
        }

        private static void Whirlwind( ProjectileImpactEventArgs e )
        {
            Actor victim = e.Victim;
            Tile target = e.Victim.CurrentTile;

            for ( int i = e.Distance; i < e.Range; ++i )
            {
                Tile adjTile = target.AdjacentTile( e.Direction );

                if ( !adjTile.IsObstruction() )
                    target = adjTile;
                else
                    break;
            }

            victim.TryPushTo( target );
        }

        public static ProjectileEmitter Get( ProjectileType type )
        {
            return (ProjectileEmitter) instance.emitters[ type ].Clone();
        }
    }

    public class ProjectileEmitter
    {
        public int Range;
        public int Damage;
        public ProjectileImpact HitEffect;
        public Sprite Sprite;
        public bool PassThrough;

        public ProjectileEmitter( int range, bool passThrough, int damage,
                                  Sprite sprite, ProjectileImpact hitEffect = null )
        {
            Range = range;
            PassThrough = passThrough;
            Damage = damage;
            Sprite = sprite;
            HitEffect = hitEffect;
        }

        public virtual object Clone()
        {
            var emitter = this.MemberwiseClone() as ProjectileEmitter;
            emitter.Sprite = Sprite.Spawn();
            return emitter;
        }

        public Projectile Emit( object owner, Point loc, Direction dir, int? range = null )
        {
            return Emit( owner, loc.X, loc.Y, dir, range );
        }

        protected virtual Projectile Emit( object owner, int x, int y, Direction dir, int? range )
        {
            return new Projectile( owner, x, y, dir, range ?? Range,
                                   PassThrough, Damage, Sprite, HitEffect );
        }
    }
}
