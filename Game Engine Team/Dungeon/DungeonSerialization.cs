using Microsoft.Xna.Framework;
using Python_Team;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

// Andrew Meckling
namespace Game_Engine_Team
{
    [Serializable]
    public partial class Dungeon : ISerializable
    {

        public static readonly string Default = new Dungeon( false ).Serialize();


        private DungeonProxy __proxy;

        public void GetObjectData( SerializationInfo info, StreamingContext context )
        {
            DungeonProxy proxy = new DungeonProxy();

            for ( int x = 0 ; x < WIDTH ; ++x )
                for ( int y = 0 ; y < HEIGHT ; ++y )
                    proxy.Grid[ x, y ] = this[ x, y ].GetProxy();

            foreach ( var pair in NamedEnemies )
                proxy.Enemies.Add( new KeyValuePair<string, EnemyProxy>( pair.Key, pair.Value.GetProxy() ) );

            foreach ( var pair in NamedTraps )
                proxy.Traps.Add( new KeyValuePair<string, TrapProxy>( pair.Key, pair.Value.GetProxy() ) );

            proxy.PlayerSpawnPoint = playerSpawn;
            proxy.PlayerExitPoint = playerExit;

            proxy.Script = PythonScript.Script;

            foreach ( var wp in waypoints )
                proxy.Waypoints.Add( wp );

            info.AddValue( "proxy", proxy, typeof( DungeonProxy ) );
        }


        public Dungeon( SerializationInfo info, StreamingContext context )
            : this()
        {
            __proxy = (DungeonProxy) info.GetValue( "proxy", typeof( DungeonProxy ) );
        }

        [OnDeserialized]
        private void SerializedConstruct( StreamingContext context )
        {
            for ( int x = 0 ; x < WIDTH ; ++x )
                for ( int y = 0 ; y < HEIGHT ; ++y )
                    this[ x, y ] = __proxy.Grid[ x, y ].New( x, y );

            foreach ( var pair in __proxy.Enemies )
                this.InsertNamedEntity( pair.Value.New( this ), pair.Key );

            foreach ( var pair in __proxy.Traps )
                this.InsertNamedEntity( pair.Value.New( this ), pair.Key );

            playerSpawn = new SpawnPoint( __proxy.PlayerSpawnPoint );
            playerExit = new ExitPoint( __proxy.PlayerExitPoint );

            PythonScript = new UserScript( __proxy.Script ?? new UserScript.Code() );

            foreach ( Point p in __proxy.Waypoints ?? new List<Point>() )
                waypoints.Add( new Waypoint( p ) );

            __proxy = null;
        }


        public static void Save( Dungeon dungeon, string name )
        {
            IFormatter formatter = new BinaryFormatter();
            using ( Stream stream = new FileStream( name, FileMode.Create, FileAccess.Write, FileShare.None ) )
            {
                formatter.Serialize( stream, dungeon );
            }
        }

        public static Dungeon Load( string name )
        {
            try
            {
                IFormatter formatter = new BinaryFormatter();
                using ( Stream stream = new FileStream( name, FileMode.Open, FileAccess.Read, FileShare.Read ) )
                {
                    return (Dungeon) formatter.Deserialize( stream );
                }
            }
            catch ( InvalidCastException ex )
            {
                Console.Error.WriteLine( ex.Message );
                return null;
            }
            catch ( System.Runtime.Serialization.SerializationException ex )
            {
                Console.Error.WriteLine( ex.Message );
                return null;
            }
            catch ( System.IO.FileNotFoundException ex )
            {
                Console.Error.WriteLine( ex.Message );
                return null;
            }
        }

        public static string Serialize( Dungeon dungeon )
        {
            using ( MemoryStream stream = new MemoryStream() )
            {
                IFormatter formatter = new BinaryFormatter();

                formatter.Serialize( stream, dungeon );
                stream.Position = 0;
                using ( MemoryStream output = new MemoryStream() )
                using ( DeflateStream deflateStream = new DeflateStream( output, CompressionMode.Compress ) )
                {
                    stream.CopyTo( deflateStream );

                    output.Position = 0;
                    deflateStream.Close();
                    return Convert.ToBase64String( output.ToArray() );
                }
            }
        }

        public static Dungeon Deserialize( string data )
        {
            IFormatter formatter = new BinaryFormatter();
            using ( MemoryStream stream = new MemoryStream( Convert.FromBase64String( data ) ) )
            using ( DeflateStream deflateStream = new DeflateStream( stream, CompressionMode.Decompress ) )
            using ( MemoryStream output = new MemoryStream() )
            {
                deflateStream.CopyTo( output );
                deflateStream.Close();

                output.Position = 0;

                return (Dungeon) formatter.Deserialize( output );
            }
        }
    }

    public static class DungeonExtentions
    {
        public static void Save( this Dungeon dungeon, string name )
        {
            Dungeon.Save( dungeon, name );
        }

        public static string Serialize( this Dungeon dungeon )
        {
            return Dungeon.Serialize( dungeon );
        }
    }
}
