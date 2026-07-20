using System.Runtime.Serialization;

namespace Pg.DataverseSync.Engine.Application.Tests
{
    public class JsonFormatServiceTests
    {
        [Fact]
        public void DeserializeJsonString_ValidJson_ReturnsDeserializedObject()
        {
            // Arrange
            var json = "{\"Id\":1,\"Name\":\"Alice\"}";

            // Act
            var result = JsonFormatService.DeserializeJsonString<TestDto>(json);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("Alice", result.Name);
        }

        [Fact]
        public void DeserializeJsonString_ValidJsonWithMissingOptionalField_ReturnsObjectWithDefaultValue()
        {
            // Arrange
            var json = "{\"Id\":42}";

            // Act
            var result = JsonFormatService.DeserializeJsonString<TestDto>(json);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(42, result.Id);
            Assert.Null(result.Name);
        }

        [Fact]
        public void DeserializeJsonString_InvalidJsonValueType_ThrowsSerializationException()
        {
            // Arrange
            var invalidJson = "{\"Id\":\"abc\",\"Name\":\"Alice\"}";

            // Act & Assert
            Assert.Throws<SerializationException>(() => JsonFormatService.DeserializeJsonString<TestDto>(invalidJson));
        }

        [DataContract]
        private class TestDto
        {
            [DataMember]
            public int Id { get; set; }

            [DataMember]
            public string? Name { get; set; }
        }
    }
}
