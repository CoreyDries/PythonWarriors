using Microsoft.Xna.Framework;
using System;

namespace Game_Engine_Team
{

    /// <summary>
    /// Enumerated type of the 4 basic directions.
    /// </summary>
    public enum Direction
    {
        Down = 0,
        Left = 1,
        Right = 2,
        Up = 3
    }

    public static class DirectionHelper
    {

        /// <summary>
        /// Gets the opposite direction of the current direction object.
        /// </summary>
        /// <param name="dir">A direction.</param>
        /// <returns>A direction facing the opposite way.</returns>
        public static Direction Opposite( this Direction dir )
        {
            switch ( dir )
            {
                case Direction.Down:
                    return Direction.Up;

                case Direction.Left:
                    return Direction.Right;

                case Direction.Right:
                    return Direction.Left;

                case Direction.Up:
                    return Direction.Down;

                default:
                    throw new ArgumentException( "Invalid direction." );
            }
        }

        /// <summary>
        /// Converts a direction to a Point object expressed as an offset from 
        /// the point (0,0).
        /// </summary>
        /// <param name="dir">The direction to convert.</param>
        /// <returns>A unit vector expressed as a Point object.</returns>
        public static Point ToPoint( this Direction dir )
        {
            switch ( dir )
            {
                case Direction.Down:
                    return new Point( 0, 1 );

                case Direction.Left:
                    return new Point( -1, 0 );

                case Direction.Right:
                    return new Point( 1, 0 );

                case Direction.Up:
                    return new Point( 0, -1 );

                default:
                    throw new ArgumentException( "Invalid direction." );
            }
        }

        /// <summary>
        /// Gets the direction which points to the specified point.
        /// </summary>
        /// <param name="a">The starting point.</param>
        /// <param name="b">The target point.</param>
        /// <returns>A direction or null.</returns>
        public static Direction? DirectionTo( this Point a, Point b )
        {
            Point vector = a.Diff( b );

            Direction? dir = null;

            if ( vector.X > 0 )
                dir = Direction.Left;
            else if ( vector.X < 0 )
                dir = Direction.Right;

            bool wasSet = dir.HasValue;


            if ( vector.Y > 0 )
                dir = Direction.Up;
            else if ( vector.Y < 0 )
                dir = Direction.Down;

            else if ( wasSet )
                return dir;

            return wasSet ? null : dir;
        }

        public static Point Add( this Point a, Point b )
        {
            return new Point( a.X + b.X, a.Y + b.Y );
        }

        public static Point Diff( this Point a, Point b )
        {
            return new Point( a.X - b.X, a.Y - b.Y );
        }

        public static Point Multiply( this Point a, Point b )
        {
            return new Point( a.X * b.X, a.Y * b.Y );
        }

        public static Point Multiply( this Point a, int x, int y )
        {
            return new Point( a.X * x, a.Y * y );
        }

        public static Point Negate( this Point a )
        {
            return new Point( -a.X, -a.Y );
        }

        public static Point Add( this Point a, Direction b )
        {
            return a.Add( b.ToPoint() );
        }

        public static Point Diff( this Point a, Direction b )
        {
            return a.Diff( b.ToPoint() );
        }

    }
}
