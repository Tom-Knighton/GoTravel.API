using GoTravel.API.Domain.Services;
using GoTravel.API.Domain.Services.Repositories;
using GoTravel.API.Services.Services;
using GoTravel.API.Services.Services.Mappers;
using GoTravel.Standard.Models.MessageModels;
using Moq;
using Is = NUnit.Framework.Is;

namespace GoTravel.API.UnitTests.Services;

[TestFixture]
public class StopPointServiceUnitTests
{
    private Mock<IStopPointRepository> _mockRepo;
    private IStopPointService _sut;

    [SetUp]
    public void SetUp()
    {
        _mockRepo = new Mock<IStopPointRepository>();
        _sut = new StopPointService(_mockRepo.Object, new StopPointMapper(new LineModeMapper(new FlagsMapper())), new StopPointUpdateMapper(), new StopPointInfoMapper());
    }

    [Test]
    public async Task GetStopPointChildren_ReturnsDeepChildrenCorrectly()
    {
        // Arrange
        var allStopPoints = new List<GLStopPoint>
        {
            new()
            {
                StopPointId = "2",
                StopPointType = GLStopPointType.TrainStopPoint,
                StopPointParentId = "1",
                StopPointName = "Child 1",
                StopPointLines = new List<GLStopPointLine>() { }
            },
            new()
            {
                StopPointId = "3",
                StopPointType = GLStopPointType.TrainStopPoint,
                StopPointParentId = "2",
                StopPointName = "Child 1-1",
                StopPointLines = new List<GLStopPointLine>() { }
            },
            new()
            {
                StopPointId = "4",
                StopPointType = GLStopPointType.TrainStopPoint,
                StopPointParentId = "1",
                StopPointName = "Child 2",
                StopPointLines = new List<GLStopPointLine>() { }
            }
        };
        
        _mockRepo.Setup(x => x.GetAllChildrenOf(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((string stopPointId, CancellationToken ct) =>
            {
                return allStopPoints.Where(s => s.StopPointParentId == stopPointId).ToList();
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
        _mockRepo.Verify(x => x.GetAllChildrenOf(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Exactly(4));
    }

    [Test]
    public async Task SearchStopPoint_FiltersOut_ByLineMode()
    {
        // Arrange
        var hiddenLineModes = new List<string> { "LINE MODE 3" };
        _mockRepo.Setup(x => x.GetStopPoints(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<GLStopPoint>()
            {
                new()
                {
                    StopPointId = "1",
                    StopPointType = GLStopPointType.TrainStopPoint,
                    StopPointName = "Stop 001",
                    StopPointLines = new List<GLStopPointLine>
                    {
                        new()
                        {
                            StopPointId = "1",
                            IsEnabled = true,
                            LineId = "Line 1",
                            Line = new()
                            {
                                IsEnabled = true,
                                LineId = "1",
                                LineMode = new()
                                {
                                    IsEnabled = true,
                                    LineModeName = "LINE MODE 3"
                                }
                            }
                        },
                        new()
                        {
                            StopPointId = "1",
                            IsEnabled = true,
                            LineId = "Line 2",
                            Line = new()
                            {
                                IsEnabled = true,
                                LineId = "2",
                                LineMode = new()
                                {
                                    IsEnabled = true,
                                    LineModeName = "LINE MODE 2"
                                }
                            }
                        }
                    }
                }
            });

        _mockRepo.Setup(x => x.GetAllChildrenOf(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<GLStopPoint>());
        
        // Act
        var result = await _sut.GetStopPointsByNameAsync("Stop", hiddenLineModes);
        
        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(result.First().LineModes.Count, Is.EqualTo(1));
    }
}