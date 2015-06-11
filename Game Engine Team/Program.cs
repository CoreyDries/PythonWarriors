// Sets whether the program is launched through the authentication launcher

using System;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Game_Engine_Team 
{
#if WINDOWS || XBOX
    static class Program 
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args) 
        {
            ppLauncher launcher = new ppLauncher();
            launcher.ShowDialog();

            if(launcher.DialogResult == DialogResult.OK) 
            {
                //Set dimensions/fullscreen of game
                using(RegistryKey key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey("Software").CreateSubKey("Python Warriors")) 
                {
                    if(key.GetValue("Width") != null)
                        MainController.WIDTH = (int)key.GetValue("Width");
                    else 
                    {
                        MainController.WIDTH = 640;
                        key.SetValue("Width", 640);
                    }

                    if(key.GetValue("Height") != null)
                        MainController.HEIGHT = (int)key.GetValue("Height");
                    else 
                    {
                        MainController.HEIGHT = 480;
                        key.SetValue("Height", 480);
                    }

                    if(key.GetValue("FullScreen") != null)
                        MainController.FULLSCREEN = (int)key.GetValue("FullScreen") == 1;
                    else 
                    {
                        MainController.FULLSCREEN = false;
                        key.SetValue("FullScreen", 0);
                    }

                }

                using( MainController game = new MainController( launcher.UserInstance ) )
                {
                    game.Run();
                }
            }

        }
    }
#endif
}
