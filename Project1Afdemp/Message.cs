using System;

namespace Project1Afdemp
{
    public class Message
    {
        public int Id { get; set; }

        public DateTime TimeSent { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public bool IsRead { get; set; }

        public int SenderId { get; set; }
        public int ReceiverId { get; set; }

        // Virtual in this context means nullable
        public virtual User Sender { get; set; }
        public virtual User Receiver { get; set; }

        public Message() { }

        public Message(int senderid, int receiverid, string title, string body)
        {
            SenderId = senderid;
            ReceiverId = receiverid;
            Title = title;
            Body = body;
            TimeSent = DateTime.Now;
            IsRead = false;
        }
    }
}
