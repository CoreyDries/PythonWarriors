using IronPython.Hosting;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

// Andrew Meckling
namespace Python_Team
{

    /// <summary>
    /// Abstract base class representing the 'Game' constant in the context of 
    /// the UserScript. This is used as an efficient way to share variables 
    /// which are frequently accessed or mutated in both the C# and the Python 
    /// code. The Python code has access to only the public members of the this 
    /// object. Default implementation is empty (and abstract!)
    /// </summary>
    public abstract class GameState
    {
    }

    /// <summary>
    /// Object which maintains the Python script the user will write to 
    /// interface with their dungeon.
    /// </summary>
    public sealed class UserScript
    {

        public const string MAIN_CODE_FILENAME = "python_warriors_main.py";
        public const string SETUP_CODE_FILENAME = "python_warriors_setup.py";

        public const string DEFUALT_MAIN = "# This file contains the main loop of code which is run each turn.\n";
        public const string DEFUALT_SETUP = "# This file contains the setup logic for your script.\n"
                                          + "# Declare any persistent variables or functions you may need in this file.\n";

        [Serializable]
        public class Code
        {
            public readonly string MainCode = DEFUALT_MAIN;
            public readonly string SetupCode = DEFUALT_SETUP;

            public Code()
            {
            }

            public Code( ScriptSource setup, ScriptSource main )
            {
                MainCode = main.GetCode();
                SetupCode = setup.GetCode();
            }
        }

        /// <summary>
        /// Identifier of the Game constant in the Python script.
        /// </summary>
        public const string GAME_STATE_NAME = "Game";

        /// <summary>
        /// Regular expression used to detect assignment to the Game variable. 
        /// Excludes matches that appear in a Python comment (starts with a #).
        /// </summary>
        private static readonly Regex assignToGameRegex = new Regex( @"(?<!#.*)\b(" + GAME_STATE_NAME + @")\s*=[^=].*" );

        /// <summary>
        /// Regular expression used to validate Python identifiers.
        /// </summary>
        private static readonly Regex identifierRegex = new Regex( @"^[_A-Za-z][_A-Za-z0-9]*$" );

        /// <summary>
        /// The engine which actually runs the Python code.
        /// </summary>
        private static ScriptEngine pyEngine = Python.CreateEngine();


        /// <summary>
        /// The ScriptScope object all the Python code runs inside of.
        /// </summary>
        private ScriptScope pyScope = pyEngine.CreateScope();

        /// <summary>
        /// The compiled Python code which is ran with each call to Run().
        /// </summary>
        private CompiledCode mainCode;

        /// <summary>
        /// The compiled Python code which is ran on the first call to Run().
        /// </summary>
        private CompiledCode setupCode;


        /// <summary>
        /// The exception that will be thrown when the script is 
        /// exectuted. If this is null then no exception will be thrown.
        /// </summary>
        public readonly SyntaxErrorException Error;

        /// <summary>
        /// Indicates whether the script has a syntax error and will throw an 
        /// exception if Run() is called. This only provides a guarantee that 
        /// it will throw an error, not that it won't.
        /// </summary>
        public bool HasSyntaxError
        {
            get {
                return Error != null;
            }
        }

        /// <summary>
        /// Flag indicating whether the setup code has been run.
        /// </summary>
        public bool IsSetUp { get; private set; }


        /// <summary>
        /// Backing field for the Game property.
        /// </summary>
        private GameState gameState;

        /// <summary>
        /// Gets or sets the Game constant object in the script. The Python 
        /// script can never assign to the 'Game' object, but it may attempt 
        /// to access or mutate any of its public members.
        /// </summary>
        public GameState Game
        {
            get {
                return gameState;
            }
            set {
                gameState = value;
                pyScope.SetVariable( GAME_STATE_NAME, gameState );
            }
        }

#if DEBUG
        /// <summary>
        /// The number of times the main loop code has been executed. Used for 
        /// debugging purposes only.
        /// </summary>
        private int run_count = 0;

        /// <summary>
        /// The number of times the setup code has been executed. Used for 
        /// debugging purposes only.
        /// </summary>
        private int setup_count = 0;

        /// <summary>
        /// The number of times the script has been reset. Used for debugging 
        /// purposes only.
        /// </summary>
        private int reset_count = 0;
#endif

        public readonly Code Script;

