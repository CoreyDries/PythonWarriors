using Controls;
using Game_Engine_Team.Actors;
using Microsoft.Xna.Framework;

// Andrew Meckling
namespace Game_Engine_Team
{
    /// <summary>
    /// Interface for objects which have a physical location in the game world 
    /// and a visual representation or that may change state over time.
    /// </summary>
    public interface IEntity
    {
        /// <summary>
        /// Gets the x-coordinate of the entity.
        /// </summary>
        int X { get; }

        /// <summary>
        /// Gets the y-coordinate of the entity.
        /// </summary>
        int Y { get; }

        /// <summary>
        /// Gets the pos of the entity.
        /// </summary>
        Point Position { get; }

        /// <summary>
        /// Gets a value indicating whether the entity should be removed from 
        /// the game world.
        /// </summary>
        bool Expired { get; }

        /// <summary>
        /// Gets the navigation domain of the entity.
        /// </summary>
        NavigationType NavMode { get; }

        /// <summary>
        /// Runs update logic for the entity.
        /// </summary>
        /// <param name="gameTime">Time passed since the last call to Update.</param>
        void Update( GameTime gameTime );

        /// <summary>
        /// Runs drawing logic for the entity.
        /// </summary>
        /// <param name="canvas">Canvas object on which to draw the entity.</param>
        void Draw( Canvas canvas );
    }
}
