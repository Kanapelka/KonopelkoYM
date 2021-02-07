using Athena.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Athena.Infrastructure.Entities.Configuration
{
    public class ObserverEntityConfiguration : IEntityTypeConfiguration<Observer>
    {
        public void Configure(EntityTypeBuilder<Observer> builder)
        {
            builder.ToTable("observers");

            builder.HasKey(observer => observer.ObserverId);

            builder.Property(observer => observer.TicketId).IsRequired();

            builder.Property(observer => observer.TicketId).IsRequired();
        }
    }
}