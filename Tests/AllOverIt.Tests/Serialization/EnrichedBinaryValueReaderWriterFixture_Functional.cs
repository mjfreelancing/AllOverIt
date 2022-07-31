using AllOverIt.Fixture;
using AllOverIt.Serialization.Binary;
using AllOverIt.Serialization.Binary.Extensions;
using AllOverIt.Tests.Serialization.Binary.FunctionalTypes.Models;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace AllOverIt.Tests.Serialization
{
    public class EnrichedBinaryValueReaderWriterFixture : FixtureBase
    {
        [Fact]
        public void Should_Read_Write_Using_Custom_Reader_Writer()
        {
            var expected = CreateMany<Classroom>();
            IEnumerable<Classroom> actual = default;

            byte[] bytes;

            using (var stream = new MemoryStream())
            {
                using (var writer = new EnrichedBinaryWriter(stream, Encoding.UTF8, true))
                {
                    // Using writers will result in a larger stream because type information is stored
                    // for each user-defined type. A pure reflection based approach will provide a smaller
                    // stream but at the expense of reduced performance when deserializing.
                    writer.Writers.Add(new StudentWriter());
                    writer.Writers.Add(new TeacherWriter());
                    writer.Writers.Add(new ClassroomWriter());

                    writer.WriteEnumerable(expected);
                }

                bytes = stream.ToArray();
            }

            using (var stream = new MemoryStream(bytes))
            {
                using (var reader = new EnrichedBinaryReader(stream, Encoding.UTF8, true))
                {
                    reader.Readers.Add(new StudentReader());
                    reader.Readers.Add(new TeacherReader());
                    reader.Readers.Add(new ClassroomReader());

                    actual = reader.ReadEnumerable<Classroom>();
                }
            }

            actual.Should().BeEquivalentTo(expected);
        }
    }
}