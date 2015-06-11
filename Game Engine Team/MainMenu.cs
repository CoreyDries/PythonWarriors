using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Controls;
using Game_Engine_Team.Texture;

namespace Game_Engine_Team
{
    /// <summary>
    /// This is the main menu screen of the game. It provides several buttons to allow the user to switch
    /// between screens, depending on what the player wants to do.
    /// 
    /// Author: Jonathan Gribble
    /// Secondary Authors:
    /// 
    /// </summary>
    public class MainMenu : Screen
    {
        /// <summary>
        /// The button controls that exist in the screen.
        /// </summary>
        private List<Button> buttons = new List<Controls.Button>();

        /// <summary>
        /// The middle of the screen.
        /// </summary>
        private int middle;

        /// <summary>
        /// The character select popup
        /// </summary>
        private CharacterSelect popup;

        /// <summary>
        /// Creates a main menu for the first time, and properly centres the buttons based on how big the screen is.
        /// NEEDS A SPRITEFONT IN CONTENT FOLDER AT Fonts/Monospace
        /// </summary>
        /// <param name="content">The content manager used by the game.</param>
        public MainMenu( MainController game )
            : base( game )
        {
            var btnCreate = new Button( Textures.Monospace, "Create Character", 0, 0, 200, 25 );
            btnCreate.On_Button_Click += new Button.ButtonClick( createCharacterClick );
            buttons.Add( btnCreate );

            var btnSelect = new Button( Textures.Monospace, "Select Character", 0, 0, 200, 25 );
            btnSelect.On_Button_Click += new Button.ButtonClick( selectCharacterClick );
            buttons.Add( btnSelect );

            var btnEditCharacter = new Button( Textures.Monospace, "Edit Character", 0, 0, 200, 25 );
            btnEditCharacter.On_Button_Click += new Button.ButtonClick( EditCharacterClick );
            buttons.Add( btnEditCharacter );

            var btnStore = new Button( Textures.Monospace, "Store", 0, 0, 200, 25 );
            btnStore.On_Button_Click += new Button.ButtonClick(StoreClick);
            buttons.Add(btnStore);

            var btnEditBase = new Button( Textures.Monospace, "Edit Base", 0, 0, 200, 25 );
            btnEditBase.On_Button_Click += new Button.ButtonClick( editClick );
            buttons.Add( btnEditBase );

            var btnAttackBase = new Button( Textures.Monospace, "Attack Base", 0, 0, 200, 25 );
            btnAttackBase.On_Button_Click += new Button.ButtonClick( attackClick );

            buttons.Add( btnAttackBase );

            var btnQuit = new Button( Textures.Monospace, "Quit", 0, 0, 200, 25 );
            btnQuit.On_Button_Click += new Button.ButtonClick( exitClick );
            buttons.Add( btnQuit );

            Music = Sounds.SoundDaemon.GetSound( MusicTypes.Menus );
            Centre();
        }

        /// <summary>
        /// Centres the controls into the middle of the gamescreen.
        /// </summary>
        private void Centre()
        {
            middle = Screen.Width / 2;
            int vertical = Screen.Height / 2;
            int verticalPadding = 5;

            int width = buttons[ 0 ].Location.Width;
            int height = buttons[ 0 ].Location.Height;

            int startX = middle - width / 2;
            int startY = vertical - ( 5 * height + 4 * verticalPadding ) / 2;

            foreach ( Button btn in buttons )
            {
                btn.Location = new Rectangle( startX, startY, width, height );
                btn.Text = btn.Text;
                startY += height + verticalPadding;
            }

        }

        /// <summary>
        /// Updates all the buttons in the main menu, and moves the button if the screen has been
        /// resized.
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update( GameTime gameTime )
        {
            if ( User.Instance.Characters.Count < 1 )
                Game.Launch( new CharacterCreate( Game ) );

            if ( middle != Screen.Width / 2 )
                Centre();
            if ( popup == null )
                foreach ( Button btn in buttons )
                    btn.Update();
            else
            {
                popup.Update( gameTime );
                if ( !popup.display )
                    popup = null;
            }

        }

        /// <summary>
        /// Draws the screen.
        /// </summary>
        /// <param name="canvas">The spritebatch used for drawing.</param>
        public override void Draw( Canvas canvas )
        {
            canvas.Draw( Textures.Background, new Rectangle( 0, 0, Screen.Width, Screen.Height ), Color.White );
            //canvas.End();
            foreach ( Button btn in buttons )
                btn.Draw( canvas );
            //canvas.Begin();
            if ( popup != null )
                popup.Draw( canvas );
        }

        /// <summary>
        /// 
        /// </summary>
        public void createCharacterClick()
        {
            Game.Launch( new CharacterCreate( Game ) );
        }

        /// <summary>
        /// Adds StoreEnvironment to the Screen Stack
        /// </summary>
        public void StoreClick()
        {
            Game.Launch(new StoreEnvironment(Game));
        }

        /// <summary>
        /// Creates new CharacterSelect screen and assigns it to popup
        /// </summary>
        public void selectCharacterClick()
        {
            popup = new CharacterSelect( Game );
        }

        public void EditCharacterClick()
        {
            Game.Launch( new CharacterEdit( Game, User.Instance.CurrentCharacter ) );
        }

        /// <summary>
        /// Quit Program
        /// </summary>
        public void exitClick()
        {
            Game.Exit();
        }

        private void attackClick()
        {
            Game.Launch( new SearchBase( Game ) );
        }

        /// <summary>
        /// Adds EditEnvironment to the Screen Stack
        /// </summary>
        public void editClick()
        {
            Dungeon dungeon = User.Instance.CurrentCharacter.GetDungeon();

            Game.Launch( new EditEnvironment( Game, dungeon ) );
        }

        /// <summary>
        /// Creates new CharacterSelect screen and assigns it to popup
        /// </summary>
        public void rClick()
        {
            popup = new CharacterSelect( Game );
        }
    }
}
