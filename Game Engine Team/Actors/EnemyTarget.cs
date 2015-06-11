using IronPython.Runtime;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game_Engine_Team.Actors
{
    public partial class Enemy
    {
        /// <summary>
        /// A list of Points which mark a path to the Enemy's current 
        /// destination. The first element is the final pos and the last 
        /// element is the next pos.
        /// </summary>
        private List<Point> path = new List<Point>();

        /// <summary>
        /// Gets the Enemy's current destination, or null if no destinaion 
        /// is set.
        /// </summary>
        public Point? Destination
        {
            get
            {
                return path.Count > 0
                       ? path[ 0 ]
                       : (Point?) null;
            }
        }

        /// <summary>
        /// Recalculates the path to the Enemy's current destination.
        /// </summary>
        public void RecalculateCurrentPath()
        {
            path = FindPathTo( Destination );
        }

        /// <summary>
        /// Checks a pos for the Player.
        /// </summary>
        /// <param name="x">The X coordinate to check.</param>
        /// <param name="y">The Y coordinate to check.</param>
        /// <returns>True if a Player is found; false otherwise.</returns>
        private bool CheckTile( int x, int y )
        {
            return Stage.FindEntity<Player>( x, y ) != null;
        }

        /// <summary>
        /// A actor the Enemy is currently chasing.
        /// </summary>
        private Actor pursuit = null;

        public bool Chasing { get { return pursuit != null; } }

        private Point? investigationPoint = null;

        public bool Investigating { get { return investigationPoint != null; } }

        /// <summary>
        /// Checks the Enemy's surroundings for the Player and will cause the 
        /// Enemy to begin chasing the Player if found.
        /// </summary>
        /// <param name="sightRange">The ~radius around the Enemy to check.</param>
        private void CheckSurroundings( double sightRange )
        {
            if ( Chasing )
                sightRange += SightRadiusBonus;

            int sight = (int) Math.Round( sightRange );

            foreach ( Tile tile in Stage.VisibleTilesFrom( this.Position, sightRange ) )
            {
                Actor actor = tile.GetActor();

                if ( actor is Player )
                {
                    pursuit = actor;
                    investigationPoint = pursuit.Position;
                    return;
                }

                //if ( !Chasing && tile.Position == investigationPoint )
                //    investigationPoint = null;
            }

            pursuit = null;
        }

        /// <summary>
        /// Gets a value which indicates whether the Enemy is currently 
        /// attempting to go to a specific pos in the game world.
        /// </summary>
        public bool EnRoute
        {
            get {
                return path.Count > 0;
            }
        }

        /// <summary>
        /// Gets the next tile in the path to the Enemy's destination.
        /// </summary>
        internal Tile NextTile
        {
            get {
                return Stage[ path[ path.Count - 1 ] ];
            }
        }

        /// <summary>
        /// Removes the next step in the Enemy's path to its destination. 
        /// Call this after successfully moving into the NextTile.
        /// </summary>
        internal void RemoveNextTile()
        {
            path.RemoveAt( path.Count - 1 );
        }

        /// <summary>
        /// Gets or sets the Enemy's current target.
        /// </summary>
        public Point? Target { get; set; }

        /****************************** Patrol ******************************/

        /// <summary>
        /// Current waypoint index. Cycles from [0, n) where n is the number 
        /// of waypoints.
        /// </summary>
        private int way_i = 0;

        /// <summary>
        /// List of waypoints the Enemy will cycle through.
        /// </summary>
        private List<Point> waypoints = new List<Point>();

        /// <summary>
        /// Gets or sets the list of waypoints the Enemy will move between 
        /// until its Target is not null. When Target is set to null, it will 
        /// resume its patrol where it left off.
        /// <i>Hint: assigning a single Point is akin to setting a guard post.</i>
        /// </summary>
        public IEnumerable<Point> Patrol
        {
            get {
                return waypoints;
            }
            set {
                waypoints = value != null
                            ? value.ToList()
                            : new List<Point>();
                way_i = 0;
            }
        }

        /// <summary>
        /// Gets the current waypoint in the Enemy's Patrol. This is the point 
        /// the Enemy will move towards if it has no Target (Target == null).
        /// </summary>
        public Point? CurrentWaypoint
        {
            get {
                return waypoints.Count > 0
                       ? waypoints[ way_i ]
                       : (Point?) null;
            }
        }

        /// <summary>
        /// Cycles the Enemy's CurrentWaypoint. Call this when the Enemy 
        /// reaches its current waypoint.
        /// </summary>
        private void IncrementWaypoint()
        {
            ++way_i;
            way_i %= waypoints.Count;
        }

        /// <summary>
        /// Causes the Enemy to calculate where its destination should be. If 
        /// the Enemy is not in pursuit, it will select the Target. If its 
        /// Target is null, it will select the current waypoint; if it is 
        /// already at the current waypoint, it will set the current waypoint 
        /// to the next waypoint.
        /// </summary>
        private void EvaluateDestination()
        {
            if ( this.Position == CurrentWaypoint )
                IncrementWaypoint();

            if ( this.Position == Target )
                Target = null;

            if ( this.Position == investigationPoint )
                investigationPoint = null;


            Point? dest;

            if ( Chasing )
                dest = pursuit.Position;
            else
                dest = investigationPoint ?? Target ?? CurrentWaypoint;

            path = FindPathTo( dest );
        }

    }
}
