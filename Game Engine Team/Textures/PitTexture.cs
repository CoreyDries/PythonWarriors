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
    /// of the pit).
    /// </summary>
    public class PitTexture : SmartTexture
    {
        public const int CHUNK_WIDTH = 8;
        public const int CHUNK_HEIGHT = 2;

        private const int ICON_X = 4;
        private const int ICON_Y = 0;

        public PitTexture( int col, int row, params Texture2D[] textures )
            : base( col, row,
                    CHUNK_WIDTH, CHUNK_HEIGHT,
                    ICON_X, ICON_Y,
                    textures )
        {
        }

        protected override Point GetTexturePermutation( int key )
        {
            return pitShapes[ key & 63 ]; // mask to lowest 6 bits
        }

        /// <summary>
        /// Very verbose, but it works. A more elegant solution probably 
        /// exists, but putting in the effort necessary would be untenable.
        /// </summary>
        private static readonly Point[] pitShapes =
        {   ///////////////////// NE NW N E S W
            new Point( 4, 0 ), //  0 0  0 0 0 0  * same for all
            new Point( 2, 0 ), //  0 0  0 0 0 1  * same for all
            new Point( 4, 0 ), //  0 0  0 0 1 0  * same for all
            new Point( 2, 0 ), //  0 0  0 0 1 1  * same for all
            new Point( 0, 0 ), //  0 0  0 1 0 0  * same for all
            new Point( 1, 0 ), //  0 0  0 1 0 1  * same for all
            new Point( 0, 0 ), //  0 0  0 1 1 0  * same for all
            new Point( 1, 0 ), //  0 0  0 1 1 1  * same for all
            new Point( 4, 1 ), //  0 0  1 0 0 0  * same for all
            new Point( 5, 1 ), //  0 0  1 0 0 1
            new Point( 4, 1 ), //  0 0  1 0 1 0  * same for all
            new Point( 5, 1 ), //  0 0  1 0 1 1
            new Point( 7, 1 ), //  0 0  1 1 0 0
            new Point( 6, 0 ), //  0 0  1 1 0 1
            new Point( 7, 1 ), //  0 0  1 1 1 0
            new Point( 6, 0 ), //  0 0  1 1 1 1

            ///////////////////// NE NW N E S W
            new Point( 4, 0 ), //  0 1  0 0 0 0  *
            new Point( 2, 0 ), //  0 1  0 0 0 1  *
            new Point( 4, 0 ), //  0 1  0 0 1 0  *
            new Point( 2, 0 ), //  0 1  0 0 1 1  *
            new Point( 0, 0 ), //  0 1  0 1 0 0  *
            new Point( 1, 0 ), //  0 1  0 1 0 1  *
            new Point( 0, 0 ), //  0 1  0 1 1 0  *
            new Point( 1, 0 ), //  0 1  0 1 1 1  *
            new Point( 4, 1 ), //  0 1  1 0 0 0  *
            new Point( 2, 1 ), //  0 1  1 0 0 1
            new Point( 4, 1 ), //  0 1  1 0 1 0  *
            new Point( 2, 1 ), //  0 1  1 0 1 1
            new Point( 7, 1 ), //  0 1  1 1 0 0
            new Point( 7, 0 ), //  0 1  1 1 0 1
            new Point( 7, 1 ), //  0 1  1 1 1 0
            new Point( 7, 0 ), //  0 1  1 1 1 1

            ///////////////////// NE NW N E S W
            new Point( 4, 0 ), //  1 0  0 0 0 0  *
            new Point( 2, 0 ), //  1 0  0 0 0 1  *
            new Point( 4, 0 ), //  1 0  0 0 1 0  *
            new Point( 2, 0 ), //  1 0  0 0 1 1  *
            new Point( 0, 0 ), //  1 0  0 1 0 0  *
            new Point( 1, 0 ), //  1 0  0 1 0 1  *
            new Point( 0, 0 ), //  1 0  0 1 1 0  *
            new Point( 1, 0 ), //  1 0  0 1 1 1  *
            new Point( 4, 1 ), //  1 0  1 0 0 0  *
            new Point( 5, 1 ), //  1 0  1 0 0 1
            new Point( 4, 1 ), //  1 0  1 0 1 0  *
            new Point( 5, 1 ), //  1 0  1 0 1 1
            new Point( 0, 1 ), //  1 0  1 1 0 0
            new Point( 5, 0 ), //  1 0  1 1 0 1
            new Point( 0, 1 ), //  1 0  1 1 1 0
            new Point( 5, 0 ), //  1 0  1 1 1 1

            ///////////////////// NE NW N E S W
            new Point( 4, 0 ), //  1 1  0 0 0 0  *
            new Point( 2, 0 ), //  1 1  0 0 0 1  *
            new Point( 4, 0 ), //  1 1  0 0 1 0  *
            new Point( 2, 0 ), //  1 1  0 0 1 1  *
            new Point( 0, 0 ), //  1 1  0 1 0 0  *
            new Point( 1, 0 ), //  1 1  0 1 0 1  *
            new Point( 0, 0 ), //  1 1  0 1 1 0  *
            new Point( 1, 0 ), //  1 1  0 1 1 1  *
            new Point( 4, 1 ), //  1 1  1 0 0 0  *
            new Point( 2, 1 ), //  1 1  1 0 0 1
            new Point( 4, 1 ), //  1 1  1 0 1 0  *
            new Point( 2, 1 ), //  1 1  1 0 1 1
            new Point( 0, 1 ), //  1 1  1 1 0 0
            new Point( 1, 1 ), //  1 1  1 1 0 1
            new Point( 0, 1 ), //  1 1  1 1 1 0
            new Point( 1, 1 ), //  1 1  1 1 1 1
        };

    }

}
