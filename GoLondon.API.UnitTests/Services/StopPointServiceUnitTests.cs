using GoLondon.API.Domain.Data;
using GoLondon.API.Domain.Services;
using GoLondon.API.Services.Services;
using GoLondon.API.Services.Services.Mappers;
using Moq;
using Moq.EntityFrameworkCore;

namespace GoLondon.API.UnitTests.Services;

[TestFixture]
public class StopPointServiceUnitTests
{
    private Mock<GoLondonContext> _mockContext;
    private IStopPointService _sut;

    [SetUp]
    public void SetUp()
    {
        _mockContext = new Mock<GoLondonContext>();
        _sut = new StopPointService(_mockContext.Object, new StopPointMapper());
    }

    [Test]
    public async Task GetStopPointChildren_ReturnsDeepChildrenCorrectly()
    {
        // Arrange
        _mockContext.Setup(x => x.StopPoints)
            .ReturnsDbSet(new List<GLStopPoint>
            {
                new()
                {
                    StopPointId = "2",
                    StopPointType = GLStopPointType.TrainStopPoint,
                    StopPointParentId = "1",
                    StopPointName = "Child 1",
                    StopPointLines = new List<GLStopPointLine>() {}
                },
                new()
                {
                    StopPointId = "3",
                    StopPointType = GLStopPointType.TrainStopPoint,
                    StopPointParentId = "2",
                    StopPointName = "Child 1-1",
                    StopPointLines = new List<GLStopPointLine>() {}
                },
                new()
                {
                    StopPointId = "4",
                    StopPointType = GLStopPointType.TrainStopPoint,
                    StopPointParentId = "1",
                    StopPointName = "Child 2",
                    StopPointLines = new List<GLStopPointLine>() {}
                }
            });

        var parent = new StopPointBaseDto
        {
            StopPointId = "1"
        };
        
        // Act
        var children = await _sut.GetStopPointChildrenAsync(parent);
        
        // Assert
        Assert.That(children, Is.Not.Empty);
        Assert.That(children.Count, Is.EqualTo(2));
        Assert.That(children.First().Children.Count, Is.EqualTo(1));
        Assert.That(children.ElementAt(1).Children.Count, Is.EqualTo(0));
    }

    [TestCase("Test", 4)]
    public async Task SearchByName_CorrectlySearches(string query, int expectedCount)
    {
        // Arrange
        _mockContext.Setup(x => x.StopPoints)
            .ReturnsDbSet(new List<GLStopPoint>
            {
                new GLStopPoint
                {
                    StopPointId = "1",
                    StopPointType = GLStopPointType.TrainStopPoint,
                    StopPointName = "Test Stop 1",
                },
                new GLStopPoint
                {
                    StopPointId = "2",
                    StopPointType = GLStopPointType.TrainStopPoint,
                    StopPointName = "tesTinG",
                },
                new GLStopPoint
                {
                    StopPointId = "3",
                    StopPointType = GLStopPointType.TrainStopPoint,
                    StopPointName = "Test Stop 2",
                },
                new GLStopPoint
                {
                    StopPointId = "4",
                    StopPointType = GLStopPointType.TrainStopPoint,
                    StopPointName = "Stratford Test 3",
                },
                new GLStopPoint
                {
                    StopPointId = "5",
                    StopPointType = GLStopPointType.TrainStopPoint,
                    StopPointName = "XXX Place",
                },
            });
        
        // Act
        var results = await _sut.GetStopPointsByNameAsync(query);
        
        // Assert
        Assert.That(results, Is.Not.Empty);
        Assert.That(results.Count, Is.EqualTo(expectedCount));
    }

    [TestCase(0, true)]
    [TestCase(1, true)]
    [TestCase(25, false)]
    public async Task SearchByName_LimitsToMaxResults(int maxResults, bool shouldBeEqual)
    {
        // Arrange
        _mockContext.Setup(x => x.StopPoints)
            .ReturnsDbSet(new List<GLStopPoint>
            {
                new GLStopPoint
                {
                    StopPointId = "1",
                    StopPointType = GLStopPointType.TrainStopPoint,
                    StopPointName = "Test Stop 1",
                },
                new GLStopPoint
                {
                    StopPointId = "2",
                    StopPointType = GLStopPointType.TrainStopPoint,
                    StopPointName = "tesTinG",
                },
                new GLStopPoint
                {
                    StopPointId = "3",
                    StopPointType = GLStopPointType.TrainStopPoint,
                    StopPointName = "Test Stop 2",
                },
                new GLStopPoint
                {
                    StopPointId = "4",
                    StopPointType = GLStopPointType.TrainStopPoint,
                    StopPointName = "Stratford Test 3",
                },
                new GLStopPoint
                {
                    StopPointId = "5",
                    StopPointType = GLStopPointType.TrainStopPoint,
                    StopPointName = "XXX Place",
                },
            });
        
        // Act
        var results = await _sut.GetStopPointsByNameAsync("test", maxResults);
        
        // Assert
        Assert.That(results.Count, shouldBeEqual ? Is.EqualTo(maxResults) : Is.LessThanOrEqualTo(maxResults));
    }
}