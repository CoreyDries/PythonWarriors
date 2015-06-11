using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Python_Team;
using Controls;
using Game_Engine_Team.Texture;
using Game_Engine_Team.Actors;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Scripting;
using System.Windows.Forms;
using IronPython.Runtime;
using System.Net;

namespace Game_Engine_Team
{

    public class PlayEnvironment : Screen
    {
        private Player player;
        private Dungeon dungeon;
        private UserScript script;

        public bool PlayerTurn { get; private set; }

        public const double TURN_LENGTH = 2d / 3;

        private readonly GameState gameState = new GameState();

        public bool EditMode { get; private set; }

        private EndState endState;

        public PlayEnvironment( MainController game, Dungeon dungeon, Player _player, bool editMode = false )
            : base( game )
        {
            PlayerTurn = true;
            this.dungeon = dungeon;
            this.player = _player;

            EditMode = editMode;

            dungeon.Player = player;

            script = dungeon.PythonScript;
            script.Game = gameState;
            script[ "player" ] = player;

            script.AddVariables( dungeon.NamedEntities );

            gameState.waypoints = dungeon.Waypoints;

            if ( script.HasSyntaxError )
            {
                string message = "This Dungeon has some errors with its script; the script will not run.\nWe're sorry :(";

                if ( EditMode )
                {
                    message += "\nMessage: " + script.Error.Message
                             + "\nError Code: " + script.Error.ErrorCode
                             + "\nLine: " + script.Error.Line
                             + "\nCol: " + script.Error.Column
                             + "\nSource:\n" + script.Error.SourceCode;
                }
                script = new UserScript();

                MessageBox.Show( message, "Oops" );
            }

            try {
                script.Setup();
            }
            catch ( Exception ex )
            {
                string message = "The Python script ran into an error :(";

                if ( EditMode )
                    message += "\nMessage: " + ex.Message;

                script = new UserScript();
                MessageBox.Show( message, "Oops" );
            }
            
            Music = Sounds.SoundDaemon.GetSound(MusicTypes.Battle);
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Time passed since the last call to Update.</param>
        public override void Update( GameTime gameTime )
        {
            base.Update( gameTime );

            gameState.time += gameTime.ElapsedGameTime.TotalSeconds;

            dungeon.Update( gameTime );
            if ( dungeon.RequestsDelay() )
                return;

            if ( PlayerTurn ) {
                PlayerTurn = player.TakeTurn();
            }
            else
            {
                try {
                    script.Run();
                }
                catch ( Exception ex )
                {
                    string message = "The Python script ran into an error :(";

                    if ( EditMode )
                        message += "\nMessage: " + ex.Message;

                    script = new UserScript();
                    MessageBox.Show( message, "Oops" );
                }

                int waitOn = 0; // Number of enemies who have action points left.
                foreach ( Enemy enemy in dungeon.Enemies )
                    waitOn += enemy.TakeTurn() ? 1 : 0;

                if ( waitOn == 0 ) {
                    IncrementTurn();
                    PlayerTurn = true;
                }
            }

            if ( dungeon.GameOver )
            {
                if ( !EditMode )
                {
                    User.Instance.Balance += dungeon.EndState.GoldGain;
                    User.Instance.CurrentCharacter.Experience += dungeon.EndState.ExpGain;
                    
                    try {
                        User.Instance.CurrentCharacter.Save();
                        ServerCommunicationDaemon.Instance.SaveBalance( User.Instance.Balance, User.Instance.AuthToken );
                    } catch ( WebException e )
                    {
                        System.Windows.Forms.MessageBox.Show( e.Message, "Unable to connect to server" );
                        Game.Exit();
                    }
                }
                Game.AddPopUp( new Results( Game, dungeon.EndState.ExpGain, dungeon.EndState.GoldGain, null, dungeon.EndState.Type == EndType.Win ) );
            }
        }

        private void IncrementTurn()
        {
            gameState.turnNo++;
            player.ResetActions();

            foreach ( Hazard hazard in dungeon.Hazards )
                hazard.TakeTurn();

            foreach ( Enemy enemy in dungeon.Enemies )
                enemy.ResetActions();
        }

        public override void Draw( Canvas canvas )
        {
            dungeon.Draw( canvas );

            canvas.Finish += c => c.DrawString( "Xp: " + dungeon.EndState.ExpGain,
                                                Textures.Monospace, Point.Zero,
                                                Color.Magenta, 1, Color.Black );

            canvas.Finish += c => c.DrawString( "Gold: " + dungeon.EndState.GoldGain,
                                                Textures.Monospace, new Point( 0, Tile.HEIGHT / 2 ),
                                                Color.Yellow, 1, Color.Black );
        }

    }
}
