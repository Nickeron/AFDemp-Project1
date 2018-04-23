using System;
using System.Collections.Generic;

namespace Project1Afdemp
{
    public class ChatMessage
    {
        public int Id { get; set; }

        public DateTime TimeSent { get; set; }
        public string Text { get; set; }

        public int SenderId { get; set; }
        public User Sender { get; set; }

        public ICollection<User> UnreadUsers { get; set; }

        public ChatMessage()
        {
            UnreadUsers = new List<User>();
        }

        public ChatMessage(User sender, string text, ICollection<User> unreadUsers)
        {
            Sender = sender;
            Text = text;
            UnreadUsers = unreadUsers;
            TimeSent = DateTime.Now;
        }
    }
}
