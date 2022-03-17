using AllOverIt.Patterns.Specification;
using BenchmarkDotNet.Attributes;
using SpecificationBenchmarking.Specifications;

namespace SpecificationBenchmarking
{

    /*

    |                                    Method |      Mean |     Error |    StdDev | Allocated |
    |------------------------------------------ |----------:|----------:|----------:|----------:|
    |      Using_Specification_IsSatisfied_Once |  9.737 ns | 0.2270 ns | 0.4207 ns |         - |
    |     Using_Specification_IsSatisfied_Twice | 18.806 ns | 0.3978 ns | 0.6968 ns |         - |
    |  Using_LinqSpecification_IsSatisfied_Once |  5.620 ns | 0.1392 ns | 0.1429 ns |         - |
    | Using_LinqSpecification_IsSatisfied_Twice | 12.189 ns | 0.2529 ns | 0.3198 ns |         - |

    */

    [MemoryDiagnoser]
    public class SpecificationTest
    {
        private readonly Person _candidate;
        private readonly Specification<Person> _criteria;
        private readonly LinqSpecification<Person> _criteriaLinq;

        public SpecificationTest()
        {
            _candidate = new Person(20, Sex.Male, "WE");

            var isMale = new IsOfSex(Sex.Male);
            var isFemale = new IsOfSex(Sex.Female);
            var minimumAge = new IsOfMinimumAge(20);

            _criteria = isMale && minimumAge || isFemale && !minimumAge;

            var isMaleLinq = new IsOfSexLinq(Sex.Male);
            var isFemaleLinq = new IsOfSexLinq(Sex.Female);
            var minimumAgeLinq = new IsOfMinimumAgeLinq(20);

            _criteriaLinq = isMaleLinq && minimumAgeLinq || isFemaleLinq && !minimumAgeLinq;
        }

        // ================

        [Benchmark]
        public void Using_Specification_IsSatisfied_Once()
        {
            _criteria.IsSatisfiedBy(_candidate);
        }

        [Benchmark]
        public void Using_Specification_IsSatisfied_Twice()
        {
            _criteria.IsSatisfiedBy(_candidate);
            _criteria.IsSatisfiedBy(_candidate);
        }

        // ================

        [Benchmark]
        public void Using_LinqSpecification_IsSatisfied_Once()
        {
            _criteriaLinq.IsSatisfiedBy(_candidate);
        }

        [Benchmark]
        public void Using_LinqSpecification_IsSatisfied_Twice()
        {
            _criteriaLinq.IsSatisfiedBy(_candidate);
            _criteriaLinq.IsSatisfiedBy(_candidate);
        }

        [Benchmark]
        public void Using_LinqSpecification_IsSatisfied_Thrice()
        {
            _criteriaLinq.IsSatisfiedBy(_candidate);
            _criteriaLinq.IsSatisfiedBy(_candidate);
            _criteriaLinq.IsSatisfiedBy(_candidate);
        }

        // ================
    }
}