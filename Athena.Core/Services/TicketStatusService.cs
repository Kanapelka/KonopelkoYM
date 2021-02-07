using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Athena.Core.Result;
using Athena.Infrastructure;
using Athena.Infrastructure.Models;
using Microsoft.AspNetCore.Http;

namespace Athena.Core.Services
{
    public class TicketStatusService : BaseService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        private int UserId => int.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);


        public TicketStatusService(IInfrastructureOptionsProvider optionsProvider, IHttpContextAccessor contextAccessor) : base(optionsProvider)
        {
            _httpContextAccessor = contextAccessor;
        }


        public async Task<Result<TicketStatus>> AddNewStatusAsync(TicketStatus status)
        {
            await using var context = Context;

            bool userHasAccedToProject = context.Members.Any(member => member.ProjectId == status.ProjectId);
            if (!userHasAccedToProject) {
                return new Result<TicketStatus>
                {
                    ResultType = ResultType.Forbidden, Message = "Sorry, but you don't have access to this project."
                };
            }

            await context.Statuses.AddAsync(status);
            await context.SaveChangesAsync();

            return new Result<TicketStatus>{ ResultType = ResultType.Created, Payload = status };
        }

        public async Task<Result<TicketStatus>> UpdateStatusAsync(TicketStatus status)
        {
            await using var context = Context;

            bool userHasAccedToProject = context.Members.Any(member => member.ProjectId == status.ProjectId);
            if (!userHasAccedToProject) {
                return new Result<TicketStatus>
                {
                    ResultType = ResultType.Forbidden, Message = "Sorry, but you don't have access to this project."
                };
            }

            context.Statuses.Update(status);
            await context.SaveChangesAsync();

            return new Result<TicketStatus>{ ResultType = ResultType.Created, Payload = status };
        }

        public async Task<Result<string>> DeleteStatus(int statusId)
        {
            await using var context = Context;

            TicketStatus status = await context.Statuses.FindAsync(statusId);
            if (status == null) {
                return new Result<string>
                {
                    ResultType = ResultType.Bad,
                    Message = "There is no such a status",
                };
            }

            bool userHasAccedToProject = context.Members.Any(member => member.ProjectId == status.ProjectId);
            if (!userHasAccedToProject) {
                return new Result<string>
                {
                    ResultType = ResultType.Forbidden, Message = "Sorry, but you don't have access to this project."
                };
            }

            context.Statuses.Remove(status);
            context.Tickets.RemoveRange(context.Tickets.Where(t => t.StatusId == statusId));
            await context.SaveChangesAsync();

            return new Result<string>{ ResultType = ResultType.Deleted, Message = "Status was deleted." };
        }
    }
}