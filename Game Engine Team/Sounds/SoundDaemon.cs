using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using Game_Engine_Team.Texture;

namespace Game_Engine_Team.Sounds
{
    /// <summary>
    /// Contains all the sound effects and music used in the game. Use the enums SoundEffectTypes or MusicTypes
    /// to load sound effects and music from this class.
    /// </summary>
    class SoundDaemon
    {
        /// <summary>
        /// The current instance of the class.
        /// </summary>
        private static SoundDaemon instance;

        /// <summary>
        /// The collection of music used in the game.
        /// </summary>
        private Dictionary<MusicTypes, Song> music;

        /// <summary>
        /// The collection of sound effects used in the game.
        /// </summary>
        private Dictionary<SoundEffectTypes, SoundEffect> soundEffects;

        /// <summary>
        /// Initializes the SoundDaemon class so that assets can be loaded.
        /// </summary>
        /// <param name="content">The content manager being used by the game.</param>
        public static void Initialize(ContentManager content)
        {
            var manager = new AssetLoader(content);
            instance = new SoundDaemon(manager);
        }

        /// <summary>
        /// Creates the sounddaemon class and loads the necessary dictionary information.
        /// </summary>
        /// <param name="manager">The asset loader used to load the assets</param>
        private SoundDaemon(AssetLoader manager)
        {
            SoundEffect.MasterVolume = 0.25f;
            music = new Dictionary<MusicTypes, Song>();
            foreach (MusicTypes type in EnumUtil.GetValues<MusicTypes>())
                music[type] = manager.GetSong(type.ToString());

            soundEffects = new Dictionary<SoundEffectTypes, SoundEffect>();
            foreach (SoundEffectTypes type in EnumUtil.GetValues<SoundEffectTypes>())
                soundEffects[type] = manager.GetSoundEffect(type.ToString());
        }

        /// <summary>
        /// Catches all attempts to get a sound with an invalid type.
        /// </summary>
        /// <param name="type">The type to retrieve.</param>
        /// <returns>The sound object for the sound requested.</returns>
        public static object GetSound(object type) 
        {
            throw new ArgumentException("The sound type " + type.ToString() + " is invalid.");
        }

        /// <summary>
        /// Gets the Song for the music type provided.
        /// </summary>
        /// <param name="type">The type of music to retrieve.</param>
        /// <returns>The song object representing the music.</returns>
        public static Song GetSound(MusicTypes type)
        {
            if (instance == null)
                throw new Exception("Sound Daemon must be initialized before using the class.");
            return instance.music[type];
        }

        /// <summary>
        /// Returns a sound effect object for the type of sound effect requested.
        /// </summary>
        /// <param name="type">The sound effect to get.</param>
        /// <returns>The sound effect object for the type requested.</returns>
        public static SoundEffect GetSound(SoundEffectTypes type)
        {
            if (instance == null)
                throw new Exception("Sound Daemon must be initialized before using the class.");
            return instance.soundEffects[type];
        }
    }
}
