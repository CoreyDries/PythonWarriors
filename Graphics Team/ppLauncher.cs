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
        public partial class ppLauncher : Form
        {
            /// <summary>
            /// Calculate the possible minimum resolutions, based off of the size of the dungeon and sprites.
            /// </summary>
            private const int MIN_WIDTH = Dungeon.WIDTH * Sprite.SIZE;
            private const int MIN_HEIGHT = Dungeon.HEIGHT * Sprite.SIZE;

            /// <summary>
            /// Ensures that the default dimesnions, and smallest dimensions will not be smaller than 640x480
            /// </summary>
            private const int DEF_WIDTH = MIN_WIDTH < 640 ? MIN_WIDTH * 2 : MIN_WIDTH;
            private const int DEF_HEIGHT = MIN_WIDTH != DEF_WIDTH ? MIN_HEIGHT * 2 : MIN_HEIGHT;

            /// <summary>
            /// The constructor.
            /// </summary>
            public ppLauncher()
            {
                SetStyle(ControlStyles.SupportsTransparentBackColor, true);
                InitializeComponent();
                this.BackColor = ControlPaint.Light(Color.Black);
                this.TransparencyKey = ControlPaint.Light(Color.Black);

                //Create a registry entry if it does not exist under HKEY\Current_User\Software\Python Warriors
                using (RegistryKey key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey("Software").CreateSubKey("Python Warriors"))
                {
                    // Ensure that Registry values exit and are digits.
                    if (key.GetValue("FullScreen") == null || !Regex.IsMatch(key.GetValue("FullScreen").ToString(), "^[0-9]$"))
                        key.SetValue("FullScreen", 0);
                    if (key.GetValue("Remember") == null || !Regex.IsMatch(key.GetValue("FullScreen").ToString(), "^[0-9]$"))
                        key.SetValue("Remember", 0);
                    if (key.GetValue("Name") == null)
                        key.SetValue("Name", "");
                    if (key.GetValue("Width") == null || !Regex.IsMatch(key.GetValue("Width").ToString(), "^[0-9]+$"))
                        key.SetValue("Width", DEF_WIDTH);
                    if (key.GetValue("Height") == null || !Regex.IsMatch(key.GetValue("Height").ToString(), "^[0-9]+$"))
                        key.SetValue("Height", DEF_HEIGHT);

                    // Ensure that Registry values are valid.
                    if (Convert.ToInt32(key.GetValue("FullScreen")) > 1 || Convert.ToInt32(key.GetValue("FullScreen")) < 0)
                        key.SetValue("FullScreen", 0);
                    if (Convert.ToInt32(key.GetValue("Remember")) > 1 || Convert.ToInt32(key.GetValue("Remember")) < 0)
                        key.SetValue("Remember", 0);
                    if (Convert.ToInt32(key.GetValue("Width")) % MIN_WIDTH != 0 || Convert.ToInt32(key.GetValue("Width")) / MIN_WIDTH > 4 || Convert.ToInt32(key.GetValue("Width")) / MIN_WIDTH < 1 ||
                            Convert.ToInt32(key.GetValue("Height")) % MIN_HEIGHT != 0 || Convert.ToInt32(key.GetValue("Height")) / MIN_WIDTH > 4 || Convert.ToInt32(key.GetValue("Height")) / MIN_WIDTH < 1)
                    {
                        //Width or Height was not a multiple of MIN_WIDTH or MIN_HEIGHT respectively
                        key.SetValue("Width", DEF_WIDTH);
                        key.SetValue("Height", DEF_WIDTH);
                    }

                    //If remember user is flagged, set username.
                    if (Convert.ToInt32(key.GetValue("Remember").ToString()) == 1)
                        tbUsername.Text = key.GetValue("Name").ToString();

                    // Set combobox values
                    for (int i = DEF_WIDTH / MIN_WIDTH; MIN_WIDTH * i < 1600 || MIN_HEIGHT * i < 1200; i++)
                    {
                        string item = "" + (MIN_WIDTH * i) + " x " + (MIN_HEIGHT * i);
                        cbResolution.Items.Add(item);
                    }

                    // Set fullscreen textbox if necessary.
                    cbFullScreen.Checked = Convert.ToInt32(key.GetValue("FullScreen")) == 1;

                    // Set screen dimesnions.
                    cbResolution.Text = key.GetValue("Width").ToString() + " x " + key.GetValue("Height").ToString();
                    cbResolution.SelectedValue = key.GetValue("Width").ToString() + " x " + key.GetValue("Height").ToString();
                }
            }

            /// <summary>
            /// Exits the program when the user clicks on it.
            /// </summary>
            /// <param name="sender">The object sending the request.</param>
            /// <param name="e">The event arguments.</param>
            private void btnExit_Click(object sender, EventArgs e)
            {
                Application.Exit();
            }

            /// <summary>
            /// Handles the functionality when the user clicks on Log In. First it
            /// ensures that all fields have been completed, and then sends the log in
            /// information to the server to determine if it is correct.
            /// </summary>
            /// <param name="sender">The object sending the event.</param>
            /// <param name="e">The event arguments.</param>
            private void btnLogIn_Click(object sender, EventArgs e)
            {
                // Ensure that all the fields have been filled.
                if (tbUsername.Text.Length == 0)
                {
                    MessageBox.Show("Please enter a username.", "Invalid Username");
                    return;
                }
                else if (tbPassword.Text.Length == 0)
                {
                    MessageBox.Show("Please enter a password.", "Invalid Password");
                    return;
                }

                // Update Registry as needed.
                using (RegistryKey key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey("Software").CreateSubKey("Python Warriors"))
                {
                    if (cbRemember.Checked)
                    {
                        key.SetValue("Remember", 1);
                        key.SetValue("Name", tbUsername.Text);
                    }
                    else
                    {
                        key.SetValue("Remember", 0);
                        key.SetValue("Name", "");
                    }
                }

                // This is an example of retrieving data from the server

                // The server url that we'll be retrieving data from
                string endpoint = "http://techproprojects.bcit.ca:6363/loginservice.svc/player/";

                // Add request specifics to the server url
                endpoint = endpoint + tbUsername.Text + "/" + tbPassword.Text;

                using (var server = new WebClient())
                {
                    try
                    {
                        string result = server.DownloadString(endpoint);
                        if ( result.Contains( "INVALID" ) )
                        {
                            MessageBox.Show("Username/Password combination is incorrect.", "Invalid Login");
                            return;
                        }
                    }
                    catch (WebException excep)
                    {
                        if(((HttpWebResponse)excep.Response).StatusCode == HttpStatusCode.BadRequest)
                        MessageBox.Show("Username/Password combination is incorrect.", "Invalid Login");
                        return;
                    }
                }

                // Send the data off
                MessageBox.Show("Correct Credentials");
                //TODO pass player object and auth string into creation of game.
            }

            /// <summary>
            /// Clears all entries in the signup form, making it easier for the user to start from scratch all over again.
            /// </summary>
            /// <param name="sender">The object sending the event.</param>
            /// <param name="e">The event arguments</param>
            private void btnClear_Click(object sender, EventArgs e)
            {
                tbSUUsername.Text = tbSUPass.Text = tbRePass.Text = tbEmail.Text = "";
            }

            /// <summary>
            /// Registers a user with the game assuming everything is valid. An account is considered valid if the following is
            /// true: All the fields are completed, a unique username, a unique email address and the passwords match.
            /// </summary>
            /// <param name="sender">The object sending the request.</param>
            /// <param name="e">The event arguments.</param>
            private void btnSignUp_Click(object sender, EventArgs e)
            {
                // Ensure all fields are completed.
                if (tbSUUsername.Text.Length == 0)
                {
                    MessageBox.Show("Please enter a username.", "Invalid Username");
                    return;
                }
                else if (!Regex.IsMatch(tbSUUsername.Text, "^[a-zA-Z0-9_]*$"))
                {
                    ///Ensures that username is alphanumeric and underscores only.
                    MessageBox.Show("Username can only contain letters, numbers and underscores.", "Invalid Username");
                    return;
                }
                else if (tbSUPass.Text.Length == 0)
                {
                    MessageBox.Show("Please enter a password.", "Invalid Password");
                    return;
                }
                else if (tbRePass.Text.Length == 0)
                {
                    MessageBox.Show("Please enter a password confirmation.", "Invalid Password");
                    return;
                } // Ensure the two password entries match.
                else if (tbSUPass.Text != tbRePass.Text)
                {
                    MessageBox.Show("Passwords do not match.", "Invalid Password");
                    return;
                }
                else if (tbEmail.Text.Length == 0)
                {
                    MessageBox.Show("Please enter an email.", "Invalid email");
                    return;
                } //Ensure email address is valid
                else if (!Regex.IsMatch(tbEmail.Text, "^([a-zA-Z0-9_\\-\\.]+)@((\\[[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\.)|(([a-zA-Z0-9\\-]+\\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\\]?)$"))
                {
                    MessageBox.Show("Please enter a valid email address", "Invalid Email");
                    return;
                }

                // This is an example of writing data to the database
                string endpoint = "http://techproprojects.bcit.ca:6363/RegisterService.svc/player";

                // Creates the initial http request to the server
                var httpWebRequest = WebRequest.CreateHttp(endpoint);
                httpWebRequest.ContentType = "text/json";
                httpWebRequest.Method = "POST";

                // The exact json string that will be sent to the server
                    string json = "{\"username\":\"" + tbSUUsername.Text + "\"," +
                                  "\"password\":\"" + tbSUPass.Text + "\"," +
                                  "\"email\":\"" + tbEmail.Text + "\"}";
                MessageBox.Show(json);

                // Opens and writes to the http request to the server
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(json);
                }

                // Reads back and prints the response
                    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();
                    MessageBox.Show( result );
                    }

            }

            /// <summary>
            /// This will reset the current setings that have been set up, back to whatever is saved in the configure file on 
            /// the client's computer.
            /// </summary>
            /// <param name="sender">The object sending the request.</param>
            /// <param name="e">The event arguments.</param>
            private void btnReset_Click(object sender, EventArgs e)
            {
                //Contact registry to get default information.
                using (RegistryKey key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey("Software").CreateSubKey("Python Warriors"))
                {
                    // Set fullscreen textbox if necessary.
                    cbFullScreen.Checked = Convert.ToInt32(key.GetValue("FullScreen")) == 1;

                    // Set screen dimesnions.
                    cbResolution.Text = key.GetValue("Width").ToString() + " x " + key.GetValue("Height").ToString();
                }
            }

            /// <summary>
            /// Saves the graphical information entered from the user into the configure file on the client's computer so that the
            /// proper graphics are initialized when the game loads.
            /// </summary>
            /// <param name="sender">The object sending the request.</param>
            /// <param name="e">The event arguements.</param>
            private void btnSave_Click(object sender, EventArgs e)
            {
                // Retrieve width and height information from drop down.
                int width = Convert.ToInt32(Regex.Match(cbResolution.Text, "^[0-9]+").Value);
                int height = Convert.ToInt32(Regex.Match(cbResolution.Text, "[0-9]+$").Value);

                // Retrieve value for whether or not fullscreen is checked.
                int fullscreen = cbFullScreen.Checked ? 1 : 0;

                using (RegistryKey key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey("Software").CreateSubKey("Python Warriors"))
                {
                    key.SetValue("Width", width);
                    key.SetValue("Height", height);
                    key.SetValue("Fullscreen", fullscreen);
                }

            }
        }
    }
