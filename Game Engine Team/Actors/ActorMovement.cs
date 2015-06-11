using Microsoft.Xna.Framework;
using System;

namespace Game_Engine_Team.Actors
{
    // Andrew Meckling
    public abstract partial class Actor
    {
        /// <summary>
        /// The time delay (in seconds) between when a movement animation 
        /// begins and ends.
        /// </summary>
        public const double MOVE_DELAY = 0.1;

        /// <summary>
        /// The time (in seconds) the actor has been waiting to update the 
        /// animation frame.
        /// </summary>
        private double waitTime = 0;

        /// <summary>
        /// The vector used for the tweening animation between tiles.
        /// </summary>
        private Point moveVector;

        /// <summary>
        /// The pixel offset for rendering the actor.
        /// </summary>
        protected Point pixelOff;

        /// <summary>
        /// Indicates that the actor is being nudged (usually in addition to 
        /// being tweened.)
        /// </summary>
        private bool nudging = false;

        /// <summary>
        /// Indicates that the actor is in the process of tweening between two 
        /// tiles.
        /// </summary>
        public bool Tweening { get; protected set; }

        /// <summary>
        /// Specifies that the actor should animate to a specified location. 
        /// Call this method before updating the CurrentTile.
        /// </summary>
        /// <param name="loc">The location in screen-tile-coordinates to 
        /// animate to.</param>
        private void AnimateTo( Point loc )
        {
            moveVector = loc.Diff( this.Position ).Multiply( Tile.WIDTH, Tile.HEIGHT );
            pixelOff = moveVector.Negate();
            Tweening = true;
        }

        /// <summary>
        /// Updates the internal state of the actor which describes the 
        /// tweening animation frame at the specified GameTime snapshot.
        /// </summary>
        /// <param name="gameTime">Snapshot of the GameTime at the time the 
        /// method is called.</param>
        private void TweenMovement( GameTime gameTime )
        {
            if ( Tweening )
            {
                Point off = nudging ? Point.Zero : moveVector;
                double timeScale = waitTime / MOVE_DELAY;

                pixelOff.X = (int) (moveVector.X * timeScale) - off.X;
                pixelOff.Y = (int) (moveVector.Y * timeScale) - off.Y;

                if ( waitTime < MOVE_DELAY )
                {
                    waitTime += gameTime.ElapsedGameTime.TotalSeconds;
                }
                else
                {
                    waitTime -= Math.Min( waitTime, MOVE_DELAY );
                    Tweening = false;
                    nudging = false;
                }
            }
            else
            {
                moveVector = Point.Zero;
                pixelOff = Point.Zero;
            }
        }

        /// <summary>
        /// Moves and animates the actor to a specified tile.
        /// </summary>
        /// <param name="tile">The tile to move to.</param>
        internal void MoveTo( Tile tile )
        {
            AnimateTo( tile.Position );
            CurrentTile = tile;
        }

        /// <summary>
        /// Attempts to move the actor to the specified tile. This method will 
        /// only succeed if the actor is able to be on the tile and it does not 
        /// already contain an actor.
        /// </summary>
        /// <param name="tile">The destination tile.</param>
        /// <returns>True if successful, false otherwise.</returns>
        internal virtual bool TryMoveTo( Tile tile )
        {
            if ( tile.IsPassable( NavMode ) )
            {
                MoveTo( tile );
                return true;
            }
            return (tile == this.CurrentTile);
        }

        /// <summary>
        /// Attempts to move the actor to the adjacent tile in the specified 
        /// direction. This method will only succeed if the actor is able to 
        /// be on the adjacent tile and it does not already contain an actor.
        /// </summary>
        /// <param name="dir">The direction to move.</param>
        /// <returns>True if successful, false otherwise.</returns>
        internal virtual bool TryMove( Direction dir )
        {
            return TryMoveTo( AdjacentTile( dir ) );
        }

        /// <summary>
        /// Attempts to push the actor to the specified tile. This method will 
        /// only succeed if the tile is not an obstruction (ie. non-flying 
        /// actors can be pushed into pits, but not into walls.)
        /// </summary>
        /// <param name="tile">The destination tile.</param>
        /// <returns>True if successful, false otherwise.</returns>
        internal virtual bool TryPushTo( Tile tile )
        {
            if ( !tile.IsObstruction() )
            {
                MoveTo( tile );
                return true;
            }
            return false;
        }
        
        /// <summary>
        /// Attempts to push the actor to the adjacent tile in the specified 
        /// direction. This method will only succeed if the adjacent tile is 
        /// not an obstruction (ie. non-flying actors can be pushed into pits, 
        /// but not into walls.)
        /// </summary>
        /// <param name="dir">The direction to push.</param>
        /// <returns>True if successful, false otherwise.</returns>
        internal virtual bool TryPush( Direction dir )
        {
            return TryPushTo( AdjacentTile( dir ) );
        }

        /// <summary>
        /// Animates the actor half a tile length in the specified direction 
        /// before returning it to its original pos. The actor's physical 
        /// location doesn't change during the animation.
        /// </summary>
        /// <param name="dir">The direction to nudge the actor.</param>
        internal virtual void Nudge( Direction dir )
        {
            moveVector = dir.ToPoint().Multiply( Tile.WIDTH / 2, Tile.HEIGHT / 2 );
            pixelOff = Point.Zero;
            Tweening = true;
            nudging = true;
        }
    }
}
