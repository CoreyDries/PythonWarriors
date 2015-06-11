using Controls;
using Game_Engine_Team.Actors;
using Game_Engine_Team.Texture;
using Microsoft.Xna.Framework;
using System;
using System.Diagnostics;

namespace Game_Engine_Team
{

    /// <summary>
    /// Base class for all tile objects which appear in a dungeon.
    /// </summary>
    [DebuggerDisplay( "{Position} -- {GetType().Name}" )]
    public abstract class Tile : IEquatable<Tile>
    {
        /// <summary>
        /// The smallest radius of a tile.
        /// </summary>
        public const int RADIUS = 16;

        /// <summary>
        /// The width of a tile in pixels.
        /// </summary>
        public const int WIDTH = Tile.RADIUS * 2;

        /// <summary>
        /// The height of a tile in pixels.
        /// </summary>
        public const int HEIGHT = Tile.RADIUS * 2;

        /// <summary>
        /// The position of the tile in screen-tile-coordinates.
        /// </summary>
        public Point Position { get; protected set; }

        /// <summary>
        /// The x-coordinate of the tile in screen-tile-coordinates.
        /// </summary>
        public int X { get { return Position.X; } }

        /// <summary>
        /// The y-coordinate of the tile in screen-tile-coordinates.
        /// </summary>
        public int Y { get { return Position.Y; } }

        /// <summary>
        /// Gets or sets the texture the tile will use for rendering.
        /// </summary>
        public SmartTexture Texture { get; private set; }

        /// <summary>
        /// Gets or sets the dungeon this tile is on.
        /// </summary>
        public Dungeon Stage { get; set; }


        /// <summary>
        /// Creates and instance of a tile object.
        /// </summary>
        /// <param name="x">The x-coordinate of the tile.</param>
        /// <param name="y">The y-coordinate of the tile.</param>
        /// <param name="texture">The texture of the tile.</param>
        public Tile( int x, int y, SmartTexture texture )
        {
            Position = new Point( x, y );
            Texture = texture;
        }

        /// <summary>
        /// Hash function for tile objects.
        /// </summary>
        /// <returns>A hash code for the current tile object.</returns>
        public override int GetHashCode()
        {
            // Very efficient hash function. It is unlikely we will ever have 
            // a dungeon whose width or height is greater than 2^16 (65536).
            return ((ushort) this.X << 16) | ((ushort) this.Y);
        }

        /// <summary>
        /// Converts a tile to its position.
        /// </summary>
        /// <param name="tile">A tile object.</param>
        /// <returns>The tile's position.</returns>
        public static explicit operator Point( Tile tile )
        {
            return tile.Position;
        }

        /// <summary>
        /// Gets an actor who position is the same as this tile's position and 
        /// which is in the same dungeon.
        /// </summary>
        /// <returns>An actor object or null if there was no actor on the tile.</returns>
        public Actor GetActor()
        {
            return Stage != null ? Stage.FindEntity<Actor>( Position ) : null;
        }

        public Rectangle GetRekt()
        {
            return new Rectangle(
                this.X * Tile.WIDTH,
                this.Y * Tile.HEIGHT,
                Tile.WIDTH,
                Tile.HEIGHT );
        }

        public bool ContainsPixel( Point loc )
        {
            return GetRekt().Contains( new Point( loc.X, loc.Y ) );
        }

        public abstract Tile PlaceCopy( int x, int y );


        /// <summary>
        /// Indicates that the tile can be traversed by actors with the 
        /// specified navigation type.
        /// </summary>
        /// <param name="type">A NavMode property of an actor.</param>
        /// <returns>True in the tile allows traversal; false otherwise.</returns>
        public abstract bool IsTraversable( NavigationType type );

        /// <summary>
        /// Indicates that the tile can be occupied by an actor with the 
        /// specified navigation type. Unlike IsTraversable(), this also 
        /// checks to see if there is already an actor present on the tile.
        /// </summary>
        /// <param name="type">A NavMode property of an actor.</param>
        /// <returns>True in the tile can be occupied; false otherwise.</returns>
        public virtual bool IsPassable( NavigationType type )
        {
            if ( !IsTraversable( type ) )
                return false;

            Actor actor = GetActor();
            return actor == null || !actor.IsAlive;
        }

