using AllOverIt.Fixture;
using AllOverIt.Serialization.Binary.Readers;
using AllOverIt.Serialization.Binary.Readers.Extensions;
using AllOverIt.Serialization.Binary.Tests.FunctionalTypes.Models;
using AllOverIt.Serialization.Binary.Tests.FunctionalTypes.Readers;
using AllOverIt.Serialization.Binary.Tests.FunctionalTypes.Writers;
using AllOverIt.Serialization.Binary.Writers;
using AllOverIt.Serialization.Binary.Writers.Extensions;
using Shouldly;
using System.Text;
using AllOverIt.Shouldly.Extensions;

namespace AllOverIt.Serialization.Binary.Tests
{
    public class EnrichedBinaryValueReaderWriterFixture : FixtureBase
    {
        [Fact]
        public void Should_Read_Write_Enumerable_Using_Custom_Reader_Writer()
        {
            var expected = CreateMany<Classroom>();
            IEnumerable<Classroom> actual = null;

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

            var actualList = actual.ToList();
            var expectedList = expected.ToList();
            actualList.Count.ShouldBe(expectedList.Count);
            for (var i = 0; i < actualList.Count; i++)
            {
                AssertClassroom(actualList[i], expectedList[i]);
            }
        }

        [Fact]
        public void Should_Read_Write_Object_Using_Custom_Reader_Writer()
        {
            var expected = Create<Classroom>();

            object actual = null;

            byte[] bytes;

            using (var stream = new MemoryStream())
            {
                using (var writer = new EnrichedBinaryWriter(stream, Encoding.UTF8, true))
                {
                    writer.Writers.Add(new StudentWriter());
                    writer.Writers.Add(new TeacherWriter());
                    writer.Writers.Add(new ClassroomWriter());

                    writer.WriteObject(expected);
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

                    actual = reader.ReadObject<Classroom>();
                }
            }

            AssertClassroom((Classroom)actual, expected);
        }

        [Fact]
        public void Should_Read_Write_Object_Using_Dynamic_Reader_Writer()
        {
            var expected = Create<Classroom>();

            object actual = null;

            byte[] bytes;

            using (var stream = new MemoryStream())
            {
                using (var writer = new EnrichedBinaryWriter(stream, Encoding.UTF8, true))
                {
                    writer.WriteObject(expected);
                }

                bytes = stream.ToArray();
            }

            using (var stream = new MemoryStream(bytes))
            {
                using (var reader = new EnrichedBinaryReader(stream, Encoding.UTF8, true))
                {
                    actual = reader.ReadObject<Classroom>();
                }
            }

            AssertClassroom((Classroom)actual, expected);
        }

        private static void AssertClassroom(Classroom actual, Classroom expected)
        {
            actual.RoomId.ShouldBe(expected.RoomId);
            actual.Teacher.ShouldBeEquivalentTo(expected.Teacher);
            var actualStudents = actual.Students.ToList();
            var expectedStudents = expected.Students.ToList();
            actualStudents.Count.ShouldBe(expectedStudents.Count);
            for (var i = 0; i < actualStudents.Count; i++)
            {
                actualStudents[i].ShouldBeEquivalentTo(expectedStudents[i]);
            }
        }
    }
}





