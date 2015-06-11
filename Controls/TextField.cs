using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Controls
{

    /// <summary>
    /// This creates a text field to handle user input. In order to be properly used, be sure to call
    /// Update and Draw methods appropaitely.
    /// 
    /// Primary Author: Jonathan Gribble
    /// Secondary Authors: None
    /// 
    /// Last Edited: October 27/14
    /// </summary>
    public class TextField
    {
        /// <summary>
        /// The location of top-left point of the textfield.
        /// </summary>
        public Vector2 Location { get; set; }

        /// <summary>
        /// The size of the text field in pixels. The height should never be changed.
        /// </summary>
        public Vector2 Size { get; set; }

        /// <summary>
        /// The current string inside the textfield.
        /// </summary>
        private string text;
        public string Text
        {
            set { text = value; }
            get
            {
                if (promptShow)
                    return text.Replace(prompt, "");
                return text;
            }
        }

        /// <summary>
        /// Only used if background image not set. It is the background color of the control.
        /// </summary>
        public Color BackgroundColor { get; set; }

        /// <summary>
        /// The border color of the panel. It is only used if background image not set.
        /// </summary>
        public Color BorderColor { get; set; }

        /// <summary>
        /// The color of the text used in the text box.
        /// </summary>
        public Color ForeColor { get; set; }

        /// <summary>
        /// Whether or not to allow the user to add spaces. Default false.
        /// </summary>
        public bool AllowSpaces { get; set; }

        /// <summary>
        /// The maximum number of characters allowed by the field. The default is 20.
        /// </summary>
        public int MaxCharacters { get; set; }

        /// <summary>
        /// This is the background of the text box. If set, it will override the Width
        /// and Length of the text box. Height should be 20 pixels.
        /// </summary>
        public Texture2D Background { get; set; }

        /// <summary>
        /// Whether or not the text field currently has the focus of the user.
        /// </summary>
        private bool focus;

        /// <summary>
        /// The state of the mouse prior to the current game Update.
        /// </summary>
        private MouseState prevMouse;

        /// <summary>
        /// The keyboard states used to determine if a keyboard button was pressed.
        /// </summary>
        private KeyboardState prevKeyboard;
        private KeyboardState currKeyboard;

        /// <summary>
        /// The keyboard strokes that will be considered "Valid" keyboard strokes.
        /// </summary>
        private Keys[] keysToCheck = new Keys[] {
            Keys.A, Keys.B, Keys.C, Keys.D, Keys.E,
            Keys.F, Keys.G, Keys.H, Keys.I, Keys.J,
            Keys.K, Keys.L, Keys.M, Keys.N, Keys.O,
            Keys.P, Keys.Q, Keys.R, Keys.S, Keys.T,
            Keys.U, Keys.V, Keys.W, Keys.X, Keys.Y,
            Keys.Z, Keys.Back, Keys.OemMinus, Keys.D1,
            Keys.D2, Keys.D3, Keys.D4, Keys.D5, Keys.D6,
            Keys.D7, Keys.D8, Keys.D9, Keys.D0, Keys.NumPad0,
            Keys.NumPad1, Keys.NumPad2, Keys.NumPad3,
            Keys.NumPad4, Keys.NumPad5, Keys.NumPad6,
            Keys.NumPad7, Keys.NumPad8, Keys.NumPad9,
            Keys.Right, Keys.Left, Keys.Space};

        /// <summary>
        /// The prompt to show where the cursor is currently.
        /// </summary>
        private string prompt = "|";

        /// <summary>
        /// The current position of the prompt along the string.
        /// </summary>
        private int promptPos;

        /// <summary>
        /// The amount of time between blinks of the prompt.
        /// </summary>
        private const float promptDelay = 0.5f;

        /// <summary>
        /// The amount of time the prompt has been displayed/hidden.
        /// </summary>
        private float promptTime;

        /// <summary>
        /// Whether or not to show the prompt.
        /// </summary>
        private bool promptShow = false;

        /// <summary>
        /// The spritefont used by the textbox.
        /// </summary>
        public SpriteFont Font { get; set; }

        /// <summary>
        /// Creates a text field to take and display input characters from a user. The width should not be changed
        /// lightly, otherwise the string will stretch past the edges of the text box.
        /// </summary>
        /// <param name="content">The content manager used for loading information.</param>
        /// <param name="fontLocalation">The location of the font to use in the content folder.</param>
        /// <param name="x">The x location of the Text field.</param>
        /// <param name="y">The y location of the Text field.</param>
        /// <param name="width">The width of the Text field.</param>
        /// <param name="text">The starting text of the text field.</param>
        public TextField(SpriteFont font, int x = 0, int y = 0, int width = 170, string text = "")
        {
            Font = font;
            LoadContent(new Vector2(width, 20), new Vector2(x, y), text);
        }

        /// <summary>
        /// Loads the necessary content, and sets the various controls of the text field to their intial
        /// values.
        /// </summary>
        public void LoadContent(Vector2 size, Vector2 location, string text)
        {
            this.Location = location;
            this.Size = size;
            this.Text = text;
            this.BackgroundColor = Color.White;
            this.BorderColor = Color.LightGray;
            this.ForeColor = Color.Black;
            this.MaxCharacters = 16;
            promptPos = Text.Length;
        }

        /// <summary>
        /// Updates the text box based on keyboard strokes done by the user. It will also check to see if
        /// the text field has focus depending on the user click.
        /// </summary>
        /// <param name="gameTime">The amount of game time that has passed.</param>
        public void Update(GameTime gameTime)
        {
            MouseState curr = Mouse.GetState();

            // Remove prompt before making any changes to string.
            text = text.Replace(prompt, "");

            //Check if there was a mouse click.
            if (curr.LeftButton == ButtonState.Pressed && prevMouse.LeftButton == ButtonState.Released)
            {
                //Check if mouse click was within textfield, if it was, get focus, else release focus
                focus = curr.X >= Location.X && curr.X <= Location.X + Size.X && curr.Y >= Location.Y && curr.Y <= Location.Y + Size.Y;
                promptPos = Text.Length;
            }

            //If Textfield does not have focus, do not continue.
            if (!focus)
                return;

            //Update size if background is set.
            if (Background != null && (Size.X != Background.Width || Size.Y != Background.Height))
            {
                Size = new Vector2(Background.Width, Background.Height);
            }


            currKeyboard = Keyboard.GetState();

            // Remove prompt before making any changes to string.
            text = text.Replace(prompt, "");

            // Check which keys, if any, have been typed.
            foreach (Keys key in keysToCheck)
            {
                if (CheckKey(key))
                {//Check for backspace
                    if (key == Keys.Back && promptPos != 0)
                    {
                        text = text.Remove(promptPos - 1, 1);
                        promptPos -= 1;
                    }
                    //Check for left and right arrows
                    else if (key == Keys.Left || key == Keys.Right)
                    {
                        promptPos = key == Keys.Left ? promptPos - 1 : promptPos + 1;
                        MathHelper.Clamp(promptPos, 0, Text.Length);
                    }
                    else //Add key character to string.
                    {
                        string keyChar = AddKeyToText(key);
                        if (text.Length < MaxCharacters)
                        {
                            text = text.Insert(promptPos, keyChar);
                            promptPos += keyChar.Length;
                        }
                    }
                }
            }

            //Update previous mouse and Keyboard states.
            prevKeyboard = currKeyboard;
            prevMouse = curr;

            //Determine if enough time has passed to add/remove prompt
            promptTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            while (promptTime > promptDelay)
            {
                promptTime -= promptDelay;
                promptShow = !promptShow;
            }

            //Add the prompt back in if it is time to show the prompt
            if (promptShow)
                text = text.Insert(promptPos, prompt);

        }

        /// <summary>
        /// Draws the background and frame of the text box followed by the current text.
        /// </summary>
        /// <param name="spriteBatch">The spritebatch used for editting. NOT STARTED prior to call.</param>
        public void Draw( Canvas spriteBatch )
        {
            Vector2 padding = new Vector2(1, 1);
            
            //spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, new RasterizerState() { ScissorTestEnable = true });

            //Set scissoring rectangles.
            Rectangle spriteBatchRect = spriteBatch.GraphicsDevice.ScissorRectangle;
            Rectangle rect = new Rectangle((int)Location.X, (int)Location.Y, (int)Size.X, (int)Size.Y);

            //Draw textbox background.
            if (Background != null)
                spriteBatch.Draw(Background, Location, Color.White);
            else
            {
                PrimitiveShapes.FillRectangle(spriteBatch, rect, BackgroundColor);
                PrimitiveShapes.DrawRectangle(spriteBatch, rect, BorderColor);
            }
            //Draw text in scissor rectangle.
            spriteBatch.GraphicsDevice.ScissorRectangle = rect;
            spriteBatch.DrawString( Font, text, Location + padding, Color.Black );
            //spriteBatch.DrawString( Font, prompt, Location + padding, Color.Black );

            spriteBatch.GraphicsDevice.ScissorRectangle = spriteBatchRect;

            //spriteBatch.End();

        }

        /// <summary>
        /// Checks to see if the key passed in the parameters was pressed on the keyboard. Keyboard presses
        /// are measured as one press, regardless of how long the button is held down.
        /// </summary>
        /// <param name="keyToCheck">The key to see if it was pressed.</param>
        /// <returns>True if key was pressed, false otherwise.</returns>
        private bool CheckKey(Keys keyToCheck)
        {
            return !prevKeyboard.IsKeyDown(keyToCheck) && currKeyboard.IsKeyDown(keyToCheck);
        }

        /// <summary>
        /// Convers the keystroke into the corresponding string representing that keystroke.
        /// </summary>
        /// <param name="key">The keystroke to change to a string.</param>
        /// <returns>The string representing the keystroke.</returns>
        private String AddKeyToText(Keys key)
        {
            string newChar = "";


            switch (key)
            {
                case Keys.A:
                    newChar += "a";
                    break;
                case Keys.B:
                    newChar += "b";
                    break;
                case Keys.C:
                    newChar += "c";
                    break;
                case Keys.D:
                    newChar += "d";
                    break;
                case Keys.E:
                    newChar += "e";
                    break;
                case Keys.F:
                    newChar += "f";
                    break;
                case Keys.G:
                    newChar += "g";
                    break;
                case Keys.H:
                    newChar += "h";
                    break;
                case Keys.I:
                    newChar += "i";
                    break;
                case Keys.J:
                    newChar += "j";
                    break;
                case Keys.K:
                    newChar += "k";
                    break;
                case Keys.L:
                    newChar += "l";
                    break;
                case Keys.M:
                    newChar += "m";
                    break;
                case Keys.N:
                    newChar += "n";
                    break;
                case Keys.O:
                    newChar += "o";
                    break;
                case Keys.P:
                    newChar += "p";
                    break;
                case Keys.Q:
                    newChar += "q";
                    break;
                case Keys.R:
                    newChar += "r";
                    break;
                case Keys.S:
                    newChar += "s";
                    break;
                case Keys.T:
                    newChar += "t";
                    break;
                case Keys.U:
                    newChar += "u";
                    break;
                case Keys.V:
                    newChar += "v";
                    break;
                case Keys.W:
                    newChar += "w";
                    break;
                case Keys.X:
                    newChar += "x";
                    break;
                case Keys.Y:
                    newChar += "y";
                    break;
                case Keys.Z:
                    newChar += "z";
                    break;
                case Keys.D0:
                case Keys.NumPad0:
                    newChar += "0";
                    break;
                case Keys.D1:
                case Keys.NumPad1:
                    newChar += "1";
                    break;
                case Keys.D2:
                case Keys.NumPad2:
                    newChar += "2";
                    break;
                case Keys.D3:
                case Keys.NumPad3:
                    newChar += "3";
                    break;
                case Keys.D4:
                case Keys.NumPad4:
                    newChar += "4";
                    break;
                case Keys.D5:
                case Keys.NumPad5:
                    newChar += "5";
                    break;
                case Keys.D6:
                case Keys.NumPad6:
                    newChar += "6";
                    break;
                case Keys.D7:
                case Keys.NumPad7:
                    newChar += "7";
                    break;
                case Keys.D8:
                case Keys.NumPad8:
                    newChar += "8";
                    break;
                case Keys.D9:
                case Keys.NumPad9:
                    newChar += "9";
                    break;
                case Keys.OemMinus:
                    if (currKeyboard.IsKeyDown(Keys.RightShift) ||
                            currKeyboard.IsKeyDown(Keys.LeftShift))
                        newChar += "_";
                    break;
                case Keys.Space:
                    if (AllowSpaces)
                        newChar += " ";
                    break;
            }
            if (currKeyboard.IsKeyDown(Keys.RightShift) ||
                currKeyboard.IsKeyDown(Keys.LeftShift))
                newChar = newChar.ToUpper();

            return newChar;
        }
    }
}
