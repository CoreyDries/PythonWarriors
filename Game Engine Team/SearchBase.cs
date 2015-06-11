using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Controls;
using System.Net;


namespace Game_Engine_Team
{
    /// <summary>
    /// Creates a screen that displays dungeons from the server that you can search through and select
    /// to open the game environment with your currently selected character and the selected dungeon.
    /// </summary>
    /// <designer>Enoch Yip</designer>
    /// <developer>Morgan Wynne</developer>
    class SearchBase : Screen
    {
        /// <summary>
        /// A list for the Search, Cancel, ^ and v buttons
        /// </summary>
        private List<Button> StaticButtons;

        /// <summary>
        /// A list of tuple objects containing dungeon information and the button pressed to access
        /// the dungeon. Populated by all dungeons acquired from the server
        /// </summary>
        private List<Tuple<Button, DungeonInfo>> AquiredDungeons = new List<Tuple<Button, DungeonInfo>>();

        /// <summary>
        /// A list of tuples mentioned before that act as a results of the specified search
        /// </summary>
        private List<Tuple<Button, DungeonInfo>> Dungeons = new List<Tuple<Button, DungeonInfo>>();

        /// <summary>
        /// A list of up to 4 dungeons that are displayed on the screen to select from
        /// Iterated through using the ^ and v keys
        /// </summary>
        private List<Tuple<Button, DungeonInfo>> DisplayedDungeons = new List<Tuple<Button, DungeonInfo>>( 4 );

        /// <summary>
        /// The starting index of the selected set of dungeons
        /// </summary>
        public int SelectedBaseSet = 0;

        /// <summary>
        /// The bounds of the popup Dungeon select screen
        /// </summary>
        public Rectangle bounds;

        /// <summary>
        /// The bounds of the displayed dungeon buttons
        /// </summary>
        public Rectangle DungeonBounds;

        /// <summary>
        /// Status of the screen
        /// </summary>
        public bool display { get; set; }

        /// <summary>
        /// Content manager passed in from MainMenu
        /// </summary>
        public ContentManager Content { get; private set; }

        /// <summary>
        /// Font of the different texts
        /// </summary>
        public SpriteFont monospace;

        /// <summary>
        /// The character name search textfield
        /// </summary>
        private TextField CharacterNameTextField;

        /// <summary>
        /// The username search textfield
        /// </summary>
        private TextField UsernameTextField;

        /// <summary>
        /// Creates a Search Base screen
        /// </summary>
        /// <param name="content"></param>
        public SearchBase( MainController game ) : base( game )
        {
            display = true;
            // Defines the width and height of the bounding box
            int width = (int) (Screen.Width * .8);
            int height = (int) (Screen.Height * .8);

            // Defines the top-left corner of the bounding box
            int x = (Screen.Width - width) / 2;
            int y = (Screen.Height - height) / 2;

            bounds = new Rectangle( x, y, width, height );
            List<DungeonInfo> bases = new List<DungeonInfo>();

            // Specifies the dungeon buttons bounds with relation to 
            DungeonBounds = new Rectangle( ((x + width) / 3), (y + height) / 4, (int) (width * .7025), (int) (height * .675) );

            // Loads the monospace font from the game's content
            monospace = Game.Content.Load<SpriteFont>( "Fonts/MonoSpace" );

            // Y position of the cancel, up, and down buttons
            int yPosition = DungeonBounds.Y + DungeonBounds.Height + 6;

            // Creates the action button objects
            Button searchButton = new Button( monospace, "Search", 0, 0 );
            Button cancelButton = new Button( monospace, "Cancel", 0, yPosition );
            Button upArrowButton = new Button( monospace, "^", 0, yPosition, 40 );
            Button downArrowButton = new Button( monospace, "v", 0, yPosition, 40 );

            // Assigns action listener to the action buttons
            searchButton.On_Button_Click += new Button.ButtonClick( SearchClick );
            cancelButton.On_Button_Click += new Button.ButtonClick( CancelClick );
            upArrowButton.On_Button_Click += new Button.ButtonClick( UpArrow );
            downArrowButton.On_Button_Click += new Button.ButtonClick( DownArrow );

            // Creates the list of the action buttons
            StaticButtons = new List<Button>() {
                searchButton,
                cancelButton,
                upArrowButton,
                downArrowButton };

            // Search and acquire all of the characters and dungeons
            try {
                bases = ServerCommunicationDaemon.Instance.SearchCharacter( "", "" );
            } catch ( WebException e )
            {
                System.Windows.Forms.MessageBox.Show( e.Message, "Unable to connect to server" );
                Game.Exit();
            }

            // Creates tuple of buttons to dungeon info
            foreach ( DungeonInfo dungeon in bases )
                AquiredDungeons.Add( new Tuple<Button,DungeonInfo>( new Button( monospace ), dungeon ) );

            // Adds event listeners to each of the buttons in the tuple
            foreach ( Tuple<Button, DungeonInfo> tuple in AquiredDungeons )
                tuple.Item1.On_Select_Button_Click += new Button.SelectButtonClick( SelectClick );

            // Passes all of the aquired dungeons to the dungeons list
            Dungeons = AquiredDungeons;

            // Text field for Username search entry
            CharacterNameTextField = new TextField(
                monospace,
                bounds.X + 20,
                DungeonBounds.Y + 20,
                (int) ((DungeonBounds.X - bounds.X) * .75) );

            // Textfield for Character name search entry
            UsernameTextField = new TextField(
                monospace,
                bounds.X + 20,
                DungeonBounds.Y + 80,
                (int) ( (DungeonBounds.X - bounds.X) * .75 ) );
        }

