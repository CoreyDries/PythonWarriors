using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Game_Engine_Team.Texture;
using Game_Engine_Team.Actors;
using Controls;
using Game_Engine_Team.Sounds;
using Game_Engine_Team.Equipment;
using System.Net;

namespace Game_Engine_Team
{
    /// <summary>
    /// Controls which screen of the game is currently being displayed, and handles basic communication between
    /// the various screens and also handles media playing of songs.
    /// </summary>
    public class MainController : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager graphics;

        public static int WIDTH;
        public static int HEIGHT;
        public static bool FULLSCREEN;

        /// <summary>
        /// The current keyboard state.
        /// </summary>
        public KeyboardState Keyboard { get; private set; }
        /// <summary>
        /// The current mouse state.
        /// </summary>
        public MouseState Mouse { get; private set; }
        /// <summary>
        /// The current gamepad state.
        /// </summary>
        public GamePadState GamePad { get; private set; }

        /// <summary>
        /// The previous keyboard state.
        /// </summary>
        public KeyboardState PastKeyboard { get; private set; }
        /// <summary>
        /// The previous mouse state.
        /// </summary>
        public MouseState PastMouse { get; private set; }
        /// <summary>
        /// The previous gamepad state.
        /// </summary>
        public GamePadState PastGamePad { get; private set; }
        


        private Stack<Screen> screenStack = new Stack<Screen>();

        /// <summary>
        /// Any popup to be drawn/updated on the screen.
        /// </summary>
        private Screen popup;

        private Screen CurrentScreen
        {
            get {
                return screenStack.Count > 0
                       ? screenStack.Peek()
                       : null;
            }
        }

        protected void PushScreen( Screen newScreen )
        {
            screenStack.Push( newScreen );
            PlayMusic( newScreen.Music );
        }

        protected Screen PopScreen()
        {
            Screen screen = screenStack.Pop();
            MediaPlayer.Stop();
            screen.Dispose();

            if ( CurrentScreen != null )
                PlayMusic( CurrentScreen.Music );

            this.SuppressDraw();
            return screen;
        }

        public void PlayMusic( Song song )
        {
            if (song != null && song != MediaPlayer.Queue.ActiveSong)
                MediaPlayer.Play(song);
            else if (song == null)
                MediaPlayer.Stop();
        }

        /// <summary>
        /// Adds a screen to the stack and begins redirecting all Update and 
        /// Draw calls to it.
        /// </summary>
        /// <param name="nextScreen"></param>
        public void Launch( Screen nextScreen )
        {
            PushScreen( nextScreen );
        }

        /// <summary>
        /// Removes the current screen from the stack and stops directing any 
        /// Update or Draw calls to it.
        /// </summary>
        public void Back()
        {
            PopScreen();
        }

        public MainController( User UserInstance )
        {
            this.Window.Title = "Python Warriors";
            graphics = new GraphicsDeviceManager( this );
            Screen.Width = graphics.PreferredBackBufferWidth = ( Dungeon.WIDTH + 2 ) * Tile.WIDTH;
            Screen.Height = graphics.PreferredBackBufferHeight = Dungeon.HEIGHT * Tile.HEIGHT;
            IsMouseVisible = true;
            MediaPlayer.IsRepeating = true;

            Content.RootDirectory = "Content";

            // Set the aquired authentication token to the user
            User.Instance = UserInstance;
            try {
                ServerCommunicationDaemon.Instance.PopulateUserCharacters();
            }
            catch ( WebException e )
            {
                System.Windows.Forms.MessageBox.Show( e.Message, "Unable to connect to server" );
                this.Exit();
            }
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            Textures.Inititialize( Content );
            SoundDaemon.Initialize( Content );
            Equipment.Equipment.Database.Initialize();
            Projectiles.Initialize();
            Enemy.Database.Initialize();
            Trap.Database.Initialize();

            PushScreen( new MainMenu( this ) );
        }


        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Time passed since the last call to Update.</param>
        protected override void Update( GameTime gameTime )
        {
            base.Update( gameTime );

            if ( CurrentScreen == null )
            {
                this.Exit();
                return;
            }

            Keyboard = Microsoft.Xna.Framework.Input.Keyboard.GetState();
            Mouse = Microsoft.Xna.Framework.Input.Mouse.GetState();
            GamePad = Microsoft.Xna.Framework.Input.GamePad.GetState( PlayerIndex.One );

            if ( popup == null )
                CurrentScreen.Update( gameTime );
            else
                popup.Update( gameTime );

            PastKeyboard = Keyboard;
            PastMouse = Mouse;
            PastGamePad = GamePad;
        }


        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw( GameTime gameTime )
        {
            base.Draw( gameTime );

            GraphicsDevice.Clear( new Color( 20, 12, 28 ) );

            using ( Canvas canvas = new Canvas( GraphicsDevice ) )
            {
                CurrentScreen.Draw( canvas );
                if ( popup != null )
                    popup.Draw( canvas );
            }
        }

        /// <summary>
        /// Creates a popup screen, replacing the previous on if there was one.
        /// </summary>
        /// <param name="screen">The screen to apply</param>
        public virtual void AddPopUp( Screen screen )
        {
            popup = screen;
            PlayMusic( popup.Music );
        }

        /// <summary>
        /// Removes the popup by setting it to null;
        /// </summary>
        public virtual void RemovePopUp()
        {
            popup = null;
        }
    }
}
