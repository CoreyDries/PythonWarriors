using Controls;
using Microsoft.Xna.Framework;
using System;

namespace Game_Engine_Team.Texture
{
    public interface ITexture
    {

        /// <summary>
        /// Draws the texture onto the game screen.
        /// </summary>
        /// <param name="canvas">Canvas object used to do the drawing.</param>
        /// <param name="loc">The location (in screen coordinates) to draw the texture.</param>
        /// <param name="pixelCoords">Indicates whether to draw the texture using tile coordinates
        /// or exact pixel coordinates.</param>
        [Obsolete( "Use the Canvas extention method: Canvas.Draw(ITexture, Point, bool)" )]
        void Draw( Canvas canvas, Point loc, bool pixelCoords = false );

    }

    public static class TextureExtentions
    {
        /// <summary>
        /// Adds a sprite to a batch of sprites for rendering using the specified texture
        /// and location.
        /// </summary>
        /// <param name="canvas">Canvas object used to do the drawing.</param>
        /// <param name="texture">A texture.</param>
        /// <param name="loc">The location (in screen coordinates) to draw the texture.</param>
        /// <param name="pixelCoords">Indicates whether to draw the texture using pixel coordinates
        /// or tile coordinates.</param>
        public static void Draw( this Canvas canvas, ITexture texture, Point loc, bool pixelCoords = false )
        {
            texture.Draw( canvas, loc, pixelCoords );
        }

        /// <summary>
        /// Adds a sprite to a batch of sprites for rendering using the specified texture,
        /// location and pixel offset.
        /// </summary>
        /// <param name="canvas">Canvas object used to do the drawing.</param>
        /// <param name="texture">A texture.</param>
        /// <param name="loc">The location (in screen coordinates) to draw the texture in 
        /// number of tiles.</param>
        /// <param name="pixelOff">The offset (in screen coordinates) to shift the texture 
        /// by in number of pixels.</param>
        public static void Draw( this Canvas canvas, ITexture texture, Point loc, Point pixelOff )
        {
            loc.X *= Tile.WIDTH;
            loc.Y *= Tile.HEIGHT;

            loc.X += pixelOff.X;
            loc.Y += pixelOff.Y;

            texture.Draw( canvas, loc, true );
        }
    }
}
