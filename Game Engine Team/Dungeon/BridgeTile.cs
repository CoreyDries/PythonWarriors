using Game_Engine_Team.Texture;

namespace Game_Engine_Team
{
    public class BridgeTile : GroundTile
    {
        public override ITileProxy GetProxy()
        {
            return new TileProxy<GroundType>(
                Textures.Find( this.Texture ),
                ( x, y, type ) => new BridgeTile( x, y, Textures.Get( type ) )
            );
        }

        public BridgeTile( int x, int y, GroundTexture texture )
            : base( x, y, texture )
        {
        }

        public override Tile PlaceCopy( int x, int y )
        {
            return new BridgeTile( x, y, (GroundTexture) Texture.CloneSmart() );
        }

        protected override bool AcceptOverride( Tile adjacent )
        {
            if ( adjacent is GroundTile )
                return adjacent.Texture == this.Texture;

            return false;
        }
        
    }
}
