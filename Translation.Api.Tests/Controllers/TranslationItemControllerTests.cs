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
        private readonly string currentLanguage;
        private enum SupportedLanguagesEnum
        {
            en,
            sv
        }

        public TranslationItemControllerTests()
        {
            fixture = new Fixture();

            currentLanguage = fixture.Create<SupportedLanguagesEnum>().ToString();

            controller = new TranslationItemController(context)
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
                context.Database.EnsureDeleted();
                _translationItems = fixture.CreateMany<TranslationItem>();
                foreach (var translationItem in _translationItems)
                {
                    translationItem.Language = currentLanguage;
                    context.TranslationItems.Add(translationItem);
                }
                context.SaveChanges();
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
                    var result = controller.GetTranslationItems(currentLanguage).Result as OkObjectResult;

                    result.Should().NotBeNull();
                }

                [Fact]
                public void AllUsersAreReturned()
                {
                    var expectedDtos = _translationItems.Select(TranslationItemDto.Construct);

                    var result = (OkObjectResult)controller.GetTranslationItems(currentLanguage).Result;

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
                    context.Database.EnsureDeleted();

                    _translationItem = fixture.Create<TranslationItem>();
                    _translationItem.Language = currentLanguage;

                    context.TranslationItems.Add(_translationItem);
                    context.SaveChanges();

                    _translationItem = context.TranslationItems.FirstOrDefault();
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

                    var result = (OkObjectResult)controller.GetTranslationItem(currentLanguage, _translationItem.Key).Result;

                    result.Value.Should().BeEquivalentTo(expectedDto);
                }
            }

            public class TranslationItemDoesNotExist : GetTranslationItemTests
            {
                [Fact]
                public void StatusCodeIsNotFound()
                {
                    var result = controller.GetTranslationItem(currentLanguage, fixture.Create<string>()).Result as NotFoundResult;

                    result.Should().NotBeNull();
                }
            }
        }

        public class CreateTranslationItemTests : TranslationItemControllerTests
        {
            private readonly TranslationItemDto dto;

            public CreateTranslationItemTests()
            {
                dto = fixture.Create<TranslationItemDto>();
            }

            public class Always : CreateTranslationItemTests
            {
                [Fact]
                public void HttpVerbIsCorrect()
                {
                    var verb = AttributeReader.GetMethodAttribute<HttpPostAttribute>(
                        typeof(TranslationItemController),
                        nameof(TranslationItemController.CreateTranslationItem));

                    verb.Should().NotBeNull();
                }

                [Fact]
                public void RouteIsCorrect()
                {
                    var route = AttributeReader.GetMethodAttribute<HttpPostAttribute>(
                        typeof(TranslationItemController),
                        nameof(TranslationItemController.CreateTranslationItem))
                        .Template;

                    route.Should().BeNull();
                }
            }

            public class DtoIsValid : CreateTranslationItemTests
            {
                public DtoIsValid()
                {
                    context.Database.EnsureDeleted();
                }

                [Fact]
                public void StatusCodeIsCreated()
                {
                    var result = controller.CreateTranslationItem(dto, currentLanguage).Result as CreatedResult;

                    result.Should().NotBeNull();
                }

                [Fact]
                public void LocationIsReturned()
                {
                    var result = (CreatedResult)controller.CreateTranslationItem(dto, currentLanguage).Result;

                    var persistedTranslationItem = context.TranslationItems.FirstOrDefault();

                    result.Location.Should().Be($"api/{currentLanguage}/translationItems/{persistedTranslationItem?.Key}");
                }

                [Fact]
                public void TranslationItemIsReturned()
                {
                    var result = (CreatedResult)controller.CreateTranslationItem(dto, currentLanguage).Result;

                    var persistedTranslationItem = context.TranslationItems.FirstOrDefault();

                    var expectedDto = TranslationItemDto.Construct(persistedTranslationItem);

                    result.Value.Should().BeEquivalentTo(expectedDto);
                }

                [Fact]
                public void TranslationItemIsPersisted()
                {
                    var expectedTranslationItem = TranslationItemDto.Deconstruct(dto);
                    expectedTranslationItem.Language = currentLanguage;

                    controller.CreateTranslationItem(dto, currentLanguage);

                    var persistedTranslationItem = context.TranslationItems.FirstOrDefault();

                    persistedTranslationItem.Should().BeEquivalentTo(expectedTranslationItem);
                }
            }

            public class TranslationItemWithSameKeyExists : CreateTranslationItemTests
            {
                public TranslationItemWithSameKeyExists()
                {
                    var translationItem = fixture.Create<TranslationItem>();
                    translationItem.Language = currentLanguage;
                    context.TranslationItems.Add(translationItem);
                    context.SaveChanges();

                    dto.Key = translationItem.Key;
                }

                [Fact]
                public void StatusCodeIsBadRequest()
                {
                    var result = controller.CreateTranslationItem(dto, currentLanguage).Result as BadRequestObjectResult;

                    result.Should().NotBeNull();
                }

                [Fact]
                public void ErrorMessageIsReturned()
                {
                    var result = controller.CreateTranslationItem(dto, currentLanguage).Result;
                    var errors = GetErrors(result, nameof(TranslationItem.Key));

                    errors.Should().BeEquivalentTo($"{nameof(TranslationItemDto.Key)} is Duplicate.");
                }
            }
        }

        public class DeleteTests : TranslationItemControllerTests
        {
            public class Always : DeleteTests
            {
                [Fact]
                public void HttpVerbIsCorrect()
                {
                    var verb = AttributeReader.GetMethodAttribute<HttpDeleteAttribute>(
                        typeof(TranslationItemController),
                        nameof(TranslationItemController.DeleteTranslationItem));

                    verb.Should().NotBeNull();
                }

                [Fact]
                public void RouteIsCorrect()
                {
                    var route = AttributeReader.GetMethodAttribute<HttpDeleteAttribute>(
                        typeof(TranslationItemController),
                        nameof(TranslationItemController.DeleteTranslationItem))
                        .Template;

                    route.Should().Be("{key}");
                }
            }

            public class TranslationItemExists : DeleteTests
            {
                private readonly TranslationItem translationItem;

                public TranslationItemExists()
                {
                    translationItem = fixture.Create<TranslationItem>();
                    translationItem.Language = currentLanguage;
                    context.TranslationItems.Add(translationItem);
                    context.SaveChanges();
                }

                [Fact]
                public void StatusCodeIsOk()
                {
                    var result = controller.DeleteTranslationItem(currentLanguage, translationItem.Key) as OkResult;

                    result.Should().NotBeNull();
                }

                [Fact]
                public void TranslationItemIsDeleted()
                {
                    controller.DeleteTranslationItem(currentLanguage, translationItem.Key);

                    context.TranslationItems.FirstOrDefault(m => m.Key == translationItem.Key).Should().BeNull();
                }
            }

            public class TranslationItemDoesNotExist : DeleteTests
            {
                [Fact]
                public void StatusCodeIsNotFound()
                {
                    var result = controller.DeleteTranslationItem(currentLanguage, fixture.Create<string>()) as NotFoundResult;

                    result.Should().NotBeNull();
                }
            }
        }
    }
}