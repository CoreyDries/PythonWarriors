using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Controls
{
    /// <summary>
    /// A collection of drawing commands used to draw simple shapes. This allows simple drawing
    /// of shapes without having to recreate the methods each tie.
    /// 
    /// Primary Author: Jonathan Gribble
    /// Secondary Authors: None
    /// 
    /// Last Edited: October 27/14
    /// </summary>
    public class PrimitiveShapes
    {
        /// <summary>
        /// XNA requries a texture in order to draw. This texture of one pixel is used to be
        /// stretched in order to draw the required shape.
        /// </summary>
        private static Texture2D pixel;

        /// <summary>
        /// The percentage to shift the colour by.
        /// </summary>
        private static float COLORSHIFT = 0.2f;

        /// <summary>
        /// Creates a colored pixel used for drawing the shapes. Needs to be set before any drawing
        /// can be done.
        /// </summary>
        /// <param name="spritebatch">The spritebatch being used.</param>
        private static void SetPixel(Canvas spritebatch)
        {
            if ( pixel != null && !pixel.IsDisposed )
                return;

            pixel = new Texture2D(spritebatch.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            pixel.SetData(new[] { Color.White });
        }

        /// <summary>
        /// Draws and fills a rectangle as specified.
        /// </summary>
        /// <param name="spriteBatch">The spritebatch used for drawing.</param>
        /// <param name="rect">The rectangle to draw.</param>
        /// <param name="color">The color to draw the rectangle.</param>
        public static void FillRectangle( Canvas spriteBatch, Rectangle rect, Color color )
        {
            spriteBatch.DrawRect( rect, color );
        }

        /// <summary>
        /// Draws and fills a rectangle with the parameters specified.
        /// </summary>
        /// <param name="spriteBatch">The spritebatch used for drawing.</param>
        /// <param name="location">The location of the top left of the rectangle.</param>
        /// <param name="size">The width and height of the rectangle.</param>
        /// <param name="color">The color of the rectangle.</param>
        public static void FillRectangle( Canvas spriteBatch, Vector2 location, Vector2 size, Color color )
        {
            Rectangle rect = new Rectangle((int)location.X, (int)location.Y, (int)size.X, (int)size.Y);
            FillRectangle(spriteBatch, rect, color);
        }

        /// <summary>
        /// Draws and fills a rectangle with a rotation of the angle provided.
        /// </summary>
        /// <param name="spriteBatch">The spriteBatch used for drawing.</param>
        /// <param name="rect">The size and location of the rectangle</param>
        /// <param name="color">The color of the rectangle.</param>
        /// <param name="angle">The angle to draw the rectangle at, in radians.</param>
        public static void FillRectangle( Canvas spriteBatch, Rectangle rect, Color color, float angle )
        {
            SetPixel(spriteBatch);

            spriteBatch.DrawRect( rect, color );


            //spriteBatch.Draw(pixel, rect, null, color, angle, Vector2.Zero, SpriteEffects.None, 0);
        }

        /// <summary>
        /// Draws and fills a rectangle with a rotation of the angle provided.
        /// </summary>
        /// <param name="spriteBatch">The spriteBatch used for drawing.</param>
        /// <param name="location">The location of the top left of the rectangle.</param>
        /// <param name="size">The size of the rectangle</param>
        /// <param name="color">The colour of the rectangle.</param>
        /// <param name="angle">The angle to draw the rectangle, in radians.</param>

        public static void FillRectangle( Canvas spriteBatch, Vector2 location, Vector2 size, Color color, float angle )
        {
            SetPixel(spriteBatch);

            Rectangle rect = new Rectangle((int)location.X, (int)location.Y, (int)size.X, (int)size.Y);
            FillRectangle(spriteBatch, rect, color, angle);
        }

        /// <summary>
        /// Draws a rectangle with different colors for top left and bottom right.
        /// </summary>
        /// <param name="spriteBatch">The sprite batche used for drawing.</param>
        /// <param name="rect">The rectangle to draw.</param>
        /// <param name="topLeft">The top Left Colour</param>
        /// <param name="bottomRight">The Bottom Right colour.</param>
        public static void DrawShadedRectangle( Canvas spriteBatch, Rectangle rect, Color topLeft, Color bottomRight )
        {
            DrawShadedRectangle(spriteBatch, rect, topLeft, bottomRight, 1.0f);
        }

        /// <summary>
        /// Draws a rectangle with different colors for top left and bottom right.
        /// </summary>
        /// <param name="spriteBatch">The sprite batche used for drawing.</param>
        /// <param name="rect">The rectangle to draw.</param>
        /// <param name="topLeft">The top Left Colour</param>
        /// <param name="bottomRight">The Bottom Right colour.</param>
        /// <param name="thickness">The thickness of the lines.</param>
        public static void DrawShadedRectangle( Canvas spriteBatch, Rectangle rect, Color topLeft, Color bottomRight, float thickness )
        {
            DrawLine(spriteBatch, new Vector2(rect.X, rect.Y), new Vector2(rect.Right, rect.Y), topLeft, thickness); // top
            DrawLine(spriteBatch, new Vector2(rect.X + 1f, rect.Y), new Vector2(rect.X + 1f, rect.Bottom + thickness), topLeft, thickness); // left
            DrawLine(spriteBatch, new Vector2(rect.X, rect.Bottom), new Vector2(rect.Right, rect.Bottom), bottomRight, thickness); // bottom
            DrawLine(spriteBatch, new Vector2(rect.Right + 1f, rect.Y), new Vector2(rect.Right + 1f, rect.Bottom + thickness), bottomRight, thickness); // right
        }

        /// <summary>
        /// Draws a line of thickness 1.0f between two points.
        /// </summary>
        /// <param name="spriteBatch">The spriteBatch used for drawing.</param>
        /// <param name="p1">The first point.</param>
        /// <param name="p2">The second point.</param>
        /// <param name="color">The color of the line to draw.</param>
        public static void DrawLine( Canvas spriteBatch, Vector2 p1, Vector2 p2, Color color )
        {
            DrawLine(spriteBatch, p1, p2, color, 1.0f);
        }

        /// <summary>
        /// Draws a line of specified thickness between two points.
        /// </summary>
        /// <param name="spriteBatch">The spritebatch used for drawing.</param>
        /// <param name="p1">The first point.</param>
        /// <param name="p2">The second point.</param>
        /// <param name="color">The color of the line.</param>
        /// <param name="thickness">The thickness of the line to draw.</param>
        public static void DrawLine( Canvas spriteBatch, Vector2 p1, Vector2 p2, Color color, float thickness )
        {
            SetPixel(spriteBatch);

            float distance = Vector2.Distance(p1, p2);
            float angle = (float)Math.Atan((p2.Y - p1.Y) / (p2.X - p1.X));

            spriteBatch.Draw(pixel, p1, null, color, angle, Vector2.Zero, new Vector2(distance, thickness), SpriteEffects.None, 0);
        }

        /// <summary>
        /// Draws a rectangle that is empty in the middle with lines of 1.0 thickness.
        /// </summary>
        /// <param name="spriteBatch">The spritebatch used for drawing.</param>
        /// <param name="rect">The rectangle to draw.</param>
        /// <param name="color">The color used for drawing.</param>
        public static void DrawRectangle( Canvas spriteBatch, Rectangle rect, Color color )
        {
            DrawRectangle(spriteBatch, rect, color, 1.0f);
        }

        /// <summary>
        /// Draws a rectangle that is empty using the specified thickenss for the lines.
        /// </summary>
        /// <param name="spriteBatch">The spriteBatch used for drawing.</param>
        /// <param name="rect">The rectangle.</param>
        /// <param name="color">The color of the rectangle.</param>
        /// <param name="thickness">The thickness of the lines.</param>
        public static void DrawRectangle( Canvas spriteBatch, Rectangle rect, Color color, float thickness )
        {
            DrawLine(spriteBatch, new Vector2(rect.X, rect.Y), new Vector2(rect.Right, rect.Y), color, thickness); // top
            DrawLine(spriteBatch, new Vector2(rect.X + 1f, rect.Y), new Vector2(rect.X + 1f, rect.Bottom + thickness), color, thickness); // left
            DrawLine(spriteBatch, new Vector2(rect.X, rect.Bottom), new Vector2(rect.Right, rect.Bottom), color, thickness); // bottom
            DrawLine(spriteBatch, new Vector2(rect.Right + 1f, rect.Y), new Vector2(rect.Right + 1f, rect.Bottom + thickness), color, thickness); // right
        }

        /// <summary>
        /// Darkens the colour provided.
        /// </summary>
        /// <param name="colour">The colour to darken.</param>
        /// <returns>A darker version of the colour.</returns>
        public static Color Darken(Color colour)
        {
            Color result = colour;
            colour.B = (byte)MathHelper.Clamp(colour.B - colour.B * COLORSHIFT, 0, 255);
            colour.R = (byte)MathHelper.Clamp(colour.R - colour.R * COLORSHIFT, 0, 255);
            colour.G = (byte)MathHelper.Clamp(colour.G - colour.G * COLORSHIFT, 0, 255);

            return colour;
        }

        /// <summary>
        /// Lightens the colour provided.
        /// </summary>
        /// <param name="colour">The colour to lighten.</param>
        /// <returns>The lighter version of the colour.</returns>
        public static Color Lighten(Color colour)
        {
            Color result = colour;
            colour.B = (byte)MathHelper.Clamp(colour.B + (255 - colour.B) * COLORSHIFT, 0, 255);
            colour.R = (byte)MathHelper.Clamp(colour.R + (255 - colour.R) * COLORSHIFT, 0, 255);
            colour.G = (byte)MathHelper.Clamp(colour.G + (255 - colour.G) * COLORSHIFT, 0, 255);

            return colour;
        }
    }
}
