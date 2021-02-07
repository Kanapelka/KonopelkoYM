using Athena.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Athena.Infrastructure.Entities.Configuration
{
    public class CommentEntityConfiguration : IEntityTypeConfiguration<Comment>
    {
        public void Configure(EntityTypeBuilder<Comment> builder)
        {
            builder.ToTable("comments");

            builder.HasKey(comment => comment.CommentId);

            builder.Property(comment => comment.AuthorId).IsRequired();

            builder.Property(comment => comment.TicketId).IsRequired();

            builder.Property(comment => comment.CreatedDate).IsRequired();

            builder.Property(comment => comment.Message).HasMaxLength(500).IsRequired();
        }
    }
}