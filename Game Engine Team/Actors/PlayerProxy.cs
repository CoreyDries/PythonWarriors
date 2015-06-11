using Game_Engine_Team.Equipment;
using Game_Engine_Team.Texture;
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

namespace Game_Engine_Team.Actors
{

    public interface IPlayerProxy
    {
        Stats Stats { get; set; }

        EquipmentSet Gear { get; set; }

        Stats BaseStats { get; }

        PlayerSprite Sprite { get; }

        Player New( Dungeon stage );
    }

    [Serializable]
    public class PlayerProxy<PlayerType> : IPlayerProxy
        where PlayerType : Player
    {
        public Stats Stats { get; set; }

        public EquipmentSet Gear { get; set; }

        public PlayerProxy( Stats stats, EquipmentSet gear )
        {
            this.Stats = stats;
            this.Gear = gear;
        }

        public Stats BaseStats
        {
            get {
                return Player.GetBaseStats<PlayerType>();
            }
        }

        public PlayerSprite Sprite
        {
            get {
                return PlayerProxy.GetSprite( typeof( PlayerType ) );
            }
        }

        public Player New( Dungeon stage )
        {
            return Activator.CreateInstance( typeof( PlayerType ), stage, Stats, Gear ) as PlayerType;
        }
    }

    public static class PlayerProxy
    {
        public static PlayerSprite GetSprite( Type playerType )
        {
            if ( playerType == typeof( Warrior ) )
                return Textures.Get( PlayerType.Warrior );

            if ( playerType == typeof( Mage ) )
                return Textures.Get( PlayerType.Mage );

            if ( playerType == typeof( Rogue ) )
                return Textures.Get( PlayerType.Rogue );

            return null;
        }

        public static string Serialize( IPlayerProxy player )
        {
            using ( MemoryStream stream = new MemoryStream() )
            {
                IFormatter formatter = new BinaryFormatter();

                formatter.Serialize( stream, player );
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

        public static IPlayerProxy Deserialize( string data )
        {
            IFormatter formatter = new BinaryFormatter();
            using ( MemoryStream stream = new MemoryStream( Convert.FromBase64String( data ) ) )
            using ( DeflateStream deflateStream = new DeflateStream( stream, CompressionMode.Decompress ) )
            using ( MemoryStream output = new MemoryStream() )
            {
                deflateStream.CopyTo( output );
                deflateStream.Close();

                output.Position = 0;

                return (IPlayerProxy) formatter.Deserialize( output );
            }
        }
    }

    public static class PlayerProxyExtentions
    {
        public static string Serialize( this IPlayerProxy player )
        {
            return PlayerProxy.Serialize( player );
        }
    }
}
