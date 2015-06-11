using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Controls
{
    [Obsolete( "Use Game_Engine_Team.Textures.AnimatedTexture" )]
    public class Animator
    {
        /// <summary>
        /// The selections of sprite sheets to be used for drawing.
        /// </summary>
        private ArrayList spriteSheet;

        /// <summary>
        /// The location of the sprite on the sprite sheet (not in pixels, but in array)
        /// </summary>
        private Vector2 source;

        /// <summary>
        /// The size of a sprite in pixels.
        /// </summary>
        private const int size = 16;

        /// <summary>
        /// The current playing index of the Animator.
        /// </summary>
        private int index;

        /// <summary>
        /// The number of indexes to do before looping back to one.
        /// </summary>
        private int maxIndex;

        /// <summary>
        /// The amount of time between frames.
        /// </summary>
        private const float frameLength = 0.5f;

        /// <summary>
        /// How long the current frame has been playing;
        /// </summary>
        private float time;

        /// <summary>
        /// Creates an animator for a creature/item/object.
        /// </summary>
        /// <param name="col">The column the sprite is on.</param>
        /// <param name="row">The row the sprite is on.</param>
        /// <param name="sheets">The sprite sheets containing the sprite.</param>
        public Animator(int col, int row, params Texture2D[] sheets)
        {
            source = new Vector2(col, row);
            spriteSheet = new ArrayList();
            for (int i = 0; i < sheets.Length; i++)
            {
                spriteSheet.Add(sheets[i]);
            }
            maxIndex = sheets.Length;
        }

        /// <summary>
        /// Updates the animation sheet being played depending on how much time has passed.
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            time += (float)gameTime.ElapsedGameTime.TotalSeconds;

            while (time >= frameLength)
            {
                time -= frameLength;
                index = (index + 1) % maxIndex;
            }
        }

        /// <summary>
        /// Draws the sprite in its current position and current frame.
        /// </summary>
        /// <param name="spriteBatch">The spriteBatch used for drawing.</param>
        /// <param name="location">The location in the game to draw the sprite, in pixel space.</param>
        public void Draw(SpriteBatch spriteBatch, Vector2 location)
        {
            Rectangle source = new Rectangle((int)this.source.X * size, (int)this.source.Y * size, size, size);
            Rectangle local = new Rectangle((int)location.X, (int)location.Y, size * 2, size * 2);
            spriteBatch.Draw((Texture2D)spriteSheet[index], local, source, Color.White);
        }

    }

    // Andrew Meckling
    public static class AnimatedTextureExtentions
    {
        public static void Draw( this SpriteBatch spriteBatch, Animator anim, Vector2 location )
        {
            anim.Draw( spriteBatch, location );
        }
    }
}
