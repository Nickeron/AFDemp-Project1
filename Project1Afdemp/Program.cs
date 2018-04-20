﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Project1Afdemp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Chat@Bootcamp";
            // If database is empty create the 2 basic users firsthand.
            using (var database = new DatabaseStuff())
            {
                if (database.Users.Count().Equals(0))
                {
                    try
                    {
                        UserManager newAdmin = new UserManager("admin", "aDmI3$", true);
                        UserManager newGuest = new UserManager("guest", "guest", true);
                    }
                    catch (Exception e) { Console.WriteLine(e); }
                }
            }
            // Create the main object for managing user's functions
            UserManager activeUserManager;

            while (true)
            {
                // Everytime a user logs out the active user manager get's another object.
                activeUserManager = MenuFunctions.LoginScreen();
                MainMenu(activeUserManager);
            }
        }

        public static void MainMenu(UserManager activeUserManager)
        {
            while (true)
            {
                using (var database = new DatabaseStuff())
                {
                    // Probe the database for the nuber of unread messages in chat and unread mail
                    int unreadMessages = database.Messages.Count(message => message.IsRead == false && message.Receiver.Id == activeUserManager.TheUser.Id);
                    int sentMessages   = database.Messages.Count(message => message.Sender.Id == activeUserManager.TheUser.Id);
                    int unreadChatMessages = database.Users.Include("UnreadChatMessages").Single(c=> c.UserName == activeUserManager.UserName).UnreadChatMessages.Count;

                    // Create the Menu items common to all users
                    List<string> mainMenuItems = new List<string> { $"Chat  ({unreadChatMessages})", "Send Email", $"Inbox ({unreadMessages})", $"Sent  ({sentMessages})", "Log Out", "Exit" };

                    // Add more options for User and Administrator access.
                    if (activeUserManager.UserAccess == Accessibility.administrator)
                    {
                        mainMenuItems.Insert(3, "Manage Users");
                        mainMenuItems.Insert(4, "Messages History");
                    }
                    else if (activeUserManager.UserAccess == Accessibility.user)
                    {
                        mainMenuItems.Insert(4, "Messages History");
                    }

                    // Acquire the choice of function from the user using a vertical menu
                    string userChoice = Menus.VerticalMenu(StringsFormatted.MainMenu, mainMenuItems);

                    // Check what the user chose and act accordingly
                    if (userChoice.Contains("Chat"))
                    {
                        MenuFunctions.ShowChat(activeUserManager);
                    }
                    else if (userChoice.Contains("Send Email"))
                    {
                        MenuFunctions.SendEmail(activeUserManager);
                    }
                    else if (userChoice.Contains("Inbox"))
                    {
                        MenuFunctions.PresentAndManipulateMessage(activeUserManager);
                    }
                    else if (userChoice.Contains("Sent"))
                    {
                        MenuFunctions.PresentAndManipulateMessage(activeUserManager, Received:false);
                    }
                    else if (userChoice.Contains("Messages History"))
                    {
                        MenuFunctions.CommunicationHistory(activeUserManager);
                    }
                    else if (userChoice.Contains("Manage Users"))
                    {
                        MenuFunctions.ManageUsers(activeUserManager);
                    }
                    else if (userChoice.Contains("Log Out"))
                    {
                        return;
                    }
                    else if (userChoice.Contains("Exit"))
                    {
                        Environment.Exit(0);
                    }
                }
            }
        }    
    }
}
