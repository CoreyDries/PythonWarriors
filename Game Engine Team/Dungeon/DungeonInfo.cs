namespace Game_Engine_Team
{
    /// <summary>
    /// A struct representing info about a dungeon. Passed around from the SearchCharacter method
    /// in the ServerCommunicationDaemon and the SearchBase screen. Does not hold the actual
    /// dungeon object.
    /// </summary>
    public struct DungeonInfo
    {
        /// <summary>
        /// Public cont
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="name"></param>
        /// <param name="exp"></param>
        public DungeonInfo( string userName, string name, int exp )
        {
            this.UserName = userName;
            this.Name = name;
            this.Exp = exp;
        }

        /// <summary>
        /// The username of the owner of the dungeon
        /// </summary>
        public string UserName;

        /// <summary>
        /// The name of the character who owns the dungeon, represents the dungeon
        /// </summary>
        public string Name;
        
        /// <summary>
        /// The total exp value assigned to the dungeon
        /// </summary>
        public int Exp;
    }
}
