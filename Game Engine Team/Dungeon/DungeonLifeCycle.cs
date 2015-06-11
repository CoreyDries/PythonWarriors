using Controls;
using Game_Engine_Team.Actors;
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

        public void Draw( Canvas canvas )
        {
            foreach ( Tile tile in grid )
                tile.Draw( canvas );

            foreach ( IEntity entity in Entities )
                entity.Draw( canvas );

            if ( canvas.EditMode )
            {
                int i = 0;
                foreach ( Waypoint waypoint in waypoints )
                    waypoint.Draw( canvas, i++ );

                playerSpawn.Draw( canvas );
            }
            playerExit.Draw( canvas );
        }

        public void Update( GameTime gameTime )
        {
            foreach ( Tile tile in grid )
                tile.Update( gameTime );

            foreach ( Hazard hazard in Hazards )
                hazard.InflictDamage( this );

            foreach ( IEntity entity in Entities )
                entity.Update( gameTime );

            foreach ( Particle particle in particles.Values.ToArray() )
                particle.DeliverEffect( this );

            foreach ( Trap trap in Traps )
            {
                Actor actor = FindEntity<Actor>( trap.Position );
                if ( actor != null )
                    trap.Activate( actor );
            }

            if ( Player.Position == playerExit.Position )
                this.EndGame( EndType.Win );

            namedEntities.RemoveValue( e => e.Expired );
            particles.RemoveValue( p => p.Expired );

            hazards.RemoveAll( h => h.Expired );
            loots.RemoveAll( l => l.Expired );
        }

    }

    public static class DictionaryExtentions
    {
        public static void RemoveAll<K, V>( this IDictionary<K, V> dict, Func<K, V, bool> pred )
        {
            foreach ( var pair in dict.ToArray() )
                if ( pred( pair.Key, pair.Value ) )
                    dict.Remove( pair );
        }

        public static void RemoveValue<K, V>( this IDictionary<K, V> dict, Func<V, bool> pred )
        {
            foreach ( var pair in dict.ToArray() )
                if ( pred( pair.Value ) )
                    dict.Remove( pair.Key );
        }

        public static void RemoveValue<K, V>( this IDictionary<K, V> dict, V match )
        {
            foreach ( var pair in dict.ToArray() )
                if ( pair.Value.Equals( match ) )
                    dict.Remove( pair.Key );
        }
    }
}
