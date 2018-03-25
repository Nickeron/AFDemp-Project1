using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1Afdemp
{
    class Menus
    {
        public static short VerticalMenu(string message, string[] menuItems)
        {
            short currentItem = 0, item;
            ConsoleKeyInfo keyInfo;

            do
            {
                Console.Clear();
                Console.WriteLine(message);

                for (item = 0; item < menuItems.Length; item++)
                {
                    if (currentItem == item)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write($"\t|{menuItems[item]}|".PadRight(10));
                    }
                    else
                    {
                        Console.Write($"\t{menuItems[item]}".PadRight(10));
                    }
                    Console.ResetColor();
                }
                do
                {
                    keyInfo = Console.ReadKey(true);
                } while (keyInfo.Key.ToString() == "RightArrow" || keyInfo.Key.ToString() == "LeftArrow" || keyInfo.KeyChar == 13);

                if (keyInfo.Key.ToString() == "RightArrow")
                {
                    currentItem++;
                    if (currentItem > menuItems.Length - 1) currentItem = 0;
                }
                else if (keyInfo.Key.ToString() == "LeftArrow")
                {
                    currentItem--;
                    if (currentItem < 0) currentItem = Convert.ToInt16(menuItems.Length - 1);
                }
                // Loop around until the user presses enter.
            } while (keyInfo.KeyChar != 13);
            return currentItem;
        }

        public static short HorizontalMenu(string message, string[] menuItems)
        {
            short currentItem = 0, item;
            ConsoleKeyInfo keyInfo;

            do
            {
                Console.Clear();
                Console.WriteLine(message);

                for (item = 0; item < menuItems.Length; item++)
                {
                    if (currentItem == item)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write($"\t|{menuItems[item]}|".PadRight(10));
                    }
                    else
                    {
                        Console.Write($"\t{menuItems[item]}".PadRight(10));
                    }
                    Console.ResetColor();
                }
                keyInfo = Console.ReadKey(true);

                if (keyInfo.Key.ToString() == "RightArrow")
                {
                    currentItem++;
                    if (currentItem > menuItems.Length - 1) currentItem = 0;
                }
                else if (keyInfo.Key.ToString() == "LeftArrow")
                {
                    currentItem--;
                    if (currentItem < 0) currentItem = Convert.ToInt16(menuItems.Length - 1);
                }
                // Loop around until the user presses enter.
            } while (keyInfo.KeyChar != 13);
            return currentItem;
        }
    }
}
