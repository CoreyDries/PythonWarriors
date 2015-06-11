using Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game_Engine_Team.Texture
{
    /// <summary>
    /// A smart texture which can render itself based on the tiles 
    /// which surround it. It should be cloned when passing to an 
    /// object ctor so multiple objects can each maintain a unique
    /// state of the texture (in this case that state is the shape 
    /// of the ground).
    /// </summary>
    public class GroundTexture : SmartTexture
    {
        public const int CHUNK_WIDTH = 7;
        public const int CHUNK_HEIGHT = 3;

        private const int ICON_X = 5;
        private const int ICON_Y = 0;

        public bool DrawConvexCorners { get; set; }

        public GroundTexture( int col, int row, params Texture2D[] textures )
            : base( col, row,
                    CHUNK_WIDTH, CHUNK_HEIGHT,
                    ICON_X, ICON_Y,
                    textures )
        {
        }

        public override SmartTexture CloneSmart()
        {
            var sprite = (GroundTexture) base.CloneSmart();
            sprite.DrawConvexCorners = this.DrawConvexCorners;
            return sprite;
        }

        public override void Draw( Canvas canvas, Point loc, bool pixelCoords = false )
        {
            base.Draw( canvas, loc, pixelCoords );

            if ( !DrawConvexCorners )
                return;

            const int width = 5;
            const int height = 5;

            const int szX = width * (Tile.WIDTH / Sprite.WIDTH);
            const int szY = height * (Tile.HEIGHT / Sprite.HEIGHT);

            const int rectX = Tile.WIDTH - szX;
            const int rectY = Tile.HEIGHT - szY;

            int srcX = this.X * GroundTexture.CHUNK_WIDTH * Sprite.WIDTH;
            int srcY = this.Y * GroundTexture.CHUNK_HEIGHT * Sprite.HEIGHT;

            if ( !pixelCoords )
            {
                loc.X *= Tile.WIDTH;
                loc.Y *= Tile.HEIGHT;
            }

            Rectangle source = new Rectangle( srcX, srcY, width, height );
            Rectangle destination = new Rectangle( loc.X, loc.Y, szX, szY );

            if ( North && East && !NorthEast )
                DrawSquare( canvas, destination, source, rectX, 0 );

            if ( North && West && !NorthWest )
                DrawSquare( canvas, destination, source, 0, 0 );

            if ( South && East && !SouthEast )
                DrawSquare( canvas, destination, source, rectX, rectY );

            if ( South && West && !SouthWest )
                DrawSquare( canvas, destination, source, 0, rectY );
        }

        private void DrawSquare( Canvas canvas, Rectangle dest, Rectangle source, int rectX, int rectY )
        {
            dest.X += rectX;
            dest.Y += rectY;
            canvas.Draw( Texture, dest, source, Tint );
        }

        protected override Point GetTexturePermutation( int key )
        {
            return groundShapes[ key & 15 ];
        }

        /// <summary>
        /// A map of offset values for the ground texture where the 
        /// corresponding offset is mapped to a bit pattern which indicates 
        /// the presence of a ground tile to the north--east--south or west of 
        /// its position.
        /// </summary>
        private static readonly Point[] groundShapes =
        {                      // N E S W
            new Point( 5, 0 ), // 0 0 0 0
            new Point( 6, 1 ), // 0 0 0 1
            new Point( 3, 0 ), // 0 0 1 0
            new Point( 2, 0 ), // 0 0 1 1
            new Point( 4, 1 ), // 0 1 0 0
            new Point( 5, 1 ), // 0 1 0 1
            new Point( 0, 0 ), // 0 1 1 0
            new Point( 1, 0 ), // 0 1 1 1
            new Point( 3, 2 ), // 1 0 0 0
            new Point( 2, 2 ), // 1 0 0 1
            new Point( 3, 1 ), // 1 0 1 0
            new Point( 2, 1 ), // 1 0 1 1
            new Point( 0, 2 ), // 1 1 0 0
            new Point( 1, 2 ), // 1 1 0 1
            new Point( 0, 1 ), // 1 1 1 0
            new Point( 1, 1 )  // 1 1 1 1
        };

    }

}
