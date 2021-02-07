using System;
using System.Text.Json.Serialization;

namespace Athena.Infrastructure.Models
{
    public class Notification
    {
        public int NotificationId { get; set; }
        public int RecipientId { get; set; }

        public string Title { get; set; }
        public string Message { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime? ExpiredDate { get; set; }

        [JsonIgnore]
        public User Recipient { get; set; }
    }
}