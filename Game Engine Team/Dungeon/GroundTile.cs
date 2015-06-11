using Game_Engine_Team.Actors;
using Game_Engine_Team.Texture;
using IronPython.Runtime;
using Microsoft.Xna.Framework;

namespace Game_Engine_Team
{
    public class GroundTile : Tile
    {
        public new GroundTexture Texture
        {
            get {
                return base.Texture as GroundTexture;
            }
        }

        public override ITileProxy GetProxy()
        {
            return new TileProxy<GroundType>(
                Textures.Find( this.Texture ),
                ( x, y, type ) => new GroundTile( x, y, Textures.Get( type ) )
            );
        }

        //protected override bool RejectOverride( Tile adjacent )
        //{
        //    if ( adjacent is GroundTile )
        //        return adjacent.Texture != this.Texture;
        //
        //    return false;
        //}

        public Trap Trap { get; internal set; }


        public GroundTile( int x, int y, GroundTexture texture )
            : base( x, y, texture )
        {
        }

        public override Tile PlaceCopy( int x, int y )
        {
            return new GroundTile( x, y, (GroundTexture) Texture.CloneSmart() );
        }

        public override void Update( GameTime gameTime )
        {
            base.Update( gameTime );

            if ( Trap != null )
            {
                Trap.Update( gameTime );
                if ( Trap.Expired )
                    Trap = null;
            }
        }

        public override bool IsTraversable( NavigationType type )
        {
            return true;
        }

        public override bool IsObstruction()
        {
            return false;
        }
    }
}
