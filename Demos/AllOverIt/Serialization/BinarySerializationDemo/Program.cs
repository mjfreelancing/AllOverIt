using AllOverIt.Serialization.Binary;
using AllOverIt.Serialization.Binary.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;


// TODO: Benchmark the reader and writer

SerializeEnumerableIntsViaReadAndWriteObject();
SerializeEnumerableIntsViaReadAndWriteEnumerable();
SerializeEnumerableObjectsViaReadAndWriteEnumerable();

SerializeDictionaryAsObject();
SerializeDictionary();
SerializeDictionaryAsIDictionary();
SerializeDictionaryAsObjectObject();
SerializeDictionaryAsIntString();
SerializeUsingCustomReadersAndWriters();

Console.WriteLine();
Console.WriteLine("All Over It.");
Console.ReadKey();

static void OutputObjectAsJson(string prefix, object @object)
{
    var serializerOptions = new JsonSerializerOptions
    {
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() }
    };

    var output = JsonSerializer.Serialize(@object);

    Console.WriteLine(prefix);
    Console.WriteLine("============================================");
    Console.WriteLine(output);
    Console.WriteLine();
}

static void SerializeEnumerableIntsViaReadAndWriteObject()
{
    byte[] bytes;

    using (var stream = new MemoryStream())
    {
        using (var writer = new EnrichedBinaryWriter(stream, Encoding.UTF8, true))
        {
            var range = Enumerable
                .Range(1, 5)
                //.Cast<int?>()
                //.Concat(new int?[] { null, 6, 7, 8, 9, 10 });
                ;

            OutputObjectAsJson("Enumerable (int?), via WriteObject()", range);

            writer.WriteObject(range);
        }

        bytes = stream.ToArray();
    }

    using (var stream = new MemoryStream(bytes))
    {
        using (var reader = new EnrichedBinaryReader(stream, Encoding.UTF8, true))
        {
            var range = reader.ReadObject();
            OutputObjectAsJson("Decoded, via ReadObject()", range);
        }
    }
}

static void SerializeEnumerableIntsViaReadAndWriteEnumerable()
{
    byte[] bytes;

    using (var stream = new MemoryStream())
    {
        using (var writer = new EnrichedBinaryWriter(stream, Encoding.UTF8, true))
        {
            var range = Enumerable
                .Range(1, 5)
                .Cast<int?>()
                .Concat(new int?[] { null, 6, 7, 8, 9, 10 });

            OutputObjectAsJson("Enumerable (int?), via WriteEnumerable()", range);

            writer.WriteEnumerable(range);
        }

        bytes = stream.ToArray();
    }

    using (var stream = new MemoryStream(bytes))
    {
        using (var reader = new EnrichedBinaryReader(stream, Encoding.UTF8, true))
        {
            var range = reader.ReadEnumerable();
            OutputObjectAsJson("Decoded, via ReadEnumerable()", range);
        }
    }
}

static void SerializeEnumerableObjectsViaReadAndWriteEnumerable()
{
    byte[] bytes;

    using (var stream = new MemoryStream())
    {
        using (var writer = new EnrichedBinaryWriter(stream, Encoding.UTF8, true))
        {
            // cannot use 'var' when there is a null value
            IEnumerable<int?> range = Enumerable
                .Range(1, 5)
                .Cast<int?>()
                .Concat(new int?[] { null, 6, 7, 8, 9, 10 });

            OutputObjectAsJson("Enumerable (object), via WriteEnumerable()", range);

            writer.WriteEnumerable(range);
        }

        bytes = stream.ToArray();
    }

    using (var stream = new MemoryStream(bytes))
    {
        using (var reader = new EnrichedBinaryReader(stream, Encoding.UTF8, true))
        {
            var range = reader.ReadEnumerable();
            OutputObjectAsJson("Decoded, via ReadEnumerable()", range);
        }
    }
}

