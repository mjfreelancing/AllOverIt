using AllOverIt.Patterns.Specification;
using BenchmarkDotNet.Attributes;
using SpecificationBenchmarking.Specifications;

namespace SpecificationBenchmarking
{

    /*

    |                                  Method |       Mean |     Error |    StdDev |     Median | Allocated |
    |---------------------------------------- |-----------:|----------:|----------:|-----------:|----------:|
    |     Using_Specification_IsSatisfied_One |  11.446 ns | 0.2415 ns | 0.2016 ns |  11.352 ns |         - |
    |     Using_Specification_IsSatisfied_Two |  22.988 ns | 0.4898 ns | 0.9554 ns |  22.629 ns |         - |
    |     Using_Specification_IsSatisfied_Ten | 122.344 ns | 3.2704 ns | 9.6428 ns | 118.632 ns |         - |
    | Using_LinqSpecification_IsSatisfied_One |   6.557 ns | 0.1562 ns | 0.1462 ns |   6.525 ns |         - |
    | Using_LinqSpecification_IsSatisfied_Two |  13.144 ns | 0.2891 ns | 0.3860 ns |  13.091 ns |         - |
    | Using_LinqSpecification_IsSatisfied_Ten |  75.657 ns | 1.4839 ns | 1.5878 ns |  75.864 ns |         - |

    */

    [MemoryDiagnoser]
    public class BenchmarkTests
    {
        private readonly Person _candidate;
        private readonly Specification<Person> _criteria;
        private readonly LinqSpecification<Person> _criteriaLinq;

        public BenchmarkTests()
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
        public void Using_Specification_IsSatisfied_One()
        {
            _criteria.IsSatisfiedBy(_candidate);
        }

        [Benchmark]
        public void Using_Specification_IsSatisfied_Two()
        {
            for (var i = 0; i < 2; i++)
            {
                _criteria.IsSatisfiedBy(_candidate);
            }
        }

        [Benchmark]
        public void Using_Specification_IsSatisfied_Ten()
        {
            for (var i = 0; i < 10; i++)
            {
                _criteria.IsSatisfiedBy(_candidate);
            }
        }

        // ================

        [Benchmark]
        public void Using_LinqSpecification_IsSatisfied_One()
        {
            _criteriaLinq.IsSatisfiedBy(_candidate);
        }

        [Benchmark]
        public void Using_LinqSpecification_IsSatisfied_Two()
        {
            for (var i = 0; i < 2; i++)
            {
                _criteriaLinq.IsSatisfiedBy(_candidate);
            }
        }

        [Benchmark]
        public void Using_LinqSpecification_IsSatisfied_Ten()
        {
            for (var i = 0; i < 10; i++)
            {
                _criteriaLinq.IsSatisfiedBy(_candidate);
            }
        }

        // ================
    }
}