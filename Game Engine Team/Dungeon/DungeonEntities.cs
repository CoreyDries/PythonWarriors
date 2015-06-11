using Game_Engine_Team.Actors;
using Microsoft.Xna.Framework;
using Python_Team;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game_Engine_Team
{
    public delegate void EntitiesAltered( string name, IEntity entity );

    public partial class Dungeon
    {
        public event EntitiesAltered EntityAdded;
        public event EntitiesAltered EntityRemoved;

        private List<Waypoint> waypoints = new List<Waypoint>();

        private CompositeKeyDictionary<object, string, Particle> particles = new CompositeKeyDictionary<object,string,Particle>();

        private List<Hazard> hazards = new List<Hazard>();

        private SpawnPoint playerSpawn = new SpawnPoint( 0, 0 );

        private ExitPoint playerExit = new ExitPoint( Dungeon.WIDTH - 1, Dungeon.HEIGHT - 1 );

        private List<Loot> loots = new List<Loot>();


        private Dictionary<string, IEntity> namedEntities = new Dictionary<string, IEntity>();


        private Player player;

        public Player Player
        {
            get {
                return player;
            }
            set {
                player = value;
                player.CurrentTile = this[ playerSpawn ];
            }
        }

        public ReadOnlyCollection<Hazard> Hazards
        {
            get {
                return hazards.AsReadOnly();
            }
        }

        private IEnumerable<KeyValuePair<string, E>> ExtractNamedEntities<E>()
            where E : class, IEntity
        {
            foreach ( var pair in namedEntities )
                if ( pair.Value is E )
                    yield return new KeyValuePair<string, E>( pair.Key, pair.Value as E );
        }

        private IEnumerable<E> ExtractEntities<E>()
            where E : class, IEntity
        {
            foreach ( var entity in namedEntities.Values )
                if ( entity is E )
                    yield return entity as E;
        }

        public IEnumerable<KeyValuePair<string, Trap>> NamedTraps
        {
            get {
                return ExtractNamedEntities<Trap>();
            }
        }

        public IEnumerable<KeyValuePair<string, Enemy>> NamedEnemies
        {
            get {
                return ExtractNamedEntities<Enemy>();
            }
        }

        public IEnumerable<KeyValuePair<string, IEntity>> NamedEntities
        {
            get {
                return ExtractNamedEntities<IEntity>();
            }
        }

        public IEnumerable<Trap> Traps
        {
            get {
                return ExtractEntities<Trap>();
            }
        }

        public IEnumerable<Enemy> Enemies
        {
            get {
                return ExtractEntities<Enemy>();
            }
        }

        public ReadOnlyCollection<Waypoint> Waypoints
        {
            get {
                return waypoints.AsReadOnly();
            }
        }

        public int GetDungeonExpCost()
        {
            int sum = 0;

            foreach ( var enemy in Enemies )
                sum += enemy.ExpCost;

            foreach ( var trap in Traps )
                sum += trap.ExpCost;

            return sum;
        }

        public IEnumerable<Actor> Actors
        {
            get {
                if ( Player != null )
                    yield return Player;

                foreach ( var enemy in Enemies )
                    yield return enemy;
            }
        }

        public IEnumerable<IEntity> Entities
        {
            get {
                foreach ( Loot loot in loots )
                    yield return loot;

                foreach ( Actor actor in Actors )
                    yield return actor;

                foreach ( Hazard hazard in Hazards )
                    yield return hazard;

                foreach ( Trap trap in Traps )
                    yield return trap;

                foreach ( Particle particle in particles.Values )
                    yield return particle;

                foreach ( Waypoint waypoint in Waypoints )
                    yield return waypoint;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>True if the player spawn was set.</returns>
        public bool SetPlayerSpawn( int x, int y )
        {
            if ( this[ x, y ].IsPassable( NavigationType.Ground ) )
            {
                playerSpawn = new SpawnPoint( x, y );
                return true;
            }
            return false;
        }

        public bool SetPlayerExit( int x, int y )
        {
            if ( this[ x, y ].IsPassable( NavigationType.Ground ) )
            {
                playerExit = new ExitPoint( x, y );
                return true;
            }
            return false;
        }

        // ******************************************************
        // ************************ MISC ************************
        // ******************************************************

        public void AddLoot( Loot loot )
        {
            loot.Stage = this;
            loots.Add( loot );
        }

        public bool AddHazard( Hazard hazard )
        {
            foreach ( var haz in Hazards )
                if ( haz.Position == hazard.Position )
                    haz.Expire();

            if ( this[ hazard.Position ].IsTraversable( hazard.NavMode ) )
            {
                this.hazards.Add( hazard );
                return true;
            }
            return false;
        }

        public void AddWaypoint( int x, int y )
        {
            Waypoint waypoint = new Waypoint( x, y );

            if ( !waypoints.Contains( waypoint ) )
                waypoints.Add( waypoint );
        }

        // ******************************************************
        // ********************** PARICLES **********************
        // ******************************************************

        public void AddParticle( object sender, string key, Particle particle )
        {
            particles[ sender, key ] = particle;
        }

        public void RemoveParticle( object sender, string key )
        {
            particles.Remove( sender, key );
        }

        public void RemoveParticles( object sender )
        {
            particles.RemoveAll( sender );
        }

        public Particle FindParticle( object sender, string key )
        {
            if ( particles.ContainsKey( sender, key ) )
                return particles[ sender, key ];

            return null;
        }

        // ******************************************************
        // ********************** ENTITIES **********************
        // ******************************************************

        public EType FindEntity<EType>( int x, int y )
            where EType : class, IEntity
        {
            return FindEntity<EType>( new Point( x, y ) );
        }

        public EType FindEntity<EType>( Point loc )
            where EType : class, IEntity
        {
            foreach ( var entity in Entities )
                if ( entity.Position == loc && entity is EType )
                    return entity as EType;

            return null;
        }

        public IEntity FindEntity( int x, int y )
        {
            return FindEntity( new Point( x, y ) );
        }

        public IEntity FindEntity( Point loc )
        {
            foreach ( var entity in Entities )
                if ( entity.Position == loc )
                    return entity;

            return null;
        }

        public IEnumerable<IEntity> FindEntities( int x, int y )
        {
            return FindEntities( new Point( x, y ) ).AsEnumerable();
        }

        public IEnumerable<IEntity> FindEntities( Point loc )
        {
            foreach ( var entity in Entities )
                if ( entity.Position == loc )
                    yield return entity;
        }

        // ******************************************************
        // ******************* NAMED ENTITIES *******************
        // ******************************************************

        public string AddNamedEntity( IEntity entity, string name = null )
        {
            if ( entity == null )
                return null;

            name = GenerateName( entity, name );

            namedEntities[ name ] = entity;

            if ( EntityAdded != null )
                EntityAdded( name, entity );

            return name;
        }

        public bool RemoveEntityAt<EType>( int x, int y )
            where EType : class, IEntity
        {
            return RemoveEntityAt( new Point( x, y ) );
        }

        public bool RemoveEntityAt<EType>( Point pos )
            where EType : class, IEntity
        {
            EType entity = FindEntity<EType>( pos );

            RemoveEntity( entity );

            return entity != null;
        }

        public void RemoveEntity<EType>( EType entity )
            where EType : class, IEntity
        {
            if ( entity == null )
                return;

            if ( entity is Waypoint )
            {
                waypoints.Remove( entity as Waypoint );
                return;
            }

            string key = namedEntities.First( kv => kv.Value == entity ).Key;

            namedEntities.Remove( key );

            if ( EntityRemoved != null )
                EntityRemoved( key, entity );
        }

        public bool RemoveEntityAt( int x, int y )
        {
            return RemoveEntityAt<IEntity>( new Point( x, y ) );
        }

        public bool RemoveEntityAt( Point pos )
        {
            return RemoveEntityAt<IEntity>( pos );
        }

        private string GenerateName( IEntity entity, string name = null )
        {
            string simpleName = name ?? entity.ToString().ToLower();

            name = name ?? (simpleName + "1");

            for ( int i = 2 ; namedEntities.ContainsKey( name ) ; ++i )
                name = simpleName + i;

            return name.SqueezeSpaces();
        }

        public string GetEntityName( IEntity entity )
        {
            if ( entity == null )
                return null;

            return namedEntities.First( kv => kv.Value == entity ).Key;
        }

        public bool RenameEntity( string oldName, string newName )
        {
            if ( newName == null || oldName == null
                 || !UserScript.ValidateIdentifier( newName ) )
                return false;

            if ( namedEntities.ContainsKey( oldName )
                 && !namedEntities.ContainsKey( newName ) )
            {
                namedEntities[ newName ] = namedEntities[ oldName ];
                namedEntities.Remove( oldName );
                return true;
            }
            return false;
        }

        public bool InsertNamedEntity<EType>( EType entity, string name = null )
            where EType : class, IEntity
        {
            if ( this[ entity.Position ].IsTraversable( entity.NavMode ) )
            {
                RemoveEntityAt<EType>( entity.Position );
                AddNamedEntity( entity, name );
                return true;
            }
            return false;
        }

        /// <summary>
        /// Places an Actor onto the Dungeon at the specified location. Returns
        /// true if the Actor was able to be placed on the Dungeon, false 
        /// otherwise.
        /// </summary>
        /// <param name="x">The x coordinate to place the Actor at</param>
        /// <param name="y">The y coordinate to place the Actor at</param>
        /// <param name="enemyType">The Actor object to place on the Dungeon</param>
        /// <returns>True if the Actor was placed on the Dungeon at the specified
        /// pos; false otherwise (ie, the tile found at that location does
        /// not allow Enemies on it)</returns>
        public bool PlaceEnemy( int x, int y, EnemyType enemyType, string name = null )
        {
            Enemy enemy = Enemy.Database.Get( enemyType, this );
            enemy.Position = new Point( x, y );

            return InsertNamedEntity( enemy, name );
        }

        public bool PlaceTrap( int x, int y, TrapType trapType, string name = null )
        {
            Trap trap = Trap.Database.Get( trapType, this );
            trap.Position = new Point( x, y );

            return InsertNamedEntity( trap, name );
        }
    }
}
