using AdventureWorks.Controllers;
using AdventureWorks.Models.Person;
using AdventureWorks.Models.Shared;
using AdventureWorks.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net;

namespace AdventureWorks_Tests.Controllers
{
    public class PersonControllerTests
    {
        private readonly PersonDto person1;
        private readonly PersonDto person2;
        private readonly MetaData metaData;
        private readonly List<PersonDto> persons;
        private readonly PersonQuery query;

        public PersonControllerTests()
        {
            person1 = new() { BusinessEntityId = 1, FirstName = "Test1", LastName = "Test2" };
            person2 = new() { BusinessEntityId = 2, FirstName = "Test3", LastName = "Test4" };
            metaData = new(30, 1, 3, 10);
            persons = new();
            query = new();
        }

        [Fact]
        public async Task GetPersonsAsync_NoData_Returns_NotFound()
        {
            // Arrange
            var logger = new Mock<ILogger<PersonController>>();
            var personService = new Mock<IPersonService>();
            var controller = new PersonController(logger.Object, personService.Object);

            PaginatedList<PersonDto> list = new() { Data = persons, MetaData = metaData };
            personService.Setup(service => service.GetPersonsAsync(query)).ReturnsAsync(list);

            // Act
            var result = await controller.GetPersonsAsync(query);

            // Assert
            Assert.NotNull(result);
            Assert.True(result is NotFoundResult);
        }

        [Fact]
        public async Task GetPersonsAsync_WithData_Returns_OK()
        {
            // Arrange
            var logger = new Mock<ILogger<PersonController>>();
            var personService = new Mock<IPersonService>();
            var controller = new PersonController(logger.Object, personService.Object);
            persons.Add(person1);
            persons.Add(person2);

            PaginatedList<PersonDto> list = new() { Data = persons, MetaData = metaData };
            personService.Setup(service => service.GetPersonsAsync(query)).ReturnsAsync(list);

            // Act
            var result = await controller.GetPersonsAsync(query);

            // Assert
            Assert.NotNull(result);
            Assert.True(result is OkObjectResult);
            Assert.True((result as OkObjectResult).Value is PaginatedList<PersonDto>);
            Assert.NotEmpty(((result as OkObjectResult).Value as PaginatedList<PersonDto>).Data);
            Assert.True(((result as OkObjectResult).Value as PaginatedList<PersonDto>).Data.Count == 2);
            Assert.True(((result as OkObjectResult).Value as PaginatedList<PersonDto>).MetaData.Total == 30);
        }

        [Fact]
        public async Task GetPersonsAsyncException_Returns_Problem()
        {
            // Arrange
            var logger = new Mock<ILogger<PersonController>>();
            var personService = new Mock<IPersonService>();


            PaginatedList<PersonDto> list = new() { Data = null, MetaData = metaData };
            personService.Setup(service => service.GetPersonsAsync(query)).ReturnsAsync(list);

            var controller = new PersonController(logger.Object, personService.Object);

            // Act
            var result = await controller.GetPersonsAsync(query);

            // Assert
            Assert.NotNull(result);
            Assert.True(result is ObjectResult);
            Assert.True((result as ObjectResult).StatusCode.GetValueOrDefault() == (int)HttpStatusCode.InternalServerError);

        }

        [Fact]
        public async Task GetPersonByIdAsync_NoData_Returns_NotFound()
        {
            // Arrange
            var logger = new Mock<ILogger<PersonController>>();
            var personService = new Mock<IPersonService>();
            personService.Setup(service => service.GetPersonAsync(It.IsAny<int>(), false)).ReturnsAsync(() => null);
            var controller = new PersonController(logger.Object, personService.Object);

            // Act
            var result = await controller.GetPersonByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.True(result is NotFoundResult);
        }

        [Fact]
        public async Task GetPersonByIdAsync_WithData_Returns_OK()
        {
            // Arrange
            var logger = new Mock<ILogger<PersonController>>();
            var personService = new Mock<IPersonService>();
            personService.Setup(service => service.GetPersonAsync(It.IsAny<int>(), true)).ReturnsAsync(person1);
            var controller = new PersonController(logger.Object, personService.Object);

            // Act
            var result = await controller.GetPersonByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.True(result is OkObjectResult);
            Assert.True((result as OkObjectResult).Value is PersonDto);
            Assert.True(((result as OkObjectResult).Value as PersonDto).BusinessEntityId == 1);
        }
    }
}
