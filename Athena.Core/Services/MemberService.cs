using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Athena.Core.Result;
using Athena.Infrastructure;
using Athena.Infrastructure.Models;
using Athena.Infrastructure.Models.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Athena.Core.Services
{
    public class MemberService : BaseService
    {
        private readonly int _userId;


        public MemberService(IInfrastructureOptionsProvider optionsProvider, IHttpContextAccessor contextAccessor)
            : base(optionsProvider)
        {
            _userId = int.Parse(contextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
        }


        public async Task<Result<Member>> CreateMember(Member member)
        {
            await using var context = Context;

            Project project = await context.Projects.FindAsync(member.ProjectId);

            if (project == null) {
                return new Result<Member>{ ResultType = ResultType.NotFound, Message = "Corresponding project not found."};
            }

            await context.Members.AddAsync(member);
            await context.SaveChangesAsync();

            return new Result<Member> { ResultType = ResultType.Created, Message = "Create", Payload = member };
        }

        public async Task<Result<string>> DeleteMember(int memberId)
        {
            await using var context = Context;

            Member member = await context.Members.FindAsync(memberId);

            if (member == null) {
                return new Result<string>{ ResultType = ResultType.NotFound, Message = "Member not found."};
            }

            Project project = await context.Projects.FindAsync(member.ProjectId);

            if (project == null) {
                return new Result<string>{ ResultType = ResultType.NotFound, Message = "Corresponding project not found."};
            }

            bool userIsOwner = context.Members
                .Any(m => m.ProjectId == project.ProjectId && m.Role == MemberRole.Owner && m.UserId == _userId);

            if (!userIsOwner) {
                return new Result<string>{ ResultType = ResultType.Forbidden, Message = "You don't have permission for this."};
            }

            User notifier = await context.Users.FindAsync(_userId);
            IReadOnlyCollection<User> userToNotify = await context.Users.Where(u => u.UserId == member.UserId).ToListAsync();
            var notificationBuilder = new NotificationBuilder(notifier, userToNotify);
            IReadOnlyCollection<Notification> notifications = notificationBuilder.BuildRemoveMemberNotifications(project);

            context.Members.Remove(member);
            await context.Notifications.AddRangeAsync(notifications);
            await context.SaveChangesAsync();

            return new Result<string>{ ResultType = ResultType.Deleted, Message = "Removed" };
        }

        public async Task<Result<string>> UpdateRole(int memberId, int role)
        {
            await using var context = Context;

            Member member = await context.Members.FindAsync(memberId);

            if (member == null) {
                return new Result<string>{ ResultType = ResultType.NotFound, Message = "Member not found."};
            }

            Project project = await context.Projects.FindAsync(member.ProjectId);

            if (project == null) {
                return new Result<string>{ ResultType = ResultType.NotFound, Message = "Corresponding project not found."};
            }

            bool userIsOwner = context.Members
                .Any(m => m.ProjectId == project.ProjectId && m.Role == MemberRole.Owner && m.UserId == _userId);

            if (!userIsOwner) {
                return new Result<string>{ ResultType = ResultType.Forbidden, Message = "You don't have permission for this."};
            }

            User notifier = await context.Users.FindAsync(_userId);
            IReadOnlyCollection<User> userToNotify = await context.Users.Where(u => u.UserId == member.UserId).ToListAsync();
            var notificationBuilder = new NotificationBuilder(notifier, userToNotify);
            IReadOnlyCollection<Notification> notifications = notificationBuilder.BuildChangeRoleNotifications(project, role);

            member.Role = role;
            context.Members.Update(member);
            await context.Notifications.AddRangeAsync(notifications);
            await context.SaveChangesAsync();

            return new Result<string> { ResultType = ResultType.Ok, Message = "Changed role" };
        }
    }
}