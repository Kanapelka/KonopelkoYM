using System.Linq;

using Xunit;

using Athena.Core.Services;
using Athena.Infrastructure;
using Athena.Tests.Mocks;

namespace Athena.Tests
{
    public class ProjectServiceTests
    {
        private readonly int DefaultUser = 1;

        private readonly ProjectService _projectService;


        public ProjectServiceTests()
        {
            IInfrastructureOptionsProvider contextOptions = new InMemoryContextOptionsProvider();
            _projectService = new ProjectService(contextOptions, DefaultUser);
        }


        [Fact]
        public async void CreateProject_ProjectWithSpecificNameShouldBeCreated()
        {
            const string projectName = "Athena";
            await _projectService.CreateProjectAsync(projectName, DefaultUser);

            var project = _projectService.Context.Projects.FirstOrDefault(p => p.Name == projectName);
            Assert.Equal(projectName, project?.Name);
        }


    }
}