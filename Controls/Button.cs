using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


//TODO:
// change button rectangle to white
// let user of library set size of button (rectangle)
//          
//

namespace Controls
{

    /// <summary>
    /// Creates a button control
    /// Allows the manipulation of the location of the button, colour of the button text.
    /// Changes colors when hovered upon.
    /// 
    /// Jonathan Gribble: Added functionality if there is no image set to the button, and
    /// added properties to control button background and border if there is no image set.
    /// Also edited the update method so that it will return a bool value of whether or
    /// not the button was clicked.
    /// 
    /// Primary Author: Enoch Yip
    /// Secondary Authors: Jonathan Gribble
    /// Last Edited: October Nov 3, 2014
    /// </summary>
    public class Button
    {
        /// <summary>
        /// The background colour of the button if no image is used.
        /// </summary>
        private Color BackgroundColour { get; set; }

        /// <summary>
        /// The colour of the button's border, assuming no image is
        /// used.
        /// </summary>
        private Color BorderColour { get; set; }

        /// <summary>
        /// Image/bounds of the button
        /// </summary>
        private Texture2D Image { get; set; }

        /// <summary>
        /// Image/bounds of the button when pressed
        /// </summary>
        private Texture2D ImagePressed { get; set; }

        /// <summary>
        /// Image/bounds of the button when hovered over
        /// </summary>
        private Texture2D ImageHovered { get; set; }

        /// <summary>
        /// Font of the button
        /// </summary>
        private SpriteFont Font { get; set; }

        /// <summary>
        /// Location and size of the button.
        /// </summary>
        public Rectangle Location;

        /// <summary>
        /// Text of the button
        /// </summary>
        private string text { get; set; }

        /// <summary>
        /// Location of the text of the button
        /// </summary>
        Vector2 textLocation;

        /// <summary>
        /// Current mouse state
        /// </summary>
        MouseState mouse;

        /// <summary>
        /// Old mouse state
        /// </summary>
        MouseState oldMouse;

        /// <summary>
        /// Bool of whether the button is clicked or not.
        /// Default is false.
        /// </summary>
        bool clicked = false;

        /// <summary>
        /// Bool of whether the button is a toggle button or not
        /// Default state is false.
        /// </summary>
        bool keepPressed = false;

        /// <summary>
        /// The delegate function for methods to follow when the button is clicked.
        /// </summary>
        public delegate void ButtonClick();

        /// <summary>
        /// The delegate function for buttons that need the event handler to know what object sent it.
        /// </summary>
        public delegate void SelectButtonClick( Object sender );

        /// <summary>
        /// This contains all the functionality for the button clicks.
        /// </summary>
        public event ButtonClick On_Button_Click;

        /// <summary>
        /// This contains all the functionality for the select button clicks.
        /// </summary>
        public event SelectButtonClick On_Select_Button_Click;

        /// <summary>
        /// Creates a button without using an image, and uses the default colours. The button
        /// Colours can be changed manually.
        /// </summary>
        /// <param name="font">The font used to write the text.</param>
        /// <param name="text">The text to use (if it's larger than button, will be cut off).</param>
        /// <param name="x">The starting x location.</param>
        /// <param name="y">The starting y location.</param>
        /// <param name="width">The width of the button..</param>
        /// <param name="height">The height of the button.</param>
        public Button(SpriteFont font, string text = "Submit", int x = 0, int y = 0, int width = 75, int height = 25)
        {
            this.Font = font;
            this.BackgroundColour = Color.LightGray;
            this.BorderColour = Color.Gray;
            this.Location = new Rectangle(x, y, width, height);
            this.Text = text;
        }

        /// <summary>
        /// Creates button with all necessary parameters
        /// </summary>
        /// <param name="texture">image of the normal state of the button</param>
        /// <param name="textureHovered">image of the hovered state of the button</param>
        /// <param name="texturePressed">image of the pressed state of the button</param>
        /// <param name="font">font for the string</param>
        /// <param name="s">string to be put over the button</param>
        public Button(Texture2D texture, Texture2D textureHovered, Texture2D texturePressed, SpriteFont font, string s)
        {
            if (texture.Bounds != textureHovered.Bounds && textureHovered.Bounds != texturePressed.Bounds && texture.Bounds != texturePressed.Bounds)
            {
                // deny creation of button
                return;
            }
            Image = texture;
            ImagePressed = texturePressed;
            ImageHovered = textureHovered;
            this.Font = font;
            Location = new Rectangle(0, 0, Image.Width, Image.Height);
            Text = s;
        }

        /// <summary>
        /// Creates button with all necessary parameters
        /// </summary>
        /// <param name="texture">image of the normal state of the button</param>
        /// <param name="textureHovered">image of the hovered state of the button</param>
        /// <param name="texturePressed">image of the pressed state of the button</param>
        /// <param name="font">font for the string</param>
        public Button(Texture2D texture, Texture2D textureHovered, Texture2D texturePressed, SpriteFont font)
        {
            if (texture.Bounds != textureHovered.Bounds && textureHovered.Bounds != texturePressed.Bounds && texture.Bounds != texturePressed.Bounds)
            {
                // deny creation of button
                return;
            }
            Image = texture;
            ImagePressed = texturePressed;
            ImageHovered = textureHovered;
            this.Font = font;
            Location = new Rectangle(0, 0, Image.Width, Image.Height);
        }

