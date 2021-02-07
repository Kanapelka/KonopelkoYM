using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Athena.Core.Models;
using Athena.Infrastructure;
using Athena.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace Athena.Core.Services
{
    public class DashboardService : BaseService
    {
        public DashboardService(IInfrastructureOptionsProvider optionsProvider) : base(optionsProvider)
        {
        }


        public async Task<IReadOnlyCollection<TicketThumbnail>> GetHighestPriorityTickets(int userId)
        {
            await using var context = Context;

            IReadOnlyCollection<TicketThumbnail> tickets = await context.Tickets
                .Include(ticket => ticket.Assignee)
                .Include(ticket => ticket.CorrespondingProject)
                .Include(ticket => ticket.Status)
                .Where(ticket => ticket.AssigneeId == userId)
                .OrderBy(ticket => ticket.Priority)
                .ThenByDescending(ticket => ticket.CreatedDate)
                .Take(6)
                .Select(ticket => new TicketThumbnail
                {
                    TicketId = ticket.TicketId,
                    ProjectId = ticket.ProjectId,
                    AssigneeId = ticket.AssigneeId,
                    AssigneeFirstName = ticket.Assignee.FirstName,
                    AssigneeLastName = ticket.Assignee.LastName,
                    ProjectName = ticket.CorrespondingProject.Name,
                    TicketTitle = ticket.Title,
                    Priority = ticket.Priority,
                    StatusDisplayed = ticket.Status.Title,
                    StatusId = ticket.Status.TicketStatusId,
                }).ToListAsync();

            return tickets;
        }

        public async Task<IReadOnlyCollection<UserProfile>> GetRecentTeammates(int userId)
        {
            await using var context = Context;

            IReadOnlyCollection<Member> correspondingUserMembers =
                await context.Members.Where(m => m.UserId == userId).ToListAsync();

            IReadOnlyCollection<int> projectIds = correspondingUserMembers.Select(m => m.ProjectId).ToList();
            IReadOnlyCollection<int> memberIds = correspondingUserMembers.Select(m => m.MemberId).ToList();

            IReadOnlyCollection<Project> projectsParticipated =
                await context.Projects.Where(p => projectIds.Contains(p.ProjectId)).ToListAsync();

            IReadOnlyCollection<Member> userMates = await context.Members
                .Where(m => !memberIds.Contains(m.MemberId) && projectIds.Contains(m.ProjectId))
                .OrderByDescending(m => m.MemberId)
                .ToListAsync();

            IReadOnlyCollection<int> userIds = userMates.Select(u => u.UserId).Distinct().ToList();

            IReadOnlyCollection<UserProfile> result = await context.Users
                .Where(u => userIds.Contains(u.UserId)).Select(user => new UserProfile
                {
                    UserId = user.UserId,
                    EmailAddress = user.EmailAddress,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    IsActive = user.IsActive,
                    JobTitle = user.JobTitle,
                    Location = user.Location,
                }).ToListAsync();

            return result;
        }
    }
}