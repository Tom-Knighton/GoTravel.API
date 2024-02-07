using GoTravel.API.Services.Services.Mappers;
using Moq;
using NetTopologySuite.Geometries;

namespace GoTravel.API.UnitTests.Services.Mappers;

[TestFixture]
public class StopPointMapperUnitTests
{
    private Mock<IMapper<GLLineMode, LineModeDto>> _mockLineModeMapper;
    private IMapper<GTStopPoint, StopPointBaseDto> _sut;

    private GTStopPoint _modelGtStopPoint;
    
    [SetUp]
    public void SetUp()
    {
        _mockLineModeMapper = new Mock<IMapper<GLLineMode, LineModeDto>>();
        _sut = new StopPointMapper(_mockLineModeMapper.Object);
        _modelGtStopPoint = new GTStopPoint
        {
            StopPointId = "123",
            StopPointName = "Test Stop",
            StopPointCoordinate = new Point(255, 555),
            StopPointType = GTStopPointType.TrainStopPoint,
            StopPointLines = new List<GLStopPointLine>
            {
                new()
                {
                    Line = new GLLine
                    {
                        LineName = "Test Line",
                        LineId = "789",
                        LineMode = new GLLineMode
                        {
                            LineModeName = "Test Mode"
                        }
                    }
                }
            }
        };
    }
    
    [TestCase(GTStopPointType.TrainStopPoint, typeof(TrainStopPointDto))]
    [TestCase(GTStopPointType.BusStopPoint, typeof(BusStopPointDto))]
    [TestCase(GTStopPointType.BikeStopPoint, typeof(BikeStopPointDto))]
    public void Map_MapsToCorrectType(GTStopPointType dbType, Type expectedType)
    {
        // Arrange
        var stopPoint = new GTStopPoint
        {
            StopPointType = dbType
        };
        
        // Act
        var result = _sut.Map(stopPoint);
        
        // Assert
        Assert.That(result, Is.InstanceOf(expectedType));
    }

    [Test]
    public void Map_MapsDefaultExpectedProperties()
    {
        // Arrange
        var stopPoint = _modelGtStopPoint;

        // Act
        var result = _sut.Map(stopPoint);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.StopPointId, Is.EqualTo("123"));
        Assert.That(result.StopPointName, Is.EqualTo("Test Stop"));
        Assert.That(result.StopPointCoordinate.X, Is.EqualTo(255));
        Assert.That(result.StopPointCoordinate.Y, Is.EqualTo(555));
        Assert.That(result.StopPointType, Is.EqualTo(StopPointType.Train));
        Assert.That(result, Is.InstanceOf(typeof(TrainStopPointDto)));
    }

    [Test]
    public void Map_MapsBusProperties()
    {
        // Arrange
        var stopPoint = _modelGtStopPoint;
        stopPoint.StopPointType = GTStopPointType.BusStopPoint;
        stopPoint.BusStopIndicator = "X";
        stopPoint.BusStopLetter = "Y";
        stopPoint.BusStopSMSCode = "XYXYXY";
        
        // Act
        var result = _sut.Map(stopPoint) as BusStopPointDto;

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.InstanceOf(typeof(BusStopPointDto)));
        Assert.That(result.StopPointType, Is.EqualTo(StopPointType.Bus));
        Assert.That(result.StopPointId, Is.EqualTo("123"));
        Assert.That(result.StopPointName, Is.EqualTo("Test Stop"));
        Assert.That(result.BusStopIndicator, Is.EqualTo("X"));
        Assert.That(result.BusStopLetter, Is.EqualTo("Y"));
        Assert.That(result.BusStopSMSCode, Is.EqualTo("XYXYXY"));
    }
    
    [Test]
    public void Map_MapsBikeProperties()
    {
        // Arrange
        var stopPoint = _modelGtStopPoint;
        stopPoint.StopPointType = GTStopPointType.BikeStopPoint;
        stopPoint.BikesAvailable = 10;
        stopPoint.EBikesAvailable = 20;
        
        // Act
        var result = _sut.Map(stopPoint) as BikeStopPointDto;

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.InstanceOf(typeof(BikeStopPointDto)));
        Assert.That(result.StopPointType, Is.EqualTo(StopPointType.Bike));
        Assert.That(result.StopPointId, Is.EqualTo("123"));
        Assert.That(result.StopPointName, Is.EqualTo("Test Stop"));
        Assert.That(result.BikesRemaining, Is.EqualTo(10));
        Assert.That(result.EBikesRemaining, Is.EqualTo(20));
    }
}