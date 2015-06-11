using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Controls;

namespace Game_Engine_Team
{
    /// <summary>
    /// Creates a popup screen that displays characters created by the user
    /// Allows options to select the wanted character or create a character
    /// Cancel closes the popup screen and goes back to the main menu
    /// 
    /// Primary Auhor: Enoch Yip
    /// 
    /// </summary>
    /// 

    //to do 
    //  display one character with stats and such with arrows and index of character displayed. 
    //  arrows will increment/decrement index which changes the character on screen
    class CharacterSelect : Screen
    {
        //Possible animation of the character sprite in the select screen

        /// <summary>
        /// The button controls that exist in the screen.
        /// </summary>
        private LinkedList<Button> buttons;

        /// <summary>
        /// The bounds of the popup character select screen
        /// </summary>
        public Rectangle bounds;

        /// <summary>
        /// The bounds of the displayed character
        /// </summary>
        public Rectangle CharacterBounds;

        // Dictionary<string, int> characters

        private KeyValuePair<string, Character> currentCharacter
        {
            get {
                return User.Instance.Characters.ToList()[ currIndex ];
            }
        }

        /// <summary>
        /// The index of the currently selected character
        /// </summary>
        int currIndex = 0;

        /// <summary>
        /// Status of the screen
        /// </summary>
        public bool display { get; set; }

        /// <summary>
        /// Content manager passed in from MainMenu
        /// </summary>
        public ContentManager Content { get; private set; }

        /// <summary>
        /// The middle of the screen.
        /// </summary>
        private int middle;

        /// <summary>
        /// Font of the different texts
        /// </summary>
        public SpriteFont monospace;

        /// <summary>
        /// Creates a Character select screen
        /// </summary>
        /// <param name="content"></param>
        public CharacterSelect( MainController game )
            : base( game )
        {
            display = true;
            int width = Screen.Width / 2;
            int height = (int) (Screen.Height * .8);

            int x = (Screen.Width - width) / 2;
            int y = (Screen.Height - height) / 2;

            bounds = new Rectangle( x, y, width, height );

            CharacterBounds = new Rectangle( ((x + width) / 2) + 3, (y + height) / 4, width / 2, height / 2 );

            Content = game.Content;

            buttons = new LinkedList<Button>();

            monospace = Content.Load<SpriteFont>( "Fonts/MonoSpace" );

            buttons.AddLast( new Button( monospace, "<--", 0, 0, 40 ) );
            buttons.Last.Value.On_Button_Click += new Button.ButtonClick( LeftArrow );
            buttons.AddLast( new Button( monospace, "-->", 0, 0, 40 ) );
            buttons.Last.Value.On_Button_Click += new Button.ButtonClick( RightArrow );
            buttons.AddLast( new Button( monospace, "Select", 0, 0, 100 ) );
            buttons.Last.Value.On_Button_Click += new Button.ButtonClick( SelectClick );
            buttons.AddLast( new Button( monospace, "Delete", 0, 0, 100 ) );
            buttons.Last.Value.On_Button_Click += new Button.ButtonClick( DeleteClick );
            buttons.AddLast( new Button( monospace, "Cancel", 0, 0, 100 ) );
            buttons.Last.Value.On_Button_Click += new Button.ButtonClick( CancelClick );

            currIndex = 0;
        }

