using GoTravel.API.Services.Services.Mappers;
using NetTopologySuite.Geometries;
using Newtonsoft.Json;

namespace GoTravel.API.UnitTests.Services.Mappers;

[TestFixture]
public class StopPontMapperUnitTests
{
    private IMapper<GLStopPoint, StopPointBaseDto> _sut;

    private GLStopPoint _modelGLStopPoint;
    
    [SetUp]
    public void SetUp()
    {
        _sut = new StopPointMapper();
        _modelGLStopPoint = new GLStopPoint
        {
            StopPointId = "123",
            StopPointName = "Test Stop",
            StopPointCoordinate = new Point(255, 555),
            StopPointType = GLStopPointType.TrainStopPoint,
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
    
    [TestCase(GLStopPointType.TrainStopPoint, typeof(TrainStopPointDto))]
    [TestCase(GLStopPointType.BusStopPoint, typeof(BusStopPointDto))]
    [TestCase(GLStopPointType.BikeStopPoint, typeof(BikeStopPointDto))]
    public void Map_MapsToCorrectType(GLStopPointType dbType, Type expectedType)
    {
        // Arrange
        var stopPoint = new GLStopPoint
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
        var stopPoint = _modelGLStopPoint;

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
        var stopPoint = _modelGLStopPoint;
        stopPoint.StopPointType = GLStopPointType.BusStopPoint;
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
        var stopPoint = _modelGLStopPoint;
        stopPoint.StopPointType = GLStopPointType.BikeStopPoint;
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