using AllOverIt.Serialization.Binary.Readers;
using AllOverIt.Serialization.Binary.Writers;
using BinarySerializationDemo.Models;
using BinarySerializationDemo.Readers;
using BinarySerializationDemo.Writers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BinarySerializationDemo
{
    internal class Program
    {
        private static void Main()
        {
            SerializeUsingCustomReadersAndWriters();

            Console.WriteLine();
            Console.WriteLine("All Over It.");
            Console.ReadKey();
        }

        private static void OutputObjectAsJson(string prefix, object @object)
        {
#pragma warning disable CA1869 // Cache and reuse 'JsonSerializerOptions' instances
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Converters = { new JsonStringEnumConverter() }
            };
#pragma warning restore CA1869 // Cache and reuse 'JsonSerializerOptions' instances

            var output = JsonSerializer.Serialize(@object, options);

            Console.WriteLine(prefix);
            Console.WriteLine("============================================");
            Console.WriteLine(output);
            Console.WriteLine();
        }

        private static void SerializeUsingCustomReadersAndWriters()
        {
            byte[] bytes;

            using (var stream = new MemoryStream())
            {
                using (var writer = new EnrichedBinaryWriter(stream, Encoding.UTF8, true))
                {
                    var classroom = CreateClassroom();

                    // Using writers will result in a larger stream because type information is stored
                    // for each user-defined type. A pure reflection based approach will provide a smaller
                    // stream but at the expense of reduced performance when deserializing.
                    writer.Writers.Add(new StudentWriter());
                    writer.Writers.Add(new TeacherWriter());
                    writer.Writers.Add(new ClassroomWriter());

                    writer.WriteObject(classroom);
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

                    var classroom = (Classroom) reader.ReadObject();
                    OutputObjectAsJson("Decoded, via ReadObject()", classroom);
                }
            }
        }

        static Classroom CreateClassroom()
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
                        LastName = null,                // testing null is ok
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
                        LastName = string.Empty,        // Testing empty strings are ok
                        Gender = Gender.Male,
                        Age = 13
                    }
                ]
            };
        }
    }
}