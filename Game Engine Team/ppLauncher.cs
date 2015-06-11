using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Game_Engine_Team;
using Game_Engine_Team.Texture;
using Graphics_Team.LoginServiceReference;
using Graphics_Team.RegistrationServiceReference;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Win32;

namespace Game_Engine_Team
{
    /// <summary>
    /// This class creates a launcher to allow a user to sign-in before starting the game, as
    /// well as edit the resolution of the game and sign up for an account if they have not already
    /// done so.
    /// </summary>
    /// <author>Ross Gribble</author>
    public partial class ppLauncher : Form
    {
        /// <summary>
        /// Calculate the possible minimum resolutions, based off of the size of the dungeon andse sprites.
        /// </summary>
        private const int MIN_WIDTH = Dungeon.WIDTH * Sprite.SIZE;
        private const int MIN_HEIGHT = Dungeon.HEIGHT * Sprite.SIZE;

        /// <summary>
        /// Ensures that the default dimesnions, and smallest dimensions will not be smaller than 640x480
        /// </summary>
        private const int DEF_WIDTH = MIN_WIDTH < 640 ? MIN_WIDTH * 2 : MIN_WIDTH;
        private const int DEF_HEIGHT = MIN_WIDTH != DEF_WIDTH ? MIN_HEIGHT * 2 : MIN_HEIGHT;

        /// <summary>
        /// A fully populated User object that's info is gathered from the server and is aquired by the main application
        /// </summary>
        public User UserInstance { 
            get;
            set;
        }

        /// <summary>
        /// The constructor.
        /// </summary>
        public ppLauncher()
        {
            InitializeComponent();
            SetStyle( ControlStyles.SupportsTransparentBackColor, true );
            this.BackColor = ControlPaint.Light( Color.Black );
            this.TransparencyKey = ControlPaint.Light( Color.Black );

            //Create a registry entry if it does not exist under HKEY\Current_User\Software\Python Warriors
            using ( RegistryKey key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey( "Software" ).CreateSubKey( "Python Warriors" ) )
            {
                // Ensure that Registry values exit and are digits.
                if ( key.GetValue( "FullScreen" ) == null || !Regex.IsMatch( key.GetValue( "FullScreen" ).ToString(), "^[0-9]$" ) )
                    key.SetValue( "FullScreen", 0 );
                if ( key.GetValue( "Remember" ) == null || !Regex.IsMatch( key.GetValue( "FullScreen" ).ToString(), "^[0-9]$" ) )
                    key.SetValue( "Remember", 0 );
                if ( key.GetValue( "Name" ) == null )
                    key.SetValue( "Name", "" );
                if ( key.GetValue( "Width" ) == null || !Regex.IsMatch( key.GetValue( "Width" ).ToString(), "^[0-9]+$" ) )
                    key.SetValue( "Width", DEF_WIDTH );
                if ( key.GetValue( "Height" ) == null || !Regex.IsMatch( key.GetValue( "Height" ).ToString(), "^[0-9]+$" ) )
                    key.SetValue( "Height", DEF_HEIGHT );

                // Ensure that Registry values are valid.
                if ( Convert.ToInt32( key.GetValue( "FullScreen" ) ) > 1 || Convert.ToInt32( key.GetValue( "FullScreen" ) ) < 0 )
                    key.SetValue( "FullScreen", 0 );
                if ( Convert.ToInt32( key.GetValue( "Remember" ) ) > 1 || Convert.ToInt32( key.GetValue( "Remember" ) ) < 0 )
                    key.SetValue( "Remember", 0 );
                if ( Convert.ToInt32( key.GetValue( "Width" ) ) % MIN_WIDTH != 0 || Convert.ToInt32( key.GetValue( "Width" ) ) / MIN_WIDTH > 4 || Convert.ToInt32( key.GetValue( "Width" ) ) / MIN_WIDTH < 1 ||
                        Convert.ToInt32( key.GetValue( "Height" ) ) % MIN_HEIGHT != 0 || Convert.ToInt32( key.GetValue( "Height" ) ) / MIN_WIDTH > 4 || Convert.ToInt32( key.GetValue( "Height" ) ) / MIN_WIDTH < 1 )
                {
                    //Width or Height was not a multiple of MIN_WIDTH or MIN_HEIGHT respectively
                    key.SetValue( "Width", DEF_WIDTH );
                    key.SetValue( "Height", DEF_WIDTH );
                }

                //If remember user is flagged, set username.
                if ( Convert.ToInt32( key.GetValue( "Remember" ).ToString() ) == 1 )
                    tbUsername.Text = key.GetValue( "Name" ).ToString();

                // Set combobox values
                for ( int i = DEF_WIDTH / MIN_WIDTH; MIN_WIDTH * i < 1600 || MIN_HEIGHT * i < 1200; i++ )
                {
                    string item = "" + (MIN_WIDTH * i) + " x " + (MIN_HEIGHT * i);
                    cbResolution.Items.Add( item );
                }

                // Set fullscreen textbox if necessary.
                cbFullScreen.Checked = Convert.ToInt32( key.GetValue( "FullScreen" ) ) == 1;

                // Set screen dimesnions.
                cbResolution.Text = key.GetValue( "Width" ).ToString() + " x " + key.GetValue( "Height" ).ToString();
                cbResolution.SelectedValue = key.GetValue( "Width" ).ToString() + " x " + key.GetValue( "Height" ).ToString();
            }
        }

