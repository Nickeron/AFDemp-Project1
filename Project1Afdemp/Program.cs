using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1Afdemp
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] signOrLogItems = { "Sign Up", "Log In" };
            short userChoice = Menus.HorizontalMenu(StringsFormatted.Welcome, signOrLogItems);

            if (userChoice == 1)
            {
                User activeUser = new User("", new System.Security.SecureString());
            }
            else
            {
                User activeUser = new User("", new System.Security.SecureString(), true);
            }

            Console.ReadKey();
        }

        
    }
}
