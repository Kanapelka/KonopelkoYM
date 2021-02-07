using Athena.Infrastructure.Entities.Enums;
using Athena.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Athena.Infrastructure.Entities.Configuration
{
    public class TicketEntityConfiguration : IEntityTypeConfiguration<Ticket>

    {
        public void Configure(EntityTypeBuilder<Ticket> builder)
        {
            builder.ToTable("tickets");

            builder.HasKey(ticket => ticket.TicketId);

            builder.Property(ticket => ticket.ProjectId).IsRequired();

            builder.Property(ticket => ticket.Title).HasMaxLength(100).IsRequired();

            builder.Property(ticket => ticket.Description).HasMaxLength(1000).IsRequired();

            builder.Property(ticket => ticket.CreatedDate).IsRequired();

            builder.Property(ticket => ticket.LastModifiedDate).IsRequired();

            builder.Property(ticket => ticket.Priority).HasDefaultValue(Priority.High).IsRequired();

            builder
                .HasMany(ticket => ticket.Tasks)
                .WithOne(task => task.CorrespondingTicket)
                .HasForeignKey(task => task.TicketId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasMany(ticket => ticket.Comments)
                .WithOne(comment => comment.CorrespondingTicket)
                .HasForeignKey(comment => comment.TicketId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasMany(ticket => ticket.Observers)
                .WithOne(observer => observer.TicketObserved)
                .HasForeignKey(observer => observer.TicketId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}