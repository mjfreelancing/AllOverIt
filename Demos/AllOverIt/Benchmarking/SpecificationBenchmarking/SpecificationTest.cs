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
        private static readonly Person Candidate;
        private static readonly Specification<Person> Criteria;
        private static readonly LinqSpecification<Person> CriteriaLinq;

        static SpecificationTest()
        {
            Candidate = new Person(20, Sex.Male, "WE");

            var isMale = new IsOfSex(Sex.Male);
            var isFemale = new IsOfSex(Sex.Female);
            var minimumAge = new IsOfMinimumAge(20);

            Criteria = isMale && minimumAge || isFemale && !minimumAge;

            var isMaleLinq = new IsOfSexLinq(Sex.Male);
            var isFemaleLinq = new IsOfSexLinq(Sex.Female);
            var minimumAgeLinq = new IsOfMinimumAgeLinq(20);

            CriteriaLinq = isMaleLinq && minimumAgeLinq || isFemaleLinq && !minimumAgeLinq;
        }

        // ================

        [Benchmark]
        public void Using_Specification_IsSatisfied_Once()
        {
            Criteria.IsSatisfiedBy(Candidate);
        }

        [Benchmark]
        public void Using_Specification_IsSatisfied_Twice()
        {
            Criteria.IsSatisfiedBy(Candidate);
            Criteria.IsSatisfiedBy(Candidate);
        }

        // ================

        [Benchmark]
        public void Using_LinqSpecification_IsSatisfied_Once()
        {
            CriteriaLinq.IsSatisfiedBy(Candidate);
        }

        [Benchmark]
        public void Using_LinqSpecification_IsSatisfied_Twice()
        {
            CriteriaLinq.IsSatisfiedBy(Candidate);
            CriteriaLinq.IsSatisfiedBy(Candidate);
        }

        // ================
    }
}