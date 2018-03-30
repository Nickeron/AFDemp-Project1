using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project1Afdemp
{
    public class Message
    {
        public int Id { get; set; }

        public DateTime TimeSent { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }

        public int SenderId { get; set; }
        public int ReceiverId { get; set; }

        public virtual User Sender { get; set; }
        public virtual User Receiver { get; set; }

        private Message() { }
        
        public Message(int senderid, int receiverid, string title, string body)
        {
            SenderId = senderid;
            ReceiverId = receiverid;
            Title = title;
            Body = body;
            TimeSent = DateTime.Now;
        }
    }
}
