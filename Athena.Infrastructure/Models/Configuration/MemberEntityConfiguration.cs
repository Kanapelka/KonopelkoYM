using Athena.Infrastructure.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Athena.Infrastructure.Models.Configuration
{
    public class MemberEntityConfiguration : IEntityTypeConfiguration<Member>
    {
        public void Configure(EntityTypeBuilder<Member> builder)
        {
            builder.ToTable("members");

            builder.HasKey(member => member.MemberId);

            builder.Property(member => member.UserId).IsRequired();

            builder.Property(member => member.ProjectId).IsRequired();

            builder.Property(member => member.Role).HasDefaultValue(MemberRole.Member).IsRequired();
        }
    }
}