        /// <summary>
        /// Draws the screen
        /// </summary>
        /// <param name="spriteBatch"></param>
        public override void Draw( Canvas spriteBatch )
        {
            //draw buttons
            //todo draw rectangle around selected character
            //todo foreach character in list, draw character name and xp
            PrimitiveShapes.FillRectangle( spriteBatch, bounds, Color.Black );
            PrimitiveShapes.DrawRectangle( spriteBatch, bounds, PrimitiveShapes.Darken( Color.Chocolate ), 3.0f );
            PrimitiveShapes.DrawRectangle( spriteBatch, CharacterBounds, PrimitiveShapes.Darken( Color.Chocolate ), 2.0f );
            //characters.ElementAt( SelectedPlayer ).Draw( spriteBatch, new Vector2( CharacterBounds.X + (characters.ElementAt( 0 ).Size() / 2), CharacterBounds.Y + (characters.ElementAt( 0 ).Size() / 2) ) );

            foreach ( Button btn in buttons )
                btn.Draw( spriteBatch );

            spriteBatch.DrawString( monospace, "name: ", new Vector2( bounds.Center.X - (bounds.Center.X / 4), bounds.Center.Y + (bounds.Center.Y / 4) + 6 ), Color.White );
            spriteBatch.DrawString( monospace, currentCharacter.Key, new Vector2( bounds.Center.X - (bounds.Center.X / 4) + 50, bounds.Center.Y + (bounds.Center.Y / 4) + 6 ), Color.White );
            spriteBatch.DrawString( monospace, "XP: ", new Vector2( bounds.Center.X - (bounds.Center.X / 4), bounds.Center.Y + (bounds.Center.Y / 4) + 26 ), Color.White );
            spriteBatch.DrawString( monospace, currentCharacter.Value.Experience.ToString(), new Vector2( bounds.Center.X - (bounds.Center.X / 4) + 50, bounds.Center.Y + (bounds.Center.Y / 4) + 26 ), Color.White );
        }

        /// <summary>
        /// Fires when the left arrow button is clicked
        /// </summary>
        public void LeftArrow()
        {

            //todo decrement index of character list
            if ( currIndex > 0 )
                currIndex--;
            else
                currIndex = User.Instance.Characters.Count - 1;
        }

        /// <summary>
        /// Fires when the right arrow button is clicked
        /// </summary>
        public void RightArrow()
        {
            currIndex = (currIndex + 1) % User.Instance.Characters.Count;
        }

        /// <summary>
        /// Fires when the submit button is clicked
        /// </summary>
        public void SelectClick()
        {
            //todo set displayed character to selected character 
            //todo implement way to pass data of character index or character object into game
            User.Instance.CurrentCharacterName = currentCharacter.Key;
            display = false;
        }

        /// <summary>
        /// Fires when the create button is clicked
        /// </summary>
        public void DeleteClick()
        {
            ServerCommunicationDaemon.Instance.DeleteCharacter( currentCharacter.Key, User.Instance.AuthToken );
            User.Instance.Characters.Remove( currentCharacter.Key );

            if ( User.Instance.Characters.Count < 1 )
            {
                display = false;
            }
            else
            {
                LeftArrow();
                User.Instance.CurrentCharacterName = currentCharacter.Key;
            }
        }

        /// <summary>
        /// Fires when the cancel button is clicked
        /// </summary>
        public void CancelClick()
        {
            display = false;
        }

        /// <summary>
        /// Updates the screen
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update( Microsoft.Xna.Framework.GameTime gameTime )
        {
            //update buttons
            if ( middle != Screen.Width / 2 )
                middle = Screen.Width / 2;
            int vertical = Screen.Height / 2;
            int verticalPadding = 6;

            int width = buttons.ElementAt( 0 ).Location.Width;
            int height = buttons.First.Value.Location.Height;

            int startX = middle - width - (width / 2) - verticalPadding;
            int startY = vertical + (7 * height - 6 * verticalPadding) - height;

            for ( int i = 0; i < 2; i++ )
            {
                buttons.ElementAt( i ).Location = new Rectangle( startX, startY, width, height );
                buttons.ElementAt( i ).Text = buttons.ElementAt( i ).Text;
                startX += width * 2 + verticalPadding * 2;
            }

            width = buttons.ElementAt( 2 ).Location.Width;
            startX = middle - width - (width / 2) - verticalPadding;
            startY = vertical + (7 * height - 6 * verticalPadding) + verticalPadding;

            for ( int i = 2; i < buttons.Count; i++ )
            {
                buttons.ElementAt( i ).Location = new Rectangle( startX, startY, width, height );
                buttons.ElementAt( i ).Text = buttons.ElementAt( i ).Text;
                startX += width + verticalPadding;
            }

            foreach ( Button btn in buttons )
                btn.Update();

        }
    }
}
