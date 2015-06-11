using Controls;
using Game_Engine_Team.Actors;
using Game_Engine_Team.Texture;
using Microsoft.Xna.Framework;

namespace Game_Engine_Team
{
    public class PitTile : Tile
    {
        public new PitTexture Texture
        {
            get {
                return base.Texture as PitTexture;
            }
        }

        public override ITileProxy GetProxy()
        {
            return new TileProxy<PitType>(
                Textures.Find( this.Texture ),
                ( x, y, type ) => new PitTile( x, y, Textures.Get( type ) )
            );
        }

        /// <summary>
        /// Gets or sets a value indicating that the pit should render with a 
        /// shadow above it as though it were covered by a bridge.
        /// </summary>
        public bool Covered { get; set; }


        public PitTile( int x, int y, PitTexture texture )
            : base( x, y, texture )
        {
        }

        public override Tile PlaceCopy( int x, int y )
        {
            return new PitTile( x, y, (PitTexture) Texture.CloneSmart() );
        }

        protected override bool RejectOverride( Tile adjacent )
        {
            if ( adjacent is PitTile )
                return adjacent.Texture != this.Texture;

            return false;
        }

        protected override bool AcceptOverride( Tile adjacent )
        {
            return adjacent is BridgeTile || adjacent is NullTile;
        }

        public override bool IsTraversable( NavigationType type )
        {
            return type != NavigationType.Ground;
        }

        public override bool IsObstruction()
        {
            return false;
        }

        public override void Draw( Canvas canvas )
        {
            base.Draw( canvas );

            if ( Covered )
            {
                Rectangle destination = new Rectangle( X * Tile.WIDTH,
                                                       Y * Tile.HEIGHT,
                                                       Tile.WIDTH,
                                                       Tile.HEIGHT / 6 );

                canvas.DrawRect( destination, new Color( 0, 0, 0, 100 ) );
            }
        }

    }
}
