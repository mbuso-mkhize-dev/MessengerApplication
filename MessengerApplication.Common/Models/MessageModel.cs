using System;

namespace MessengerApplication.Common.Models
{
    /// <summary>
    /// Message Model
    /// </summary>
    public class MessageModel
    {
        public Guid Id { get; set; }

        public string MessageBody { get; set; }

        public string Sender { get; set; }

        public DateTime SentAt { get; set; }
    }
}