static void SerializeDictionaryAsObject()
{
    byte[] bytes;

    using (var stream = new MemoryStream())
    {
        using (var writer = new EnrichedBinaryWriter(stream, Encoding.UTF8, true))
        {
            var dict = new Dictionary<object, object>
            {
                { 1, "1" },
                { true, 12.3 },
                { Gender.Male, "It's a boy" },
                //{ new Student(), new Teacher() }
            };

            writer.WriteObject(dict);
        }

        bytes = stream.ToArray();
    }

    using (var stream = new MemoryStream(bytes))
    {
        using (var reader = new EnrichedBinaryReader(stream, Encoding.UTF8, true))
        {
            var dict = reader.ReadObject();
            OutputObjectAsJson("Decoded, via ReadObject()", dict);
        }
    }
}

static void SerializeDictionary()
{
    byte[] bytes;

    using (var stream = new MemoryStream())
    {
        using (var writer = new EnrichedBinaryWriter(stream, Encoding.UTF8, true))
        {
            var dict = new Dictionary<object, object>
            {
                { 1, "1" },
                { true, 12.3 },
                { Gender.Male, "It's a boy" },
            };

            writer.WriteDictionary(dict);
        }

        bytes = stream.ToArray();
    }

    using (var stream = new MemoryStream(bytes))
    {
        using (var reader = new EnrichedBinaryReader(stream, Encoding.UTF8, true))
        {
            var dict = reader.ReadDictionary();
            OutputObjectAsJson("Decoded, via ReadDictionary()", dict);
        }
    }
}

static void SerializeDictionaryAsObjectObject()
{
    byte[] bytes;

    using (var stream = new MemoryStream())
    {
        using (var writer = new EnrichedBinaryWriter(stream, Encoding.UTF8, true))
        {
            var dict = new Dictionary<int, string>
            {
                { 1, "1" },
                { 2, "2" },
                { 3, "3" },
            };

            writer.WriteDictionary(dict);
        }

        bytes = stream.ToArray();
    }

    using (var stream = new MemoryStream(bytes))
    {
        using (var reader = new EnrichedBinaryReader(stream, Encoding.UTF8, true))
        {
            var dict = reader.ReadDictionary();
            OutputObjectAsJson("Decoded, via ReadDictionary()", dict);
        }
    }
}

static void SerializeDictionaryAsIntString()
{
    byte[] bytes;

    using (var stream = new MemoryStream())
    {
        using (var writer = new EnrichedBinaryWriter(stream, Encoding.UTF8, true))
        {
            var dict = new Dictionary<int, string>
            {
                { 1, "1" },
                { 2, "2" },
                { 3, "3" },
            };

            writer.WriteDictionary(dict);
        }

        bytes = stream.ToArray();
    }

    using (var stream = new MemoryStream(bytes))
    {
        using (var reader = new EnrichedBinaryReader(stream, Encoding.UTF8, true))
        {
            var dict = reader.ReadDictionary<int, string>();
            OutputObjectAsJson("Decoded, via ReadDictionary<int, string>()", dict);
        }
    }
}

static void SerializeDictionaryAsIDictionary()
{
    byte[] bytes;

    using (var stream = new MemoryStream())
    {
        using (var writer = new EnrichedBinaryWriter(stream, Encoding.UTF8, true))
        {
            var dict = Environment.GetEnvironmentVariables();

            writer.WriteDictionary(dict);
        }

        bytes = stream.ToArray();
    }

    using (var stream = new MemoryStream(bytes))
    {
        using (var reader = new EnrichedBinaryReader(stream, Encoding.UTF8, true))
        {
            var dict = reader.ReadDictionary();
            OutputObjectAsJson("Decoded, via ReadDictionary()", dict);
        }
    }
}

static void SerializeUsingCustomReadersAndWriters()
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
        Students = new List<Student>
        {
            new Student
            {
                FirstName = "Mary",
                LastName = "Lamb",
                Gender = Gender.Female,
                Age = 12
            },
            new Student
            {
                FirstName = "Charlette",
                LastName = "Web",
                Gender = Gender.Female
            },
            new Student
            {
                FirstName = "Shrek",
                Gender = Gender.Male,
                Age = 13
            }
        }
    };
}