        /// <summary>
        /// Exits the program when the user clicks on it.
        /// </summary>
        /// <param name="sender">The object sending the request.</param>
        /// <param name="e">The event arguments.</param>
        private void btnExit_Click( object sender, EventArgs e )
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }


        /// <summary>
        /// Handles the functionality when the user clicks on Log In. First it
        /// ensures that all fields have been completed, and then sends the log in
        /// information to the server to determine if it is correct.
        /// </summary>
        /// <param name="sender">The object sending the event.</param>
        /// <param name="e">The event arguments.</param>
        private void btnLogIn_Click( object sender, EventArgs e )
        {
            // Login information acquired from associated textboxes
            string username = tbUsername.Text;
            string password = tbPassword.Text;

            // Validates username and password text fields
            if ( username.Length == 0 )
            {
                MessageBox.Show( "Please enter a username.", "Invalid Username" );
                return;
            }
            if ( password.Length == 0 )
            {
                MessageBox.Show( "Please enter a password.", "Invalid Password" );
                return;
            }

            // Update Registry as needed.
            using ( RegistryKey key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey( "Software" ).CreateSubKey( "Python Warriors" ) )
            {
                if ( cbRemember.Checked )
                {
                    key.SetValue( "Remember", 1 );
                    key.SetValue( "Name", tbUsername.Text );
                }
                else
                {
                    key.SetValue( "Remember", 0 );
                    key.SetValue( "Name", "" );
                }
            }

            // Acquires user information from the server if login information was valid
            // Jacob - To-do:
            // Make this have the authentication key part of the login (to check if the login is valid) run first
            // then run the "get user data" part of the login, in that order, for the purpose of organization, cleaning up code.
            try
            {
                UserInstance = ServerCommunicationDaemon.Instance.Login( username, password );

                // Returns control to the main program with the OK result
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                this.Close();
            } catch ( WebException exception ) 
            {
                MessageBox.Show( exception.Message, "Unable to connect to the server." );
                this.DialogResult = System.Windows.Forms.DialogResult.Abort;
            }
        }

        /// <summary>
        /// Clears all entries in the signup form, making it easier for the user to start from scratch all over again.
        /// </summary>
        /// <param name="sender">The object sending the event.</param>
        /// <param name="e">The event arguments</param>
        private void btnClear_Click( object sender, EventArgs e )
        {
            tbSUUsername.Text = tbSUPass.Text = tbRePass.Text = tbEmail.Text = "";
        }

