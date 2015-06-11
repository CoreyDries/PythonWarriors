using Controls;
using Game_Engine_Team.Actors;
using Game_Engine_Team.Equipment;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Game_Engine_Team
{

    /// <summary>
    /// The screen that allows users to create a new character.
    /// At the moment a character can be either Warrior,
    /// Ranger or Rogue, and the only other parameter is a name.
    /// 
    /// Created by: Corey Dries
    /// </summary>
    class CharacterCreate : Screen
    {
        private Texture2D background;
        private SpriteFont monospace;
        private LinkedList<Button> buttons;
        private TextField characterName;

        private const int DENOM = 16;

        private enum character
        {
            Warrior, Mage, Rogue
        }
        character selected = character.Warrior;

        public ContentManager Content;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="game"></param>
        public CharacterCreate( MainController game )
            : base( game )
        {
            buttons = new LinkedList<Button>();

            Content = game.Content;
            LoadContent( Content );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update( GameTime gameTime )
        {
            base.Update( gameTime );
            characterName.Update( gameTime );
            foreach ( Button btn in buttons )
                btn.Update();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="content"></param>
        private void LoadContent( ContentManager content )
        {
            monospace = content.Load<SpriteFont>( "Fonts/MonoSpace" );
            background = content.Load<Texture2D>( "Backgrounds/MainMenu" );

            buttons.AddLast( new Button( monospace, "Warrior", ( Screen.Width * 5 / DENOM ) - 50, Screen.Height * 4 / DENOM, 100, 100 ) );
            buttons.Last.Value.On_Button_Click += new Button.ButtonClick( Choose_Warrior );
            buttons.AddLast( new Button( monospace, "Mage", ( Screen.Width * 8 / DENOM ) - 50, Screen.Height * 4 / DENOM, 100, 100 ) );
            buttons.Last.Value.On_Button_Click += new Button.ButtonClick( Choose_Ranger );
            buttons.AddLast( new Button( monospace, "Rogue", ( Screen.Width * 11 / DENOM ) - 50, Screen.Height * 4 / DENOM, 100, 100 ) );
            buttons.Last.Value.On_Button_Click += new Button.ButtonClick( Choose_Rogue );
            buttons.AddLast( new Button( monospace, "Submit", ( Screen.Width * 8 / DENOM ) - 50, Screen.Height * 10 / DENOM, 100, 30 ) );
            buttons.Last.Value.On_Button_Click += new Button.ButtonClick( Submit );

            characterName = new TextField( monospace, ( Screen.Width * 8 / DENOM ) - 105, ( Screen.Height * 8 / DENOM ) + 20, 210 );
            characterName.MaxCharacters = 20;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="canvas"></param>
        public override void Draw( Canvas canvas )
        {
            canvas.Draw( background, new Rectangle( 0, 0, Screen.Width, Screen.Height ), Color.White );

            characterName.Draw( canvas );
            canvas.DrawString( monospace, "Character Name:", new Vector2( ( Screen.Width * 8 / DENOM ) - 105, ( Screen.Height * 8 / DENOM ) ), Color.Blue );
            Rectangle highlight;
            if ( selected == character.Warrior )
                highlight = new Rectangle( ( Screen.Width * 5 / DENOM ) - 55, ( Screen.Height * 4 / DENOM ) - 5, 110, 110 );
            else if ( selected == character.Mage )
                highlight = new Rectangle( ( Screen.Width * 8 / DENOM ) - 55, ( Screen.Height * 4 / DENOM ) - 5, 110, 110 );
            else
                highlight = new Rectangle( ( Screen.Width * 11 / DENOM ) - 55, ( Screen.Height * 4 / DENOM ) - 5, 110, 110 );
            canvas.DrawRect( highlight, Color.Blue );

            foreach ( Button btn in buttons )
                btn.Draw( canvas );
        }

        /// <summary>
        /// 
        /// </summary>
        private void Choose_Warrior()
        {
            selected = character.Warrior;
        }

        /// <summary>
        /// 
        /// </summary>
        private void Choose_Ranger()
        {
            selected = character.Mage;
        }

        /// <summary>
        /// 
        /// </summary>
        private void Choose_Rogue()
        {
            selected = character.Rogue;
        }

        /// <summary>
        /// 
        /// </summary>
        private void Submit()
        {
            IPlayerProxy player = null;

            if ( characterName.Text == string.Empty )
                return;

            switch ( selected )
            {
                case character.Warrior:
                    player = new PlayerProxy<Warrior>( new Stats( 200 ), new EquipmentSet() );
                    break;

                case character.Mage:
                    player = new PlayerProxy<Mage>( new Stats( 200 ), new EquipmentSet() );
                    break;

                case character.Rogue:
                    player = new PlayerProxy<Rogue>( new Stats( 200 ), new EquipmentSet() );
                    break;

                default:
                    return;
            }

            Character tempChar = new Character( characterName.Text, player );
            try {
                ServerCommunicationDaemon.Instance.CreateANewCharacter( tempChar, User.Instance.AuthToken );

                User.Instance.Characters[ tempChar.Name ] = tempChar;

                User.Instance.CurrentCharacterName = tempChar.Name;
            } catch ( WebException e ) 
            {
                System.Windows.Forms.MessageBox.Show( e.Message, "Unable to connect to the server" );
                Game.Exit();
            }

            Game.Back();
        }
    }
}
