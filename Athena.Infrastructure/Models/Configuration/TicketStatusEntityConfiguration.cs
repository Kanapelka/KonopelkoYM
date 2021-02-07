using Athena.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Athena.Infrastructure.Entities.Configuration
{
    public class TicketStatusEntityConfiguration : IEntityTypeConfiguration<TicketStatus>
    {
        public void Configure(EntityTypeBuilder<TicketStatus> builder)
        {
            builder.ToTable("ticket_statuses");

            builder.HasKey(status => status.TicketStatusId);

            builder.Property(status => status.ProjectId).IsRequired();

            builder.Property(status => status.Title).HasMaxLength(50).IsRequired();

            builder
                .HasMany(status => status.TicketsWithThisStatus)
                .WithOne(ticket => ticket.Status)
                .HasForeignKey(ticket => ticket.StatusId);
        }
    }
}