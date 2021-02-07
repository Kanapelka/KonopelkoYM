using Athena.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Athena.Infrastructure.Entities.Configuration
{
    public class ProjectEntityConfiguration : IEntityTypeConfiguration<Project>

    {
        public void Configure(EntityTypeBuilder<Project> builder)
        {
            builder.ToTable("projects");

            builder.HasKey(project => project.ProjectId);

            builder.HasIndex(project => project.Name);
            builder.Property(project => project.Name).HasMaxLength(50).IsRequired();

            builder
                .HasMany(project => project.Tickets)
                .WithOne(ticket => ticket.CorrespondingProject)
                .HasForeignKey(ticket => ticket.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasMany(project => project.Members)
                .WithOne(member => member.CorrespondingProject)
                .HasForeignKey(member => member.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasMany(project => project.CustomStatuses)
                .WithOne(status => status.BelongsToProject)
                .HasForeignKey(status => status.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}