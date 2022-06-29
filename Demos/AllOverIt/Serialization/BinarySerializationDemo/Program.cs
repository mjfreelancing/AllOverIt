using AllOverIt.Serialization.Binary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;


// TODO: Create a reflection based reader / writer
// TODO: Compare stream sizes of JSON, Custom Writer, Reflection based writer
// TODO: Benchmark the Custom and Reflection based writers


var classroom = new Classroom
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

        writer.WriteObject(classroom);
    }

    bytes = stream.ToArray();
}

var serialized = Convert.ToBase64String(bytes);

Console.WriteLine("Serialized");
Console.WriteLine("==========");
Console.WriteLine(serialized);
Console.WriteLine();
Console.WriteLine($"  => {bytes.Length} bytes");
Console.WriteLine();

Classroom deserializedClassroom = default;
var deserializedBytes = Convert.FromBase64String(serialized);

using (var stream = new MemoryStream(deserializedBytes))
{
    using (var reader = new EnrichedBinaryReader(stream, Encoding.UTF8, true))
    {
        reader.Readers.Add(new StudentReader());
        reader.Readers.Add(new TeacherReader());
        reader.Readers.Add(new ClassroomReader());

        deserializedClassroom = (Classroom)reader.ReadObject();
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


Console.WriteLine();
Console.WriteLine("All Over It.");
Console.ReadKey();
