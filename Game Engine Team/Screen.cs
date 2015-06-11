using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Controls;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Game_Engine_Team
{
    /// <summary>
    /// This class represents any screen the player may end up on while playing the game.
    /// </summary>
    public abstract class Screen : IDisposable
    {
        protected MainController Game { get; private set; }

        /// <summary>
        /// The background music for this screen, if any.
        /// </summary>
        public Song Music { get; protected set; }

        public Screen( MainController game )
        {
            Game = game;
        }

        /// <summary>
        /// The width and height of the current client's game screen.
        /// </summary>
        public static int Width;
        public static int Height;

        /// <summary>
        /// Updates the game screen based on how much game time has
        /// passed and whether or not the user has interacted with it.
        /// </summary>
        /// <param name="gameTime">Time passed since the last call to Update.</param>
        public virtual void Update( GameTime gameTime )
        {
            if ( Game.Keyboard != Game.PastKeyboard )
                if ( Game.Keyboard.IsKeyDown( Keys.Escape ) )
                    Game.Back();
        }

        /// <summary>
        /// Draws the game screen onto the client using the spriteBatch provided.
        /// </summary>
        /// <param name="canvas">The spriteBatch used for drawing.</param>
        public abstract void Draw( Canvas canvas );


        public virtual void Dispose()
        {
        }
    }
}
