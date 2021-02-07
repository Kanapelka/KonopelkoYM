using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Athena.Core.Models;
using Athena.Core.Result;
using Athena.Infrastructure;
using Athena.Infrastructure.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TicketTask = Athena.Infrastructure.Models.Task;


namespace Athena.Core.Services
{
    public class TicketService : BaseService
    {
        private readonly IHttpContextAccessor _contextAccessor;

        private int UserId => int.Parse(_contextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);


        public TicketService(IInfrastructureOptionsProvider optionsProvider, IHttpContextAccessor contextAccessor) : base(optionsProvider)
        {
            _contextAccessor = contextAccessor;
        }


        public async Task<Result<Ticket>> GetTicketByIdAsync(int ticketId)
        {
            await using var context = Context;

            Ticket ticket = await context.Tickets.FindAsync(ticketId);
            if (ticket == null) {
                return new Result<Ticket>{ ResultType = ResultType.NotFound, Message = "Ticket not found" };
            }

            return new Result<Ticket>{ ResultType = ResultType.Ok, Payload = ticket };
        }

        public async Task<Result<IReadOnlyCollection<TicketTask>>> GetTicketTasksAsync(int ticketId)
        {
            await using var context = Context;

            IReadOnlyCollection<TicketTask> tasks = await context.Tasks.Where(t => t.TicketId == ticketId).ToListAsync();

            return new Result<IReadOnlyCollection<TicketTask>>{ ResultType = ResultType.Ok, Payload = tasks };
        }

        public async Task<Result<IReadOnlyCollection<CommentModel>>> GetTicketComments(int ticketId)
        {
            await using var context = Context;

            IReadOnlyCollection<CommentModel> comments = await context.Comments
                .Include(comment => comment.Author)
                .Where(comment => comment.TicketId == ticketId)
                .Select(comment => new CommentModel
                {
                    CommentId = comment.CommentId,
                    TicketId = comment.TicketId,
                    AuthorId = comment.AuthorId,
                    AuthorFirstName = comment.Author.FirstName,
                    AuthorLastName = comment.Author.LastName,
                    CreatedDate = comment.CreatedDate,
                    Message = comment.Message,
                }).ToListAsync();

            return new Result<IReadOnlyCollection<CommentModel>>{ ResultType = ResultType.Ok, Payload = comments };
        }

        public async Task<Result<Ticket>> UpdateOrCreateTicketAsync(Ticket ticket)
        {
            await using var context = Context;

            Project project = await context.Projects.FindAsync(ticket.ProjectId);
            if (project == null) {
                return new Result<Ticket>{ ResultType = ResultType.NotFound, Message = "Corresponding project not found" };
            }

            bool userHasAccess = context.Members.Any(member => member.ProjectId == project.ProjectId && member.UserId == UserId);
            if (!userHasAccess) {
                return new Result<Ticket>
                {
                    ResultType = ResultType.Forbidden,
                    Message = "Sorry, but you don't have permission to update this ticket",
                };
            }

            User user = await context.Users.FindAsync(UserId);
            IReadOnlyCollection<User> members =
                await context.Members
                    .Where(member => member.ProjectId == project.ProjectId)
                    .Include(member => member.CorrespondingUser)
                    .Select(member => member.CorrespondingUser)
                    .ToListAsync();

            var notificationsBuilder = new NotificationBuilder(user, members);

            if (ticket.TicketId != 0 && context.Tickets.Any(t => t.TicketId == ticket.TicketId)) {
                if (ticket.AssigneeId == 0) {
                    ticket.AssigneeId = null;
                }
                context.Tickets.Update(ticket);
                await context.Notifications.AddRangeAsync(notificationsBuilder.BuildTicketUpdateNotifications(project, ticket));
            }
            else {
                await context.Tickets.AddAsync(ticket);
                await context.Notifications.AddRangeAsync(notificationsBuilder.BuildTicketCreateNotifications(project));
            }

            await context.SaveChangesAsync();

            return new Result<Ticket>{ ResultType = ResultType.Ok, Payload = ticket };
        }
    }
}