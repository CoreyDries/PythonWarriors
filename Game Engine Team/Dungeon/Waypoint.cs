using Controls;
using Game_Engine_Team.Texture;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game_Engine_Team
{
    public class Waypoint : IEntity, IEquatable<Waypoint>
    {
        public Point Position { get; internal set; }

        public int X { get { return Position.X; } }
        public int Y { get { return Position.Y; } }

        public bool Expired { get; protected set; }

        public Sprite Sprite { get; private set; }


        protected Waypoint( Point pos, Sprite sprite )
        {
            Position = pos;
            Sprite = sprite;
        }

        protected Waypoint( int x, int y, Sprite sprite )
            : this( new Point( x, y ), sprite )
        {
        }

        public Waypoint( int x, int y )
            : this( x, y, Textures.Get( EffectType.Waypoint ) )
        {
        }

        public Waypoint( Point pos )
            : this( pos, Textures.Get( EffectType.Waypoint ) )
        {
        }

        public static implicit operator Point( Waypoint waypoint )
        {
            return waypoint.Position;
        }

        public void Update( GameTime gameTime )
        {
            Sprite.Update( gameTime );
        }

        public virtual void Draw( Canvas canvas )
        {
            if ( canvas.EditMode )
                canvas.Draw( Sprite, Position );
        }

        public void Draw( Canvas canvas, int num )
        {
            this.Draw( canvas );

            if ( canvas.EditMode )
                canvas.DrawString( num.ToString(), Textures.Monospace,
                    Position.Multiply( Tile.WIDTH, Tile.HEIGHT ), Color.White, 1, Color.Black );
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public bool Equals( Waypoint other )
        {
            return this.Position == other.Position;
        }

        public override bool Equals( object other )
        {
            if ( other is Waypoint )
                return this.Equals( (Waypoint) other );

            return false;
        }

        public static bool operator !=( Waypoint a, object b )
        {
            return !(a == b);
        }

        public static bool operator ==( Waypoint a, object b )
        {
            if ( (object) a == null )
                return b == null;

            return a.Equals( b );
        }


        public Actors.NavigationType NavMode
        {
            get { return Actors.NavigationType.All; }
        }
    }

    public class SpawnPoint : Waypoint
    {

        public SpawnPoint( Point pos )
            : base( pos, Textures.Get( PlayerType.Base ) )
        {
        }

        public SpawnPoint( int x, int y )
            : this( new Point( x, y ) )
        {
        }

    }

    public class ExitPoint : Waypoint
    {

        public ExitPoint( Point pos )
            : base( pos, Textures.Get( SpriteType.ExitPoint ) )
        {
        }

        public ExitPoint( int x, int y )
            : this( new Point( x, y ) )
        {
        }

        public override void Draw( Canvas canvas )
        {
            canvas.Draw( Sprite, Position );

            if ( canvas.EditMode )
                canvas.DrawString( "Exit", Textures.Monospace,
                    Position.Multiply( Tile.WIDTH, Tile.HEIGHT ), Color.White, 1, Color.Black );
        }

    }
}
