using AllOverIt.Serialization.Binary;
using AllOverIt.Serialization.Binary.Extensions;

namespace AllOverIt.Tests.Serialization.Binary.FunctionalTypes.Models
{
    internal sealed class TeacherReader : EnrichedBinaryValueReader<Teacher>
    {
        public override object ReadValue(IEnrichedBinaryReader reader)
        {
            var firstName = reader.ReadSafeString();
            var lastName = reader.ReadSafeString();
            var gender = (Gender) reader.ReadEnum();
            var age = reader.ReadNullable<int>();

            return new Teacher
            {
                FirstName = firstName,
                LastName = lastName,
                Gender = gender,
                Age = age
            };
        }
    }
}