        /// <summary>
        /// Draws the screen
        /// </summary>
        public override void Draw( Canvas canvas )
        {
            // Draw the bounding rectangles
            PrimitiveShapes.FillRectangle( canvas, bounds, Color.Black );
            PrimitiveShapes.DrawRectangle( canvas, bounds, PrimitiveShapes.Darken( Color.Chocolate ), 3.0f );
            PrimitiveShapes.DrawRectangle( canvas, DungeonBounds, PrimitiveShapes.Darken( Color.Chocolate ), 1.0f );

            // Draws the strings to describe the text boxes
            canvas.DrawString( monospace, "Charname:", new Vector2( bounds.X + 20, DungeonBounds.Y ), Color.White );
            canvas.DrawString( monospace, "Username:", new Vector2( bounds.X + 20, DungeonBounds.Y + 60 ), Color.White );

            //Draws all buttons
            DisplayedDungeons.ForEach( b => b.Item1.Draw( canvas ) );
            StaticButtons.ForEach( b => b.Draw( canvas ) ) ;

            //Draw character name and username text fields
            CharacterNameTextField.Draw( canvas );
            UsernameTextField.Draw( canvas );
        }

        /// <summary>
        /// Updates the screen
        /// </summary>
        public override void Update( Microsoft.Xna.Framework.GameTime gameTime )
        {
            base.Update( gameTime );

            // Location variables
            int padding = 4;
            int xPosition = DungeonBounds.X;
            int startX = DungeonBounds.X + padding;
            int startY = DungeonBounds.Y + padding;

            // Updates text fields
            CharacterNameTextField.Update( gameTime );
            UsernameTextField.Update( gameTime );

            // Updates the Search button
            StaticButtons[ 0 ].Location.X = bounds.X + 20;
            StaticButtons[ 0 ].Location.Y = (int)( UsernameTextField.Location.Y + UsernameTextField.Size.Y ) + (padding * 3);

            // Sets the position for the Cancel, ^ and v buttons
            for ( int i = 1; i < StaticButtons.Count; i++ )
            {
                StaticButtons[ i ].Location.X = xPosition;
                // Increases the x position to show the next button
                xPosition += StaticButtons[ i ].Location.Width + padding;
            }

            // Makes all the button text show (dunno why you have to do this)
            StaticButtons.ForEach( b => b.Text = b.Text );

            // Gets maximum of 4 buttons from the Dungeon list
            DisplayedDungeons = Dungeons.GetRange( SelectedBaseSet, (Dungeons.Count - SelectedBaseSet < 4) ? Dungeons.Count - SelectedBaseSet : 4 );

            // Iterates through all the dungeon buttons to be displayed
            for ( int i = 0; i < DisplayedDungeons.Count; i++ )
            {
                // Extracts the tuplets for readability
                Button button = DisplayedDungeons[ i ].Item1;
                DungeonInfo dungeon = DisplayedDungeons[ i ].Item2;

                // Sets the location and size of the button
                button.Location.X = startX;
                button.Location.Y = startY;
                button.Location.Width = (DungeonBounds.Width / 2) - (int) (padding * 1.5);
                button.Location.Height = (DungeonBounds.Height / 2) - (int) (padding * 1.5);

                // Sets the button text, possibly put elsewhere
                button.Text = "Base: "       + dungeon.Name     + "\n"
                            + "Username:\n"  + dungeon.UserName + "\n"
                            + "XP: "         + dungeon.Exp;

                // Display the 3rd and 4th buttons on the bottom row
                if ( i % 2 == 1 )
                {
                    startY += (DungeonBounds.Height / 2) - (int) (padding / 2);
                    startX = DungeonBounds.X + padding;
                }
                else
                {
                    // Display the 2nd and 4th buttons on the right column
                    startX += (DungeonBounds.Width / 2) - (int) (padding / 2);
                }
            }

            // Updates control buttons
            foreach ( Button button in StaticButtons )
                button.Update();

            // Updates Base selection buttons
            foreach ( Tuple<Button, DungeonInfo> tuple in DisplayedDungeons )
                tuple.Item1.Update();
        }

