using Game_Engine_Team.Texture;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.CSharp.RuntimeBinder;
using Game_Engine_Team.Actors;
using System.Diagnostics;
using System.IO;
using Python_Team;

namespace Game_Engine_Team
{

    public delegate void SelectTile( Tile tile );
    public delegate void SelectEnemy( EnemyType type );
    public delegate void SelectTrap( TrapType type );
    public delegate void SelectWaypoint();

    public partial class EditToolbox : Form
    {

        public event SelectTile TileSelected;
        public event SelectEnemy EnemySelected;
        public event SelectTrap TrapSelected;
        public event SelectWaypoint SpawnSelected;
        public event SelectWaypoint ExitSelected;
        public event SelectWaypoint WaypointSelected;

        private ImageList imageList = new ImageList();
        
        // Very dangerous, just like that song.
        private Dictionary<string, object> placeables = new Dictionary<string, object>();

        private TreeNode tilesNode;
        private TreeNode floorsNode;
        private TreeNode wallsNode;
        private TreeNode pitsNode;

        private TreeNode actorsNode;
        private TreeNode trapsNode;


        private void AddImage( Image image )
        {
            imageList.Images.Add( image );
        }

        private void AddImage( string key, Image image, Image imageAlt = null )
        {
            imageList.Images.Add( key, image );
            imageList.Images.Add( key + "_alt", imageAlt ?? image );
        }


        public EditToolbox()
        {
            InitializeComponent();

            AddImage( new Bitmap( 1, 1 ) );

            AddImage( "Spawn", Textures.Get( PlayerType.Base ).ToBitmap() );
            AddImage( "Exit", Textures.Get( SpriteType.ExitPoint ).ToBitmap() );
            AddImage( "Waypoint", Textures.Get( EffectType.Waypoint ).ToBitmap() );

            var spawnNode = treeView1.Nodes.Find( "Spawn", false )[ 0 ];
            spawnNode.ImageKey = "Spawn";
            spawnNode.SelectedImageKey = "Spawn";

            var exitNode = treeView1.Nodes.Find( "Exit", false )[ 0 ];
            exitNode.ImageKey = "Exit";
            exitNode.SelectedImageKey = "Exit";

            var waypointNode = treeView1.Nodes.Find( "Waypoint", false )[ 0 ];
            waypointNode.ImageKey = "Waypoint";
            waypointNode.SelectedImageKey = "Waypoint";

            tilesNode = treeView1.Nodes.Find( "Tiles", false )[ 0 ];
            actorsNode = treeView1.Nodes.Find( "Enemies", false )[ 0 ];
            trapsNode = treeView1.Nodes.Find( "Traps", false )[ 0 ];

            floorsNode = tilesNode.Nodes.Find( "Floors", false )[ 0 ];
            wallsNode = tilesNode.Nodes.Find( "Walls", false )[ 0 ];
            pitsNode = tilesNode.Nodes.Find( "Pits", false )[0];

            treeView1.ImageList = imageList;
            listView1.SmallImageList = imageList;
        }


        public void AddTile( string key, Tile placeable )
        {
            placeables[ key ] = placeable;
            imageList.Images.Add( key, placeable.Texture.Icon.ToBitmap() );

            TreeNode node = tilesNode;

            if ( placeable is GroundTile )
                node = floorsNode;

            else if ( placeable is WallTile )
                node = wallsNode;

            else if ( placeable is PitTile )
                node = pitsNode;

            node.Nodes.Add( key, key, key, key );
        }

        public void AddActor( EnemyType enemy )
        {
            string key = enemy.ToString();

            placeables[ key ] = enemy;
            AddImage( key,
                Textures.Get( enemy ).Icon.ToBitmap(),
                Textures.Get( enemy ).IconAlt.ToBitmap() );

            actorsNode.Nodes.Add( key, key, key, key + "_alt" );
        }

        public void AddTrap( TrapType trap )
        {
            string key = trap.ToString();

            placeables[ key ] = trap;
            AddImage( key,
                Textures.Get( trap ).Icon.ToBitmap(),
                Textures.Get( trap ).IconAlt.ToBitmap() );

            trapsNode.Nodes.Add( key, key, key, key + "_alt" );
        }

        private Dungeon dungeon;

        public void LinkDungeon( Dungeon dungeon )
        {
            this.dungeon = dungeon;

            dungeon.EntityAdded += PlaceEntity;
            dungeon.EntityRemoved += UnplaceEntity;

            foreach ( var pair in dungeon.NamedEntities )
                PlaceEntity( pair.Key, pair.Value );
        }

        public void PlaceEntity( string name, IEntity entity )
        {
            listView1.Items.Add( name, name, entity.ToString() );
        }

        public void UnplaceEntity( string name, IEntity entity )
        {
            listView1.Items.RemoveByKey( name );
        }

        private void listView1_AfterLabelEdit( object sender, LabelEditEventArgs e )
        {
            var item = listView1.Items[ e.Item ];

            string oldName = item.Name;
            e.CancelEdit = !dungeon.RenameEntity( oldName, e.Label );

            if ( !e.CancelEdit )
                item.Name = e.Label;
        }

        private void treeView1_NodeMouseClick( object sender, TreeNodeMouseClickEventArgs e )
        {
            Select( e.Node.Name );
        }

        private void treeView1_AfterSelect( object sender, TreeViewEventArgs e )
        {
            Select( e.Node.Name );
        }

        private void Select( string name )
        {
            if ( placeables.ContainsKey( name ) )
            {
                object selected = placeables[ name ];

                if ( selected is Tile && TileSelected != null )
                    TileSelected( (Tile) selected );

                else if ( selected is EnemyType && EnemySelected != null )
                    EnemySelected( (EnemyType) selected );

                else if ( selected is TrapType && TrapSelected != null )
                    TrapSelected( (TrapType) selected );
            }

            else if ( name == "Spawn" )
            {
                if ( SpawnSelected != null )
                    SpawnSelected();
            }
            else if ( name == "Exit" )
            {
                if ( ExitSelected != null )
                    ExitSelected();
            }
            else if ( name == "Waypoint" )
            {
                if ( WaypointSelected != null )
                    WaypointSelected();
            }
        }

        private void button1_Click( object sender, EventArgs e )
        {
            File.AppendText( UserScript.MAIN_CODE_FILENAME );
            Process.Start( UserScript.MAIN_CODE_FILENAME );
            File.AppendText( UserScript.SETUP_CODE_FILENAME );
            Process.Start( UserScript.SETUP_CODE_FILENAME );
        }

    }

    public static class SpriteExtentions
    {

        public static Bitmap ToBitmap( this Sprite sprite )
        {
            Texture2D texture = (Texture2D) sprite;

            Microsoft.Xna.Framework.Rectangle rect = new Microsoft.Xna.Framework.Rectangle(
                sprite.X * Sprite.WIDTH, sprite.Y * Sprite.HEIGHT, Sprite.WIDTH, Sprite.HEIGHT );

            Microsoft.Xna.Framework.Color[] data = new Microsoft.Xna.Framework.Color[ Sprite.WIDTH * Sprite.HEIGHT ];


            texture.GetData( 0, rect, data, 0, data.Length );

            Texture2D tex = new Texture2D( texture.GraphicsDevice, Sprite.WIDTH, Sprite.HEIGHT );

            tex.SetData( data );


            System.IO.MemoryStream ms = new System.IO.MemoryStream();

            tex.SaveAsPng( ms, Sprite.WIDTH, Sprite.HEIGHT );

            return new System.Drawing.Bitmap( ms );
        }

    }
}
