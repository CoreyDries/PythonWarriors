using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game_Engine_Team
{
    public static class StringExtentions
    {

        public static string SqueezeSpaces( this string str )
        {
            StringBuilder sb = new StringBuilder( str.Length );
            for ( int i = 0 ; i < str.Length ; ++i )
            {
                char c = str[ i ];
                if ( !char.IsWhiteSpace( c ) )
                    sb.Append( c );
            }
            return sb.ToString();
        }

    }
}
