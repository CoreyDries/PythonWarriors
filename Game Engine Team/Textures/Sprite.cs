using Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Python_Team;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game_Engine_Team.Texture
{
    public class Sprite : ITexture, IEquatable<Sprite>
    {
        public const int SIZE = 16;
        public const int WIDTH = SIZE;
        public const int HEIGHT = SIZE;

        private readonly Point pos;
        private readonly Texture2D texture;

        public int X { get { return pos.X; } }
        public int Y { get { return pos.Y; } }

        public bool Mirrored { get; set; }

        public float Rotation { get; set; }

        /// <summary>
        /// The color to tint the sprite. Use Color.White for no tint.
        /// </summary>
        public Color Tint { get; set; }

        public Sprite( int col, int row, Texture2D texture )
        {
            pos = new Point( col, row );
            this.texture = texture;
            Tint = Color.White;
        }

        public virtual Sprite Spawn()
        {
            var sprite = this.MemberwiseClone() as Sprite;
            sprite.Mirrored = false;
            sprite.Rotation = 0;
            return sprite;
        }

        public static explicit operator Texture2D( Sprite sprite )
        {
            return sprite.Texture;
        }

        public virtual Sprite Icon
        {
            get {
                return this;
            }
        }

        public virtual Sprite IconAlt
        {
            get {
                return this.Icon;
            }
        }

        protected virtual Texture2D Texture
        {
            get {
                return texture;
            }
        }

        public virtual void Update( GameTime time )
        {
        }

        /// <summary>
        /// Gets the X-offset of the sprite relative to its Texture2D sprite 
        /// sheet.
        /// </summary>
        protected virtual int StartX
        {
            get {
                return pos.X;
            }
        }

        /// <summary>
        /// Gets the Y-offset of the sprite relative to its Texture2D sprite 
        /// sheet.
        /// </summary>
        protected virtual int StartY
        {
            get {
                return pos.Y;
            }
        }

        public virtual void Draw( Canvas canvas, Point loc, bool pixelCoords = false )
        {
            if ( !pixelCoords ) {
                loc.X *= Tile.WIDTH;
                loc.Y *= Tile.HEIGHT;
            }

            Rectangle destination = new Rectangle( loc.X, loc.Y, Tile.WIDTH, Tile.HEIGHT );
            Rectangle source = new Rectangle( StartX * Sprite.WIDTH,
                                              StartY * Sprite.HEIGHT,
                                              Sprite.WIDTH,
                                              Sprite.HEIGHT );

            canvas.Draw( Texture, destination, source, Tint,
                         Rotation, new Vector2(),
                         Mirrored ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0 );
        }


        public override int GetHashCode()
        {
            return Tuple.Create( this.X, this.Y, this.Texture ).GetHashCode();
        }

        public virtual bool Equals( Sprite other )
        {
            return this.X == other.X
                   && this.Y == other.Y
                   && this.texture == other.texture;
        }

        public override bool Equals( object other )
        {
            if ( other is Sprite )
                return this.Equals( (Sprite) other );

            return false;
        }

        public static bool operator !=( Sprite a, object b )
        {
            return !(a == b);
        }

        public static bool operator ==( Sprite a, object b )
        {
            if ( (object) a == null )
                return b == null;

            return a.Equals( b );
        }
    }
}
