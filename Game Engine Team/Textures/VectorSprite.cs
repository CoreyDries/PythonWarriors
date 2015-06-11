using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game_Engine_Team.Texture
{
    public class VectorSprite : AnimatedTexture
    {
        public Direction Direction { get; set; }

        public VectorSprite( int col, int row, params Texture2D[] textures )
            : base( col, row, textures )
        {
            Direction = Direction.Down;
        }

        protected override int StartX
        {
            get {
                int x_off = 0;

                if ( Direction == Direction.Left || Direction == Direction.Right )
                    x_off = 1;

                return base.StartX + x_off;
            }
        }

    }
}
