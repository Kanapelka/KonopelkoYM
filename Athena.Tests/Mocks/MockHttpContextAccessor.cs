using Microsoft.AspNetCore.Http;

namespace Athena.Tests.Mocks
{
    public class MockHttpContextAccessor : IHttpContextAccessor
    {
        public HttpContext HttpContext { get; set; }
    }
}