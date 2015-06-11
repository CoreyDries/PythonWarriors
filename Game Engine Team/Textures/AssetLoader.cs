using Controls;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace Game_Engine_Team.Texture
{
    // Andrew Meckling
    public class AssetLoader
    {

        public ContentManager Content { get; private set; }

        public AssetLoader( ContentManager contentManager )
        {
            Content = contentManager;
        }


        public Texture2D GetTexture2D( string name )
        {
            return Content.Load<Texture2D>( name );
        }


        public Sprite GetSprite( int x, int y, string name )
        {
            return new Sprite( x, y, Content.Load<Texture2D>( name ) );
        }

        public PlayerSprite GetPlayerSprite( string name )
        {
            return new PlayerSprite( Content.Load<Texture2D>( "Sprites/Commissions/" + name ) );
        }

        public VectorSprite GetVectorSprite( int x, int y, string name, int nFrames, int startIndex = 0 )
        {
            Texture2D[] textures = new Texture2D[ nFrames ];

            for ( int i = 0 ; i < nFrames ; ++i )
                textures[ i ] = Content.Load<Texture2D>( name + (i + startIndex) );

            return new VectorSprite( x, y, textures );
        }

        public WallTexture GetWallTexture( int x, int y )
        {
            return new WallTexture( x, y, Content.Load<Texture2D>( "Sprites/Objects/Wall" ) );
        }


        public GroundTexture GetFloorTexture( int x, int y, bool drawConvexCorners = false )
        {
            var tex = new GroundTexture( x, y, Content.Load<Texture2D>( "Sprites/Objects/Floor" ) );
            tex.DrawConvexCorners = drawConvexCorners;
            return tex;
        }


        public PitTexture GetPitTexture( int y )
        {
            return new PitTexture( 0, y,
                Content.Load<Texture2D>( "Sprites/Objects/Pit0" ),
                Content.Load<Texture2D>( "Sprites/Objects/Pit1" ) );
        }

        public AnimatedTexture GetAnimatedTexture( int x, int y, string name, int nFrames, int startIndex = 0 )
        {
            Texture2D[] textures = new Texture2D[ nFrames ];

            for ( int i = 0 ; i < nFrames ; ++i )
                textures[ i ] = Content.Load<Texture2D>( name + (i + startIndex) );

            return new AnimatedTexture( x, y, textures );
        }

        /// <summary>
        /// Loads a sprite from a non-animated sprite sheet as an 
        /// AnimatedTexture with only 1 frame. Use this if the sprite sheet's 
        /// file name doesn't end with a number.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public AnimatedTexture GetAnimatedTexture( int x, int y, string name )
        {
            return new AnimatedTexture( x, y, Content.Load<Texture2D>( name ) );
        }

        /// <summary>
        /// Loads a sound effect from the content folder and returns it. The name
        /// has to match the name of the sound effect, not its location.
        /// </summary>
        /// <param name="name">The name of the sound effect to load.</param>
        /// <returns>The sound effect matching the name.</returns>
        public SoundEffect GetSoundEffect(string name)
        {
            return Content.Load<SoundEffect>("SoundEffects/" + name);
        }

        /// <summary>
        /// Loads a song from the content folder and returns it. The name has to mach
        /// the name of the song, not its location.
        /// </summary>
        /// <param name="name">The name of the song to load.</param>
        /// <returns>The song matching the name.</returns>
        public Song GetSong(string name)
        {
            return Content.Load<Song>("Music/" + name);
        }
    }
}
