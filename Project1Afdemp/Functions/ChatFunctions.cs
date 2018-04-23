using System;
using System.Collections.Generic;
using System.Linq;

namespace Project1Afdemp
{
    static class ChatFunctions
    {
        public static void EditChatMessages()
        {
            int chosenMessageID = SelectChatMessage();
            List<string> editOptions = new List<string>() { "Update", "Delete", "Back"};
            string editChoice = Menus.VerticalMenu(StringsFormatted.Options, editOptions);
            if (editChoice.Contains("Update"))
            {
                UpdateChatMessage(chosenMessageID);
            }
            else if (editChoice.Contains("Delete"))
            {
                DeleteChatMessage(chosenMessageID);
            }
        }

        public static int SelectChatMessage()
        {
            Console.Clear();
            string chat = StringsFormatted.Chat + "\n\n";
            List<string> chatMessages = new List<string>();

            using (var database = new DatabaseStuff())
            {
                List<ChatMessage> DBChatMessages = database.Chat.Include("Sender").ToList();
                foreach (ChatMessage chatMessage in DBChatMessages)
                {
                    chatMessages.Add($"{chatMessage.Id}. {chatMessage.TimeSent.ToString("dd/MM HH:mm")}   {chatMessage.Sender.UserName}: {chatMessage.Text}");
                }
            }
            return int.Parse(Menus.VerticalMenu(chat, chatMessages).Split('.').First());
        }

        public static void UpdateChatMessage(int chosenMessageID)
        {
            Console.Clear();
            using (var database = new DatabaseStuff())
            {
                ChatMessage editedMessage = database.Chat.Single(c => c.Id == chosenMessageID);
                Console.Write("\n\n\tOLD TEXT: "+ editedMessage.Text+ "\n\n\tNEW TEXT: * ");
                editedMessage.Text = "* " + Console.ReadLine();
                Console.WriteLine("\n\n\tSAVE");
                Console.ReadKey(true);
                database.SaveChanges();
            }
        }

        public static void DeleteChatMessage(int chosenMessageID)
        {
            if (Menus.HorizontalMenu("\n\n\tAre you sure you want to delete the selected messages?", new List<string> { "Yes", "No" }).Contains('Y'))
            {
                using (var database = new DatabaseStuff())
                {
                    database.Chat.Remove(database.Chat.Single(c => c.Id == chosenMessageID));
                    database.SaveChanges();
                }
            }
        }

        public static void DeleteAllChatMessages()
        {
            if (Menus.HorizontalMenu("\n\n\tAre you sure you want to delete ALL chat messages?", new List<string> { "Yes", "No" }).Contains('Y'))
            {
                using (var database = new DatabaseStuff())
                {
                    // When deleting all the contents of a table the ExecuteSqlCommand
                    // is used for execution speed purposes.
                    database.Database.ExecuteSqlCommand("DELETE FROM ChatMessages");
                    database.SaveChanges();
                }
                Console.Clear();
                Console.WriteLine("\n\n\tCHAT successfully DELETED\n\n\tOK");
                Console.ReadKey();
            }
        }

        public static void AddReplyToChat(UserManager activeUserManager, string chat)
        {
            using (var database = new DatabaseStuff())
            {
                // Access the active user from Database
                User thisUser = database.Users.Single(u => u.UserName == activeUserManager.UserName);
                // Rewrite the whole chat with the username added at the bottom
                Console.Clear();
                Console.Write(chat + "\n\n\t" + activeUserManager.UserName + ": ");

                // Collect all the other users in a list
                var unreadUsers = database.Users.Where(u => u.UserName != thisUser.UserName).ToList();
                string replyText = Console.ReadLine();
                // Create the new chat message
                database.Chat.Add(new ChatMessage (thisUser, replyText, unreadUsers));
                database.SaveChanges();
            }
        }
    }
}
