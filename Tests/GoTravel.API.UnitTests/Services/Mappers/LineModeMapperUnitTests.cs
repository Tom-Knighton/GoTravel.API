using GoTravel.API.Services.Services.Mappers;

namespace GoTravel.API.UnitTests.Services.Mappers;

[TestFixture]
public class LineModeMapperUnitTests
{
    private IMapper<GLLineMode, LineModeDto> _sut;
    
    private GLLineMode _modelGLLineMode;

    [SetUp]
    public void SetUp()
    {
        _sut = new LineModeMapper(new FlagsMapper());

        _modelGLLineMode = new GLLineMode
        {
            LineModeName = "LINE MODE 1",
            IsEnabled = true,
            Lines = new List<GLLine>
            {
                new()
                {
                    IsEnabled = true,
                    LineId = "1",
                    LineName = "LINE 1",
                    LineModeId = "LINE MODE 1"
                }
            },
            BrandingColour = "#FFFFFF",
            LogoUrl = "https://www.google.com",
            PrimaryColour = "#000000",
        };
    }

    [Test]
    public void Map_MapsAsExpected_AreaDefaultsToUK()
    {
        // Arrange
        
        // Act
        var result = _sut.Map(_modelGLLineMode);
        
        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.LineModeName, Is.EqualTo("LINE MODE 1"));
        Assert.That(result.PrimaryAreaName, Is.EqualTo("UK")); // Default as no area set
        Assert.That(result.Branding.LineModeLogoUrl, Is.EqualTo("https://www.google.com"));
        Assert.That(result.Branding.LineModePrimaryColour, Is.EqualTo("#000000"));
        Assert.That(result.Branding.LineModeBackgroundColour, Is.EqualTo("#FFFFFF"));
        Assert.That(result.Branding.LineModeSecondaryColour, Is.Null);
        Assert.That(result.Lines.Count, Is.EqualTo(1));
    }
    
    [Test]
    public void Map_MapsAsExpected_AreaWhenNotNull()
    {
        // Arrange
        _modelGLLineMode.PrimaryArea = new GTArea
        {
            AreaName = "AREA 1"
        };
        
        // Act
        var result = _sut.Map(_modelGLLineMode);
        
        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.LineModeName, Is.EqualTo("LINE MODE 1"));
        Assert.That(result.PrimaryAreaName, Is.EqualTo("AREA 1"));
        Assert.That(result.Branding.LineModeLogoUrl, Is.EqualTo("https://www.google.com"));
        Assert.That(result.Branding.LineModePrimaryColour, Is.EqualTo("#000000"));
        Assert.That(result.Branding.LineModeBackgroundColour, Is.EqualTo("#FFFFFF"));
        Assert.That(result.Branding.LineModeSecondaryColour, Is.Null);
        Assert.That(result.Lines.Count, Is.EqualTo(1));
    }
}