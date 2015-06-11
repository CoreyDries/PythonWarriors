using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game_Engine_Team.Texture
{
    public sealed class NullSprite : Sprite
    {
        public NullSprite()
            : base( 0, 0, (Texture2D) Textures.Get( SpriteType.Null ) )
        {
        }
    }
}
