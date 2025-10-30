using AdventureWorks.Models.Person;
using AdventureWorks.Repositories;
using AdventureWorks.Services;
using Moq;

namespace AdventureWorks.Tests.Services;

public class PersonServiceTests
{
    private readonly Mock<IPersonRepository> _mockRepository;
    private readonly PersonService _service;

    public PersonServiceTests()
    {
        _mockRepository = new Mock<IPersonRepository>();
        _service = new PersonService(_mockRepository.Object);
    }

    [Fact]
    public async Task GetPersonsAsync_ReturnsCorrectPaginatedList()
    {
        // Arrange
        var query = new PersonQuery
        {
            PageNumber = 2,
            PageSize = 10
        };

        var persons = new List<Person>
        {
            new Person
            {
                BusinessEntityID = 1,
                FirstName = "John",
                LastName = "Doe",
                PersonType = "EM"
            }
        };

        int totalCount = 15;
        int expectedTotalPages = (int)Math.Ceiling((double)totalCount / query.PageSize);

        _mockRepository.Setup(r => r.GetTotalCount(query))
            .ReturnsAsync(totalCount);
        _mockRepository.Setup(r => r.GetPersonsAsync(query))
            .ReturnsAsync(persons);

        // Act
        var result = await _service.GetPersonsAsync(query);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Data);
        Assert.NotNull(result.MetaData);
        Assert.Single(result.Data);
        Assert.Equal(totalCount, result.MetaData.Total);
        Assert.Equal(query.PageNumber, result.MetaData.PageIndex);
        Assert.Equal(query.PageSize, result.MetaData.PageSize);
        Assert.Equal(expectedTotalPages, result.MetaData.PageCount); // 15 items with page size 10 = 2 pages

        _mockRepository.Verify(r => r.GetTotalCount(query), Times.Once);
        _mockRepository.Verify(r => r.GetPersonsAsync(query), Times.Once);
    }

    [Fact]
    public async Task GetPersonsAsync_WithEmptyResult_ReturnsEmptyList()
    {
        // Arrange
        var query = new PersonQuery
        {
            PageNumber = 1,
            PageSize = 10
        };

        _mockRepository.Setup(r => r.GetTotalCount(query))
            .ReturnsAsync(0);
        _mockRepository.Setup(r => r.GetPersonsAsync(query))
            .ReturnsAsync(new List<Person>());

        // Act
        var result = await _service.GetPersonsAsync(query);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.Data);
        Assert.Equal(0, result.MetaData.Total);
        Assert.Equal(1, result.MetaData.PageIndex);
        Assert.Equal(10, result.MetaData.PageSize);
        Assert.Equal(0, result.MetaData.Total);
    }

    [Theory]
    [InlineData(10, 10, 1)]
    [InlineData(11, 10, 2)]
    [InlineData(20, 10, 2)]
    [InlineData(21, 10, 3)]
    public async Task GetPersonsAsync_CalculatesCorrectTotalPages(int totalCount, int pageSize, int expectedTotalPages)
    {
        // Arrange
        var query = new PersonQuery
        {
            PageNumber = 1,
            PageSize = pageSize
        };

        var persons = new List<Person>
        {
            new Person
            {
                BusinessEntityID = 1,
                FirstName = "John",
                LastName = "Doe",
                PersonType = "EM"
            }
        };

        _mockRepository.Setup(r => r.GetTotalCount(query))
            .ReturnsAsync(totalCount);
        _mockRepository.Setup(r => r.GetPersonsAsync(query))
            .ReturnsAsync(persons);

        // Act
        var result = await _service.GetPersonsAsync(query);

        // Assert
        Assert.Equal(expectedTotalPages, result.MetaData.PageCount);
    }
}