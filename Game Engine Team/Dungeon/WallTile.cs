using Controls;
using Game_Engine_Team.Actors;
using Game_Engine_Team.Texture;
using Microsoft.Xna.Framework;

namespace Game_Engine_Team
{
    public class WallTile : Tile
    {
        public new WallTexture Texture
        {
            get {
                return base.Texture as WallTexture;
            }
        }

        public override ITileProxy GetProxy()
        {
            return new TileProxy<WallType>(
                Textures.Find( this.Texture ),
                ( x, y, type ) => new WallTile( x, y, Textures.Get( type ) )
            );
        }

        public WallTile( int x, int y, WallTexture texture )
            : base( x, y, texture )
        {
        }

        public override Tile PlaceCopy( int x, int y )
        {
            return new WallTile( x, y, (WallTexture) Texture.CloneSmart() );
        }

        protected override bool AcceptOverride( Tile adjacent )
        {
            return adjacent is NullTile;
        }

        /// <summary>
        /// Indicates whether this tile can moved into. The WallTile override 
        /// always returns false.
        /// </summary>
        /// <returns>False.</returns>
        public override bool IsTraversable( NavigationType type )
        {
            return false;
        }
    }
}
