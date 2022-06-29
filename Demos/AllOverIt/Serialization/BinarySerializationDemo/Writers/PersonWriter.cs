using AllOverIt.Serialization.Binary;
using AllOverIt.Serialization.Binary.Extensions;

internal abstract class PersonWriter<TPerson> : EnrichedBinaryTypeWriter<TPerson> where TPerson : Person
{
    public override void WriteValue(EnrichedBinaryWriter writer, object value)
    {
        var person = (Person) value;

        writer.WriteSafeString(person.FirstName);
        writer.WriteSafeString(person.LastName);
        writer.WriteEnum(person.Gender);           // writes the type and value
        writer.WriteNullable(person.Age);            // caters for nullable values
    }
}
