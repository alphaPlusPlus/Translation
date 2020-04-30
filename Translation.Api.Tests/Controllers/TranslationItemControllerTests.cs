using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Translation.Api.Controllers;
using Translation.Api.Models;
using Translation.Api.Tests.TestHelpers;
using Translation.Data.Entities;
using Xunit;

namespace Translation.Api.Tests.Controllers
{
    public class TranslationItemControllerTests : BaseControllerTest
    {
        private readonly IFixture fixture;
        private readonly TranslationItemController controller;
        private readonly string _language;

        public TranslationItemControllerTests()
        {
            fixture = new Fixture();

            _language = "en";

            controller = new TranslationItemController(Context)
            {
                ControllerContext = ControllerContextMock
            };
        }

        public class AttributeTests : TranslationItemControllerTests
        {
            [Fact]
            public void RouteIsCorrect()
            {
                var route = AttributeReader.GetClassAttribute<RouteAttribute>(typeof(TranslationItemController))
                    .Template;

                route.Should().Be("api/{language:language}/translationitems");
            }
        }

        public class GetTranslationItemsTests : TranslationItemControllerTests
        {
            private readonly IEnumerable<TranslationItem> _translationItems;

            public GetTranslationItemsTests()
            {
                Context.Database.EnsureDeleted();
                _translationItems = fixture.CreateMany<TranslationItem>();
                foreach (var translationItem in _translationItems)
                {
                    translationItem.Language = _language;
                    Context.TranslationItems.Add(translationItem);
                }
                Context.SaveChanges();
            }

            public class Always : GetTranslationItemsTests
            {
                [Fact]
                public void HttpVerbIsCorrect()
                {
                    var verb = AttributeReader.GetMethodAttribute<HttpGetAttribute>(
                        typeof(TranslationItemController),
                        nameof(TranslationItemController.GetTranslationItems));

                    verb.Should().NotBeNull();
                }

                [Fact]
                public void RouteIsCorrect()
                {
                    var route = AttributeReader.GetMethodAttribute<HttpGetAttribute>(
                            typeof(TranslationItemController),
                            nameof(TranslationItemController.GetTranslationItems))
                        .Template;

                    route.Should().BeNull();
                }

                [Fact]
                public void StatusCodeIsOk()
                {
                    var result = controller.GetTranslationItems(_language).Result as OkObjectResult;

                    result.Should().NotBeNull();
                }

                [Fact]
                public void AllUsersAreReturned()
                {
                    var expectedDtos = _translationItems.Select(TranslationItemDto.Construct);

                    var result = (OkObjectResult)controller.GetTranslationItems(_language).Result;

                    result.Value.Should().BeEquivalentTo(expectedDtos);
                }
            }
        }

        public class GetTranslationItemTests : TranslationItemControllerTests
        {
            public class Always : GetTranslationItemTests
            {
                [Fact]
                public void HttpVerbIsCorrect()
                {
                    var verb = AttributeReader.GetMethodAttribute<HttpGetAttribute>(
                        typeof(TranslationItemController),
                        nameof(TranslationItemController.GetTranslationItem));

                    verb.Should().NotBeNull();
                }

                [Fact]
                public void RouteIsCorrect()
                {
                    var route = AttributeReader.GetMethodAttribute<HttpGetAttribute>(
                            typeof(TranslationItemController),
                            nameof(TranslationItemController.GetTranslationItem))
                        .Template;

                    route.Should().Be("{key}");
                }
            }

            public class TranslationItemExists : GetTranslationItemTests
            {
                private readonly TranslationItem _translationItem;

                public TranslationItemExists()
                {
                    Context.Database.EnsureDeleted();

                    _translationItem = fixture.Create<TranslationItem>();
                    _translationItem.Language = _language;

                    Context.TranslationItems.Add(_translationItem);
                    Context.SaveChanges();

                    _translationItem = Context.TranslationItems.FirstOrDefault();
                }

                [Fact]
                public void StatusCodeIsOk()
                {
                    var result = controller.GetTranslationItem(_translationItem.Language, _translationItem.Key).Result as OkObjectResult;

                    result.Should().NotBeNull();
                }

                [Fact]
                public void UserIsReturned()
                {
                    var expectedDto = TranslationItemDto.Construct(_translationItem);

                    var result = (OkObjectResult)controller.GetTranslationItem(_language, _translationItem.Key).Result;

                    result.Value.Should().BeEquivalentTo(expectedDto);
                }
            }

            public class TranslationItemDoesNotExist : GetTranslationItemTests
            {
                [Fact]
                public void StatusCodeIsNotFound()
                {
                    var result = controller.GetTranslationItem(_language, fixture.Create<string>()).Result as NotFoundResult;

                    result.Should().NotBeNull();
                }
            }
        }
    }
}