using AllOverIt.Serialization.Binary.Writers;
using AllOverIt.Serialization.Binary.Writers.Extensions;
using BinarySerializationBenchmark.Models;

namespace BinarySerializationBenchmark.Writers
{
    internal abstract class PersonWriter<TPerson> : EnrichedBinaryValueWriter<TPerson> where TPerson : Person
    {
        public override void WriteValue(IEnrichedBinaryWriter writer, object value)
        {
            var person = (Person) value;

            writer.Write(person.FirstName);
            writer.WriteSafeString(person.LastName);
            writer.WriteEnum(person.Gender);           // writes the type and value
            writer.WriteNullable(person.Age);            // caters for nullable values
        }
    }
}