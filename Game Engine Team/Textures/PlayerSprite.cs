using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game_Engine_Team.Texture
{
    public class PlayerSprite : Sprite
    {
        public Direction Direction { get; set; }

        public bool Walking { get; set; }

        private int frame = 0;


        public PlayerSprite( Texture2D texture, Direction defDir = Direction.Down )
            : base ( 0, 0, texture )
        {
            Direction = defDir;
        }

        private bool stepping = false;

        public override void Update( GameTime time )
        {
            base.Update( time );

            if ( Walking )
                if ( !stepping ) {
                    ++frame;
                    stepping = true;
                }
            else
                if ( stepping ) {
                    //++frame;
                    stepping = false;
                }

            frame %= 4;
        }

        public void Normalize()
        {
            if ( !Walking )
                frame += frame & 1;
        }

        protected override int StartX
        {
            get {
                return base.StartX + frame;
            }
        }

        protected override int StartY
        {
            get {
                return base.StartY + (int) Direction;
            }
        }

    }
}
