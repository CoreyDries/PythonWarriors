using Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Python_Team;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game_Engine_Team.Texture
{
    /// <summary>
    /// A smart texture which can render itself based on the tiles 
    /// which surround it. It should be cloned when passing to an 
    /// object ctor so multiple objects can each maintain a unique
    /// state of the texture (in this case that state is the shape 
    /// of the wall).
    /// </summary>
    public class WallTexture : SmartTexture
    {
        public const int CHUNK_WIDTH = 7;
        public const int CHUNK_HEIGHT = 3;

        private const int ICON_X = 3;
        private const int ICON_Y = 0;

        public WallTexture( int col, int row, params Texture2D[] textures )
            : base( col, row,
                    CHUNK_WIDTH, CHUNK_HEIGHT,
                    ICON_X, ICON_Y,
                    textures )
        {
        }

        public override void Draw( Canvas canvas, Point loc, bool pixelCoords = false )
        {
            base.Draw( canvas, loc, pixelCoords );

            if ( !pixelCoords )
            {
                loc.X *= Tile.WIDTH;
                loc.Y *= Tile.HEIGHT;
            }

            bool east = East && SouthEast;
            bool west = West && SouthWest;

            if ( South && (east || west) )
            {
                int y = loc.Y + 2 * (Tile.HEIGHT / Sprite.HEIGHT);

                int width = east && west
                            ? Tile.WIDTH
                            : Tile.WIDTH / 2;

                int x = east && !west
                        ? loc.X + Tile.WIDTH / 2
                        : loc.X;

                Rectangle destination = new Rectangle( x, y, width, Tile.HEIGHT );

                Color color = canvas.EditMode ? new Color( 20, 12, 28, 128 ) : new Color( 20, 12, 28 );

                canvas.DrawRect( destination, color, true );
            }
        }

        protected override Point GetTexturePermutation( int key )
        {
            return wallShapes[ key & 15 ]; // mask the key
        }

        /// <summary>
        /// A map of offset values for the wall texture where the corresponding 
        /// offset is mapped to a bit pattern which indicates the presence of a 
        /// wall to the north--east--south or west of its pos.
        /// </summary>
        private static readonly Point[] wallShapes =
        {                      // N E S W
            new Point( 1, 1 ), // 0 0 0 0 *duplicate of (1 0 0 0)
            new Point( 2, 2 ), // 0 0 0 1 *duplicate of (1 0 0 1)
            new Point( 0, 1 ), // 0 0 1 0 *duplicate of (1 0 1 0)
            new Point( 2, 0 ), // 0 0 1 1
            new Point( 0, 2 ), // 0 1 0 0 *duplicate of (1 1 0 0)
            new Point( 1, 0 ), // 0 1 0 1
            new Point( 0, 0 ), // 0 1 1 0
            new Point( 4, 0 ), // 0 1 1 1
            new Point( 1, 1 ), // 1 0 0 0
            new Point( 2, 2 ), // 1 0 0 1
            new Point( 0, 1 ), // 1 0 1 0
            new Point( 5, 1 ), // 1 0 1 1
            new Point( 0, 2 ), // 1 1 0 0
            new Point( 4, 2 ), // 1 1 0 1
            new Point( 3, 1 ), // 1 1 1 0
            new Point( 4, 1 )  // 1 1 1 1
        };

    }

}
