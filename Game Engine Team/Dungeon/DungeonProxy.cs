using Game_Engine_Team.Actors;
using Microsoft.Xna.Framework;
using Python_Team;
using System;
using System.Collections.Generic;

// Andrew Meckling
namespace Game_Engine_Team
{
    
    public interface ITileProxy
    {
        /// <summary>
        /// Makes a new tile positioned at the specified coordinates.
        /// </summary>
        /// <param name="x">X coordinate of the new tile.</param>
        /// <param name="y">Y coordinate of the new tile.</param>
        /// <returns>A new tile object.</returns>
        Tile New( int x, int y );
    }


    /// <summary>
    /// Serializable base proxy object for the Tile classes.
    /// </summary>
    /// <typeparam name="TexType">Object type which provides information about 
    /// the texture to draw the tile with.</typeparam>
    [Serializable]
    public class TileProxy<TexType> : ITileProxy
    {
        private TexType tex;
        private Func<int, int, TexType, Tile> ctor;

        /// <summary>
        /// Creates a new proxy object for a tile.
        /// </summary>
        /// <param name="texture">The texture type to create new tiles with.</param>
        /// <param name="ctor_func">A function which returns a new Tile object.</param>
        public TileProxy( TexType texture, Func<int, int, TexType, Tile> ctor_func )
        {
            this.tex = texture;
            this.ctor = ctor_func;
        }

        /// <summary>
        /// Makes a new tile positioned at the specified coordinates.
        /// </summary>
        /// <param name="x">X coordinate of the new tile.</param>
        /// <param name="y">Y coordinate of the new tile.</param>
        /// <returns>A new tile object.</returns>
        public Tile New( int x, int y )
        {
            return ctor( x, y, tex );
        }
    }

    
    /// <summary>
    /// Serializable base proxy object for the Enemy class.
    /// </summary>
    [Serializable]
    public class EnemyProxy
    {
        /// <summary>
        /// The type of the enemy.
        /// </summary>
        private EnemyType type;

        /// <summary>
        /// The pos of the enemy.
        /// </summary>
        private Point pos;

        /// <summary>
        /// Creates a proxy object for serializing enemies.
        /// <param name="type"></param>
        /// <param name="pos"></param>
        public EnemyProxy( EnemyType type, Point pos )
        {
            this.type = type;
            this.pos = pos;
        }

        /// <summary>
        /// Makes a new enemy on the specified dungeon.
        /// </summary>
        /// <param name="stage">The dungeon the enemy is made on.</param>
        /// <returns>A non-aliasing enemy object.</returns>
        public Enemy New( Dungeon stage )
        {
            var enemy = Enemy.Database.Get( type, stage );
            enemy.Position = pos;
            return enemy;
        }
    }

    /// <summary>
    /// Serializable proxy object for the dungeon class.
    /// </summary>
    [Serializable]
    public class DungeonProxy
    {
        /// <summary>
        /// Grid of TileProxy objects. Represents the actual grid of tiles in 
        /// the dungeon.
        /// </summary>
        public readonly ITileProxy[ , ] Grid = new ITileProxy[ Dungeon.WIDTH, Dungeon.HEIGHT ];

        /// <summary>
        /// List of KeyValuePair objects (Dictionaries aren't serializable.)
        /// </summary>
        public readonly List<KeyValuePair<string, EnemyProxy>> Enemies = new List<KeyValuePair<string, EnemyProxy>>();

        /// <summary>
        /// The player's spawn location;
        /// </summary>
        public Point PlayerSpawnPoint;

        public Point PlayerExitPoint;


        public UserScript.Code Script;


        public readonly List<Point> Waypoints = new List<Point>();

        public readonly List<KeyValuePair<string, TrapProxy>> Traps = new List<KeyValuePair<string, TrapProxy>>();

    }
}
