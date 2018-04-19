﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Project1Afdemp
{
    class SideFunctions
    {
        public static User SelectUser(UserManager activeUserManager)
        {
            List<string> selectUserItems = new List<string>();
            using (var database = new DatabaseStuff())
            {
                List<User> availableUsers = database.Users.ToList();

                try
                {
                    foreach (User user in availableUsers)
                    {
                        if (user.UserName != activeUserManager.UserName)
                        {
                            selectUserItems.Add(user.UserName);
                        }
                    }
                }
                catch (Exception e) { Console.WriteLine(e); }
                if (selectUserItems.Count == 0)
                {
                    Console.WriteLine("\n\n\tNo users in database!");
                    Console.ReadKey();
                    return null;
                }
                selectUserItems.Add("Back");
                string selectedUserName = Menus.VerticalMenu(StringsFormatted.SelectUser, selectUserItems);
                if (selectedUserName.Contains("Back"))
                {
                    return null;
                }
                Console.Clear();
                return database.Users.Single(i => i.UserName == selectedUserName);
            }
        }

        public static Message SelectMessage(UserManager activeUserManager, bool Received)
        {
            List<string> selectMessageItems = new List<string>();
            using (var database = new DatabaseStuff())
            {
                List<Message> messages = database.Messages.ToList();
                int UserId = database.Users.Single(i => i.UserName == activeUserManager.UserName).Id;
                string receiverName;
                try
                {
                    string listedMessage;
                    foreach (Message message in messages)
                    {
                        if (Received)
                        {
                            if (message.ReceiverId == UserId)
                            {
                                receiverName = database.Users.Single(i => i.Id == message.SenderId).UserName;
                                if (!message.IsRead)
                                {
                                    listedMessage = "* ";
                                }
                                else
                                {
                                    listedMessage = "";
                                }
                                listedMessage += $"ID: |{message.Id}| From: |{receiverName}| Title: |{message.Title}| Time Sent: |{message.TimeSent}|";
                                selectMessageItems.Add(listedMessage);
                            }
                        }
                        else
                        {
                            if (message.SenderId == UserId)
                            {
                                receiverName = database.Users.Single(i => i.Id == message.ReceiverId).UserName;
                                listedMessage = $"ID: |{message.Id}| To: |{receiverName}| Title: |{message.Title}| Time Sent: |{message.TimeSent}|";
                                selectMessageItems.Add(listedMessage);
                            }
                        }
                    }
                }
                catch (Exception e) { Console.WriteLine(e); }

                if (selectMessageItems.Count == 0)
                {
                    Console.WriteLine("\n\n\tNo Messages to View");
                    Console.ReadKey();
                    return null;
                }
                selectMessageItems.Add("Back");
                string oMessage = Menus.VerticalMenu(StringsFormatted.OpenMessage, selectMessageItems);
                if (oMessage.Contains("Back"))
                {
                    return null;
                }
                string[] selParameters = oMessage.Split('|');
                int messageID = int.Parse(selParameters[1]);

                Console.Clear();
                return database.Messages.Single(i => i.Id == messageID);
            }
        }

        

        public static void ForwardMessage(UserManager activeUserManager, Message forwardMessage)
        {
            User receiver = SelectUser(activeUserManager);
            string forwardTitle = "FW:" + forwardMessage.Title;
            string forwardBody = forwardMessage.Body;
            Message forwardedMessage = new Message(activeUserManager.TheUser.Id, receiver.Id, forwardTitle, forwardBody);
            using (var database = new DatabaseStuff())
            {
                database.Messages.Add(forwardedMessage);
                database.SaveChanges();
            }
            Console.Clear();
            Console.WriteLine($"\n\n\tMessage successfully forwarded to {receiver.UserName}\n\n\tOK");
            Console.ReadKey();
        }

        public static void DeleteMessage(Message receivedMessage)
        {
            if (Menus.HorizontalMenu("\n\n\tAre you sure you want to delete this message?", new List<string> { "Yes", "No" }).Contains('Y'))
            {
                using (var database = new DatabaseStuff())
                {
                    database.Messages.Remove(database.Messages.Single(i => i.Id == receivedMessage.Id));
                    database.SaveChanges();
                    Console.Clear();
                    Console.WriteLine("\n\n\tMessage successfully DELETED\n\n\tOK");
                    Console.ReadKey();
                }
            }
        }

        public static void CreateNewUser()
        {
            using (var database = new DatabaseStuff())
            {
                UserManager newUser = new UserManager(true);
            }
        }

        public static void DeleteUser(User deletingUser)
        {
            if (Menus.HorizontalMenu("\n\n\tAre you sure you want to delete this user?", new List<string> { "Yes", "No" }).Contains('Y'))
            {
                using (var database = new DatabaseStuff())
                {
                    // When deleting a user you have to erase their emails and chat messages as well
                    database.Users.Remove(database.Users.Single(i => i.UserName == deletingUser.UserName));
                    var deletingMessages = database.Messages.Where(i => i.ReceiverId == deletingUser.Id || i.SenderId == deletingUser.Id);
                    foreach (Message deletingMessage in deletingMessages)
                    {
                        database.Messages.Remove(deletingMessage);
                    }
                    var deletingChatMessages = database.Chat.Where(i => i.SenderId == deletingUser.Id);
                    foreach (ChatMessage deletingChatMessage in deletingChatMessages)
                    {
                        database.Chat.Remove(deletingChatMessage);
                    }
                    database.SaveChanges();
                }
                Console.Clear();
                Console.WriteLine("\n\n\tUser successfully DELETED\n\n\tOK");
                Console.ReadKey();
            }
        }

        public static void ChangeUserPermissions(User changingUser)
        {
            List<string> manageUserItems;
            if (changingUser.UserAccess == Accessibility.administrator)
            {
                manageUserItems = new List<string>() { "downgrade to USER", "downgrade to GUEST", "Back" };
            }
            else if (changingUser.UserAccess == Accessibility.user)
            {
                manageUserItems = new List<string>() { "upgrade to ADMINISTRATOR", "downgrade to GUEST", "Back" };
            }
            else
            {
                manageUserItems = new List<string>() { "upgrade to ADMINISTRATOR", "upgrade to USER", "Back" };
            }
            string changeOfAccess = Menus.VerticalMenu($"\n\n\t{changingUser.UserName} is {changingUser.UserAccess}, how do you want to change his permissions?", manageUserItems);
            using (var database = new DatabaseStuff())
            {
                User changedUser = database.Users.Single(i => i.UserName == changingUser.UserName);
                if (changeOfAccess.Contains("ADMINISTRATOR"))
                {
                    changedUser.UserAccess = Accessibility.administrator;
                }
                else if (changeOfAccess.Contains("USER"))
                {
                    changedUser.UserAccess = Accessibility.user;
                }
                else if (changeOfAccess.Contains("GUEST"))
                {
                    changedUser.UserAccess = Accessibility.guest;
                }
                else
                {
                    return;
                }
                database.SaveChanges();
            }
            Console.Write($"\n\n\tYou did {changeOfAccess}, the user: {changingUser.UserName}\n\n\tOK");
            Console.ReadKey();
        }
    }
}