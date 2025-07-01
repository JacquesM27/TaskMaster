using System.Text.Json;
using Shouldly;
using TaskMaster.OpenAi.Services;

namespace TaskMaster.OpenAi.Tests.Unit.Services;

public sealed class ObjectSamplerServiceTests
{
    private readonly ObjectSamplerService _objectSamplerService;

    public ObjectSamplerServiceTests()
    {
        _objectSamplerService = new ObjectSamplerService();
    }

    #region GetSampleJson

    [Fact]
    public void GetSampleJson_ShouldReturnValidJson_WhenSimpleTypeProvided()
    {
        // Act
        var result = _objectSamplerService.GetSampleJson(typeof(TestSimpleClass));

        // Assert
        result.ShouldNotBeNullOrWhiteSpace();
        
        // Verify it's valid JSON
        var deserializedObject = JsonSerializer.Deserialize<TestSimpleClass>(result);
        deserializedObject.ShouldNotBeNull();
    }

    [Fact]
    public void GetSampleJson_ShouldReturnCorrectStructure_WhenComplexTypeProvided()
    {
        // Act
        var result = _objectSamplerService.GetSampleJson(typeof(TestComplexClass));

        // Assert
        result.ShouldNotBeNullOrWhiteSpace();
        result.ShouldContain("StringProperty");
        result.ShouldContain("IntProperty");
        result.ShouldContain("BoolProperty");
        result.ShouldContain("ListProperty");
        
        // Verify it's valid JSON
        var deserializedObject = JsonSerializer.Deserialize<TestComplexClass>(result);
        deserializedObject.ShouldNotBeNull();
        deserializedObject.StringProperty.ShouldBe("string");
        deserializedObject.IntProperty.ShouldBe(0);
        deserializedObject.BoolProperty.ShouldBe(false);
        deserializedObject.ListProperty.ShouldNotBeNull();
        deserializedObject.ListProperty.Count.ShouldBe(1);
    }

    [Fact]
    public void GetSampleJson_ShouldReturnIndentedJson_WhenCalled()
    {
        // Act
        var result = _objectSamplerService.GetSampleJson(typeof(TestSimpleClass));

        // Assert
        result.ShouldContain("  "); // Should contain indentation
        result.ShouldContain("\n"); // Should contain newlines
    }

    [Fact]
    public void GetSampleJson_ShouldHandleNestedObjects_WhenNestedTypeProvided()
    {
        // Act
        var result = _objectSamplerService.GetSampleJson(typeof(TestNestedClass));

        // Assert
        result.ShouldNotBeNullOrWhiteSpace();
        result.ShouldContain("NestedProperty");
        result.ShouldContain("StringProperty");
        
        // Verify it's valid JSON
        var deserializedObject = JsonSerializer.Deserialize<TestNestedClass>(result);
        deserializedObject.ShouldNotBeNull();
        deserializedObject.NestedProperty.ShouldNotBeNull();
        deserializedObject.NestedProperty.StringProperty.ShouldBe("string");
    }

    [Fact]
    public void GetSampleJson_ShouldHandleGenericList_WhenListTypeProvided()
    {
        // Act
        var result = _objectSamplerService.GetSampleJson(typeof(List<string>));

        // Assert
        result.ShouldNotBeNullOrWhiteSpace();
        result.ShouldContain("[");
        result.ShouldContain("]");
        result.ShouldContain("string");
        
        // Verify it's valid JSON
        var deserializedObject = JsonSerializer.Deserialize<List<string>>(result);
        deserializedObject.ShouldNotBeNull();
        deserializedObject.Count.ShouldBe(1);
        deserializedObject[0].ShouldBe("string");
    }

    #endregion

    #region GetStringValues

