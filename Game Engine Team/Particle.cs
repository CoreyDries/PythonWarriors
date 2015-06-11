using Controls;
using Game_Engine_Team.Actors;
using Game_Engine_Team.Texture;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game_Engine_Team
{
    public class Particle : IEntity
    {
        public Point Position { get; protected set; }

        public Sprite Sprite { get; private set; }

        public int X { get { return Position.X; } }
        public int Y { get { return Position.Y; } }

        internal Point PixelOff;

        private double age = 0;

        public double? LifeTime { get; private set; }

        public bool Expired { get; private set; }

        public bool RequestDelay { get; protected set; }

        public Particle( int x, int y, Sprite sprite, double? lifeTime = null, bool requestDelay = false )
        {
            Position = new Point( x, y );
            
            if ( sprite != null )
                this.Sprite = sprite.Spawn();
            
            LifeTime = lifeTime;

            RequestDelay = LifeTime.HasValue ? true : requestDelay;
        }

        public Particle( int x, int y, Sprite sprite, bool requestDelay )
            : this( x, y, sprite, null, requestDelay )
        {
        }

        public Particle( int x, int y, Sprite sprite, double lifeTime )
            : this( x, y, sprite, lifeTime, true )
        {
        }

        public const double TURN_LENGTH = 0.1;

        protected double waitTime = 0;

        public virtual bool DeliverEffect( Dungeon stage )
        {
            return false;
        }

        protected virtual bool TakeTurn()
        {
            if ( Expired || waitTime < TURN_LENGTH )
                return false;

            waitTime -= Math.Min( waitTime, TURN_LENGTH );
            return true;
        }

        public virtual void Draw( Canvas canvas )
        {
            if ( Sprite != null )
                canvas.Draw( Sprite, Position, PixelOff );
        }

        public virtual void Update( GameTime gameTime )
        {
            age += gameTime.ElapsedGameTime.TotalSeconds;

            if ( LifeTime.HasValue && age >= LifeTime )
                Expire();

            if ( Sprite != null )
                Sprite.Update( gameTime );

            waitTime += gameTime.ElapsedGameTime.TotalSeconds;

            TakeTurn();
        }

        internal virtual void Expire()
        {
            Expired = true;
        }




        public NavigationType NavMode
        {
            get { return NavigationType.All; }
        }
    }
}
