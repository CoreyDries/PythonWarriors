using Controls;
using Game_Engine_Team.Actors;
using Game_Engine_Team.Texture;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Python_Team;

namespace Game_Engine_Team
{

    public enum EndType
    {
        Loss,
        Win
    }

    public struct EndState
    {
        public EndType Type;

        public int ExpGain;

        public int GoldGain;
    }

    public partial class Dungeon
    {

        public const int HEIGHT = 15;
        public const int WIDTH = 20;

        public bool EditMode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the edges of the map will 
        /// wrap around to the opposite side.
        /// </summary>
        public bool IsInfiniteWorld { get; set; }


        /// <summary>
        /// A 2D array on which all of the Tiles in the Dungeon exist.
        /// </summary>
        private readonly Tile[ , ] grid = new Tile[ WIDTH, HEIGHT ];


        public UserScript PythonScript = new UserScript();


        private Dungeon()
        {
            for ( int x = 0 ; x < WIDTH ; ++x )
                for ( int y = 0 ; y < HEIGHT ; ++y )
                    grid[ x, y ] = new NullTile( x, y );

            IsInfiniteWorld = false;
        }


        public Dungeon( bool fillWall = false )
            : this()
        {
            for ( int x = 0 ; x < WIDTH ; ++x )
                for ( int y = 0 ; y < HEIGHT ; ++y )
                    if ( fillWall )
                        this.PlaceWall( x, y );
                    else
                        this.PlaceGround( x, y );
        }

        public bool RequestsDelay()
        {
            return Player.RequestsDelay
                   || Enemies.Any( e => e.RequestsDelay )
                   || particles.Values.Any( p => p.RequestDelay );
        }

        public static int WrapX( int x )
        {
            if ( x < 0 )
                x = WIDTH + (x % WIDTH);

            else if ( x >= WIDTH )
                x %= WIDTH;

            return x;
        }

        public static void WrapX( ref int x )
        {
            x = WrapX( x );
        }

        public static int WrapY( int y )
        {
            if ( y < 0 )
                y = HEIGHT + (y % HEIGHT);

            else if ( y >= HEIGHT )
                y %= HEIGHT;

            return y;
        }

        public static void WrapY( ref int y )
        {
            y = WrapY( y );
        }

        /// <summary>
        /// Gets the tile at the pos specified by x and y in screen 
        /// coordinates.
        /// </summary>
        /// <param name="x">The horizontal pos of the tile</param>
        /// <param name="y">The vertical pos of the tile</param>
        /// <returns>A tile in the Dungeon, or a new NullTile if out of bounds</returns>
        public Tile this[ int x, int y ]
        {
            get {
                if ( IsInfiniteWorld )
                {
                    x = WrapX( x );
                    y = WrapY( y );
                }
                else if ( x < 0 || x >= WIDTH  ||
                          y < 0 || y >= HEIGHT )
                {
                    return new NullTile( x, y );
                }

                return grid[ x, y ];
            }
            protected set
            {
                grid[ x, y ].Stage = null;

                grid[ x, y ] = value;
                grid[ x, y ].Stage = this;

                UpdateTileLinkage( value );
            }
        }

        public Tile this[ Point loc ]
        {
            get {
                return this[ loc.X, loc.Y ];
            }
            protected set {
                this[ loc.X, loc.Y ] = value;
            }
        }

        private EndState endState;

        public EndState EndState { get { return endState; } }

        public bool GameOver { get; private set; }


        public void EndGame( EndType winLoss )
        {
            endState.Type = winLoss;
            GameOver = true;
        }

        public void AddExp( int exp )
        {
            endState.ExpGain += exp;
        }

        public void AddGold( int gold )
        {
            endState.GoldGain += gold;
        }

        public Rectangle GetRekt()
        {
            return new Rectangle( 0, 0, WIDTH, HEIGHT );
        }

        public bool ContainsCoord( Point loc )
        {
            return GetRekt().Contains( loc );
        }
        
    }

}