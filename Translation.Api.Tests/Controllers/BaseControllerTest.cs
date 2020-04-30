using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Translation.Data;
using Microsoft.AspNetCore.Http;

namespace Translation.Api.Tests.Controllers
{
    public class BaseControllerTest
    {
        public readonly SqlLiteDbContext Context;
        public ControllerContext ControllerContextMock { get; }

        public BaseControllerTest()
        {
            var builder = new DbContextOptionsBuilder<SqlLiteDbContext>()
                .UseInMemoryDatabase("ChickenStepsDb");

            var context = new SqlLiteDbContext(builder.Options);
            this.Context = context;

            var userMock = Substitute.For<ClaimsPrincipal>();

            var httpContextMock = Substitute.For<HttpContext>();
            httpContextMock.User = userMock;

            ControllerContextMock = Substitute.For<ControllerContext>();
            ControllerContextMock.HttpContext = httpContextMock;
        }
    }
}