using Game_Engine_Team.Actors;
using Game_Engine_Team.Texture;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game_Engine_Team
{
    public partial class Dungeon
    {

        public void InsertTile( Tile tile )
        {
            this[ tile.Position ] = tile;

            foreach ( IEntity entity in FindEntities( tile.Position ).ToArray() )
                if ( !tile.IsPassable( entity.NavMode ) )
                    RemoveEntity( entity );
        }

        public void PlaceTile( int x, int y, Tile tile )
        {
            InsertTile( tile.PlaceCopy( x, y ) );
        }

        public void PlaceGround( int x, int y )
        {
            GroundTile ground = new GroundTile( x, y, Textures.Get( GroundType.Stone0 ) );

            this[ x, y ] = ground;
        }

        public void PlaceWall( int x, int y )
        {
            WallTile wall = new WallTile( x, y, Textures.Get( WallType.Stone0 ) );

            RemoveEntityAt<Actor>( x, y );
            RemoveEntityAt<Trap>( x, y );
            this[ x, y ] = wall;
        }

        public void PlacePit( int x, int y )
        {
            PitTile pit = new PitTile( x, y, Textures.Get( PitType.Empty0 ) );

            RemoveEntityAt( x, y );
            this[ x, y ] = pit;
        }

        /// <summary>
        /// Updates a tile and its adjacent tiles' awareness of each other for 
        /// rendering properly.
        /// </summary>
        /// <param name="myTile">The tile at the center of the update pattern.</param>
        private void UpdateTileLinkage( Tile myTile )
        {
            int x = myTile.X;
            int y = myTile.Y;

            // Warning: this method is very verbose and is prone to bugs 
            // if manipulated or modified. Do so with caution.
            Tile adjTile = this[ x, y - 1 ];
            myTile.North = myTile.LinksTo( adjTile );
            adjTile.South = adjTile.LinksTo( myTile );

            adjTile = this[ x + 1, y ];
            myTile.East = myTile.LinksTo( adjTile );
            adjTile.West = adjTile.LinksTo( myTile );

            adjTile = this[ x, y + 1 ];
            myTile.South = myTile.LinksTo( adjTile );
            adjTile.North = adjTile.LinksTo( myTile );

            adjTile = this[ x - 1, y ];
            myTile.West = myTile.LinksTo( adjTile );
            adjTile.East = adjTile.LinksTo( myTile );


            // Diagonals.
            adjTile = this[ x - 1, y - 1 ];
            myTile.NorthWest = myTile.LinksTo( adjTile );
            adjTile.SouthEast = adjTile.LinksTo( myTile );

            adjTile = this[ x + 1, y - 1 ];
            myTile.NorthEast = myTile.LinksTo( adjTile );
            adjTile.SouthWest = adjTile.LinksTo( myTile );

            adjTile = this[ x - 1, y + 1 ];
            myTile.SouthWest = myTile.LinksTo( adjTile );
            adjTile.NorthEast = adjTile.LinksTo( myTile );

            adjTile = this[ x + 1, y + 1 ];
            myTile.SouthEast = myTile.LinksTo( adjTile );
            adjTile.NorthWest = adjTile.LinksTo( myTile );


            // Shadow for bridges over pits.
            if ( myTile is PitTile )
                (myTile as PitTile).Covered = this[ x, y - 1 ] is BridgeTile;

            Tile southTile = this[ x, y + 1 ];
            if ( southTile is PitTile )
                (southTile as PitTile).Covered = myTile is BridgeTile;
        }

        public bool IsClearShot( Point from, Point to )
        {
            if ( from.X != to.X && from.Y != to.Y )
                return false;

            foreach ( var tile in TileRange( from, to, 1 ) )
                if ( tile.IsObstruction() )
                    return false;

            return true;
        }

        public double MeasureDistance( Point begin, Point end )
        {
            int x = end.X - begin.X;
            int y = end.Y - begin.Y;

            // include wrapping around the edge in calculation
            if ( this.IsInfiniteWorld )
            {
                int[] costsX = {
                    x,
                    (end.X - Dungeon.WIDTH) - begin.X,
                    (end.X + Dungeon.WIDTH) - begin.X
                };
                x = costsX.Min( n => Math.Abs( n ) );

                int[] costsY = {
                    y,
                    (end.Y - Dungeon.HEIGHT) - begin.Y,
                    (end.Y + Dungeon.HEIGHT) - begin.Y
                };
                y = costsY.Min( n => Math.Abs( n ) );
            }

            return Math.Sqrt( x*x + y*y );
        }

        public IEnumerable<Tile> VisibleTilesFrom( Point pos, double range )
        {
            int sight = (int) Math.Ceiling( range );

            for ( int x = pos.X - sight; x <= pos.X + sight; ++x )
            {
                for ( int y = pos.Y - sight; y <= pos.Y + sight; ++y )
                {
                    Point p = new Point( x, y );
                    if ( IsPointVisibleFrom( pos, p )
                         && MeasureDistance( pos, p ) <= range )
                        yield return this[ p ];
                }
            }
        }

        public bool IsPointVisibleFrom( Point begin, Point end )
        {
            if ( this[ end ].IsObstruction() )
                return false;

            double x0 = begin.X;
            double y0 = begin.Y;
            double x1 = end.X;
            double y1 = end.Y;

            double dx = x1 - x0;
            double dy = y1 - y0;

            if ( Math.Abs( dx ) < Math.Abs( dy ) )
            {
                int step = (y0 < y1) ? +1 : -1;

                for ( int y = (int) y0 ; y != y1 ; y += step )
                {
                    int x = (int) (dx * (y - y0) / dy + x0);

                    if ( this[ x, y ].IsObstruction() )
                        return false;
                }
            }
            else
            {
                int step = (x0 < x1) ? +1 : -1;

                for ( int x = (int) x0 ; x != x1 ; x += step )
                {
                    int y = (int) (dy * (x - x0) / dx + y0);

                    if ( this[ x, y ].IsObstruction() )
                        return false;
                }
            }

            return true;
        }

        public IEnumerable<Tile> TileRange( Point begin, Point end,
                                            int offStart = 0, int offEnd = 0 )
        {
            if ( begin == end )
            {
                yield break;
            }
            else if ( begin.X == end.X )
            {
                int step = (begin.Y < end.Y) ? +1 : -1;

                int begin_y = begin.Y + (step * offStart);
                int end_y = end.Y + (step * offEnd);

                if ( !ValidateRange( begin_y, end_y, step ) )
                    yield break;

                if ( IsInfiniteWorld )
                    WrapY( ref end_y );

                for ( int y = begin_y ; y != end_y ; RangeStepY( ref y, step ) )
                    yield return this[ end.X, y ];
            }
            else if ( begin.Y == end.Y )
            {
                int step = (begin.X < end.X) ? +1 : -1;

                int begin_x = begin.X + (step * offStart);
                int end_x = end.X + (step * offEnd);

                if ( !ValidateRange( begin_x, end_x, step ) )
                    yield break;

                if ( IsInfiniteWorld )
                    WrapX( ref end_x );

                for ( int x = begin_x ; x != end_x ; RangeStepX( ref x, step ) )
                    yield return this[ x, end.Y ];
            }
        }

        private bool ValidateRange( int begin, int end, int step )
        {
            return (step < 0 ? end < begin : begin < end) || IsInfiniteWorld;
        }

        private void RangeStepX( ref int x, int step )
        {
            if ( IsInfiniteWorld )
                x = WrapX( x + step );
            else
                x += step;
        }

        private void RangeStepY( ref int y, int step )
        {
            if ( IsInfiniteWorld )
                y = WrapY( y + step );
            else
                y += step;
        }

    }
}
