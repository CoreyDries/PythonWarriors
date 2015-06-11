using Controls;
using Game_Engine_Team.Actors;
using Game_Engine_Team.Texture;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Python_Team;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Game_Engine_Team
{
    public class EditEnvironment : Screen
    {
        private WallTexture wallGraphicA;
        private WallTexture wallGraphicB;

        private GroundTexture floorGraphicA;
        private GroundTexture floorGraphicB;

        private GroundTexture bridgeGraphicA;

        private PitTexture pitGraphicA;
        private PitTexture pitGraphicB;
        private PitTexture pitGraphicC;
        private PitTexture pitGraphicD;

        private Dungeon dungeon;

        private EditToolbox toolbox;


        Tile[] BuilderTiles;

        public enum PaintingMode
        {
            None, Tiles, Enemies, Traps, Spawn, Exit, Waypoint
        }

        private void LoadScript( Dungeon dungeon )
        {
            File.Delete( UserScript.MAIN_CODE_FILENAME );
            var main = File.AppendText( UserScript.MAIN_CODE_FILENAME );

            File.Delete( UserScript.SETUP_CODE_FILENAME );
            var setup = File.AppendText( UserScript.SETUP_CODE_FILENAME );

            main.Write( dungeon.PythonScript.Script.MainCode );
            setup.Write( dungeon.PythonScript.Script.SetupCode );

            main.Flush();
            setup.Flush();

            main.Close();
            setup.Close();
        }

        public EditEnvironment( MainController game, Dungeon _dungeon )
            : base( game )
        {
            this.dungeon = _dungeon ?? Dungeon.Load( "Dungeon.bin" );
            this.dungeon.EditMode = true;

            LoadScript( dungeon );

            LoadContent();

            //Dummy data need to be replaced with Tiles the User has access to
            BuilderTiles = new Tile[] {
                new WallTile( Dungeon.WIDTH, 0, wallGraphicA ),
                new WallTile( Dungeon.WIDTH + 1, 0, wallGraphicB ),
                new GroundTile( Dungeon.WIDTH, 1, floorGraphicA ),
                new GroundTile( Dungeon.WIDTH + 1, 1, floorGraphicB ),
                new PitTile( Dungeon.WIDTH, 2, pitGraphicA ),
                new PitTile( Dungeon.WIDTH + 1, 2, pitGraphicB ),
                new BridgeTile( Dungeon.WIDTH, 3, bridgeGraphicA ),
                new BridgeTile( Dungeon.WIDTH + 1, 3, floorGraphicA ),
                new PitTile( Dungeon.WIDTH, 4, pitGraphicC ),
                new PitTile( Dungeon.WIDTH + 1, 4, pitGraphicD ),
            };


            totalExp = User.Instance.CurrentCharacter.Experience;


            toolbox = new EditToolbox();

            toolbox.AddTile( "Light Wall", BuilderTiles[ 0 ] );
            toolbox.AddTile( "Dark Wall", BuilderTiles[ 1 ] );
            toolbox.AddTile( "Light Floor", BuilderTiles[ 2 ] );
            toolbox.AddTile( "Dark Floor", BuilderTiles[ 3 ] );
            toolbox.AddTile( "Empty Pit", BuilderTiles[ 4 ] );
            toolbox.AddTile( "Lava Pit", BuilderTiles[ 5 ] );
            toolbox.AddTile( "Wood Bridge", BuilderTiles[ 6 ] );
            toolbox.AddTile( "Stone Bridge", BuilderTiles[ 7 ] );
            toolbox.AddTile( "Pool", BuilderTiles[ 8 ] );
            toolbox.AddTile( "Water Pit", BuilderTiles[ 9 ] );

            Inventory inv = User.Instance.CurrentCharacter.InventoryItems;

            foreach ( var enemyType in EnumUtil.GetValues<EnemyType>() )
                if ( inv[ enemyType ] > 0 )
                    toolbox.AddActor( enemyType );

            foreach ( var trapType in EnumUtil.GetValues<TrapType>() )
                toolbox.AddTrap( trapType );

            toolbox.TileSelected += SetSelected;
            toolbox.EnemySelected += SetSelected;
            toolbox.TrapSelected += SetSelected;
            toolbox.SpawnSelected += SetSpawnMode;
            toolbox.ExitSelected += SetExitMode;
            toolbox.WaypointSelected += SetWaypointMode;

            toolbox.LinkDungeon( this.dungeon );

            toolbox.Show();

            MouseRightDown += DeleteEntity;
            MouseRightDown += Unselect;

            MouseMiddleDown += DuplicateEntity;

            MouseLeftDown += DetectSelectTile;
            MouseLeftDown += DetectPickUpEntity;

            MouseLeftUp += DetectDragDrop;
        }

        private bool Dragging
        {
            get {
                return dragStart != null;
            }
            set {
                if ( !value )
                    dragStart = null;
            }
        }

        private Point? dragStart = null;

        public override void Dispose()
        {
            toolbox.Close();
            User.Instance.CurrentCharacter.SerializedDungeon = dungeon.Serialize();

            // Save the serialized dungeon to the database
            try {
                if ( !User.Instance.CurrentCharacter.Save() )
                    System.Windows.Forms.MessageBox.Show( "Failed to save character to server" );

            } catch ( WebException e )
            {
                System.Windows.Forms.MessageBox.Show( e.Message, "Unable to connect to server" );
                Game.Exit();
            }

            base.Dispose();
        }

        private bool isPainting = false;
        
        private EnemyType selectedEnemy;
        private TrapType selectedTrap;
        private Waypoint selectedWaypoint;

        private string nextEntityName = null;
        private Tile  selectedTile;

        public PaintingMode PaintMode { get; private set; }

        private int totalExp;

        private void PaintAt( int x, int y )
        {
            if ( !dungeon.ContainsCoord( new Point( x, y ) ) )
                return;

            switch ( PaintMode )
            {
                case PaintingMode.None:
                    break;

                case PaintingMode.Tiles:
                    dungeon.PlaceTile( x, y, selectedTile );
                    break;

                case PaintingMode.Enemies:
                    if ( !CheckSufficiantExp( Enemy.Database.GetExpCost( selectedEnemy ) ) )
                        break;

                    if ( dungeon.PlaceEnemy( x, y, selectedEnemy, nextEntityName ) )
                        UnsetSelected();

                    break;

                case PaintingMode.Traps:
                    if ( !CheckSufficiantExp( Trap.Database.GetExpCost( selectedTrap ) ) )
                        break;

                    if ( dungeon.PlaceTrap( x, y, selectedTrap, nextEntityName ) )
                        UnsetSelected();

                    break;

                case PaintingMode.Spawn:
                    if ( dungeon.SetPlayerSpawn( x, y ) )
                        UnsetSelected();
                    break;

                case PaintingMode.Exit:
                    if ( dungeon.SetPlayerExit( x, y ) )
                        UnsetSelected();
                    break;

                case PaintingMode.Waypoint:
                    if ( selectedWaypoint != null )
                        selectedWaypoint.Position = new Point( x, y );
                    else
                        dungeon.AddWaypoint( x, y );

                    UnsetSelected();
                    break;
            }
            dragStart = null;
        }

        private int SelectedCost()
        {
            if ( PaintMode == PaintingMode.Enemies )
                return Enemy.Database.GetExpCost( selectedEnemy );

            if ( PaintMode == PaintingMode.Traps )
                return Trap.Database.GetExpCost( selectedTrap );

            return 0;
        }

        private bool CheckSufficiantExp( int cost )
        {
            int pendingCost = dungeon.GetDungeonExpCost() + cost;

            if ( pendingCost <= totalExp )
                return true;

            System.Windows.Forms.MessageBox.Show( "You do not enough unspent xp to place that item." );

            return false;
        }

        private void RemoveEntityAt( Point pos )
        {
            dungeon.RemoveEntityAt( pos );
        }

        private void RemoveEntityAt( int x, int y )
        {
            dungeon.RemoveEntityAt( x, y );
        }

        private void RemoveEntity( IEntity entity )
        {
            dungeon.RemoveEntity( entity );
        }

        public Sprite SelectedIcon
        {
            get {
                switch ( PaintMode )
                {
                    default:
                    case PaintingMode.None:
                        return new NullSprite();

                    case PaintingMode.Tiles:
                        return selectedTile.Texture.Icon;

                    case PaintingMode.Enemies:
                        return Textures.Get( selectedEnemy ).Icon;

                    case PaintingMode.Traps:
                        return Textures.Get( selectedTrap ).Icon;

                    case PaintingMode.Spawn:
                        return Textures.Get( PlayerType.Base ).Icon;

                    case PaintingMode.Exit:
                        return Textures.Get( SpriteType.ExitPoint ).Icon;

                    case PaintingMode.Waypoint:
                        return Textures.Get( EffectType.Waypoint ).Icon;
                }
            }
        }

        public void UnsetSelected()
        {
            PaintMode = PaintingMode.None;
            selectedWaypoint = null;
            nextEntityName = null;
        }

        public void SetSelected( Tile tile )
        {
            selectedTile = tile;
            PaintMode = PaintingMode.Tiles;
        }

        public void SetSelected( EnemyType type )
        {
            selectedEnemy = type;
            nextEntityName = null;
            PaintMode = PaintingMode.Enemies;
        }

        public void SetSelected( TrapType type )
        {
            selectedTrap = type;
            PaintMode = PaintingMode.Traps;
        }

        public void SetSelected( Waypoint waypoint )
        {
            selectedWaypoint = waypoint;
            PaintMode = PaintingMode.Waypoint;
        }

        public void SetSpawnMode()
        {
            PaintMode = PaintingMode.Spawn;
        }

        public void SetExitMode()
        {
            PaintMode = PaintingMode.Exit;
        }

        public void SetWaypointMode()
        {
            PaintMode = PaintingMode.Waypoint;
        }


        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected void LoadContent()
        {
            wallGraphicA = Textures.Get( WallType.Stone1 );
            wallGraphicB = Textures.Get( WallType.Stone3 );

            floorGraphicA = Textures.Get( GroundType.Stone0 );
            floorGraphicB = Textures.Get( GroundType.Stone2 );

            bridgeGraphicA = Textures.Get( GroundType.Wood1 );

            pitGraphicA = Textures.Get( PitType.Empty0 );
            pitGraphicB = Textures.Get( PitType.RoughLava );
            pitGraphicC = Textures.Get( PitType.Water0 );
            pitGraphicD = Textures.Get( PitType.RoughWater );
        }

        private void UpdateKeyboard( GameTime gameTime )
        {
            if ( Game.Keyboard != Game.PastKeyboard )
            {
                if ( Game.Keyboard.IsKeyDown( Keys.Space ) )
                {
                    dungeon.PythonScript = new Python_Team.UserScript( UserScript.SETUP_CODE_FILENAME, UserScript.MAIN_CODE_FILENAME );

                    dungeon.Save( "Dungeon.bin" );
                    Dungeon dunge = Dungeon.Load( "Dungeon.bin" );

                    Player player = User.Instance.CurrentCharacter.GetPlayer( dunge ); //new Warrior( dunge, stats );

                    Game.Launch( new PlayEnvironment( Game, dunge, player, true ) );
                }
            }
        }

        private Point MousePixelPos()
        {
            return new Point( Game.Mouse.X, Game.Mouse.Y );
        }

        private Point MouseTilePos()
        {
            int x = Game.Mouse.X / Tile.WIDTH;
            int y = Game.Mouse.Y / Tile.HEIGHT;

            return new Point( x, y );
        }

        private delegate void MouseChange( Point pixelPos, Point tilePos, GameTime time );

        private event MouseChange MouseRightDown;
        private event MouseChange MouseRightUp;
        private event MouseChange MouseLeftDown;
        private event MouseChange MouseLeftUp;
        private event MouseChange MouseMiddleDown;
        private event MouseChange MouseMiddleUp;

        private void DeleteEntity( Point pixelPos, Point tilePos, GameTime time )
        {
            if ( PaintMode == PaintingMode.None )
                RemoveEntityAt( tilePos );
        }

        private void Unselect( Point pixelPos, Point tilePos, GameTime time )
        {
            if ( PaintMode != PaintingMode.None )
                UnsetSelected();
        }

        private void DetectSelectTile( Point pixelPos, Point tilePos, GameTime time )
        {
            foreach ( Tile tile in BuilderTiles )
            {
                if ( tile.ContainsPixel( pixelPos ) )
                {
                    SetSelected( tile );
                    break;
                }
            }
        }

        private void DetectPickUpEntity( Point pixelPos, Point tilePos, GameTime time )
        {
            if ( PaintMode == PaintingMode.None )
            {
                IEntity entity = dungeon.FindEntity( tilePos );

                if ( entity is Enemy )
                    SetSelected( (entity as Enemy).MyType.Value );

                else if ( entity is Trap )
                    SetSelected( (entity as Trap).MyType );

                else if ( entity is Waypoint )
                {
                    SetSelected( entity as Waypoint );
                    dragStart = tilePos;
                    return;
                }
                else
                    return;

                nextEntityName = dungeon.GetEntityName( entity );
                RemoveEntity( entity );

                dragStart = tilePos;
            }
        }

        private void DuplicateEntity( Point pixelPos, Point tilePos, GameTime time )
        {
            if ( PaintMode == PaintingMode.None )
            {
                IEntity entity = dungeon.FindEntity( tilePos );

                if ( entity is Enemy )
                    SetSelected( (entity as Enemy).MyType.Value );

                else if ( entity is Trap )
                    SetSelected( (entity as Trap).MyType );

                else if ( entity is Waypoint )
                    SetWaypointMode();
            }
        }

        private void DetectDragDrop( Point pixelPos, Point tilePos, GameTime time )
        {
            if ( Dragging )
            {
                if ( dragStart != tilePos )
                    PaintAt( tilePos.X, tilePos.Y );
            }
        }


        private void UpdateMouse( GameTime gameTime )
        {
            Point mousePixelPos = MousePixelPos();
            Point mouseTilePos = MouseTilePos();

            if ( Game.Mouse.RightButton != Game.PastMouse.RightButton )
            {
                switch ( Game.Mouse.RightButton )
                {
                    case ButtonState.Pressed:
                        if ( MouseRightDown != null )
                            MouseRightDown( mousePixelPos, mouseTilePos, gameTime );
                        break;

                    case ButtonState.Released:
                        if ( MouseRightUp != null )
                            MouseRightUp( mousePixelPos, mouseTilePos, gameTime );
                        break;
                }
            }

            if ( Game.Mouse.LeftButton != Game.PastMouse.LeftButton )
            {
                switch ( Game.Mouse.LeftButton )
                {
                    case ButtonState.Pressed:
                        if ( PaintMode != PaintingMode.None )
                            isPainting = true;

                        if ( MouseLeftDown != null )
                            MouseLeftDown( mousePixelPos, mouseTilePos, gameTime );
                        break;

                    case ButtonState.Released:
                        if ( MouseLeftUp != null )
                            MouseLeftUp( mousePixelPos, mouseTilePos, gameTime );
                        break;
                }
            }

            if ( Game.Mouse.MiddleButton != Game.PastMouse.MiddleButton )
            {
                switch ( Game.Mouse.MiddleButton )
                {
                    case ButtonState.Pressed:
                        if ( PaintMode != PaintingMode.None )
                            isPainting = true;

                        if ( MouseMiddleDown != null )
                            MouseMiddleDown( mousePixelPos, mouseTilePos, gameTime );
                        break;

                    case ButtonState.Released:
                        if ( MouseMiddleUp != null )
                            MouseMiddleUp( mousePixelPos, mouseTilePos, gameTime );
                        break;
                }
            }

            if ( isPainting )
                if ( Game.Mouse.LeftButton == ButtonState.Pressed )
                    PaintAt( mouseTilePos.X, mouseTilePos.Y );
                else
                    isPainting = false;
        }


        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Time passed since the last call to Update.</param>
        public override void Update( GameTime gameTime )
        {
            base.Update( gameTime );

            if ( !Game.IsActive )
                return;

            UpdateKeyboard( gameTime );
            UpdateMouse( gameTime );
        }
        

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        public override void Draw( Canvas canvas )
        {
            canvas.EditMode = true;

            //draw the placed tiles
            dungeon.Draw( canvas );

            //draw the chooseable tiles
            foreach ( Tile bt in BuilderTiles )
                bt.Draw( canvas );

            //draw the current tile the user has selected on their mouse (if applicable)
            if ( !isPainting && PaintMode != PaintingMode.None )
            {
                Point mouse = MouseTilePos();

                if ( dungeon.ContainsCoord( mouse ) )
                {
                    Rectangle rect = new Rectangle( mouse.X * Tile.WIDTH,
                                                    mouse.Y * Tile.HEIGHT,
                                                    Tile.WIDTH,
                                                    Tile.HEIGHT );

                    canvas.DrawRect( rect, new Color( 0, 0, 0, 70 ) );
                }

                Point iconPos = new Point( Game.Mouse.X - (Tile.WIDTH / 2),
                                           Game.Mouse.Y - (Tile.HEIGHT / 2) );

                canvas.Finish += c => c.Draw( SelectedIcon, iconPos, true );
            }

            int remainingXp = User.Instance.CurrentCharacter.Experience - dungeon.GetDungeonExpCost();

            canvas.Finish += c => c.DrawString( "Xp: " + remainingXp + "/" + User.Instance.CurrentCharacter.Experience,
                                                Textures.Monospace, Point.Zero,
                                                Color.White, 1, Color.Black );

            int cost = SelectedCost();

            if ( cost > 0 )
                canvas.Finish += c => c.DrawString( "Cost: " + cost,
                                                Textures.Monospace, new Point( 0, Tile.HEIGHT / 2 ),
                                                Color.White, 1, Color.Black );
        }
    }
}
