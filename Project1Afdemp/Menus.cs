using System;
using System.Collections.Generic;

namespace Project1Afdemp
{
    class Menus
    {
        public static string VerticalMenu(string message, List<string> menuItems)
        {
            short currentItem = 0, item;
            ConsoleKeyInfo keyInfo;

            do
            {
                Console.Clear();
                Console.WriteLine(message+'\n');
                if (menuItems.Count == 0)
                    return "";
                for (item = 0; item < menuItems.Count; item++)
                {
                    if (currentItem == item)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"\n\t|{menuItems[item]}|".PadRight(10));
                    }
                    else
                    {
                        Console.WriteLine($"\n\t{menuItems[item]}".PadRight(10));
                    }
                    Console.ResetColor();
                }
                do
                {
                    keyInfo = Console.ReadKey(true);
                } while (keyInfo.KeyChar != 13 && keyInfo.Key.ToString() != "DownArrow" && keyInfo.Key.ToString() != "UpArrow");


                if (keyInfo.Key.ToString() == "DownArrow")
                {
                    currentItem++;
                    if (currentItem > menuItems.Count - 1) currentItem = 0;
                }
                else if (keyInfo.Key.ToString() == "UpArrow")
                {
                    currentItem--;
                    if (currentItem < 0) currentItem = Convert.ToInt16(menuItems.Count - 1);
                }
                // Loop around until the user presses enter.
            } while (keyInfo.KeyChar != 13);
            return menuItems[currentItem];
        }

        public static string HorizontalMenu(string message, List<string> menuItems)
        {
            short currentItem = 0, item;
            ConsoleKeyInfo keyInfo;

            do
            {
                Console.Clear();
                Console.WriteLine(message + '\n');
                if (menuItems.Count == 0)
                    return "";
                for (item = 0; item < menuItems.Count; item++)
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
                } while (keyInfo.KeyChar != 13 && keyInfo.Key.ToString() != "RightArrow" && keyInfo.Key.ToString() != "LeftArrow");

                if (keyInfo.Key.ToString() == "RightArrow")
                {
                    currentItem++;
                    if (currentItem > menuItems.Count - 1) currentItem = 0;
                }
                else if (keyInfo.Key.ToString() == "LeftArrow")
                {
                    currentItem--;
                    if (currentItem < 0) currentItem = Convert.ToInt16(menuItems.Count - 1);
                }
                // Loop around until the user presses enter.
            } while (keyInfo.KeyChar != 13);
            return menuItems[currentItem];
        }
    }
}
