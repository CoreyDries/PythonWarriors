using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

// Andrew Meckling
namespace Controls
{
    /// <summary>
    /// Used to simplify the interface for drawing with a SpriteBatch. Canvas 
    /// calls Begin() on the underlying SpriteBatch in the constructor and 
    /// calls End() when it is disposed. Best practice is to instantiate a new 
    /// Canvas object in a using block. Begin() and End() functions are 
    /// disabled and will throw a System.NotSupportedException if called.
    /// </summary>
    public sealed class Canvas : SpriteBatch, IDisposable
    {
        /// <summary>
        /// Dummy texture used for drawing rectangles. Consists of a single 
        /// white pixel (rgba: [255,255,255,255]).
        /// </summary>
        private static Texture2D DUMMY_TEXTURE;

        /// <summary>
        /// List of delayed rectangles to draw.
        /// </summary>
        private Dictionary<Rectangle, Color> delayedRects = new Dictionary<Rectangle,Color>();

        /// <summary>
        /// Function that draws something to the canvas on top of everything 
        /// else.
        /// </summary>
        /// <param name="canvas">The canvas to draw on.</param>
        public delegate void EndPaint( Canvas canvas );

        /// <summary>
        /// Called just before the canvas is drawn.
        /// </summary>
        public event EndPaint Finish;

        /// <summary>
        /// Gets or sets a value indicating that things should be drawn in an 
        /// edit friendly way (essentially a "debug mode".)
        /// </summary>
        public bool EditMode { get; set; }

        private bool hasPainted;

        /// <summary>
        /// Initializes a new instance of the class, which enables a group of 
        /// sprites to be drawn using the same settings. Calls Begin() before 
        /// returning.
        /// </summary>
        /// <param name="graphicsDevice">The graphics device where sprites will be drawn.</param>
        public Canvas( GraphicsDevice graphicsDevice )
            : base ( graphicsDevice )
        {
            if ( DUMMY_TEXTURE == null )
            {
                // Put 1 white pixel into the dummy texture.
                DUMMY_TEXTURE = new Texture2D( graphicsDevice, 1, 1 );
                DUMMY_TEXTURE.SetData( new Color[] { Color.White } );
            }

            base.Begin( SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, null, null );
        }

        /// <summary>
        /// Throws an exception if the object wasn't disposed.
        /// </summary>
        ~Canvas()
        {
            if ( !hasPainted )
                throw new System.Exception( "This object must be disposed before destruction."
                                            + " Initialize it in a using block." );
        }

        /// <summary>
        /// Adds a rectangle of the specified color to a batch of sprites for 
        /// rendering.
        /// </summary>
        /// <param name="rect">The shape of the rectangle to draw (in screen 
        /// coordinates.)</param>
        /// <param name="color">The color of the rectangle to draw.</param>
        /// <param name="onTop">Indicates whether to draw the rectangle on 
        /// top of everything else.</param>
        public void DrawRect( Rectangle rect, Color color, bool onTop = false )
        {
            if ( onTop )
                delayedRects[ rect ] = color;
            else
                this.Draw( DUMMY_TEXTURE, rect, color );
        }

        public void DrawString( string text, SpriteFont font, Point pos, Color color, float scale = 1, Color? borderColor = null )
        {
            Vector2 vec = new Vector2( pos.X, pos.Y );

            if ( borderColor != null )
            {
                Action<Vector2> drawStr = ( v ) => this.DrawString( font, text, v, borderColor.Value, 0, Vector2.Zero, scale, SpriteEffects.None, 0 );

                Vector2 bvec = vec + new Vector2( 0, -1 );
                drawStr( bvec );
                bvec += new Vector2( -1, 0 );
                drawStr( bvec );
                bvec += new Vector2( 0, 1 );
                drawStr( bvec );
                bvec += new Vector2( 0, 1 );
                drawStr( bvec );
                bvec += new Vector2( 1, 0 );
                drawStr( bvec );
                bvec += new Vector2( 1, 0 );
                drawStr( bvec );
                bvec += new Vector2( 0, -1 );
                drawStr( bvec );
                bvec += new Vector2( 0, -1 );
                drawStr( bvec );
            }

            this.DrawString( font, text, vec, color, 0, Vector2.Zero, scale, SpriteEffects.None, 0 );
        }

        /// <summary>
        /// Calls End() on the underlying SpriteBatch object.
        /// </summary>
        public new void Dispose()
        {
            foreach ( var pair in delayedRects )
                this.DrawRect( pair.Key, pair.Value );

            if ( Finish != null )
                Finish( this );

            base.End(); // ACTUALLY draw everything
            hasPainted = true;
        }


        /// <summary>
        /// You don't need to call End() on a Canvas object! It will be called 
        /// for you automatically when it is disposed.
        /// </summary>
        [Obsolete( "Canvas objects call End() when disposed", true )]
        public new void End()
        {
            throw new System.NotSupportedException();
        }
        /// <summary>
        /// You don't need to call Begin() on a Canvas object! It is called 
        /// automatically by the constructor.
        /// </summary>
        [Obsolete( "Canvas objects call Begin() in the constructor", true )]
        public new void Begin()
        {
            throw new System.NotSupportedException();
        }
        /// <summary>
        /// You don't need to call Begin() on a Canvas object! It is called 
        /// automatically by the constructor.
        /// </summary>
        [Obsolete( "Canvas objects call Begin() in the constructor", true )]
        public new void Begin( SpriteSortMode sortMode, BlendState blendState )
        {
            throw new System.NotSupportedException();
        }
        /// <summary>
        /// You don't need to call Begin() on a Canvas object! It is called 
        /// automatically by the constructor.
        /// </summary>
        [Obsolete( "Canvas objects call Begin() in the constructor", true )]
        public new void Begin( SpriteSortMode sortMode, BlendState blendState, SamplerState samplerState, DepthStencilState depthStencilState, RasterizerState rasterizerState )
        {
            throw new System.NotSupportedException();
        }
        /// <summary>
        /// You don't need to call Begin() on a Canvas object! It is called 
        /// automatically by the constructor.
        /// </summary>
        [Obsolete( "Canvas objects call Begin() in the constructor", true )]
        public new void Begin( SpriteSortMode sortMode, BlendState blendState, SamplerState samplerState, DepthStencilState depthStencilState, RasterizerState rasterizerState, Effect effect )
        {
            throw new System.NotSupportedException();
        }
        /// <summary>
        /// You don't need to call Begin() on a Canvas object! It is called 
        /// automatically by the constructor.
        /// </summary>
        [Obsolete( "Canvas objects call Begin() in the constructor", true )]
        public new void Begin( SpriteSortMode sortMode, BlendState blendState, SamplerState samplerState, DepthStencilState depthStencilState, RasterizerState rasterizerState, Effect effect, Microsoft.Xna.Framework.Matrix transformMatrix )
        {
            throw new System.NotSupportedException();
        }

    }
}
