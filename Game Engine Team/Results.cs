using Game_Engine_Team.Texture;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game_Engine_Team
{
    /// <summary>
    /// Creates and generates a result screen to allow a player to see the results of attacking a base.
    /// </summary>
    class Results : Screen
    {
        /// <summary>
        /// The width of the results screen.
        /// </summary>
        private int width;

        /// <summary>
        /// The height of the results screen.
        /// </summary>
        private int height;

        /// <summary>
        /// The location of the top left of the results screen.
        /// </summary>
        private Point location;

        /// <summary>
        /// The amount of xp gained from battle.
        /// </summary>
        private string xp;

        /// <summary>
        /// The amount of gp gained from battle.
        /// </summary>
        private string gp;

        /// <summary>
        /// The items gained in battle.
        /// </summary>
        private string items;

        /// <summary>
        /// Whether or not the player won the experience.
        /// </summary>
        private string victory;

        /// <summary>
        /// The font used for drawing strings.
        /// </summary>
        private static SpriteFont font;


        /// <summary>
        /// Creates a results screen filled with the information provided.
        /// </summary>
        /// <param name="game">The game in progress.</param>
        /// <param name="xpGain">The amount of experience the player gained.</param>
        /// <param name="gpGain">The amount of gold the player gained.</param>
        /// <param name="itemsFound">The number of items found.</param>
        /// <param name="victory">Whether or not the player "won".</param>
        public Results( MainController game, int xpGain, int gpGain, List<Item> itemsFound, bool victory = false )
            : base( game )
        {
            Music = Sounds.SoundDaemon.GetSound( MusicTypes.Victory );

            items = "Items Found:\r\n";

            if ( itemsFound != null && itemsFound.Count != 0 )
                foreach ( Item item in itemsFound )
                    items += item.Value.ToString() + "\r\n";
            else
                items += "    None...";

            //Draw Information
            this.victory = victory ? "Victory!" : "DEFEAT!";
            xp = "Experience: " + xpGain + " gained!";
            gp = "Gold: " + gpGain + " earned!";

            SetDimensions();

            if ( font == null )
                font = game.Content.Load<SpriteFont>( "Fonts/Monospace" );
        }

        /// <summary>
        /// Sets the dimensions of the screen based on the overall screen size.
        /// </summary>
        private void SetDimensions()
        {
            location = new Point( 4, 3 );
            width = (Screen.Width / Tile.WIDTH) - 8;
            height = (Screen.Height / Tile.HEIGHT) - 6;
        }

        /// <summary>
        /// By calling base.update twice, it will ensure that we traverse back two screens after the results screen is done.
        /// This is a quick fix so that we leave the victory & battle screen.
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update( GameTime gameTime )
        {
            base.Update( gameTime );
            if ( Game.Keyboard != Game.PastKeyboard )
                if ( Game.Keyboard.IsKeyDown( Keys.Escape ) )
                    Game.RemovePopUp();
        }

        /// <summary>
        /// Draws the results screen.
        /// </summary>
        /// <param name="canvas">The canvas used for drawing.</param>
        public override void Draw( Controls.Canvas canvas )
        {
            canvas.Finish += canvas_Finish;
        }

        void canvas_Finish( Controls.Canvas canvas )
        {
            //Draw corners
            Textures.Get( SpriteType.GUITopLeft ).Draw( canvas, new Point( location.X, location.Y ) );
            Textures.Get( SpriteType.GUITopRight ).Draw( canvas, new Point( location.X + width - 1, location.Y ) );
            Textures.Get( SpriteType.GUIBottomLeft ).Draw( canvas, new Point( location.X, location.Y + height - 1 ) );
            Textures.Get( SpriteType.GUIBottomRight ).Draw( canvas, new Point( location.X + width - 1, location.Y + height - 1 ) );

            //Draw Top & Bottom
            for ( int col = 1 ; col < width - 1 ; col++ )
            {
                Textures.Get( SpriteType.GUITop ).Draw( canvas, new Point( location.X + col, location.Y ) );
                Textures.Get( SpriteType.GUIBottom ).Draw( canvas, new Point( location.X + col, location.Y + height - 1 ) );
            }

            //Draw Left & right
            for ( int row = 1 ; row < height - 1 ; row++ )
            {
                Textures.Get( SpriteType.GUILeft ).Draw( canvas, new Point( location.X, location.Y + row ) );
                Textures.Get( SpriteType.GUIRight ).Draw( canvas, new Point( location.X + width - 1, location.Y + row ) );
            }

            //Draw center
            for ( int row = 1 ; row < height - 1 ; row++ )
                for ( int col = 1 ; col < width - 1 ; col++ )
                    Textures.Get( SpriteType.GUICentre ).Draw( canvas, new Point( location.X + col, location.Y + row ) );

            //Draw Information
            int middle = Screen.Width / 2;
            canvas.DrawString( font, victory, new Vector2( middle - font.MeasureString( victory ).X, location.Y * Tile.HEIGHT ), Color.White, 0.0f, Vector2.Zero, 2.0f, SpriteEffects.None, 0.0f );
            canvas.DrawString( font, xp, new Vector2( middle - font.MeasureString( xp ).X / 2, (location.Y + 2) * Tile.HEIGHT ), Color.White );
            canvas.DrawString( font, gp, new Vector2( middle - font.MeasureString( gp ).X / 2, (location.Y + 3) * Tile.HEIGHT ), Color.White );
            canvas.DrawString( font, items, new Vector2( middle - 64, (location.Y + 4) * Tile.HEIGHT ), Color.White );
        }
    }
}
