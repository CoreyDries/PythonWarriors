using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
    /// object ctor so multiple objects can each maintain some 
    /// unique state of the texture.
    /// </summary>
    public abstract class SmartTexture : AnimatedTexture
    {

        private int chunkWidth;
        private int chunkHeight;

        private int iconX;
        private int iconY;

        public SmartTexture( int col,  int row,
                             int w,    int h,
                             int icoX, int icoY,
                             params Texture2D[] textures )
            : base( col, row, textures )
        {
            chunkWidth = w;
            chunkHeight = h;
            iconX = icoX;
            iconY = icoY;
        }

        public override Sprite Spawn()
        {
            var clone = (SmartTexture) base.Spawn();
            clone.shapeKey = 0;
            return clone;
        }

        public virtual SmartTexture CloneSmart()
        {
            return (SmartTexture) this.Spawn();
        }

        /// <summary>
        /// A map of offset values for the texture in which the corresponding 
        /// offset is mapped to a bit pattern which indicates some knowledge 
        /// about the surrounding environment to the north--east--south or 
        /// west of its pos, as well as the compound directions in-between.
        /// </summary>
        protected abstract Point GetTexturePermutation( int key );
        

        public override Sprite Icon
        {
            get {
                return new AnimatedTexture( (base.StartX * chunkWidth) + iconX,
                                            (base.StartY * chunkHeight) + iconY,
                                            this.textures );
            }
        }

        private int shapeKey = 0;

        /// <summary>
        /// Gets the offset needed to render the texture accurately given in 
        /// number of whole sprite increments. (Multiply this by the sprite 
        /// size to get the pixel offset.)
        /// </summary>
        protected virtual Point RenderOffset
        {
            get {
                return GetTexturePermutation( shapeKey ); // mask bitfield to the 4 LSBs
            }
        }

        protected override int StartX
        {
            get {
                return (base.StartX * chunkWidth) + RenderOffset.X;
            }
        }

        protected override int StartY
        {
            get {
                return (base.StartY * chunkHeight) + RenderOffset.Y;
            }
        }

        /// <summary>
        /// Represents the bit-offset values for the shapeKey bitfield.
        /// </summary>
        public enum CardinalDirection
        {
            West = 0,
            South = 1,
            East = 2,
            North = 3,
            NorthWest = 4,
            NorthEast = 5,
            SouthEast = 6,
            SouthWest = 7
        }

        /// <summary>
        /// Gets or a sets a value indicating whether the texture will render 
        /// as though it had some knowledge of the surrounding environment in 
        /// the specified direction. This does not necessarily reflect the 
        /// actual environment the texture exists in, only how it will render.
        /// </summary>
        /// <param name="dir">North, East, South or West</param>
        /// <returns>True if the texture knows information about the 
        /// environment for the purpose of rendering.</returns>
        public bool this[ CardinalDirection dir ]
        {
            get {
                return (shapeKey & (1 << (int) dir)) != 0;
            }
            set {
                if ( value )
                    shapeKey |= 1 << (int) dir;
                else
                    shapeKey &= ~(1 << (int) dir);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the texture will render 
        /// with some knowledge of the environment directly to the NORTH of it.
        /// </summary>
        public bool North
        {
            get {
                return this[ CardinalDirection.North ];
            }
            set {
                this[ CardinalDirection.North ] = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the texture will render 
        /// with some knowledge of the environment directly to the EAST of it.
        /// </summary>
        public bool East
        {
            get {
                return this[ CardinalDirection.East ];
            }
            set {
                this[ CardinalDirection.East ] = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the texture will render 
        /// with some knowledge of the environment directly to the SOUTH of it.
        /// </summary>
        public bool South
        {
            get {
                return this[ CardinalDirection.South ];
            }
            set {
                this[ CardinalDirection.South ] = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the texture will render 
        /// with some knowledge of the environment directly to the WEST of it.
        /// </summary>
        public bool West
        {
            get {
                return this[ CardinalDirection.West ];
            }
            set {
                this[ CardinalDirection.West ] = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the texture will render 
        /// with some knowledge of the environment directly to the NORTH-EAST 
        /// of it.
        /// </summary>
        public bool NorthEast
        {
            get {
                return this[ CardinalDirection.NorthEast ];
            }
            set {
                this[ CardinalDirection.NorthEast ] = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the texture will render 
        /// with some knowledge of the environment directly to the NORTH-WEST 
        /// of it.
        /// </summary>
        public bool NorthWest
        {
            get {
                return this[ CardinalDirection.NorthWest ];
            }
            set {
                this[ CardinalDirection.NorthWest ] = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the texture will render 
        /// with some knowledge of the environment directly to the SOUTH-EAST 
        /// of it.
        /// </summary>
        public bool SouthEast
        {
            get {
                return this[ CardinalDirection.SouthEast ];
            }
            set {
                this[ CardinalDirection.SouthEast ] = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the texture will render 
        /// with some knowledge of the environment directly to the SOUTH-WEST 
        /// of it.
        /// </summary>
        public bool SouthWest
        {
            get {
                return this[ CardinalDirection.SouthWest ];
            }
            set {
                this[ CardinalDirection.SouthWest ] = value;
            }
        }
        
    }
}
