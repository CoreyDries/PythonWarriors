using Controls;
using Game_Engine_Team.Actors;
using Game_Engine_Team.Texture;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Audio;

namespace Game_Engine_Team
{
    public class Loot : IEntity
    {
        public Sprite Sprite { get; private set; }

        public Point Position { get; internal set; }

        public int X { get { return Position.X; } }
        public int Y { get { return Position.Y; } }

        public bool Expired { get; private set; }

        public Dungeon Stage { get; set; }


        public const double MOVE_DELAY = 0.3;

        private double waitTime = 0;

        protected Point pixelOff;

        public Loot( int x, int y, Sprite sprite, Dungeon stage = null )
        {
            Position = new Point( x, y );
            Sprite = sprite;
            Stage = stage;
        }

        public Loot( Point pos, Sprite sprite )
        {
            Position = pos;
            Sprite = sprite;
        }

        public virtual void PickUp( Player player )
        {
            Sounds.SoundDaemon.GetSound(SoundEffectTypes.PickUpCoin).Play();

            this.Expire();
        }

        private bool tweening = true;

        public virtual void Update( GameTime gameTime )
        {
            if ( Stage != null && !tweening
                 && !Stage[ this.Position ].IsTraversable( NavigationType.Ground ) )
                this.Expire();

            if ( Sprite != null )
                Sprite.Update( gameTime );


            if ( tweening )
            {
                const double height = Tile.HEIGHT / 3d;

                double timeScale = waitTime / MOVE_DELAY;

                double x = (timeScale - (0.5)) * 2;

                pixelOff.Y = (int) Math.Round( x * x * height - height );

                if ( waitTime < MOVE_DELAY )
                {
                    waitTime += gameTime.ElapsedGameTime.TotalSeconds;
                }
                else
                {
                    waitTime -= Math.Min( waitTime, MOVE_DELAY );
                    tweening = false;
                }
            }
            else
            {
                pixelOff = Point.Zero;
            }
        }

        public virtual void Draw( Canvas canvas )
        {
            if ( Sprite != null )
                canvas.Draw( Sprite, Position, pixelOff );
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


    public class GoldPiece : Loot
    {

        public int GoldValue { get; private set; }

        public GoldPiece( Point pos, int goldValue )
            : base( pos, GetSprite( goldValue ) )
        {
            GoldValue = goldValue;
        }

        public override void PickUp( Player player )
        {
            Stage.AddGold( GoldValue );

            player.HeadsUpText( GoldValue + "gp", Color.Yellow );

            base.PickUp( player );
        }

        private static Sprite GetSprite( int goldValue )
        {
            SpriteType type;

            switch ( goldValue / 10 )
            {
                case 0: type = SpriteType.CopperSmall; break;
                case 1: type = SpriteType.SilverSmall; break;
                case 2: type = SpriteType.GoldSmall; break;

                case 3: type = SpriteType.CopperMedium; break;
                case 4: type = SpriteType.SilverMedium; break;
                case 5: type = SpriteType.GoldMedium; break;

                case 6: type = SpriteType.CopperLarge; break;
                case 7: type = SpriteType.SilverLarge; break;
                default:
                case 8: type = SpriteType.GoldLarge; break;
            }

            return Textures.Get( type );
        }
    }

    public class Potion : Loot
    {

        public Potion( Point pos )
            : base( pos, Textures.Get( SpriteType.HealthPotion ) )
        {
        }

        public override void PickUp( Player player )
        {
            player.TakeHealing( player.MaxHealth * 0.20, this );

            base.PickUp( player );
        }
    }
}
