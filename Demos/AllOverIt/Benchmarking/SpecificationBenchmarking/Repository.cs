using System.Collections.Generic;
using System.Linq;

namespace SpecificationBenchmarking
{
    internal static class Repository
    {
        public static readonly IList<Person> Persons = new[]
        {
            new Person(20, Sex.Male, "WE"),
            new Person(18, Sex.Male, "AB"),
            new Person(16, Sex.Female, "GH"),
            new Person(20, Sex.Male, "DE"),
            new Person(24, Sex.Male, "DM"),
            new Person(25, Sex.Female, "CS"),
            new Person(15, Sex.Male, "ML"),
            new Person(20, Sex.Female, "ER"),
            new Person(39, Sex.Female, "NP"),
            new Person(30, Sex.Male, "WY"),
            new Person(18, Sex.Female, "ZS"),
            new Person(20, Sex.Female, "CD"),
            new Person(22, Sex.Male, "HK")
        }.ToList();
    }
}