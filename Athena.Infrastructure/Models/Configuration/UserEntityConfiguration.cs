using Athena.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Athena.Infrastructure.Entities.Configuration
{
    public class UserEntityConfiguration : IEntityTypeConfiguration<User>

    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("users");

            builder.HasKey(user => user.UserId);

            builder.HasIndex(user => user.EmailAddress).IsUnique();
            builder.Property(user => user.EmailAddress).HasMaxLength(100).IsRequired();

            builder.Property(user => user.Password).HasMaxLength(100).IsRequired();

            builder.Property(user => user.FirstName).HasMaxLength(50).IsRequired();
            builder.Property(user => user.LastName).HasMaxLength(50).IsRequired();
            builder.HasIndex(user => new {user.FirstName, user.LastName});

            builder.Property(user => user.IsActive).HasDefaultValue(true).IsRequired();

            builder.Property(user => user.Location).HasMaxLength(50);
            builder.Property(user => user.JobTitle).HasMaxLength(50);

            builder
                .HasMany(user => user.TeamMemberOf)
                .WithOne(member => member.CorrespondingUser)
                .HasForeignKey(member => member.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasMany(user => user.TicketsAssigned)
                .WithOne(ticket => ticket.Assignee)
                .HasForeignKey(ticket => ticket.AssigneeId);

            builder
                .HasMany(user => user.Observes)
                .WithOne(observer => observer.CorrespondingUser)
                .HasForeignKey(observer => observer.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasMany(user => user.Notifications)
                .WithOne(notification => notification.Recipient)
                .HasForeignKey(notification => notification.RecipientId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}