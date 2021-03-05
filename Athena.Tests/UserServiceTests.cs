using Athena.Core.Models;
using Athena.Core.Services;
using Athena.Infrastructure;
using Athena.Infrastructure.Models;
using Athena.Tests.Mocks;
using Xunit;

namespace Athena.Tests
{
    public class UserServiceTests
    {
        private readonly UserService _userService;


        public UserServiceTests()
        {
            IInfrastructureOptionsProvider contextOptions = new InMemoryContextOptionsProvider();
            _userService = new UserService(contextOptions);
        }


        [Fact]
        public async void UpdateUser_UserShouldBeUpdated()
        {
            await using var context = _userService.Context;
            var user = new User();
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();

            var profile = new UserProfile
            {
                UserId = user.UserId,
                FirstName = "Yana",
                LastName = "Konopelko",
                EmailAddress = "y.konopelko@gmail.com",
            };

            var updatedUserResult = await _userService.UpdateUser(profile);
            var updatedUser = updatedUserResult.Payload;

            Assert.Equal("y.konopelko@gmail.com", updatedUser.EmailAddress);
        }
    }
}