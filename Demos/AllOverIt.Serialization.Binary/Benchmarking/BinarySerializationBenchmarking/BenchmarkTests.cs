using AllOverIt.Serialization.Binary.Readers;
using AllOverIt.Serialization.Binary.Writers;
using BenchmarkDotNet.Attributes;
using BinarySerializationBenchmarking.Models;
using BinarySerializationBenchmarking.Readers;
using BinarySerializationBenchmarking.Writers;
using Newtonsoft.Json;
using System.Buffers;
using System.Text;

/*
        |               Method |      Mean |   Gen0 | Allocated |
        |--------------------- |----------:|-------:|----------:|
        | Binary_Reader_Writer | 57.051 us | 3.5400 |  22.38 KB |
        |           NewtonSoft |  9.945 us | 1.0681 |   6.56 KB |
        |           SystemText |  4.355 us | 0.3204 |   1.98 KB |
 */

namespace BinarySerializationBenchmarking
{
    [MemoryDiagnoser(true)]
    [HideColumns("Error", "StdDev", "Median")]
    public class BenchmarkTests
    {
        private readonly Classroom _classroom = CreateClassroom();

        [Benchmark]
        public void Binary_Reader_Writer()
        {
            byte[] bytes;
            var len = 0;

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

                    writer.WriteObject(_classroom);
                }

                // Testing with rented memory instead of
                // bytes = stream.ToArray();

                len = (int) stream.Length;

                // The size of this could be larger than the requested length
                bytes = ArrayPool<byte>.Shared.Rent(len);

                stream.Position = 0;
                stream.Read(bytes, 0, len);
            }

            using (var stream = new MemoryStream(bytes, 0, len))
            {
                using (var reader = new EnrichedBinaryReader(stream, Encoding.UTF8, true))
                {
                    reader.Readers.Add(new StudentReader());
                    reader.Readers.Add(new TeacherReader());
                    reader.Readers.Add(new ClassroomReader());

                    _ = (Classroom) reader.ReadObject();
                }
            }

            ArrayPool<byte>.Shared.Return(bytes);
        }

        [Benchmark]
        public void NewtonSoft()
        {
            var serialized = JsonConvert.SerializeObject(_classroom);
            _ = JsonConvert.DeserializeObject<Classroom>(serialized);
        }

        [Benchmark]
        public void SystemText()
        {
            var serialized = System.Text.Json.JsonSerializer.Serialize(_classroom);
            _ = System.Text.Json.JsonSerializer.Deserialize<Classroom>(serialized);
        }

        private static Classroom CreateClassroom()
        {
            return new Classroom
            {
                RoomId = Guid.NewGuid(),
                Teacher = new Teacher
                {
                    FirstName = "Roger",
                    LastName = "Rabbit",
                    Gender = Gender.Male
                },
                Students =
                [
                    new()
                    {
                        FirstName = "Mary",
                        LastName = "Lamb",
                        Gender = Gender.Female,
                        Age = 12
                    },
                    new()
                    {
                        FirstName = "Charlette",
                        LastName = "Web",
                        Gender = Gender.Female
                    },
                    new()
                    {
                        FirstName = "Shrek",
                        Gender = Gender.Male,
                        Age = 13
                    }
                ]
            };
        }
    }
}