using AllOverIt.Serialization.Binary;
using AllOverIt.Serialization.Binary.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;


// TODO: Create a reflection based reader / writer
// TODO: Compare stream sizes of JSON, Custom Writer, Reflection based writer
// TODO: Benchmark the Custom and Reflection based writers

UseCustomReadersAndWriters();
Console.WriteLine();
//UseReflectionReadersAndWriters();

Console.WriteLine();
Console.WriteLine("All Over It.");
Console.ReadKey();

static void UseCustomReadersAndWriters()
{
    var classroom = CreateClassroom();
    byte[] serializedBytes;

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

            writer.WriteObject(classroom);


            var e1 = Enumerable.Range(1, 3);
            writer.WriteObject(e1);


            var d1 = Environment.GetEnvironmentVariables();
            writer.WriteObject(d1);


            var d2 = new Dictionary<object, object>();
            d2.Add(1, "1");
            d2.Add(true, 1);
            d2.Add(Gender.Male, "Male");
            d2.Add(new Student(), new Teacher());
            writer.WriteObject(d2);



            var d3 = new Dictionary<int, double>
            {
                { 1, 1.1 },
                { 2, 2.2 },
                { 3, 3.3 }
            };

            writer.WriteObject(d3);
        }

        serializedBytes = stream.ToArray();
    }

    var serializedString = Convert.ToBase64String(serializedBytes);

    Console.WriteLine("Serialized");
    Console.WriteLine("==========");
    Console.WriteLine(serializedString);
    Console.WriteLine();
    Console.WriteLine($"  => {serializedBytes.Length} bytes");
    Console.WriteLine();

    Classroom deserializedClassroom = default;
    var deserializedBytes = Convert.FromBase64String(serializedString);

    using (var stream = new MemoryStream(deserializedBytes))
    {
        using (var reader = new EnrichedBinaryReader(stream, Encoding.UTF8, true))
        {
            reader.Readers.Add(new StudentReader());
            reader.Readers.Add(new TeacherReader());
            reader.Readers.Add(new ClassroomReader());

            deserializedClassroom = (Classroom) reader.ReadObject();


            var e1 = reader.ReadObject();
            
            // was written via IDictionary, assumed to be the equivalent of IDictionary<string, string>
            var d1 = reader.ReadDictionary<string, string>(); //reader.ReadObject();

            var d2 = reader.ReadDictionary<object, object>(); //reader.ReadObject();

            var d3 = reader.ReadDictionary<int, double>();

        }
    }

    // Serialize the deserialized classroom...
    var serializerOptions = new JsonSerializerOptions
    {
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() }
    };

    var compactClassroomString = JsonSerializer.Serialize(deserializedClassroom);
    var indentedClassroomString = JsonSerializer.Serialize(deserializedClassroom, serializerOptions);

    Console.WriteLine("Deserialized");
    Console.WriteLine("============");
    Console.WriteLine(indentedClassroomString);
    Console.WriteLine();
    Console.WriteLine($"  => {compactClassroomString.Length} bytes");
    Console.WriteLine();
}

//static void UseReflectionReadersAndWriters()
//{
//    var classroom = CreateClassroom();

//    byte[] serializedBytes;

//    using (var stream = new MemoryStream())
//    {
//        using (var writer = new EnrichedBinaryWriter(stream, Encoding.UTF8, true))
//        {
//            var objectWriter = new ObjectBinaryWriter(writer);


//            //var d = new Dictionary<object, object>();
//            //d.Add(1, "1");
//            //d.Add(true, 1);
//            //d.Add(Gender.Male, "Male");
//            //d.Add(new Student(), new Teacher());

//            //objectWriter.WriteObject(d);

//            objectWriter.WriteObject(classroom);
//        }

//        serializedBytes = stream.ToArray();
//    }

//    var serializedString = Convert.ToBase64String(serializedBytes);

//    Console.WriteLine("Serialized");
//    Console.WriteLine("==========");
//    Console.WriteLine(serializedString);
//    Console.WriteLine();
//    Console.WriteLine($"  => {serializedBytes.Length} bytes");
//    Console.WriteLine();

//    Classroom deserializedClassroom = default;



//}

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