        /// <summary>
        /// Creates an instance of a user script from setup code and main code. 
        /// Can optionally create the script from string literals instead of 
        /// path names.
        /// </summary>
        /// <param name="setupCode">File path to the setup code that will only 
        /// be called on the first call to Run() (before the main code).</param>
        /// <param name="mainCode">File path to the main code that will be 
        /// executed with each call to Run().</param>
        /// <param name="literalCode">Indicates that the mainCode parameter 
        /// should be treated as code instead of a file path.</param>
        /// <param name="literalSetupCode">Indicates that the setupCode parameter 
        /// should be treated as code instead of a file path. Inherits the value 
        /// from literalCode if null.</param>
        public UserScript( string setupCode, string mainCode, bool literalCode = false, bool? literalSetupCode = null )
        {
            ScriptSource setupSource = (literalSetupCode ?? literalCode)
                                     ? pyEngine.CreateScriptSourceFromString( setupCode )
                                     : pyEngine.CreateScriptSourceFromFile( setupCode );
            
            ScriptSource source = literalCode
                                ? pyEngine.CreateScriptSourceFromString( mainCode )
                                : pyEngine.CreateScriptSourceFromFile( mainCode );
            
            Script = new Code( setupSource, source );
            
            try
            {
                this.setupCode = setupSource.Compile();
                this.mainCode = source.Compile();
            }
            catch ( SyntaxErrorException ex )
            {
                Error = ex;
            }

            if ( !HasSyntaxError )
                Error = ValidateCode( setupSource ) ?? ValidateCode( source );
        }

        public UserScript( UserScript.Code script )
            : this( script.SetupCode, script.MainCode, true, true )
        {
        }

        public UserScript()
            : this( DEFUALT_SETUP, DEFUALT_MAIN, true, true )
        {
        }

        /// <summary>
        /// Executes the script. The running code has access to all of the 
        /// variables which have been added via the indexer or AddVariables 
        /// method prior to this method call. All variables (including those 
        /// instantiated by the script itself) retain their values between 
        /// each invocation.
        /// </summary>
        public void Run()
        {
            if ( !IsSetUp )
                Setup();

            Execute( mainCode );
#if DEBUG
            ++run_count;
#endif
        }

        /// <summary>
        /// Runs the setup code for the script. Only call this method once. It 
        /// will be called automatically on the first call to Run() if it was 
        /// not called beforehand.
        /// </summary>
        public void Setup()
        {
            Execute( setupCode );
            IsSetUp = true;
#if DEBUG
            ++setup_count;
#endif
        }

        /// <summary>
        /// Executes code or throws an exception if the code is not valid.
        /// </summary>
        /// <param name="code"></param>
        private void Execute( CompiledCode code )
        {
            if ( HasSyntaxError )
                throw Error;

            code.Execute( pyScope );
        }

        /// <summary>
        /// Clears the stored set of variables in the UserScript without
        /// affecting their C# counterparts and flags the script to call the 
        /// setup code on the next call to Run().
        /// </summary>
        public void Reset()
        {
            pyScope = pyEngine.CreateScope();
            IsSetUp = false;
            Game = gameState;
#if DEBUG
            ++reset_count;
#endif
        }

        /// <summary>
        /// Validates that a given ScriptSource conforms to specific restraints. 
        /// In this case, the code cannot attempt to assign to any variable 
        /// named 'Game'.
        /// </summary>
        /// <param name="source">The ScriptSource to validate.</param>
        /// <returns>An exception that should be thrown if the source code 
        /// is executed or null.</returns>
        public static SyntaxErrorException ValidateCode( ScriptSource source )
        {
            int lineNo = 0;
            foreach ( string line in source.GetCode().Split( '\n' ) )
            {
                lineNo++;
                Match match = assignToGameRegex.Match( line );

                if ( match.Success )
                    return BuildGameAssignException( source, lineNo, match.Groups[ 1 ] );
            }
            
            return null;
        }

        /// <summary>
        /// Validates that a string is a valid Python identifier. Python 
        /// identifiers follow the pattern (letter|'_') (letter|digit|'_')*
        /// For mor information, visit 
        /// https://docs.python.org/2/reference/lexical_analysis.html#identifiers
        /// </summary>
        /// <param name="identifier">The string to validate.</param>
        /// <returns>True if valid; false otherwise.</returns>
        public static bool ValidateIdentifier( string identifier )
        {
            return identifierRegex.IsMatch( identifier );
        }

