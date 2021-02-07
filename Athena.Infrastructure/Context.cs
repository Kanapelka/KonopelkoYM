using Athena.Infrastructure.Entities.Configuration;
using Athena.Infrastructure.Models;
using Athena.Infrastructure.Models.Configuration;
using Microsoft.EntityFrameworkCore;

namespace Athena.Infrastructure
{
    public sealed class Context : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Observer> Observers { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<TicketStatus> Statuses { get; set; }
        public DbSet<Task> Tasks { get; set; }
        public DbSet<Notification> Notifications { get; set; }


        public Context(IInfrastructureOptionsProvider provider) : base(provider.Options)
        {
            Database.EnsureCreated();
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .ApplyConfiguration(new UserEntityConfiguration())
                .ApplyConfiguration(new ProjectEntityConfiguration())
                .ApplyConfiguration(new MemberEntityConfiguration())
                .ApplyConfiguration(new TicketEntityConfiguration())
                .ApplyConfiguration(new TicketStatusEntityConfiguration())
                .ApplyConfiguration(new TaskEntityConfiguration())
                .ApplyConfiguration(new ObserverEntityConfiguration())
                .ApplyConfiguration(new NotificationEntityConfiguration())
                .ApplyConfiguration(new MemberEntityConfiguration())
                .ApplyConfiguration(new CommentEntityConfiguration());
        }
    }
}