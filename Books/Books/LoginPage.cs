using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Books
{
    public class LoginPage : ContentPage
    {
        public LoginPage()
        {

        }

        public static event EventHandler LoginSucceeded;
        public static event EventHandler LoginCancelled;
        public static Object sender;
        public static void LoginSuccess()
        {
            //Invoked and then sent to the App.cs
            LoginSucceeded(sender, EventArgs.Empty);
        }

        public static void LoginCancel()
        {
            //Invoked and then sent to the App.cs
            LoginCancelled(sender, EventArgs.Empty);
        }
    }
}
