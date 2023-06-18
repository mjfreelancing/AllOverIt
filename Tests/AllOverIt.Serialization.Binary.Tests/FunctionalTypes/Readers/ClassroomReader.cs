﻿using AllOverIt.Serialization.Binary.Readers;
using AllOverIt.Serialization.Binary.Readers.Extensions;
using AllOverIt.Serialization.Binary.Tests.FunctionalTypes.Models;

namespace AllOverIt.Serialization.Binary.Tests.FunctionalTypes.Readers
{
    internal sealed class ClassroomReader : EnrichedBinaryValueReader<Classroom>
    {
        public override object ReadValue(IEnrichedBinaryReader reader)
        {
            var roomId = reader.ReadGuid();
            var teacher = reader.ReadObject<Teacher>();
            var students = reader.ReadEnumerable<Student>();

            return new Classroom
            {
                RoomId = roomId,
                Teacher = teacher,
                Students = students
            };
        }
    }
}
