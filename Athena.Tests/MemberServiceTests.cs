using System.Linq;
using Athena.Core.Services;
using Athena.Infrastructure;
using Athena.Infrastructure.Models;
using Athena.Tests.Mocks;
using Xunit;

namespace Athena.Tests
{
    public class MemberServiceTests
    {
        private const int DefaultUser = 1;

        private readonly MemberService _memberService;


        public MemberServiceTests()
        {
            IInfrastructureOptionsProvider contextOptions = new InMemoryContextOptionsProvider();
            _memberService = new MemberService(contextOptions, DefaultUser);
        }


        [Fact]
        public async void CreateMember_ProjectMemberShouldBeCreated()
        {
            await using var context = _memberService.Context;
            var project = new Project();
            await context.Projects.AddAsync(project);
            await context.SaveChangesAsync();


            var member = new Member
            {
                UserId = DefaultUser,
                ProjectId = project.ProjectId,
                Role = 1,
            };

            await _memberService.CreateMember(member);

            int membersCount = context.Members.Count(m => m.ProjectId == project.ProjectId);
            Assert.Equal(1, membersCount);
        }
    }
}