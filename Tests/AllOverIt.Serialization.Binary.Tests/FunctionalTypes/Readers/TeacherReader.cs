﻿using AllOverIt.Serialization.Binary.Readers;
using AllOverIt.Serialization.Binary.Readers.Extensions;
using AllOverIt.Serialization.Binary.Tests.FunctionalTypes.Models;

namespace AllOverIt.Serialization.Binary.Tests.FunctionalTypes.Readers
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
