using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project1Afdemp
{
    public class Message
    {
        public int Id { get; set; }
        public User Sender { get; set; }
        public User Receiver { get; set; }
        public DateTime TimeSent { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }

        private Message() { }

        public Message(User sender, User receiver, string title, string body)
        {
            Sender = sender;
            Receiver = receiver;
            Title = title;
            Body = body;
            TimeSent = DateTime.Now;
        }
    }
}
