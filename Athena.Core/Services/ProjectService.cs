using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Athena.Core.Models;
using Athena.Core.Result;
using Athena.Infrastructure;
using Athena.Infrastructure.Models;
using Athena.Infrastructure.Models.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Athena.Core.Services
{
    public class ProjectService : BaseService
    {
        private readonly int _userId;


        public ProjectService(IInfrastructureOptionsProvider optionsProvider, IHttpContextAccessor contextAccessor) : base(optionsProvider)
        {
            _userId = int.Parse(contextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
        }


        public async Task<Result<Project>> CreateProjectAsync(string projectName, int ownerId)
        {
            await using var context = Context;

            var project = new Project { Name = projectName };

            var member = new Member
            {
                ProjectId = project.ProjectId,
                UserId = ownerId,
                Role = MemberRole.Owner,
            };

            project.Members = new List<Member>{ member };

            await context.Projects.AddAsync(project);
            await context.SaveChangesAsync();

            return new Result<Project>{ ResultType = ResultType.Ok, Payload = project };
        }

        public void UpdateOwner(int projectId)
        {
            using var context = Context;
            var member = context.Members.FirstOrDefault(m => m.ProjectId == projectId);
            member.Role = MemberRole.Owner;
            context.Members.Update(member);
            context.SaveChanges();
        }

        public async Task<Result<Member>> CreateMemberAsync(string email, int projectId)
        {
            await using var context = Context;

            User user = context.Users.FirstOrDefault(u => u.EmailAddress == email);

            if (user == null) {
                return new Result<Member> { ResultType = ResultType.NotFound, Message = "User not found" };
            }

            Project project = await context.Projects.FindAsync(projectId);

            if (project == null) {
                return new Result<Member>{ ResultType = ResultType.NotFound, Message = "Corresponding project not found."};
            }

            bool userHavePermission = context.Members
                .Any(m => m.UserId == _userId && m.ProjectId == projectId && (m.Role == MemberRole.Admin || m.Role == MemberRole.Owner));

            if (!userHavePermission) {
                return new Result<Member> { ResultType = ResultType.Forbidden, Message = "You don't have permission." };
            }

            var member = new Member{ ProjectId = projectId, UserId = user.UserId, Role = MemberRole.Member };
            if (context.Members.Any(m => m.ProjectId == projectId && m.UserId == user.UserId)) {
                return new Result<Member> { ResultType = ResultType.Bad, Message = "Already a member." };
            }

            User notifier = await context.Users.FindAsync(_userId);
            var usersToNotify = new List<User>{ user };
            var notificationBuilder = new NotificationBuilder(notifier, usersToNotify);
            IReadOnlyCollection<Notification> notifications = notificationBuilder.BuildInvitations(project);

            await context.Notifications.AddRangeAsync(notifications);
            await context.Members.AddAsync(member);
            await context.SaveChangesAsync();

            return new Result<Member>{ ResultType = ResultType.Created, Payload = member };
        }

        public async Task<IReadOnlyCollection<ProjectMember>> GetProjectUsers(int projectId)
        {
            await using var context = Context;

            List<ProjectMember> users = await context.Members
                .Where(member => member.ProjectId == projectId)
                .Include(member => member.CorrespondingUser)
                .Select(member => new ProjectMember
                {
                    MemberId = member.MemberId,
                    UserId = member.CorrespondingUser.UserId,
                    EmailAddress = member.CorrespondingUser.EmailAddress,
                    FirstName = member.CorrespondingUser.FirstName,
                    LastName = member.CorrespondingUser.LastName,
                    JobTitle = member.CorrespondingUser.JobTitle,
                    Location = member.CorrespondingUser.Location,
                    ProjectId = projectId,
                    ProjectRole = member.Role,
                }).ToListAsync();

            return users;
        }

        public async Task<Result<ProjectThumbnail>> GetProjectThumbnailAsync(int projectId)
        {
            await using var context = Context;

            bool userIsProjectMember =
                context.Members.Any(member => member.ProjectId == projectId && member.UserId == _userId);

            if (!userIsProjectMember) {
                return new Result<ProjectThumbnail>
                {
                    ResultType = ResultType.Forbidden, Message = "You don't have access to this project."
                };
            }

            Project project = context.Projects.FirstOrDefault(p => p.ProjectId == projectId);
            if (project == null) {
                return new Result<ProjectThumbnail>{ ResultType = ResultType.NotFound, Message = "Project not found."};
            }

            User owner = context.Members
                .Where(member => member.ProjectId == projectId)
                .Include(member => member.CorrespondingUser)
                .Select(member => member.CorrespondingUser).FirstOrDefault();

            if (owner == null) {
                return new Result<ProjectThumbnail>{ ResultType = ResultType.NotFound, Message = "Owner not found."};
            }

            int ticketCount = context.Tickets.Count(t => t.ProjectId == projectId);

            return new Result<ProjectThumbnail>
            {
                Payload = new ProjectThumbnail
                {
                    OwnerId = owner.UserId,
                    OwnerName = $"{owner.FirstName} {owner.LastName}",
                    ProjectId = projectId,
                    ProjectName = project.Name,
                    TicketsCount = ticketCount,
                }
            };
        }

        public async Task<Result<IReadOnlyCollection<TicketThumbnail>>> GetProjectTicketThumbnailsAsync(int projectId)
        {
            await using var context = Context;

            var project = context.Projects.Find(projectId);
            if (project == null) {
                return new Result<IReadOnlyCollection<TicketThumbnail>>
                {
                    ResultType = ResultType.NotFound,
                    Message = "Project with such an ID does not exists",
                };
            }

            if (!context.Members.Any(member => member.ProjectId == projectId && member.UserId == _userId)) {
                return new Result<IReadOnlyCollection<TicketThumbnail>>
                {
                    ResultType = ResultType.Forbidden,
                    Message = "Sorru, but you don't have an access to this project."
                };
            }

            var tickets = await context.Tickets
                .Include(ticket => ticket.Assignee)
                .Include(ticket => ticket.CorrespondingProject)
                .Include(ticket => ticket.Status)
                .Where(ticket => ticket.ProjectId == projectId)
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
                    StatusId = ticket.Status.TicketStatusId,
                    StatusDisplayed = ticket.Status.Title
                }).ToListAsync();

            return new Result<IReadOnlyCollection<TicketThumbnail>>{ ResultType = ResultType.Ok, Payload = tickets};
        }

        public async Task<Result<ProjectThumbnail>> UpdateProjectAsync(Project project)
        {
            await using var context = Context;

            if (!context.Members.Any(member => member.ProjectId == project.ProjectId && member.UserId == _userId)) {
                return new Result<ProjectThumbnail>
                {
                    ResultType = ResultType.Forbidden,
                    Message = "Sorru, but you don't have an access to this project."
                };
            }

            Project correspondingProject = await context.Projects.FindAsync(project.ProjectId);
            if (correspondingProject == null) {
                return new Result<ProjectThumbnail>
                {
                    ResultType = ResultType.NotFound,
                    Message = "Project not found",
                };
            }

            correspondingProject.Name = project.Name;
            context.Projects.Update(correspondingProject);
            await context.SaveChangesAsync();

            return await GetProjectThumbnailAsync(project.ProjectId);
        }

        public async Task<Result<IReadOnlyCollection<ProjectMember>>> GetProjectMemberThumbnailsAsync(int projectId)
        {
            await using var context = Context;

            var project = context.Projects.Find(projectId);
            if (project == null) {
                return new Result<IReadOnlyCollection<ProjectMember>>
                {
                    ResultType = ResultType.NotFound,
                    Message = "Project with such an ID does not exists."
                };
            }

            if (!context.Members.Any(member => member.ProjectId == projectId && member.UserId == _userId)) {
                return new Result<IReadOnlyCollection<ProjectMember>>
                {
                    ResultType = ResultType.Forbidden,
                    Message = "Sorry, but you don't have an access to this project."
                };
            }

            var members = await context.Members
                .Include(member => member.CorrespondingUser)
                .Where(member => member.ProjectId == projectId)
                .Select(member => new ProjectMember
                {
                    UserId = member.CorrespondingUser.UserId,
                    FirstName = member.CorrespondingUser.FirstName,
                    LastName = member.CorrespondingUser.LastName,
                    EmailAddress = member.CorrespondingUser.EmailAddress,
                    Location = member.CorrespondingUser.Location,
                    JobTitle = member.CorrespondingUser.JobTitle,
                    ProjectId = member.ProjectId,
                    ProjectRole = member.Role
                }).ToListAsync();

            return new Result<IReadOnlyCollection<ProjectMember>>
            {
                ResultType = ResultType.Ok,
                Payload = members
            };
        }

        public async Task<Result<IReadOnlyCollection<TicketStatus>>> GetStatusAvaibleAsync(int projectId)
        {
            await using var context = Context;

            bool userIsProjectMember =
                context.Members.Any(member => member.ProjectId == projectId && member.UserId == _userId);

            if (!userIsProjectMember) {
                return new Result<IReadOnlyCollection<TicketStatus>>
                {
                    ResultType = ResultType.Forbidden, Message = "Don't have access to this project."
                };
            }

            Project project = await context.Projects.FindAsync(projectId);

            if (project == null) {
                return new Result<IReadOnlyCollection<TicketStatus>>
                {
                    ResultType = ResultType.NotFound, Message = "Project not found"
                };
            }

            IReadOnlyCollection<TicketStatus> statuses = await context.Statuses
                .Where(status => status.ProjectId == projectId).ToListAsync();

            return new Result<IReadOnlyCollection<TicketStatus>>{ ResultType = ResultType.Ok, Payload =  statuses };
        }

        public async Task<Result<string>> DeleteProjectAsync(int projectId)
        {
            await using var context = Context;

            bool userIsAdmin = context.Members
                .Any(m => m.ProjectId == projectId && m.UserId == _userId && m.Role == MemberRole.Owner);

            if (!userIsAdmin) {
                return new Result<string>
                {
                    ResultType = ResultType.Forbidden, Message = "Only owner has permission to delete project."
                };
            }

            Project project = await context.Projects.FindAsync(projectId);
            if (project == null) {
                return new Result<string>
                {
                    ResultType = ResultType.NotFound, Message = "Project not found"
                };
            }

            context.Projects.Remove(project);
            await context.SaveChangesAsync();

            return new Result<string>{ ResultType = ResultType.Ok, Message = "Delted" };
        }
    }
}
