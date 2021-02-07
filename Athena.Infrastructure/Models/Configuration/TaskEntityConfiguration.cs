using Athena.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Athena.Infrastructure.Entities.Configuration
{
    public class TaskEntityConfiguration : IEntityTypeConfiguration<Task>
    {
        public void Configure(EntityTypeBuilder<Task> builder)
        {
            builder.ToTable("tasks");

            builder.HasKey(task => task.TaskId);

            builder.Property(task => task.TicketId).IsRequired();

            builder.Property(task => task.Title).HasMaxLength(70).IsRequired();

            builder.Property(task => task.Done).HasDefaultValue(false).IsRequired();
        }
    }
}