        /// <summary>
        /// Registers a user with the game assuming everything is valid. An account is considered valid if the following is
        /// true: All the fields are completed, a unique username, a unique email address and the passwords match.
        /// </summary>
        /// <param name="sender">The object sending the request.</param>
        /// <param name="e">The event arguments.</param>
        private void btnSignUp_Click( object sender, EventArgs e )
        {
            // Ensure all fields are completed.
            if ( tbSUUsername.Text.Length == 0 )
            {
                MessageBox.Show( "Please enter a username.", "Invalid Username" );
                return;
            }
            else if ( !Regex.IsMatch( tbSUUsername.Text, "^[a-zA-Z0-9_]*$" ) )
            {
                ///Ensures that username is alphanumeric and underscores only.
                MessageBox.Show( "Username can only contain letters, numbers and underscores.", "Invalid Username" );
                return;
            }
            else if ( tbSUPass.Text.Length == 0 )
            {
                MessageBox.Show( "Please enter a password.", "Invalid Password" );
                return;
            }
            else if ( tbRePass.Text.Length == 0 )
            {
                MessageBox.Show( "Please enter a password confirmation.", "Invalid Password" );
                return;
            } // Ensure the two password entries match.
            else if ( tbSUPass.Text != tbRePass.Text )
            {
                MessageBox.Show( "Passwords do not match.", "Invalid Password" );
                return;
            }
            else if ( tbEmail.Text.Length == 0 )
            {
                MessageBox.Show( "Please enter an email.", "Invalid email" );
                return;
            } //Ensure email address is valid
            else if ( !Regex.IsMatch( tbEmail.Text, "^([a-zA-Z0-9_\\-\\.]+)@((\\[[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\.)|(([a-zA-Z0-9\\-]+\\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\\]?)$" ) )
            {
                MessageBox.Show( "Please enter a valid email address", "Invalid Email" );
                return;
            }

            try {
                if ( !ServerCommunicationDaemon.Instance.Register( tbSUUsername.Text, tbSUPass.Text, tbEmail.Text ) )
                    MessageBox.Show( "Invalid registration" );
                else
                    MessageBox.Show( "Successful registration" );
            } catch ( WebException exception )
            {
                MessageBox.Show( exception.Message, "Unable to connect to server" );
                this.Close();
            }
        }

        /// <summary>
        /// This will reset the current setings that have been set up, back to whatever is saved in the configure file on 
        /// the client's computer.
        /// </summary>
        /// <param name="sender">The object sending the request.</param>
        /// <param name="e">The event arguments.</param>
        private void btnReset_Click( object sender, EventArgs e )
        {
            //Contact registry to get default information.
            using ( RegistryKey key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey( "Software" ).CreateSubKey( "Python Warriors" ) )
            {
                // Set fullscreen textbox if necessary.
                cbFullScreen.Checked = Convert.ToInt32( key.GetValue( "FullScreen" ) ) == 1;

                // Set screen dimesnions.
                cbResolution.Text = key.GetValue( "Width" ).ToString() + " x " + key.GetValue( "Height" ).ToString();
            }
        }

        /// <summary>
        /// Saves the graphical information entered from the user into the configure file on the client's computer so that the
        /// proper graphics are initialized when the game loads.
        /// </summary>
        /// <param name="sender">The object sending the request.</param>
        /// <param name="e">The event arguements.</param>
        private void btnSave_Click( object sender, EventArgs e )
        {
            // Retrieve width and height information from drop down.
            int width = Convert.ToInt32( Regex.Match( cbResolution.Text, "^[0-9]+" ).Value );
            int height = Convert.ToInt32( Regex.Match( cbResolution.Text, "[0-9]+$" ).Value );

            // Retrieve value for whether or not fullscreen is checked.
            int fullscreen = cbFullScreen.Checked ? 1 : 0;

            using ( RegistryKey key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey( "Software" ).CreateSubKey( "Python Warriors" ) )
            {
                key.SetValue( "Width", width );
                key.SetValue( "Height", height );
                key.SetValue( "Fullscreen", fullscreen );
            }

        }

        private void tabControl_Selected( object sender, TabControlEventArgs e )
        {
            Button btn = null;

            switch ( e.TabPageIndex )
            {
                case 0:
                    btn = this.btnLogIn;
                    break;

                case 1:
                    btn = this.btnSignUp;
                    break;

                case 2:
                    btn = this.btnSave;
                    break;
            }

            this.AcceptButton = btn;
        }
    }
}
