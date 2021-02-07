using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Athena.Core.Result;
using Athena.Infrastructure;
using Athena.Infrastructure.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TicketTask = Athena.Infrastructure.Models.Task;


namespace Athena.Core.Services
{
    public class TaskSerivce : BaseService
    {
        private readonly IHttpContextAccessor _contextAccessor;

        private int UserId => int.Parse(_contextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);


        public TaskSerivce(IInfrastructureOptionsProvider optionsProvider, IHttpContextAccessor contextAccessor) : base(optionsProvider)
        {
            _contextAccessor = contextAccessor;
        }


        public async Task<Result<TicketTask>> AddTaskAsync(TicketTask task)
        {
            await using var context = Context;

            Ticket ticket = context.Tickets
                .Include(t => t.CorrespondingProject)
                .FirstOrDefault(t => t.TicketId == task.TicketId);

            if (ticket != null) {
                User notifier = await context.Users.FindAsync(UserId);
                IReadOnlyCollection<User> usersToNotify =  await context.Members
                    .Where(member => member.ProjectId == ticket.ProjectId)
                    .Include(member => member.CorrespondingUser)
                    .Select(member => member.CorrespondingUser)
                    .ToListAsync();

                var notificationBuilder = new NotificationBuilder(notifier, usersToNotify);
                IReadOnlyCollection<Notification> notifications =
                    notificationBuilder.BuildTaskCreateNotifications(ticket.CorrespondingProject, ticket, task);

                await context.Notifications.AddRangeAsync(notifications);
            }

            await context.Tasks.AddAsync(task);
            await context.SaveChangesAsync();

            return new Result<TicketTask>{ Payload = task, ResultType = ResultType.Ok };
        }

        public async Task<Result<TicketTask>> ModifyTaskAsync(TicketTask task)
        {
            await using var context = Context;

            if (!context.Tasks.Any(t => t.TaskId == task.TaskId)) {
                return new Result<TicketTask>{ ResultType = ResultType.NotFound, Message = "No task to update."};
            }

            Ticket ticket = context.Tickets
                .Include(t => t.CorrespondingProject)
                .FirstOrDefault(t => t.TicketId == task.TicketId);

            if (task.Done && ticket != null) {
                User notifier = await context.Users.FindAsync(UserId);
                IReadOnlyCollection<User> usersToNotify =  await context.Members
                    .Where(member => member.ProjectId == ticket.ProjectId)
                    .Include(member => member.CorrespondingUser)
                    .Select(member => member.CorrespondingUser)
                    .ToListAsync();

                var notificationBuilder = new NotificationBuilder(notifier, usersToNotify);
                IReadOnlyCollection<Notification> notifications =
                    notificationBuilder.BuildTaskModifyNotifications(ticket.CorrespondingProject, ticket, task);

                await context.Notifications.AddRangeAsync(notifications);
            }

            context.Tasks.Update(task);
            await context.SaveChangesAsync();

            return new Result<TicketTask>{ ResultType = ResultType.Ok, Payload = task };
        }

        public async Task<Result<string>> DeleteTaskAsync(int taskId)
        {
            await using var context = Context;

            TicketTask task = await context.Tasks.FindAsync(taskId);

            Ticket ticket = context.Tickets
                .Include(t => t.CorrespondingProject)
                .FirstOrDefault(t => t.TicketId == task.TicketId);

            User notifier = await context.Users.FindAsync(UserId);
            IReadOnlyCollection<User> usersToNotify =  await context.Members
                .Where(member => member.ProjectId == ticket.ProjectId)
                .Include(member => member.CorrespondingUser)
                .Select(member => member.CorrespondingUser)
                .ToListAsync();

            var notificationBuilder = new NotificationBuilder(notifier, usersToNotify);
            IReadOnlyCollection<Notification> notifications =
                notificationBuilder.BuildTaskDeleteNotifications(ticket.CorrespondingProject, ticket, task);

            TicketTask correspondingTask = await context.Tasks.FindAsync(taskId);
            if (correspondingTask == null) {
                return new Result<string>{ ResultType = ResultType.NotFound, Message = "No task to delete."};
            }

            await context.Notifications.AddRangeAsync(notifications);
            context.Tasks.Remove(correspondingTask);
            await context.SaveChangesAsync();

            return new Result<string> { ResultType = ResultType.Ok, Message = "Deleted" };
        }
    }
}