        /// <summary>
        /// Indicates that the tile blocks movement and sight to actors.
        /// </summary>
        /// <returns>True in the tile obstructs; false otherwise.</returns>
        public virtual bool IsObstruction()
        {
            return !IsTraversable( NavigationType.Ground );
        }

        /// <summary>
        /// Gets the tile adjacent to this one in the specified direction.
        /// </summary>
        /// <param name="dir">A direction.</param>
        /// <returns>A tile object; never null.</returns>
        public Tile AdjacentTile( Direction dir )
        {
            return Stage[ this.Position.Add( dir ) ];
        }


        /// <summary>
        /// Draws the tile.
        /// </summary>
        /// <param name="canvas">The canvas on which to draw the tile.</param>
        public virtual void Draw( Canvas canvas )
        {
            if ( Texture != null )
                canvas.Draw( Texture, Position );
        }

        /// <summary>
        /// Updates the tile.
        /// </summary>
        /// <param name="gameTime">Time passed since the last call to Update.</param>
        public virtual void Update( GameTime gameTime )
        {
            if ( Texture != null )
                Texture.Update( gameTime );
        }

        /// <summary>
        /// Gets a light-weight proxy object used for serializing the tile.
        /// </summary>
        /// <returns>A proxy object which can be used to reconstruct this tile.</returns>
        public abstract ITileProxy GetProxy();


        /// <summary>
        /// Indicates whether the the tile should link to the specified tile 
        /// for the sake of rendering.
        /// </summary>
        /// <param name="adjacent">A tile which is assumed to be adjacent to 
        /// this tile.</param>
        /// <returns>True if this tile should link to the adjacent tile; false 
        /// otherwise.</returns>
        public bool LinksTo( Tile adjacent )
        {
            Type myType = this.GetType();
            Type otherType = adjacent.GetType();

            return !RejectOverride( adjacent )
                   && (myType.IsAssignableFrom( otherType )
                        || AcceptOverride( adjacent ));
        }

        /// <summary>
        /// Override for rejecting tiles which would normally be linked.
        /// </summary>
        /// <param name="adjacent">The tile to evaluate.</param>
        /// <returns>True if the tile is rejected; false otherwise.</returns>
        protected virtual bool RejectOverride( Tile adjacent )
        {
            return false;
        }

        /// <summary>
        /// Override for accepting tiles which normally would not be linked.
        /// </summary>
        /// <param name="adjacent">The tile to evaluate.</param>
        /// <returns>True if the tile is accepted; false otherwise.</returns>
        protected virtual bool AcceptOverride( Tile adjacent )
        {
            return false;
        }


        /// <summary>
        /// Compares this tile to another for equality.
        /// </summary>
        /// <param name="other">Another tile to compare to.</param>
        /// <returns>True if the tiles are equal; false otherwise.</returns>
        public virtual bool Equals( Tile other )
        {
            return this.Position == other.Position
                   && this.Stage == other.Stage
                   && this.Texture == other.Texture;
        }

        /// <summary>
        /// Compare this tile to another object for equality.
        /// </summary>
        /// <param name="other">Another object to compare to.</param>
        /// <returns>True if they are equal; false otherwise.</returns>
        public override bool Equals( object other )
        {
            if ( other is Tile )
                return this.Equals( (Tile) other );

            return false;
        }

        /// <summary>
        /// Compares a tile and another object for inequality.
        /// </summary>
        /// <param name="lhs">The object of the left-hand-side of the operator.</param>
        /// <param name="rhs">The object of the right-hand-side of the operator.</param>
        /// <returns>True if they are inequal; false otherwise.</returns>
        public static bool operator !=( Tile lhs, object rhs )
        {
            return !(lhs == rhs);
        }

        /// <summary>
        /// Compares a tile and another object for equality.
        /// </summary>
        /// <param name="lhs">The object of the left-hand-side of the operator.</param>
        /// <param name="rhs">The object of the right-hand-side of the operator.</param>
        /// <returns>True if they are equal; false otherwise.</returns>
        public static bool operator ==( Tile lhs, object rhs )
        {
            if ( (object) lhs == null )
                return rhs == null;

            return lhs.Equals( rhs );
        }


        // Adjacency Properties

