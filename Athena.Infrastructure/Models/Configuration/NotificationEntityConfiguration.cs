using Athena.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Athena.Infrastructure.Entities.Configuration
{
    public class NotificationEntityConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.ToTable("notifications");

            builder.HasKey(notification => notification.NotificationId);

            builder.Property(notification => notification.RecipientId).IsRequired();

            builder.Property(notification => notification.Title).HasMaxLength(70).IsRequired();

            builder.Property(notification => notification.Message).HasMaxLength(200).IsRequired();

            builder.Property(notification => notification.CreatedDate).IsRequired();
        }
    }
}