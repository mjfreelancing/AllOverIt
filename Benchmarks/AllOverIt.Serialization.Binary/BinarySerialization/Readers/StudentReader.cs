using AllOverIt.Serialization.Binary.Readers;
using AllOverIt.Serialization.Binary.Readers.Extensions;
using BinarySerializationBenchmark.Models;

namespace BinarySerializationBenchmark.Readers
{
    internal sealed class StudentReader : EnrichedBinaryValueReader<Student>
    {
        public override object ReadValue(IEnrichedBinaryReader reader)
        {
            var firstName = reader.ReadSafeString();
            var lastName = reader.ReadSafeString();
            var gender = reader.ReadEnum<Gender>();
            var age = reader.ReadNullable<int>();

            return new Student
            {
                FirstName = firstName,
                LastName = lastName,
                Gender = gender,
                Age = age
            };
        }
    }
}