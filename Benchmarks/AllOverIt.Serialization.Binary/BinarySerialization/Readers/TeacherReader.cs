﻿using AllOverIt.Serialization.Binary.Readers;
using AllOverIt.Serialization.Binary.Readers.Extensions;
using BinarySerializationBenchmark.Models;

namespace BinarySerializationBenchmark.Readers
{
    internal sealed class TeacherReader : EnrichedBinaryValueReader<Teacher>
    {
        public override object ReadValue(IEnrichedBinaryReader reader)
        {
            var firstName = reader.ReadString();
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