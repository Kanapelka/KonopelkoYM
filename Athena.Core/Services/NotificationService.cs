using System.Linq;
using System.Threading.Tasks;
using Athena.Core.Result;
using Athena.Infrastructure;
using Athena.Infrastructure.Models;

namespace Athena.Core.Services
{
    public class NotificationService : BaseService
    {
        public NotificationService(IInfrastructureOptionsProvider optionsProvider) : base(optionsProvider)
        {
        }


        public async Task<Result<Notification>> UpdateNotification(Notification notification)
        {
            await using var context = Context;

            if (!context.Notifications.Any(n => n.NotificationId == notification.NotificationId)) {
                return new Result<Notification>{ ResultType = ResultType.NotFound, Message = "Notification not found." };
            }

            context.Notifications.Update(notification);
            await context.SaveChangesAsync();

            return new Result<Notification>{ ResultType = ResultType.Ok, Payload = notification };
        }
    }
}