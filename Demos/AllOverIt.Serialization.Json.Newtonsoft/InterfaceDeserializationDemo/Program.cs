using AllOverIt.Serialization.Json.Newtonsoft;
using AllOverIt.Serialization.Json.Newtonsoft.Extensions;
using Newtonsoft.Json;

namespace InterfaceDeserializationDemo
{
    internal class Program
    {
        static void Main()
        {
            SerializeSinglePerson();
            Console.WriteLine();
            SerializeMultipleChildren();

            Console.WriteLine();
            Console.WriteLine("All Over It.");
            Console.ReadKey();
        }

        private static void SerializeSinglePerson()
        {
            var person = new Person
            {
                FirstName = "Robert",
                LastName = "Jones",
                Age = 21,
                Address = new Address               // This property is of type IAddress
                {
                    Street = "Broad St",
                    City = "BroadMeadow"
                }
            };

            // use a default serializer to serialize the object to a string
            var serialized = JsonConvert.SerializeObject(person);

            Console.WriteLine($"Serialized to: {serialized}");
            Console.WriteLine();

            // When deserializing a person, we need to tell the serializer to convert IAddress to Address.

            // This demo is using NewtonsoftJsonSerializer but it would work equally as well using the following:
            //
            // var settings = new JsonSerializerSettings();
            // settings.Converters.Add(new InterfaceConverter<IAddress, Address>());
            // settings.Converters.Add(new InterfaceConverter<IChild, Child>());
            // JsonConvert.DeserializeObject<Person>(serialized, settings);

            var serializer = new NewtonsoftJsonSerializer();

            serializer.AddInterfaceConverter<IAddress, Address>();
            serializer.AddInterfaceConverter<IChild, Child>();

            var deserialized = serializer.DeserializeObject<Person>(serialized)!;

            Console.WriteLine($"Deserialized to: {JsonConvert.SerializeObject(deserialized)}");
            Console.WriteLine();
        }

        private static void SerializeMultipleChildren()
        {
            var parent = new Parent
            {
                FirstName = "Robert",
                LastName = "Jones",
                Age = 21,
                Address = new Address               // This property is of type IAddress
                {
                    Street = "Broad St",
                    City = "BroadMeadow"
                },
                ChildrenArray =
                [
                    new Child                       // This property is of type IChild
                    {
                        FirstName = "Mary",
                        Age = 3
                    },
                    new Child
                    {
                        FirstName = "Roger",
                        Age = 5
                    }
                ],
                ChildrenEnumerable =
                [
                    new Child                       // This property is of type IChild
                    {
                        FirstName = "Peter",
                        Age = 3
                    },
                    new Child
                    {
                        FirstName = "Paul",
                        Age = 5
                    }
                ],
                ChildrenList =
                [
                    new Child                       // This property is of type IChild
                    {
                        FirstName = "Sally",
                        Age = 12
                    },
                    new Child
                    {
                        FirstName = "Lucas",
                        Age = 14
                    }
                ]
            };

            var serialized = JsonConvert.SerializeObject(parent);

            Console.WriteLine($"Serialized to: {serialized}");
            Console.WriteLine();

            var serializer = new NewtonsoftJsonSerializer();

            serializer.AddInterfaceConverter<IAddress, Address>();
            serializer.AddInterfaceConverter<IChild, Child>();

            var deserialized = serializer.DeserializeObject<Parent>(serialized)!;

            Console.WriteLine($"Deserialized to: {JsonConvert.SerializeObject(deserialized)}");
            Console.WriteLine();
        }
    }
}
