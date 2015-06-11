using Controls;
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
    public class TextParticle : Particle
    {

        public string Text { get; set; }

        public SpriteFont Font { get; protected set; }

        public Color Color { get; protected set; }

        public TextParticle( int x, int y, SpriteFont font, string text, Color color, double lifeTime = 1 )
            : base( x, y, null, lifeTime )
        {
            Font = font;
            Text = text;
            RequestDelay = false;
            Color = color;
        }

        public override void Draw( Canvas canvas )
        {
            Point pos = Position.Multiply( Tile.WIDTH, Tile.HEIGHT ).Add( PixelOff );

            pos.X -= (int) Font.MeasureString( Text ).X / 2;
            pos.Y -= Yadjust;

            //foreach ( Direction dir in EnumUtil.GetValues<Direction>() )
            //{
            //    canvas.DrawString( Text, Font, pos.Add( dir ), Color.Black );
            //}

            canvas.DrawString( Text, Font, pos, Color, 1, Color.Black );
        }


        private double WaitTime = 0;

        private int Yadjust = 0;

        public override void Update( GameTime gameTime )
        {
            base.Update( gameTime );

            const double height = Tile.HEIGHT / 2d;

            double timeScale = WaitTime / LifeTime.Value;

            Yadjust = (int) Math.Round( height * timeScale );

            WaitTime += gameTime.ElapsedGameTime.TotalSeconds;
        }

    }
}
