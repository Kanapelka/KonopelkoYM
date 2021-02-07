using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Athena.Core.Models;
using Athena.Core.Result;
using Athena.Infrastructure;
using Athena.Infrastructure.Models;
using Athena.Infrastructure.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace Athena.Core.Services
{
    public class UserService : BaseService
    {
        public UserService(IInfrastructureOptionsProvider optionsProvider) : base(optionsProvider)
        {
        }


        public async Task<UserProfile> GetUserProfile(int userId)
        {
            await using var context = Context;

            User user = await context.Users.FindAsync(userId);

            if (user == null) {
                return null;
            }

            return new UserProfile
            {
                UserId = user.UserId,
                EmailAddress = user.EmailAddress,
                FirstName = user.FirstName,
                LastName = user.LastName,
                IsActive = user.IsActive,
                JobTitle = user.JobTitle,
                Location = user.Location,
            };
        }

        public async Task<Result<UserProfile>> UpdateUser(UserProfile userProfile)
        {
            await using var context = Context;

            User correspondingUser = await context.Users.FindAsync(userProfile.UserId);

            if (correspondingUser == null) {
                return new Result<UserProfile>{ ResultType = ResultType.NotFound, Message = "User not found."};
            }

            correspondingUser.EmailAddress = userProfile.EmailAddress;
            correspondingUser.FirstName = userProfile.FirstName;
            correspondingUser.LastName = userProfile.LastName;
            correspondingUser.JobTitle = userProfile.JobTitle;
            correspondingUser.Location = userProfile.Location;

            context.Users.Update(correspondingUser);
            await context.SaveChangesAsync();

            return new Result<UserProfile>{ ResultType = ResultType.Ok, Payload = userProfile};
        }

        public async Task<IReadOnlyCollection<ProjectThumbnail>> GetUserProjectsAsync(int userId, int offset, int count)
        {
            await using var context = Context;

            var correspondingMembersQueryable = context.Members
                .Where(member => member.UserId == userId)
                .Select(member => member.ProjectId);

            List<Project> projects = await context.Projects
                .Where(project => correspondingMembersQueryable.Contains(project.ProjectId))
                .Include(project => project.Members)
                .ThenInclude(member => member.CorrespondingUser)
                .Skip(offset).Take(count).ToListAsync();

            List<Member> members = projects.SelectMany(project => project.Members).ToList();

            List<User> correspondingUsers = members.Select(member => member.CorrespondingUser).ToList();

            List<ProjectThumbnail> thumbnails = projects.Select(project =>
            {
                Member owner = members.First(member => member.ProjectId == project.ProjectId && member.Role == MemberRole.Owner);
                User correspindingUser = correspondingUsers.First(user => user.UserId == owner.UserId);
                return new ProjectThumbnail
                {
                    ProjectId = project.ProjectId,
                    OwnerId = correspindingUser.UserId,
                    ProjectName = project.Name,
                    OwnerName = $"{correspindingUser.FirstName} {correspindingUser.LastName}",
                    TicketsCount = context.Tickets.Count(ticket => ticket.ProjectId == project.ProjectId)
                };
            }).ToList();

            return thumbnails;
        }

        public async Task<IReadOnlyCollection<ProjectThumbnail>> GetUserProjectsByNameAsync(int userId, string projectName, int count, int offset)
        {
            await using var context = Context;

            var correspondingMembersQueryable = context.Members
                .Where(member => member.UserId == userId)
                .Select(member => member.ProjectId);

            List<Project> projects = await context.Projects
                .Where(project => correspondingMembersQueryable.Contains(project.ProjectId) && EF.Functions.Like(project.Name, $"%{projectName}%"))
                .Include(project => project.Members)
                .ThenInclude(member => member.CorrespondingUser)
                .Skip(offset).Take(count).ToListAsync();

            List<Member> members = projects.SelectMany(project => project.Members).ToList();

            List<User> correspondingUsers = members.Select(member => member.CorrespondingUser).ToList();

            List<ProjectThumbnail> thumbnails = projects.Select(project =>
            {
                Member owner = members.First(member => member.ProjectId == project.ProjectId);
                User correspindingUser = correspondingUsers.First(user => user.UserId == owner.UserId);
                return new ProjectThumbnail
                {
                    ProjectId = project.ProjectId,
                    OwnerId = correspindingUser.UserId,
                    ProjectName = project.Name,
                    OwnerName = $"{correspindingUser.FirstName} {correspindingUser.LastName}",
                    TicketsCount = context.Tickets.Count(ticket => ticket.ProjectId == project.ProjectId)
                };
            }).ToList();

            return thumbnails;
        }

        public async Task<IReadOnlyCollection<TicketThumbnail>> GetTicketsAssignedThumbnailsAsync(int userId, int count, int offset)
        {
            await using var context = Context;

            IReadOnlyCollection<TicketThumbnail> thumbnails = await context.Tickets
                .Include(ticket => ticket.Assignee)
                .Include(ticket => ticket.CorrespondingProject)
                .Include(ticket => ticket.Status)
                .Where(ticket => ticket.AssigneeId == userId)
                .OrderByDescending(ticket => ticket.CreatedDate)
                .Skip(offset)
                .Take(count)
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

            return thumbnails;
        }

        public async Task<IReadOnlyCollection<TicketThumbnail>> GetTicketsAssignedThumbnailsAsync(int userId, int projectId)
        {
            await using var context = Context;

            var thumbnails = await context.Tickets
                    .Include(ticket => ticket.Assignee)
                    .Include(ticket => ticket.CorrespondingProject)
                    .Include(ticket => ticket.Status)
                    .Where(ticket => ticket.AssigneeId == userId && ticket.ProjectId == projectId)
                    .OrderByDescending(ticket => ticket.CreatedDate)
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

            return thumbnails;
        }

        public async Task<IReadOnlyCollection<Notification>> GetNewUserNotifications(int userId, int offset, int count)
        {
            await using var context = Context;

            return await context.Notifications
                .Where(notification => notification.ExpiredDate == null && notification.RecipientId == userId)
                .OrderByDescending(notification => notification.CreatedDate)
                .Skip(offset)
                .Take(count)
                .ToListAsync();
        }

        public async Task<IReadOnlyCollection<Notification>> GetUserNotifications(int userId, int offset, int count)
        {
            await using var context = Context;

            return await context.Notifications
                .Where(notification => notification.RecipientId == userId)
                .OrderByDescending(notification => notification.CreatedDate)
                .Skip(offset).Take(count)
                .ToListAsync();
        }
    }
}