using Controls;
using Game_Engine_Team.Actors;
using Game_Engine_Team.Texture;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game_Engine_Team
{
    public class Hazard : IEntity
    {

        private Sprite sprite;
        private Sprite spriteLast;

        public Sprite Sprite
        {
            get {
                return age < (Lifespan / 2d + 1) && TurnsRemaining >= 1 
                       ? sprite
                       : spriteLast;
            }
        }

        private int age = 0;

        public Point Position { get; private set; }

        public int X { get { return Position.X; } }
        public int Y { get { return Position.Y; } }

        public bool Expired { get; private set; }

        public int Lifespan { get; private set; }

        public int Damage { get; private set; }


        public int TurnsRemaining
        {
            get {
                return Lifespan - age;
            }
        }

        public Hazard( int x, int y, int lifespan, int damage, Sprite sprite, Sprite spriteLast = null )
        {
            Position = new Point( x, y );
            Lifespan = lifespan;
            Damage = damage;

            this.sprite = sprite;
            this.spriteLast = spriteLast ?? sprite;
        }

        private Actor victim = null;

        public virtual void InflictDamage( Dungeon stage )
        {
            Actor actor = stage.FindEntity<Actor>( Position );

            if ( actor != null && actor != victim )
            {
                victim = actor;
                victim.TakeDamage( Damage, this );
            }
        }

        public void TakeTurn()
        {
            age++;
            victim = null;
        }

        public virtual void Update( GameTime gameTime )
        {
            if ( Sprite != null )
                Sprite.Update( gameTime );

            if ( age > Lifespan )
                this.Expire();
        }

        public virtual void Draw( Canvas canvas )
        {
            if ( Sprite != null )
                canvas.Draw( Sprite, Position );
        }

        public void Expire()
        {
            Expired = true;
        }



        public NavigationType NavMode
        {
            get { return NavigationType.Ground; }
        }
    }
}