        /// <summary>
        /// Returns text.
        /// Sets text of the button and centers it in the button.
        /// </summary>
        public string Text
        {
            get { return text; }
            set
            {
                int padding = 2;
                text = value;
                Vector2 size = Font.MeasureString(text);
                textLocation = new Vector2();
                textLocation.Y = Location.Y + ((Location.Height / 2) - (size.Y / 2)) + padding;
                textLocation.X = Location.X + ((Location.Width / 2) - (size.X / 2));

            }
        }

        /// <summary>
        /// Enables button stay toggled
        /// </summary>
        public void enableKeepPressed()
        {
            keepPressed = true;
        }


        /// <summary>
        /// Checks whether the buttton is clicked or not.
        /// The Text of the button is set here.
        /// </summary>
        public void Update()
        {

            mouse = Mouse.GetState();

            // Tracks whether the mouse is clicked or not
            if (mouse.LeftButton == ButtonState.Released &&
                oldMouse.LeftButton == ButtonState.Pressed)
            {
                // Tracks whether the mouse is over the button or not
                if (Location.Contains((int)mouse.X, (int)mouse.Y))
                    if (clicked)
                    { //The user had clicked the button before, and has no released the click on the button.
                        clicked = false;
                        if (On_Button_Click != null)
                            On_Button_Click();
                        if (On_Select_Button_Click != null)
                            On_Select_Button_Click( this );
                    }
                    else
                    {
                        clicked = true;
                        if (On_Button_Click != null)
                            On_Button_Click();
                        if ( On_Select_Button_Click != null )
                            On_Select_Button_Click( this );
                    }

            }
            // Updates mouse state
            oldMouse = mouse;
        }

        /// <summary>
        /// When the mouse cursor hovers over the button, it changes the button's color.
        /// Gives section of code where it'll handle when the button is clicked
        /// </summary>
        /// <param name="spriteBatch">The spritebatch used to draw.</param>
        public void Draw(Canvas spriteBatch)
        {
            // Sets the spriteBatch's settings
            //spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

            // Tracks mouse cursor and checks whether it's over the button
            if (Location.Contains(new Point(mouse.X, mouse.Y)))
            {
                //Draw button based on image or rectangles.
                if (Image != null)
                {
                    if (keepPressed && clicked)
                    {
                        spriteBatch.Draw(ImagePressed,
                        Location,
                        Color.White);
                    }
                    else
                        spriteBatch.Draw(ImageHovered,
                        Location,
                        Color.White);
                }
                else
                {
                    DrawPlainButton(spriteBatch, clicked && keepPressed);
                }

            }
            else
            {
                if (Image != null)
                {
                    if (keepPressed && clicked)
                    {
                        spriteBatch.Draw(ImagePressed,
                        Location,
                        Color.White);
                    }
                    else
                        spriteBatch.Draw(Image,
                        Location,
                        Color.White);
                }
                else
                {
                    DrawPlainButton(spriteBatch, clicked && keepPressed);
                }
            }

            if (Location.Contains(mouse.X, mouse.Y) && mouse.LeftButton == ButtonState.Pressed)
                if (Image != null)
                    spriteBatch.Draw(ImagePressed, Location, Color.White);
                else
                    DrawPlainButton(spriteBatch, true);

            if (text != null)
            {
                // draws the button text
                spriteBatch.DrawString(Font, text, textLocation, Color.Black);
            }


            // Flushes the sprite batch and restores the device state to how it was before Begin was called.
            //spriteBatch.End();
        }

        /// <summary>
        /// Draws a plain button based off of the current state of the button.
        /// </summary>
        /// <param name="spriteBatch">The spritebatch used for drawing.</param>
        /// <param name="pressed">Whether or not the button should appear pressed.</param>
        private void DrawPlainButton(Canvas spriteBatch, bool pressed)
        {
            //Draw button
            PrimitiveShapes.FillRectangle(spriteBatch, Location, BackgroundColour);
            PrimitiveShapes.DrawRectangle(spriteBatch, Location, BorderColour);

            //Determine shading based on if button is clicked on or not.
            Color topLeft = pressed ? PrimitiveShapes.Darken(BorderColour) : PrimitiveShapes.Lighten(BorderColour);
            Color bottomRight = pressed ? PrimitiveShapes.Lighten(BorderColour) : PrimitiveShapes.Darken(BorderColour);

            //Draw shaded rectangle
            Rectangle shade = new Rectangle(Location.X + 1, Location.Y + 1, Location.Width - 2, Location.Height - 3);
            PrimitiveShapes.DrawShadedRectangle(spriteBatch, shade, topLeft, bottomRight, 2.0f);
        }
    }
}
