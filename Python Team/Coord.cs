using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Python_Team
{
    public struct Coord : IEquatable<Coord>
    {
        public int X;
        public int Y;

        public Coord( int x, int y )
        {
            X = x;
            Y = y;
        }

        public static implicit operator Coord( IronPython.Runtime.PythonTuple tuple )
        {
            if ( tuple.Count == 2 )
                return new Coord( 1, 2 );

            throw new InvalidCastException();
        }

        public static implicit operator Microsoft.Xna.Framework.Point( Coord coord )
        {
            return new Microsoft.Xna.Framework.Point( coord.X, coord.Y );
        }

        public static bool operator ==( Coord a, Coord b )
        {
            return a.Equals( b );
        }

        public static bool operator !=( Coord a, Coord b )
        {
            return !a.Equals( b );
        }

        public bool Equals( Coord other )
        {
            return X == other.X && Y == other.Y;
        }
    }
}
