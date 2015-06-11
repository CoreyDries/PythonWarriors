using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game_Engine_Team.Texture
{
    public class AnimatedTexture : Sprite
    {
        /// <summary>
        /// The amount of time between frames.
        /// </summary>
        public const double FRAME_DURATION = 0.5;

        /// <summary>
        /// The selections of sprite sheets to be used for drawing.
        /// </summary>
        protected readonly Texture2D[] textures;

        /// <summary>
        /// The current playing index of the Animator.
        /// </summary>
        private int index = 0;

        /// <summary>
        /// Creates an animator for a creature/item/object.
        /// </summary>
        /// <param name="col">The column the sprite is on.</param>
        /// <param name="row">The row the sprite is on.</param>
        /// <param name="textures">The sprite sheets containing the sprite.</param>
        public AnimatedTexture( int col, int row, params Texture2D[] textures )
            : base( col, row, textures[ 0 ] )
        {
            this.textures = textures;
        }

        protected override Texture2D Texture
        {
            get {
                return textures[ index ];
            }
        }

        public override Sprite IconAlt
        {
            get {
                return new Sprite( this.X, this.Y,
                                   this.textures[ 1 % this.textures.Length ] );
            }
        }

        /// <summary>
        /// Updates the animation sheet being played depending on how much time has passed.
        /// </summary>
        /// <param name="gameTime">Time passed since the last call to Update.</param>
        public override void Update( GameTime time )
        {
            double frameTime = time.TotalGameTime.TotalSeconds % (FRAME_DURATION * textures.Length);

            index = (int) (frameTime / FRAME_DURATION);
        }

        public override bool Equals( Sprite other )
        {
            if ( !(other is AnimatedTexture) )
                return false;

            return this.X == other.X
                   && this.Y == other.Y
                   && this.textures == (other as AnimatedTexture).textures;
        }

    }

}