        /// <summary>
        /// Action listener for the v button, iterates forward through the list of dungeons
        /// </summary>
        public void DownArrow()
        {
            if ( SelectedBaseSet < Dungeons.Count - 3 )
            {
                SelectedBaseSet += 2;
            }
        }

        /// <summary>
        /// Action listener for the ^ button, iterates backwards through the list of dungeons
        /// </summary>
        public void UpArrow()
        {
            if ( SelectedBaseSet > 1 )
            {
                SelectedBaseSet -= 2;
            }
        }

        /// <summary>
        /// Action listener for the dungeon buttons, opens the game environment using the selected dungeon from the server
        /// </summary>
        public void SelectClick( Object sender )
        {
            // Gets the corrosponding DungeonInfo object associated with the clicked button
            DungeonInfo dungeonInfo = DisplayedDungeons.Where( info => info.Item1 == sender ).Single().Item2;

            // Gets the dungeon from the server and sets up the player instance
            Dungeon dungeon = ServerCommunicationDaemon.Instance.GetStageByUserChar( dungeonInfo.UserName, dungeonInfo.Name );
            Game_Engine_Team.Actors.Player player = User.Instance.CurrentCharacter.GetPlayer( dungeon );

            // Launches the game screen
            Game.Launch( new PlayEnvironment( Game, dungeon, player ) );
            display = false;
        }

        /// <summary>
        /// Action listener for when the search button is pressed. Extracts only dungeons from the fully populated
        /// dungeon list that match the search requirements
        /// </summary>
        public void SearchClick()
        {
            // Resets the iteration of dungeons
            SelectedBaseSet = 0;

            // This is one of those lines that I'm proud of and not proud of at the same time.
            Dungeons = AquiredDungeons.Where( tuple => ( UsernameTextField.Text != ""
                                                            ? tuple.Item2.UserName.ToLower().Contains( UsernameTextField.Text.ToLower() ) 
                                                            : true ) 
                                                    && ( CharacterNameTextField.Text != ""
                                                            ? tuple.Item2.Name.ToLower().Contains( CharacterNameTextField.Text.ToLower() ) 
                                                            : true ) ).ToList();
            

        }

        /// <summary>
        /// Action listener for the Cancel button, returns to the main menu
        /// </summary>
        public void CancelClick()
        {
            Game.Back();
            display = false;
        }

    }
}
