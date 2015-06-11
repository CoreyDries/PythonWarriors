using Game_Engine_Team.Texture;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Game_Engine_Team.Actors
{
    // Andrew Meckling
    public partial class Enemy
    {
        private bool IsHazardous( Tile tile )
        {
            foreach ( Hazard hazard in Stage.Hazards )
                if ( hazard.Position == tile.Position )
                    return true;

            return false;
        }

        protected virtual bool EvaluateTile( Tile next, Tile goal )
        {
            // This method is intentionally verbose to prevent errors.
            if ( next.IsPassable( this.NavMode ) || next == goal )
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Heuristic for determining the estimated ideal cost to get from one 
        /// tile to another.
        /// </summary>
        /// <param name="start">The tile to start from.</param>
        /// <param name="goal">The tile to end at.</param>
        /// <returns>A value used to measure the suitability of potential next 
        /// tiles.</returns>
        private double CostEstimate( Tile start, Tile goal )
        {
            double dist = Stage.MeasureDistance( start.Position, goal.Position );

            // Ranged enemies should prioritize getting into range, rather than 
            // just getting as close as possible.
            if ( this.HasRangedAttack && dist <= this.AttackRange
                 && Stage.IsClearShot( start.Position, goal.Position ) )
                dist -= this.AttackRange;

            // Tiles which contain hazards should be considered a worse choice 
            // than those which don't.
            if ( IsHazardous( start ) )
                dist += 50 / (Health / MaxHealth);

            return dist;
        }

        private List<Tile> NeighborTiles( Tile current, Tile goal )
        {
            int x = current.X;
            int y = current.Y;
            List<Tile> neighbors = new List<Tile>();

            // TODO: Modify this to add knowledge of tiles in line-of-sight.

            foreach ( Direction dir in EnumUtil.GetValues<Direction>() )
            {
                Tile adjTile = current.AdjacentTile( dir );
                if ( EvaluateTile( adjTile, goal ) )
                    neighbors.Add( adjTile );
            }

            return neighbors;
        }

        // The rest of this file is pure A* algorithm.

        /// <summary>
        /// Uses A* to generate a path to the destination.
        /// </summary>
        /// <param name="dest">Location to find a path to.</param>
        /// <returns>A list of sequential points which mark a path from the 
        /// destination to the actor. Returns an empty list if a path cannot 
        /// be found.</returns>
        internal List<Point> FindPathTo( Point? dest )
        {
            // Fair initial capacity reduces reallocations for dictionaries.
            int initial_capacity = (int) Math.Sqrt( Dungeon.WIDTH * Dungeon.HEIGHT );

            if ( dest == null )
                return new List<Point>();

            Tile start = this.CurrentTile;
            Tile goal = Stage[ dest.Value ];

            // A* algorithm begins here. The algorithm is well understood and 
            // documented online and in textbooks; hence the lack of comments 
            // in the code. If you'd like to know more, feel free to visit: 
            // http://en.wikipedia.org/wiki/A*_search_algorithm, which 
            // contains the pseduo-code I used to implement the algorithm.

            List<Tile> closed_set = new List<Tile>();
            List<Tile> open_set = new List<Tile>() { start };
            Dictionary<Tile, Tile> came_from = new Dictionary<Tile, Tile>( initial_capacity * 4 );

            Dictionary<Tile, double> g_score = new Dictionary<Tile, double>( initial_capacity );
            g_score[ start ] = 0;

            Dictionary<Tile, double?> f_score = new Dictionary<Tile, double?>( initial_capacity );
            f_score[ start ] = CostEstimate( start, goal );

            while ( open_set.Count > 0 )
            {
                Tile current = find_cheapest( open_set, f_score );
                if ( current == goal )
                    return reconstruct( came_from, goal ); // <-- exit point

                open_set.Remove( current );
                closed_set.Add( current );
                foreach ( Tile neighbor in NeighborTiles( current, goal ) ) // added goal tile parameter
                {
                    if ( closed_set.Contains( neighbor ) )
                        continue;

                    double tentative_g_score = g_score[ current ] + 1;

                    if ( !open_set.Contains( neighbor ) || tentative_g_score < g_score[ neighbor ] )
                    {
                        came_from[ neighbor ] = current;
                        g_score[ neighbor ] = tentative_g_score;
                        f_score[ neighbor ] = tentative_g_score + CostEstimate( neighbor, goal );
                        if ( !open_set.Contains( neighbor ) )
                            open_set.Add( neighbor );
                    }
                }
            }
            return new List<Point>();
        }

        private static Tile find_cheapest( List<Tile> open_set, Dictionary<Tile, double?> f_score )
        {
            Tile cheapest = open_set[ 0 ];

            foreach ( Tile tile in open_set )
            {
                if ( f_score[ tile ] != null && f_score[ cheapest ] != null
                     && f_score[ tile ] < f_score[ cheapest ] )
                    cheapest = tile;
            }
            return cheapest;
        }

        private static List<Point> reconstruct( Dictionary<Tile, Tile> came_from, Tile current )
        {
            List<Point> total_path = new List<Point>() { current.Position };

            while ( came_from.ContainsKey( current ) )
            {
                current = came_from[ current ];
                total_path.Add( current.Position );
            }
            // Remove the node the enemy is already standing on.
            total_path.RemoveAt( total_path.Count - 1 );

            return total_path;
        }

    }
}
