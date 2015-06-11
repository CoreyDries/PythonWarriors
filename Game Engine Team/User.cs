using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game_Engine_Team {

    /// <summary>
    /// A singleton object representing the currently logged in user and stores information relevant to this
    /// currently logged in session
    /// 
    /// Created by Morgan Wynne
    /// </summary>
    public class User {

        public static User Instance
        {
            get;
            set;
        }

        static User()
        {
            Instance = new User();
        }

        public User()
        {
            Characters = new Dictionary< string, Character >();
        }

        /// <summary>
        /// Identifies the user session when they login, passed to the ServerCommunicationDaemon
        /// </summary>
        public string AuthToken { get; set; }

        /// <summary>
        /// The total currency that this user has available
        /// </summary>
        public int Balance { get; set; }

        /// <summary>
        /// Dictionary of this user's characters, set through the ServerCommunicationDaemon
        /// </summary>
        public Dictionary<string, Character> Characters { get; private set; }

        public string CurrentCharacterName { get; set; }

        public Character CurrentCharacter
        { 
            get {
                return Characters[ CurrentCharacterName ];
            }
        }

    }
}
