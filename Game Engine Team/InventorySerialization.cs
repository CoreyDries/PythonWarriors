using Game_Engine_Team.Actors;
using Game_Engine_Team.Equipment;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Game_Engine_Team
{
    [Serializable]
    public class InventoryProxy
    {
        public readonly List<KeyValuePair<WeaponType, int>> Weapons = new List<KeyValuePair<WeaponType, int>>();
        public readonly List<KeyValuePair<HeadType, int>> Helmets = new List<KeyValuePair<HeadType, int>>();
        public readonly List<KeyValuePair<ChestType, int>> Shirts = new List<KeyValuePair<ChestType, int>>();
        public readonly List<KeyValuePair<LegsType, int>> Pants = new List<KeyValuePair<LegsType, int>>();
        public readonly List<KeyValuePair<EnemyType, int>> Enemies = new List<KeyValuePair<EnemyType, int>>();
        public readonly List<KeyValuePair<TrapType, int>> Traps = new List<KeyValuePair<TrapType, int>>();
    }

    [Serializable]
    public partial class Inventory : ISerializable
    {

        public static readonly string Default = new Inventory().Serialize();


        private InventoryProxy __proxy;

        private static KeyValuePair<IType, int> MakePair<IType>( IType type, int value )
        {
            return new KeyValuePair<IType, int>( type, value );
        }

        public void GetObjectData( SerializationInfo info, StreamingContext context )
        {
            InventoryProxy proxy = new InventoryProxy();

            foreach ( var pair in Weapons )
                proxy.Weapons.Add( MakePair( pair.Key, pair.Value ) );

            foreach ( var pair in Helmets )
                proxy.Helmets.Add( MakePair( pair.Key, pair.Value ) );

            foreach ( var pair in Shirts )
                proxy.Shirts.Add( MakePair( pair.Key, pair.Value ) );

            foreach ( var pair in Pants )
                proxy.Pants.Add( MakePair( pair.Key, pair.Value ) );

            foreach ( var pair in Enemies )
                proxy.Enemies.Add( MakePair( pair.Key, pair.Value ) );

            foreach ( var pair in Traps )
                proxy.Traps.Add( MakePair( pair.Key, pair.Value ) );

            info.AddValue( "proxy", proxy, typeof( InventoryProxy ) );
        }


        public Inventory( SerializationInfo info, StreamingContext context )
        {
            __proxy = (InventoryProxy) info.GetValue( "proxy", typeof( InventoryProxy ) );
        }

        [OnDeserialized]
        private void SerializedConstruct( StreamingContext context )
        {
            foreach ( var pair in __proxy.Weapons )
                this[ pair.Key ] = pair.Value;

            foreach ( var pair in __proxy.Helmets )
                this[ pair.Key ] = pair.Value;

            foreach ( var pair in __proxy.Shirts )
                this[ pair.Key ] = pair.Value;

            foreach ( var pair in __proxy.Pants )
                this[ pair.Key ] = pair.Value;

            foreach ( var pair in __proxy.Enemies )
                this[ pair.Key ] = pair.Value;

            foreach ( var pair in __proxy.Traps )
                this[ pair.Key ] = pair.Value;

            __proxy = null;
        }

        public static string Serialize( Inventory dungeon )
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

        public static Inventory Deserialize( string data )
        {
            IFormatter formatter = new BinaryFormatter();
            using ( MemoryStream stream = new MemoryStream( Convert.FromBase64String( data ) ) )
            using ( DeflateStream deflateStream = new DeflateStream( stream, CompressionMode.Decompress ) )
            using ( MemoryStream output = new MemoryStream() )
            {
                deflateStream.CopyTo( output );
                deflateStream.Close();

                output.Position = 0;

                return (Inventory) formatter.Deserialize( output );
            }
        }
    }

    public static class InventoryExtentions
    {
        public static string Serialize( this Inventory inventory )
        {
            return Inventory.Serialize( inventory );
        }
    }
}