        /// <summary>
        /// Gets or sets a value indicating whether the tile will render 
        /// with knowledge about the tile directly to the NORTH of it.
        /// </summary>
        public virtual bool North { get { return Texture.North; } set { Texture.North = value; } }

        /// <summary>
        /// Gets or sets a value indicating whether the tile will render 
        /// with knowledge about the tile directly to the EAST of it.
        /// </summary>
        public virtual bool East { get { return Texture.East; } set { Texture.East = value; } }

        /// <summary>x
        /// Gets or sets a value indicating whether the tile will render 
        /// with knowledge about the tile directly to the SOUTH of it.
        /// </summary>
        public virtual bool South { get { return Texture.South; } set { Texture.South = value; } }

        /// <summary>
        /// Gets or sets a value indicating whether the tile will render 
        /// with knowledge about the tile directly to the WEST of it.
        /// </summary>
        public virtual bool West { get { return Texture.West; } set { Texture.West = value; } }

        /// <summary>
        /// Gets or sets a value indicating whether the tile will render 
        /// with knowledge about the tile directly to the NORTH-EAST of it.
        /// </summary>
        public virtual bool NorthEast { get { return Texture.NorthEast; } set { Texture.NorthEast = value; } }

        /// <summary>
        /// Gets or sets a value indicating whether the tile will render 
        /// with knowledge about the tile directly to the NORTH-WEST of it.
        /// </summary>
        public virtual bool NorthWest { get { return Texture.NorthWest; } set { Texture.NorthWest = value; } }

        /// <summary>
        /// Gets or sets a value indicating whether the tile will render 
        /// with knowledge about the tile directly to the SOUTH-EAST of it.
        /// </summary>
        public virtual bool SouthEast { get { return Texture.SouthEast; } set { Texture.SouthEast = value; } }

        /// <summary>
        /// Gets or sets a value indicating whether the tile will render 
        /// with knowledge about the tile directly to the SOUTH-WEST of it.
        /// </summary>
        public virtual bool SouthWest { get { return Texture.SouthWest; } set { Texture.SouthWest = value; } }
    }

    /// <summary>
    /// A null tile object.
    /// </summary>
    public sealed class NullTile : Tile
    {

        /// <summary>
        /// Constructs an instance of the null tile object at the specified 
        /// coordinates.
        /// </summary>
        /// <param name="x">The x-coordinate of the null tile.</param>
        /// <param name="y">The y-coordinate of the null tile.</param>
        public NullTile( int x, int y )
            : base( x, y, null )
        {
        }

        /// <summary>
        /// NullTile objects are not traversable.
        /// </summary>
        /// <param name="type">A NavMode property of an actor.</param>
        /// <returns>False.</returns>
        public override bool IsTraversable( NavigationType type )
        {
            return false;
        }

        /// <summary>
        /// Throws an exception.
        /// </summary>
        [Obsolete( "NullTile objects cannot be placed.", true )]
        public override Tile PlaceCopy( int x, int y )
        {
            throw new System.NotSupportedException( "NullTile objects cannot be placed." );
        }

        /// <summary>
        /// Throws an exception.
        /// </summary>
        [Obsolete( "NullTile objects do not have a proxy.", true )]
        public override ITileProxy GetProxy()
        {
            throw new System.NotSupportedException( "NullTile objects do not have a proxy." );
        }

        /// <summary>
        /// Does nothing.
        /// </summary>
        public override bool North { get { return false; } set {} }

        /// <summary>
        /// Does nothing.
        /// </summary>
        public override bool East { get { return false; } set {} }

        /// <summary>
        /// Does nothing.
        /// </summary>
        public override bool South { get { return false; } set {} }

        /// <summary>
        /// Does nothing.
        /// </summary>
        public override bool West { get { return false; } set {} }

        /// <summary>
        /// Does nothing.
        /// </summary>
        public override bool NorthEast { get { return false; } set {} }

        /// <summary>
        /// Does nothing.
        /// </summary>
        public override bool NorthWest { get { return false; } set {} }

        /// <summary>
        /// Does nothing.
        /// </summary>
        public override bool SouthEast { get { return false; } set {} }

        /// <summary>
        /// Does nothing.
        /// </summary>
        public override bool SouthWest  { get { return false; } set {} }

    }
    
}