    [Fact]
    public void GetStringValues_ShouldReturnAllStringProperties_WhenObjectWithStringPropertiesProvided()
    {
        // Arrange
        var testObject = new TestStringPropertiesClass
        {
            StringProperty1 = "First string",
            StringProperty2 = "Second string",
            IntProperty = 42,
            StringProperty3 = "Third string"
        };

        // Act
        var result = _objectSamplerService.GetStringValues(testObject);

        // Assert
        result.ShouldNotBeNullOrWhiteSpace();
        result.ShouldContain("First string");
        result.ShouldContain("Second string");
        result.ShouldContain("Third string");
        result.ShouldNotContain("42");
    }

    [Fact]
    public void GetStringValues_ShouldIgnoreNullOrEmptyStrings_WhenObjectHasEmptyStrings()
    {
        // Arrange
        var testObject = new TestStringPropertiesClass
        {
            StringProperty1 = "Valid string",
            StringProperty2 = "",
            StringProperty3 = null,
            IntProperty = 42
        };

        // Act
        var result = _objectSamplerService.GetStringValues(testObject);

        // Assert
        result.ShouldContain("Valid string");
        result.ShouldNotContain("StringProperty2");
        result.ShouldNotContain("StringProperty3");
    }

    [Fact]
    public void GetStringValues_ShouldIgnoreWhitespaceOnlyStrings_WhenObjectHasWhitespaceStrings()
    {
        // Arrange
        var testObject = new TestStringPropertiesClass
        {
            StringProperty1 = "Valid string",
            StringProperty2 = "   ",
            StringProperty3 = "\t\n",
            IntProperty = 42
        };

        // Act
        var result = _objectSamplerService.GetStringValues(testObject);

        // Assert
        result.ShouldContain("Valid string");
        result.ShouldNotContain("   ");
        result.ShouldNotContain("\t\n");
    }

    [Fact]
    public void GetStringValues_ShouldReturnEmptyString_WhenObjectHasNoValidStringProperties()
    {
        // Arrange
        var testObject = new TestStringPropertiesClass
        {
            StringProperty1 = "",
            StringProperty2 = null,
            StringProperty3 = "   ",
            IntProperty = 42
        };

        // Act
        var result = _objectSamplerService.GetStringValues(testObject);

        // Assert
        result.ShouldBeEmpty();
    }

    [Fact]
    public void GetStringValues_ShouldThrowArgumentNullException_WhenNullObjectProvided()
    {
        // Act & Assert
        Should.Throw<ArgumentNullException>(() => _objectSamplerService.GetStringValues(null));
    }

    [Fact]
    public void GetStringValues_ShouldReturnEmptyString_WhenObjectHasNoStringProperties()
    {
        // Arrange
        var testObject = new TestNoStringPropertiesClass
        {
            IntProperty = 42,
            BoolProperty = true,
            DoubleProperty = 3.14
        };

        // Act
        var result = _objectSamplerService.GetStringValues(testObject);

        // Assert
        result.ShouldBeEmpty();
    }

    #endregion

    #region Test Classes

    public class TestSimpleClass
    {
        public string StringProperty { get; set; } = string.Empty;
        public int IntProperty { get; set; }
        public bool BoolProperty { get; set; }
    }

    public class TestComplexClass
    {
        public string StringProperty { get; set; } = string.Empty;
        public int IntProperty { get; set; }
        public bool BoolProperty { get; set; }
        public List<string> ListProperty { get; set; } = new();
    }

    public class TestNestedClass
    {
        public TestSimpleClass NestedProperty { get; set; } = new();
        public string Name { get; set; } = string.Empty;
    }

    public class TestStringPropertiesClass
    {
        public string StringProperty1 { get; set; } = string.Empty;
        public string StringProperty2 { get; set; } = string.Empty;
        public string StringProperty3 { get; set; } = string.Empty;
        public int IntProperty { get; set; }
    }

    public class TestNoStringPropertiesClass
    {
        public int IntProperty { get; set; }
        public bool BoolProperty { get; set; }
        public double DoubleProperty { get; set; }
    }

    #endregion
}