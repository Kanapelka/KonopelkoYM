using System;
using System.Text.Json.Serialization;

namespace Athena.Infrastructure.Models
{
    public class Comment
    {
        public int CommentId { get; set; }
        public int TicketId { get; set; }
        public int AuthorId { get; set; }

        public string Message { get; set; }

        public DateTime CreatedDate { get; set; }

        [JsonIgnore]

        public User Author { get; set; }

        [JsonIgnore]
        public Ticket CorrespondingTicket { get; set; }
    }
}