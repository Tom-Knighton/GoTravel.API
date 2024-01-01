using GoTravel.API.Domain.Services;
using GoTravel.API.Domain.Services.Repositories;
using GoTravel.API.Services.Services;
using Moq;
using NetTopologySuite.Geometries;

namespace GoTravel.API.UnitTests.Services;

[TestFixture]
public class LineModeServiceUnitTests
{
    private Mock<ILineModeRepository> _repo;
    private Mock<IAreaRepository> _areaRepo;
    private Mock<IMapper<GLLineMode, LineModeDto>> _mapper;
    private ILineModeService _sut;

    [SetUp]
    public void SetUp()
    {
        _repo = new Mock<ILineModeRepository>();
        _areaRepo = new Mock<IAreaRepository>();
        _mapper = new Mock<IMapper<GLLineMode, LineModeDto>>();
        _sut = new LineModeService(_repo.Object, _areaRepo.Object, _mapper.Object);
    }

    [Test]
    public async Task ListAsync_ReturnsLineModesInMappedOrder_WhenNoCriteria()
    {
        // Arrange
        _repo.Setup(x => x.GetLineModes(It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<GLLineMode>
            {
                new()
                {
                    LineModeName = "1",
                    AreaId = 1,
                    PrimaryArea = new GTArea
                    {
                        AreaId = 1
                    }
                }
            });

        _mapper.Setup(x => x.Map(It.IsAny<GLLineMode>()))
            .Returns(new LineModeDto
            {
                LineModeName = "1",
                Branding = new LineModeBrandingDto
                {
                    LineModeLogoUrl = "url",
                    LineModeBackgroundColour = "bg",
                    LineModePrimaryColour = "red",
                    LineModeSecondaryColour = null
                },
                PrimaryAreaName = "Area 1",
                Lines = new List<LineDto> { new() { LineName = "1" }}
            });
        
        // Act
        var result = await _sut.ListAsync(null, null, CancellationToken.None);
        
        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(result.First().AreaName, Is.EqualTo("Area 1"));
        Assert.That(result.First().LineModes.Count, Is.EqualTo(1));
        
        _areaRepo.Verify(x => x.GetAreaFromPoint(It.IsAny<Point>(), It.IsAny<CancellationToken>()), Times.Never);
    }
    
    [Test]
    public async Task ListAsync_ReturnsLineModesInPriorityOrder()
    {
        // Arrange
        _repo.Setup(x => x.GetLineModes(It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<GLLineMode>
            {
                new()
                {
                    LineModeName = "1",
                    AreaId = 1,
                    PrimaryArea = new GTArea
                    {
                        AreaId = 1
                    }
                },
                new()
                {
                    LineModeName = "2",
                    AreaId = 2,
                    PrimaryArea = new GTArea
                    {
                        AreaId = 2
                    }
                }
            });
        
        _areaRepo.Setup(x => x.GetAreaFromPoint(It.IsAny<Point>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GTArea
            {
                AreaId = 2,
                AreaName = "Area 2"
            });

        _mapper
            .Setup(x => x.Map(It.Is<GLLineMode>(l => l.LineModeName == "1")))
            .Returns(new LineModeDto
            {
                LineModeName = "1",
                Branding = new LineModeBrandingDto
                {
                    LineModeLogoUrl = "url",
                    LineModeBackgroundColour = "bg",
                    LineModePrimaryColour = "red",
                    LineModeSecondaryColour = null
                },
                PrimaryAreaName = "Area 1",
                Lines = new List<LineDto> { new() { LineName = "1" }}
            });
        
        _mapper
            .Setup(x => x.Map(It.Is<GLLineMode>(l => l.LineModeName == "2")))
            .Returns(new LineModeDto
            {
                LineModeName = "2",
                Branding = new LineModeBrandingDto
                {
                    LineModeLogoUrl = "url",
                    LineModeBackgroundColour = "bg",
                    LineModePrimaryColour = "red",
                    LineModeSecondaryColour = null
                },
                PrimaryAreaName = "Area 2",
                Lines = new List<LineDto> { new() { LineName = "2" }}
            });
        
        // Act
        var result = await _sut.ListAsync(51.0f, -0.1f, CancellationToken.None);
        
        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count, Is.EqualTo(2));
        Assert.That(result.First().AreaName, Is.EqualTo("Area 2"));
        Assert.That(result.First().LineModes.Count, Is.EqualTo(1));
        _areaRepo.Verify(x => x.GetAreaFromPoint(It.IsAny<Point>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}