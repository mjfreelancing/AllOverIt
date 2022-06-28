using AllOverIt.Extensions;
using AllOverIt.Serialization.Binary;
using AllOverIt.Serialization.Binary.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;



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



var serialized = string.Empty;

using (var stream = new MemoryStream())
{
    using (var writer = new EnrichedBinaryWriter(stream, Encoding.UTF8, true))
    {
        writer.Writers.Add(new StudentWriter());
        writer.Writers.Add(new TeacherWriter());
        writer.Writers.Add(new ClassroomWriter());

        writer.WriteObject(classroom);
    }

    stream.Position = 0;
    var bytes = stream.ToArray();

    serialized = Convert.ToBase64String(bytes);
}


Console.WriteLine("Serialized");
Console.WriteLine("==========");
Console.WriteLine(serialized);
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

var classroomString = JsonSerializer.Serialize(deserializedClassroom, serializerOptions);

Console.WriteLine("Deserialized");
Console.WriteLine("============");
Console.WriteLine(classroomString);
Console.WriteLine();


Console.WriteLine();
Console.WriteLine("All Over It.");
Console.ReadKey();




internal abstract class PersonWriter<TPerson> : EnrichedBinaryTypeWriter<TPerson> where TPerson : Person
{
    public override void WriteValue(EnrichedBinaryWriter writer, object value)
    {
        var person = (Person) value;

        writer.WriteSafeString(person.FirstName);
        writer.WriteSafeString(person.LastName);
        writer.WriteEnum(person.Gender);           // writes the type and value
        writer.WriteObject(person.Age);            // caters for nullable values
    }
}

internal sealed class StudentWriter : PersonWriter<Student>
{
}

internal sealed class TeacherWriter : PersonWriter<Teacher>
{
}


internal sealed class StudentReader : EnrichedBinaryTypeReader<Student>
{
    public override object ReadValue(EnrichedBinaryReader reader)
    {
        var firstName = reader.ReadSafeString();
        var lastName = reader.ReadSafeString();
        var gender = (Gender) reader.ReadEnum();
        var age = (int?) reader.ReadObject();           // add a <TType>

        return new Student
        {
            FirstName = firstName,
            LastName = lastName,
            Gender = gender,
            Age = age
        };
    }
}

internal sealed class TeacherReader : EnrichedBinaryTypeReader<Teacher>
{
    public override object ReadValue(EnrichedBinaryReader reader)
    {
        var firstName = reader.ReadSafeString();
        var lastName = reader.ReadSafeString();
        var gender = (Gender) reader.ReadEnum();
        var age = (int?) reader.ReadObject();           // add a <TType>

        return new Teacher
        {
            FirstName = firstName,
            LastName = lastName,
            Gender = gender,
            Age = age
        };
    }
}




internal sealed class ClassroomWriter : EnrichedBinaryTypeWriter<Classroom>
{
    public override void WriteValue(EnrichedBinaryWriter writer, object value)
    {
        var classroom = (Classroom) value;

        writer.WriteGuid(classroom.RoomId);
        writer.WriteObject(classroom.Teacher);

        var students = classroom.Students.AsReadOnlyCollection();

        writer.WriteInt32(students.Count);

        if (students.Count > 0)
        {
            foreach (var student in students)
            {
                writer.WriteObject(student);
            }
        }
    }
}


internal sealed class ClassroomReader : EnrichedBinaryTypeReader<Classroom>
{
    public override object ReadValue(EnrichedBinaryReader reader)
    {
        var roomId = reader.ReadGuid();
        var teacher = (Teacher)reader.ReadObject();             // Add a <TType> that performs the cast
        var studentCount = reader.ReadInt32();
        var students = new List<Student>();

        for (var i = 0; i < studentCount; i++)
        {
            var student = (Student)reader.ReadObject();
            students.Add(student);
        }

        return new Classroom
        {
            RoomId = roomId,
            Teacher = teacher,
            Students = students
        };
    }
}




internal enum Gender
{
    Male,
    Female
}

internal abstract class Person
{
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public Gender Gender { get; init; }
    public int? Age { get; init; }
}

internal sealed class Teacher : Person
{
}

internal sealed class Student : Person
{
}


internal sealed class Classroom
{
    public Guid RoomId { get; init; }
    public Teacher Teacher { get; init; }
    public IEnumerable<Student> Students { get; init; }
}

