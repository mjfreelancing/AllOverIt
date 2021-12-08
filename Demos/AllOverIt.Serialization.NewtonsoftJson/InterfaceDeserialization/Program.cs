using AllOverIt.Serialization.NewtonsoftJson;
using AllOverIt.Serialization.NewtonsoftJson.Converters;
using AllOverIt.Serialization.NewtonsoftJson.Extensions;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace InterfaceDeserialization
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

            // When deserializing a person, we need to tell the serializer to convert IAddress to Address.

            // This demo is using SystemTextJsonSerializer but it would work equally as well using the following:
            //
            //   var settings = new JsonSerializerSettings();
            //   settings.Converters.Add(new InterfaceConverter<IAddress, Address>());
            //   JsonConvert.DeserializeObject<Person>(serialized, settings);

            var serializer = new NewtonsoftJsonSerializer();
            serializer.AddConverters(new InterfaceConverter<IAddress, Address>());

            var deserialized = serializer.DeserializeObject<Person>(serialized);

            Console.WriteLine(
                $"Deserialized to '{deserialized.FirstName} {deserialized.LastName}', age {deserialized.Age}, " +
                $"living on {deserialized.Address.Street} in {deserialized.Address.City}");
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
                Children = new[]
                {
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
                }
            };

            var serialized = JsonConvert.SerializeObject(parent);
            Console.WriteLine($"Serialized to: {serialized}");

            var serializer = new NewtonsoftJsonSerializer();

            // The interface converters can be added via AddConverters() like this:
            //
            //   serializer.AddConverters(
            //       new InterfaceConverter<IAddress, Address>(),                // IAddress => Address
            //       new EnumerableInterfaceConverter<IChild, Child>());         // IEnumerable<IChild> => List<Child>
            //
            // Or manually using:
            //
            //   serializer.Settings.Converters.Add(...);
            //
            // or like this:
            serializer.AddInterfaceConverter<IAddress, Address>(false);     // If true is passed then IEnumerable<IAddress> => List<Address> would also be supported
            serializer.AddEnumerableInterfaceConverter<IChild, Child>();

            var deserialized = serializer.DeserializeObject<Parent>(serialized);

            var child1 = deserialized.Children.ElementAt(0);
            var child2 = deserialized.Children.ElementAt(1);

            Console.WriteLine(
                $"Deserialized to '{deserialized.FirstName} {deserialized.LastName}', age {deserialized.Age}, " +
                $"living on {deserialized.Address.Street} in {deserialized.Address.City}. Children are " +
                $"{child1.FirstName} ({child1.Age}) and {child2.FirstName} ({child2.Age}).");
        }
    }
}
