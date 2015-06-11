using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace Game_Engine_Team
{
    /// <summary>
    /// A singleton daemon object used to assist in the aquisition and storing data into the database.
    /// Called using ServerCommunicationDaemon.Instance
    /// 
    /// Creators: Jacob Lim, Morgan Wynne, Carl Kuang
    /// </summary>
    public class ServerCommunicationDaemon
    {

        /// <summary>
        /// The singleton instance getter for the daemon
        /// </summary>
        public static ServerCommunicationDaemon Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// The base url of the server
        /// </summary>
        public string URL = "http://techproprojects.bcit.ca:6363/";

        /// <summary>
        /// An instance created for ServerCommunicationDaemon, as a means of accessing ServerCommunicationDaemon.
        /// </summary>
        static ServerCommunicationDaemon()
        {
            Instance = new ServerCommunicationDaemon();
        }

        /// <summary>
        /// Private ServerCommunicationDaemon constructor - used for the Instance property of ServerCommunicationDaemon.
        /// </summary>
        private ServerCommunicationDaemon()
        {
        }

        /// <summary>
        /// Return a User consisting of the user data, using username and password.
        /// This is a helper function used for Login.
        /// </summary>
        /// <param name="username">Username of user</param>
        /// <param name="password">Password of user</param>
        /// <returns>A populated new user object</returns>
        /// <author>Jacob Lim</author>
        /// <editor>Morgan Wynne</editor>
        public User GetUserData( string username, string password )
        {
            // The server url that we'll be retrieving character data from
            string endpoint = URL + "loginservice.svc/player/" + username + "/" + password;

            JObject userData = JObject.Parse( EndPointRequest.HttpRequest( endpoint ) );

            // Return the new User object
            return new User() { Balance = (int) userData[ "Balance" ] };
        }

        /// <summary>
        /// Login, using username and password.
        /// </summary>
        /// <returns>User containing all user information</returns>
        /// <author>Jacob Lim</author>
        /// <author>Morgan Wynne</author>
        public User Login( string username, string password )
        {
            string endpoint = URL + "LoginService.svc/login";

            JObject json = new JObject();
            json[ "username" ] = username;
            json[ "password" ] = password;

            string jsonResult = EndPointRequest.HttpRequest( endpoint, json.ToString() );

            // Parse the json result to a JObject
            JObject loginResult = JObject.Parse( jsonResult );

            User user = GetUserData( username, password );
            user.AuthToken = (string) loginResult[ "LoginResult" ];

            // Returns a new user using data from the Json objects
            return user;
        }

        /// <summary>
        /// Registers a user with the game assuming everything is valid. An account is considered valid if the following is
        /// true: All the fields are completed, a unique username, a unique email address and the passwords match.
        /// 
        /// Created by: Server/Communication team, Morgan Wynne, Jacob Lim
        /// </summary>
        /// <param name="username">Username of user</param>
        /// <param name="password">Password of user</param>
        /// <param name="email">E-mail of user</param>
        /// <returns>Success or failure boolean of registration</returns>
        public Boolean Register( string username, string password, string email )
        {
            string endpoint = URL + "RegisterService.svc/player";

            JObject json = new JObject();
            json[ "username" ] = username;
            json[ "password" ] = password;
            json[ "email" ] = email;

            string jsonResult = EndPointRequest.HttpRequest( endpoint, json.ToString() );

            JObject registerResult = JObject.Parse( jsonResult );

            // Returns if the registration was successful
            return Convert.ToBoolean( (string) registerResult[ "RegisterNewUserResult" ] );
        }

        /// <summary>
        /// Get a character of the currently logged in user by its character name.
        /// </summary>
        /// <param name="Name">Character name</param>
        /// <param name="AuthToken">User's login token</param>
        /// <returns>the character</returns>
        /// <author>Carl Kuang</author>
        public Character GetCharacter( string CharName, string AuthToken )
        {
            string endpoint = URL + "GameAPI.svc/gameapi/getcharacter/" + CharName;
            JObject character = JObject.Parse( EndPointRequest.HttpRequest( endpoint, "", AuthToken ) );

            // Create a character from the character data
            Character newCharacter = new Character(
                CharName,
                Actors.PlayerProxy.Deserialize(
                    character[ "CharacterObj" ].ToString()
                ),
                character[ "Stage" ].ToString(),
                GetItemByCharName(
                    CharName,
                    User.Instance.AuthToken )
                );

            return newCharacter;
        }

        /// <summary>
        /// Return stage as the decimal number of binary string.
        /// </summary>
        /// <param name="charName">Character name</param>
        /// <param name="AuthToken">User's authentication token</param>
        /// <returns>String of Stage data</returns>
        /// <author>Jacob Lim</author>
        /// <editor>Morgan Wynne</editor>
        public string GetStage( string charName, string AuthToken )
        {
            string endpoint = URL + "GameAPI.svc/gameapi/getstagebycharname/" + charName;
            string jsonStage = EndPointRequest.HttpRequest( endpoint, "", AuthToken );

            JObject StageResult = JObject.Parse( jsonStage );

            return ((string) StageResult[ "GetStageByCharNameResult" ]);
        }

        /// <summary>
        /// Save the balance to currently logged in user.
        /// </summary>
        /// <param name="balance">balance want to save</param>
        /// <param name="token">user login token</param>
        /// <returns>Boolean representing if save operation succeed</returns>
        /// <author>Carl Kuang</author>
        public bool SaveBalance( int balance, string AuthToken ) //Requires a user ID
        {
            string endpoint = URL + "GameAPI.svc/gameapi/savebalancebyuserid";

            JObject json = new JObject();
            json[ "balance" ] = balance;

            JObject saveResult = JObject.Parse( EndPointRequest.HttpRequest( endpoint, json.ToString(), AuthToken ) );
            return Convert.ToBoolean( (String) saveResult[ "SaveBalanceByUserIdResult" ] );
        }

        /// <summary>
        /// Search characters by their usernames and/or character names.
        /// </summary>
        /// <param name="user">Username</param>
        /// <param name="character">Character name</param>
        /// <returns>List of character names</returns>
        /// <author>Carl Kuang</author>
        /// <editor>Morgan Wynne</editor>
        public List<DungeonInfo> SearchCharacter( string user, string character )
        {
            string endpoint = URL + "GameAPI.svc/gameapi/searchcharacter";
            List<DungeonInfo> dungeons = new List<DungeonInfo>();
            bool isUserNull = String.IsNullOrEmpty( user );
            bool isCharNull = String.IsNullOrEmpty( character );

            if ( isUserNull && !isCharNull)
            {
                endpoint += "bycharname/" + character;
            }
            else if ( isCharNull && !isUserNull )
            {
                endpoint += "byusername/" + user;
            }
            else if ( !isUserNull && !isCharNull )
            {
                endpoint += "byusernamecharname/" + user + "/" + character;
            }

            JObject listObject = JObject.Parse( EndPointRequest.HttpRequest( endpoint ) );
            JArray listArray = (JArray) listObject.First.First;

            foreach ( JObject charset in listArray )
                dungeons.Add( new DungeonInfo( 
                    (string) charset[ "UserName" ], 
                    (string) charset[ "CharName" ], 
                    (int) charset[ "Exp" ] ) );

            return dungeons;
        }

        /// <summary>
        /// Returns a decompressed, deserialized dungeon object by specifying the owner of the dungeon
        /// and the character name that's paired with the dungeon. Used to aquire dungeons for playing
        /// </summary>
        /// <param name="username">Username of user</param>
        /// <param name="charname">Character name assigned to dungeon</param>
        /// <returns>Deserialized Dungeon object</returns>
        /// <author>Jacob Lim</author>
        /// <editor>Morgan Wynne</editor>
        public Dungeon GetStageByUserChar( string username, string charname )
        {
            string getStageEndpoint = URL + "GameAPI.svc/gameapi/getstagebyusernamecharname/" 
                                    + username + "/" + charname;

            // Gets and parses the Json response string
            string jsonResult = EndPointRequest.HttpRequest( getStageEndpoint );
            JObject userData = JObject.Parse( jsonResult );

            // Return the new User object
            return Dungeon.Deserialize( (string) (userData[ "GetStageByUserNameCharNameResult" ]) );
        }

        /// <summary>
        /// Create a new Character for the user.
        /// This returns a boolean reflecting whether it is successful.
        /// 
        /// Created by: Jacob Lim
        /// </summary>
        /// <param name="charName">Character name</param>
        /// <param name="character">Character object</param>
        /// <param name="stage">Stage object</param>
        /// <param name="stageExp">Stage EXP (experience)</param>
        /// <param name="AuthToken">User's Authentication token</param>
        /// <returns>true or false reflecting whether creation of the Character was successful</returns>
        /// <author>Jacob Lim</author>
        public bool CreateANewCharacter( Character character, string AuthToken )
        {
            //string charName, string character, string stage, string stageExp,
            string endpoint = URL + "GameAPI.svc/gameapi/createnewcharacter";

            // Sets up the JSON object to send to the server
            JObject json = new JObject();
            json[ "charName" ] = character.Name;
            json[ "character" ] = character.SerializedPlayer;
            json[ "stage" ] = character.SerializedDungeon;
            //json[ "inventory" ] = character.SerializedInventory;
            json[ "stageExp" ] = character.Experience.ToString();

            string jsonResult = EndPointRequest.HttpRequest( endpoint, json.ToString(), AuthToken );
            JObject saveCharacterResult = JObject.Parse( jsonResult );

            bool saveInventoryResult = SaveItemByCharName( character.Name, character.SerializedInventory, AuthToken );

            return Convert.ToBoolean( (String) saveCharacterResult[ "CreateNewCharacterResult" ] ) && saveInventoryResult;
        }

        /// <summary>
        /// This directly populates the user instance object's Characters property with the characters
        /// associated with that user from the database. It uses the user instance's authentication
        /// token
        /// </summary>
        /// <author>Jacob Lim</author>
        /// <editor>Morgan Wynne</editor>
        public void PopulateUserCharacters()
        {
            // Get the Json string from the server
            string endpoint = URL + "GameAPI.svc/gameapi/getallcharacter";
            string jsonResult = EndPointRequest.HttpRequest( endpoint, "", User.Instance.AuthToken );

            // Deserializes the Json string for the Auth result
            JObject jsonUserCharsObject = JObject.Parse( jsonResult );
            JArray jsonUserCharsArray = (JArray) jsonUserCharsObject[ "GetAllCharacterResult" ];

            // For each character aquired from the server,
            foreach( JObject character in jsonUserCharsArray )
            {
                string charName = character[ "CharName" ].ToString();

                // Create a character from the character data
                Character newCharacter = new Character(
                    charName,
                    Actors.PlayerProxy.Deserialize(
                        character[ "CharacterObj" ].ToString() 
                    ),
                    character[ "Stage" ].ToString(),
                    GetItemByCharName(
                        charName,
                        User.Instance.AuthToken )
                    );

                // Insert the character into the current user's character list
                User.Instance.Characters[ charName ] = newCharacter;

            }

            // Set the default 
            if ( User.Instance.Characters.Count > 0 )
                User.Instance.CurrentCharacterName = User.Instance.Characters.First().Key;
        }

        /// <summary>
        /// This gets a string representing a compressed serialized character's inventory using the
        /// currently logged in user's authentication token and the specific characters name.
        /// </summary>
        /// <param name="charName">Character Name</param>
        /// <param name="AuthToken">User's Authentication token</param>
        /// <returns>String representing compressed serialized character inventory</returns>
        /// <author>Morgan Wynne</author>
        public string GetItemByCharName( string charName, string AuthToken )
        {
            string endpoint = URL + "/GameAPI.svc/gameapi/getitembycharname/" + charName;
            string jsonResult = EndPointRequest.HttpRequest( endpoint, "", AuthToken );

            JObject itemResult = JObject.Parse( jsonResult );

            return itemResult[ "GetItemByCharNameResult" ].ToString();
        }

        /// <summary>
        /// Saves a string directly to the item column of a character.
        /// </summary>
        /// <param name="charName">Character name</param>
        /// <param name="item">Items object</param>
        /// <param name="AuthToken">User's Authentication token</param>
        /// <returns>true or false reflecting whether saving of the Item was successful</returns>
        public bool SaveItemByCharName( string charName, string item, string AuthToken )
        {
            string endpoint = URL + "GameAPI.svc/gameapi/saveitembycharname";

            JObject json = new JObject();
            json[ "charName" ] = charName;
            json[ "item" ] = item;
            
            string jsonResult = EndPointRequest.HttpRequest( endpoint, json.ToString(), AuthToken );
            JObject saveResult = JObject.Parse( jsonResult );

            return Convert.ToBoolean( (String) saveResult[ "SaveItemByCharNameResult" ] );
        }

        /// <summary>
        /// Edit the Character with name charName for the user.
        /// This returns a boolean reflecting whether it is successful.
        /// </summary>
        /// <param name="charName">Character name</param>
        /// <param name="character">Character object</param>
        /// <param name="stage">Stage object</param>
        /// <param name="stageExp">Stage EXP (experience)</param>
        /// <param name="AuthToken">User's Authentication token</param>
        /// <returns>true or false reflecting whether editing of the Character was successful</returns>
        /// <author>Jacob Lim</author>
        public bool EditCharacter( string charName, string character, string stage, string stageExp, string AuthToken )
        {
            string endpoint = URL + "GameAPI.svc/gameapi/editcharacter";

            JObject json = new JObject();
            json[ "charName" ] = charName;
            json[ "character" ] = character;
            json[ "stage" ] = stage;
            json[ "stageExp" ] = stageExp;

            string jsonResult = EndPointRequest.HttpRequest( endpoint, json.ToString(), AuthToken );
            JObject saveResult = JObject.Parse( jsonResult );

            return Convert.ToBoolean( (String) saveResult[ "EditCharacterResult" ] );
        }


        /// <summary>
        /// Delete the user's Character with name charName.
        /// This returns a boolean reflecting whether it is successful.
        /// </summary>
        /// <param name="charName">Character name</param>
        /// <param name="character">Character object</param>
        /// <param name="AuthToken">User's Authentication token</param>
        /// <returns>true or false reflecting whether deleting the Character was successful</returns>
        /// <author>Jacob Lim</author>
        public bool DeleteCharacter( string charName, string AuthToken )
        {
            string endpoint = URL + "GameAPI.svc/gameapi/deletecharacter/" + charName;

            string jsonResult = EndPointRequest.HttpRequest( endpoint, "", AuthToken );
            JObject saveResult = JObject.Parse( jsonResult );

            return Convert.ToBoolean( (String) saveResult[ "DeleteCharacterResult" ] );
        }

        /// <summary>
        /// Updates the Character object specified by the charName.
        /// This returns a boolean reflecting whether it is successful.
        /// </summary>
        /// <param name="charName">Character name</param>
        /// <param name="character">A serialized, compressed player object</param>
        /// <param name="AuthToken">User's Authentication token</param>
        /// <returns>true or false reflecting whether update of the Character was successful</returns>
        /// <author>Jacob Lim</author>
        public bool UpdateCharacterByCharName( string charName, string character, string AuthToken )
        {
            string endpoint = URL + "GameAPI.svc/gameapi/updatecharacterbycharname";

            JObject json = new JObject();
            json[ "charName" ] = charName;
            json[ "character" ] = character;

            string jsonResult = EndPointRequest.HttpRequest( endpoint, json.ToString(), AuthToken );
            JObject saveResult = JObject.Parse( jsonResult );

            return Convert.ToBoolean( (String) saveResult[ "UpdateCharacterByCharNameResult" ] );
        }

        /// <summary>
        /// Updates the Stage object specified by the charName.
        /// This returns a boolean reflecting whether it is successful.
        /// </summary>
        /// <param name="charName">Character name</param>
        /// <param name="stage">Stage object</param>
        /// <param name="AuthToken">User's Authentication token</param>
        /// <returns>Boolean reflecting whether update of the Stage was successful</returns>
        /// <author>Jacob Lim</author>
        public bool UpdateStageByCharName( string charName, string stage, string AuthToken )
        {
            string endpoint = URL + "GameAPI.svc/gameapi/updatestagebycharname";

            JObject json = new JObject();
            json[ "charName" ] = charName;
            json[ "stage" ] = stage;

            string jsonResult = EndPointRequest.HttpRequest( endpoint, json.ToString(), AuthToken );
            JObject saveResult = JObject.Parse( jsonResult );

            return Convert.ToBoolean( (String) saveResult[ "UpdateStageByCharNameResult" ] );
        }

        /// <summary>
        /// Get the item and its quantity of the currently logged in user by its id
        /// </summary>
        /// <param name="itemID">Item's id</param>
        /// <param name="AuthToken">User's login token</param>
        /// <returns>A KeyValuePair of the item's name and value</returns>
        /// <author>Carl Kuang</author>
        [Obsolete( "Deprecated, use GetItemByCharName instead." )]
        public KeyValuePair<string, int> GetItem( string itemID, string AuthToken )
        {
            string endpoint = URL + "GameAPI.svc/gameapi/getitem/" + itemID;
            JObject itemJObject = JObject.Parse( EndPointRequest.HttpRequest( endpoint, "", AuthToken ) );
            KeyValuePair<string, int> item = new KeyValuePair<string, int>( itemJObject[ "ItemID" ].ToString(), int.Parse( itemJObject[ "Quantity" ].ToString() ) );
            return item;
        }

        /// <summary>
        /// Get all items in the currently logged in user's inventory.
        /// </summary>
        /// <param name="AuthToken">User's login token</param>
        /// <returns>Array of string of the item KeyValuePair</returns>
        /// <author>Carl Kuang</author>
        [Obsolete( "Deprecated, use GetItemByCharName instead." )]
        public KeyValuePair<string, int>[] GetAllItemIDByUsername( string AuthToken )
        {
            string endpoint = URL + "GameAPI.svc/gameapi/getallitemsbyuserid";
            JArray itemArray = JArray.Parse( EndPointRequest.HttpRequest( endpoint, "", AuthToken ) );
            List<KeyValuePair<string, int>> items = new List<KeyValuePair<string, int>>();
            foreach ( JObject item in itemArray )
            {
                items.Add( new KeyValuePair<string, int>( item[ "ItemID" ].ToString(), int.Parse( item[ "Quantity" ].ToString() ) ) );
            }
            return items.ToArray();
        }

        /// <summary>
        /// Save a list of items to the currently logged in user.
        /// </summary>
        /// <param name="items">the list of items to be saved</param>
        /// <param name="AuthToken">user's login token</param>
        /// <returns>true if saved successfully</returns>
        /// <author>Carl Kuang</author>
        [Obsolete( "Deprecated, use GetItemByCharName instead." )]
        public string SaveAllItems( Dictionary<string, int> items, string AuthToken )
        {
            string endpoint = URL + "GameAPI.svc/gameapi/saveallitemidbyusername";
            List<string> itemList = new List<string>();
            foreach ( KeyValuePair<string, int> item in items )
            {
                itemList.Add( "{\"Key\": \"" + item.Key + "\", \"Value\": " + item.Value + "}" );
            }
            string itemListString = string.Join( ", ", itemList.ToArray<string>() );
            string json = "{\"itemIdList\": [" + itemListString + "]}";
            Debug.WriteLine( json );

            return EndPointRequest.HttpRequest( endpoint, json, AuthToken );
        }

    }
}
