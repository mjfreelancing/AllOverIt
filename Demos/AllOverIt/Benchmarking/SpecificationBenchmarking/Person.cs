namespace SpecificationBenchmarking
{
    internal class Person
    {
        public int Age { get; }
        public Sex Sex { get; }
        public string Initials { get; }

        public Person(int age, Sex sex, string initials)
        {
            Age = age;
            Sex = sex;
            Initials = initials;
        }
    }
}