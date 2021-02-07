using System;

namespace Athena.Core.Models
{
    public class CommentModel
    {
        public int CommentId { get; set; }
        public int TicketId { get; set; }
        public int AuthorId { get; set; }
        public string AuthorFirstName { get; set; }
        public string AuthorLastName { get; set; }

        public string Message { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}