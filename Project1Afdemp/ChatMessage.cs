using System;

namespace Project1Afdemp
{
    public class ChatMessage
    {
        public int Id { get; set; }

        public DateTime TimeSent { get; set; }
        public string Text { get; set; }

        public int SenderId { get; set; }

        // Virtual in this context means nullable
        public virtual User Sender { get; set; }

        public ChatMessage() { }

        public ChatMessage(int senderid, string text)
        {
            SenderId = senderid;
            Text = text;
            TimeSent = DateTime.Now;
        }
    }
}