        /// <summary>
        /// Gets or sets a named variable which can be accessed by the user
        /// script. Both the script runtime and C# runtime access the same 
        /// object in memory. Will throw an exception if the given identifier 
        /// is not valid or if the assigned variable is of a different type 
        /// from a previous assignment.
        /// </summary>
        /// <param name="identifier">Identifier the variable can be referenced by, 
        /// by the executing script.</param>
        /// <returns>An object which is referenced by the given identifier in 
        /// the script runtime.</returns>
        public dynamic this[ string identifier ]
        {
            get
            {
                if ( identifier == GAME_STATE_NAME )
                    throw new ArgumentException( "Cannot get the " + GAME_STATE_NAME
                        + " script variable", "identifier" );

                if ( !ValidateIdentifier( identifier ) )
                    throw new ArgumentException( "Invalid character[s] in identifier." );

                if ( IsSetUp )
                    Console.Error.WriteLine( "Warning: Getting script variables"
                        + " while the code is running is slow. Try using a custom"
                        + " GameState object instead." );


                return pyScope.GetVariable( identifier );
            }
            set
            {
                if ( identifier == GAME_STATE_NAME )
                    throw new ArgumentException( "Cannot set the " + GAME_STATE_NAME
                        + " script variable", "identifier" );

                if ( !ValidateIdentifier( identifier ) )
                    throw new ArgumentException( "Invalid character[s] in identifier." );

                if ( IsSetUp )
                    Console.Error.WriteLine( "Warning: Setting script variables"
                        + " while the code is running is slow. Try using a custom"
                        + " GameState object instead." );


                if ( value != null && pyScope.ContainsVariable( identifier ) )
                {
                    Type former = pyScope.GetVariable( identifier ).GetType();
                    Type latter = value.GetType();

                    if ( !former.IsAssignableFrom( latter ) && !latter.IsAssignableFrom( former ) )
                        throw new ArgumentTypeException( "Cannot change the type of a variable already"
                            + " present in the script. Remove the variable first." );
                }

                pyScope.SetVariable( identifier, value );
            }
        }

        /// <summary>
        /// Removes a variable from the scope of the user script. This is not 
        /// the same as setting it to null and will cause a runtime error if 
        /// the script attempts to reference the variable afterwards.
        /// </summary>
        /// <param name="identifier">Identifier of the variable to remove.</param>
        public void RemoveVariable( string name )
        {
            pyScope.RemoveVariable( name );
        }

        /// <summary>
        /// Instantiates a set of named variables within the scope of the user 
        /// script while executing. These variables persists for each 
        /// invocation of the script and retain the values they had when the 
        /// script last finished.
        /// </summary>
        /// <param name="varTable">Dictionary of variables mapped to
        /// the string representing their identifiers.</param>
        public void AddVariables<T>( IEnumerable<KeyValuePair<string, T>> varTable )
        {
            foreach ( var pair in varTable )
                this[ pair.Key ] = pair.Value;
        }

        /// <summary>
        /// Gets the set of variables shared between the UserScript and C# 
        /// mapped to their identifier in Python.
        /// </summary>
        /// <returns>A dictionary of dynamic variables mapped to the string 
        /// representing their identifier.</returns>
        public Dictionary<string, dynamic> GetVariables()
        {
            Dictionary<string, dynamic> varTable = new Dictionary<string, dynamic>();

            foreach ( string key in pyScope.GetVariableNames() )
                if ( key != GAME_STATE_NAME )
                    varTable[ key ] = this[ key ];

            return varTable;
        }

        /// <summary>
        /// Unfortunately inefficient method which creates the correct 
        /// exception object for assignment to the 'Game' constant. 
        /// Based on the exception thrown on assignment to the 'None' 
        /// constant in Python (None is the keyword for null in Python.)
        /// </summary>
        /// <param name="source">The source script the error occurred in.</param>
        /// <param name="lineNo">The line number the error occurred on.</param>
        /// <param name="incedent">A regex group object containing the error.</param>
        /// <returns>An exception message which describes the error.</returns>
        private static SyntaxErrorException BuildGameAssignException( ScriptSource source,
                                                                      int          lineNo,
                                                                      Group        incedent )
        {
            Match match = assignToGameRegex.Match( source.GetCode() );

            int index = match.Groups[ 1 ].Index;

            int col = incedent.Index + 1;
            int length = incedent.Length;

            return new SyntaxErrorException(
                "cannot assign to " + GAME_STATE_NAME,
                source.Path,
                source.GetCode(),
                source.GetCodeLine( lineNo ),
                new SourceSpan(
                    new SourceLocation( index,          lineNo, col ),
                    new SourceLocation( index + length, lineNo, col + length )
                ),
                80,
                Severity.FatalError );
        }
    }
}
