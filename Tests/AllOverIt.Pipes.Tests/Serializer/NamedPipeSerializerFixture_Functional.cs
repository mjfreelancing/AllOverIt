using AllOverIt.Fixture;
using AllOverIt.Fixture.Extensions;
using AllOverIt.Pipes.Named.Serialization;
using AllOverIt.Serialization.Binary.Readers;
using AllOverIt.Serialization.Binary.Writers;
using AllOverIt.Serialization.Binary.Writers.Extensions;
using FluentAssertions;
using System;
using Xunit;

namespace AllOverIt.Pipes.Tests.Serializer
{
    public class NamedPipeSerializerFixture_Functional : FixtureBase
    {
        public sealed class DummyMessage
        {
            public int Id { get; set; }
        }

        public sealed class DummyMessageReader : IEnrichedBinaryValueReader
        {
            public Type Type => typeof(DummyMessage);

            public object ReadValue(IEnrichedBinaryReader reader)
            {
                var id = reader.ReadInt32();

                return new DummyMessage { Id = id };
            }
        }

        public sealed class DummyMessageWriter : IEnrichedBinaryValueWriter
        {
            public Type Type => typeof(DummyMessage);

            public void WriteValue(IEnrichedBinaryWriter writer, object value)
            {
                var message = (DummyMessage)value;
                
                writer.WriteInt32(message.Id);
            }
        }

        private readonly NamedPipeSerializer<DummyMessage> _serializer = new();

        [Collection("Pipes")]
        public class Serialize_Deserialize : NamedPipeSerializerFixture_Functional
        {
            [Fact]
            public void Should_Throw_When_Serialize_Message_Null()
            {
                Invoking(() =>
                {
                    _serializer.Serialize(null);
                })
                .Should()
                .Throw<ArgumentNullException>()
                .WithNamedMessageWhenNull("message");
            }

            [Fact]
            public void Should_Return_Null_When_Deserialize_Bytes_Null()
            {
                _serializer.Deserialize(null).Should().BeNull();
            }

            [Fact]
            public void Should_Return_Null_When_Deserialize_Bytes_Empty()
            {
                _serializer.Deserialize(Array.Empty<byte>()).Should().BeNull();
            }

            [Fact]
            public void Should_Serialize_With_Dynamic_Reader_Writer()
            {
                // No reader/writer registered, so a dynamic reader/writer will be used
                var expected = Create<DummyMessage>();

                var bytes = _serializer.Serialize(expected);

                var actual = _serializer.Deserialize(bytes);

                expected.Should().BeEquivalentTo(actual);
            }

            [Fact]
            public void Should_Serialize_With_Custom_Reader_Writer()
            {
                _serializer.Readers.Add(new DummyMessageReader());
                _serializer.Writers.Add(new DummyMessageWriter());

                var expected = Create<DummyMessage>();

                var bytes = _serializer.Serialize(expected);

                var actual = _serializer.Deserialize(bytes);

                expected.Should().BeEquivalentTo(actual);
            }
        }
    }
}