using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;

namespace Project1Afdemp
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] signOrLogItems = { "Sign Up", "Log In" };
            short userChoice = Menus.HorizontalMenu(StringsFormatted.Welcome, signOrLogItems);
            User activeUser;

            if (userChoice == 1)
            {
                activeUser = new User("", new SecureString());
            }
            else
            {
                activeUser = new User("", new SecureString(), true);
            }
            Console.WriteLine($"\n\n\tThat's it! You are now logged in as {activeUser.UserName}");

            string[] mainMenuItems = { "Send Email", "Read Received" };
            userChoice = Menus.VerticalMenu(StringsFormatted.MainMenu, mainMenuItems);
            Console.ReadKey();
        }

        
    }
}
