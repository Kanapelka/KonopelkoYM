using System;
using System.Linq;
using System.Threading.Tasks;
using Athena.Core.Models;
using Athena.Core.Result;
using Athena.Infrastructure;
using Athena.Infrastructure.Models;

namespace Athena.Core.Services
{
    public class AuthorizationService : BaseService
    {
        public AuthorizationService(IInfrastructureOptionsProvider optionsProvider) : base(optionsProvider)
        {
        }


        public async Task<Result<int>> AuthorizeAsync(User user)
        {
            await using var context = Context;

            if (context.Users.Any(u => u.EmailAddress == user.EmailAddress)) {
                return new Result<int>
                {
                    ResultType = ResultType.Bad,
                    Message = "User with such a credentials already exists"
                };
            }

            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();

            return new Result<int>{ ResultType = ResultType.Ok, Payload = user.UserId };
        }

        public async Task<Result<int>> AuthenticateAsync(string emailAddress, string password)
        {
            await using var context = Context;

            var user = context.Users.FirstOrDefault(u => u.EmailAddress == emailAddress && u.Password == password);

            return user == null
                ? new Result<int>{ ResultType = ResultType.NotFound, Message = "There is no such a user!"}
                : new Result<int>{ ResultType = ResultType.Ok, Payload = user.UserId, Message= "Found." };
        }

        public async Task<int> AuthorizeWithGoogle(UserProfile userProfile)
        {
            await using var context = Context;

            var user = context.Users.FirstOrDefault(u => u.EmailAddress == userProfile.EmailAddress);
            if (user == null) {
                var newUser = new User
                {
                    EmailAddress = userProfile.EmailAddress,
                    FirstName = userProfile.FirstName,
                    LastName = userProfile.LastName,
                    IsActive = true,
                    Password = Guid.NewGuid().ToString()
                };
                await context.Users.AddAsync(newUser);
                await context.SaveChangesAsync();
                return newUser.UserId;
            }

            return user.UserId;
        }
    }
}