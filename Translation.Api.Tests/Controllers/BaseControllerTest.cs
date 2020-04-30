using System;
using System.Collections.Generic;
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
        protected readonly SqLiteDbContext context;
        public ControllerContext ControllerContextMock { get; }

        public BaseControllerTest()
        {
            var builder = new DbContextOptionsBuilder<SqLiteDbContext>()
                .UseInMemoryDatabase("ChickenStepsDb");

            var context = new SqLiteDbContext(builder.Options);
            this.context = context;

            var userMock = Substitute.For<ClaimsPrincipal>();

            var httpContextMock = Substitute.For<HttpContext>();
            httpContextMock.User = userMock;

            ControllerContextMock = Substitute.For<ControllerContext>();
            ControllerContextMock.HttpContext = httpContextMock;
        }

        public IEnumerable<string> GetErrors(ActionResult result, string key)
        {
            if (!(result is BadRequestObjectResult badRequestResult))
            {
                throw new ArgumentException($"Must be assignable to {nameof(BadRequestObjectResult)}.", nameof(result));
            }

            var errors = (SerializableError)badRequestResult.Value;
            return (string[])errors[key];
        }
    }
}