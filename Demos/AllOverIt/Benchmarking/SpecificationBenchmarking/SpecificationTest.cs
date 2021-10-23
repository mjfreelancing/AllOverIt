using AllOverIt.Patterns.Specification;
using BenchmarkDotNet.Attributes;
using SpecificationBenchmarking.Specifications;
using System.Collections.Generic;
using System.Linq;

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
        private static readonly IList<Person> Persons;
        private static readonly IQueryable<Person> PersonsQueryable;
        private static readonly Person FirstPerson;
        private static readonly Specification<Person> Criteria;
        private static readonly LinqSpecification<Person> CriteriaLinq;

        static SpecificationTest()
        {
            Persons = Repository.Persons;
            PersonsQueryable = Persons.AsQueryable();
            FirstPerson = Persons.ElementAt(0);

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
            Criteria.IsSatisfiedBy(FirstPerson);
        }

        [Benchmark]
        public void Using_Specification_IsSatisfied_Twice()
        {
            Criteria.IsSatisfiedBy(FirstPerson);
            Criteria.IsSatisfiedBy(FirstPerson);
        }

        // ================

        [Benchmark]
        public void Using_LinqSpecification_IsSatisfied_Once()
        {
            CriteriaLinq.IsSatisfiedBy(FirstPerson);
        }

        [Benchmark]
        public void Using_LinqSpecification_IsSatisfied_Twice()
        {
            CriteriaLinq.IsSatisfiedBy(FirstPerson);
            CriteriaLinq.IsSatisfiedBy(FirstPerson);
        }

        // ================

        //[Benchmark]
        //public void Using_Specification_Once()
        //{
        //    _ = Persons.Where(Criteria).ToList();
        //}

        //[Benchmark]
        //public void Using_Specification_Twice()
        //{
        //    _ = Persons.Where(Criteria).ToList();
        //    _ = Persons.Where(Criteria).ToList();
        //}

        //// ================

        //[Benchmark]
        //public void Using_LinqSpecification_Once()
        //{
        //    _ = PersonsQueryable.Where(CriteriaLinq).ToList();
        //}

        //[Benchmark]
        //public void Using_LinqSpecification_Using_Specification_Twice()
        //{
        //    _ = PersonsQueryable.Where(CriteriaLinq).ToList();
        //    _ = PersonsQueryable.Where(CriteriaLinq).ToList();
        //}

        //// ================

        //[Benchmark]
        //public void Using_LinqSpecification_As_Func_Once()
        //{
        //    _ = Persons.Where((Func<Person, bool>) CriteriaLinq).ToList();
        //}

        //[Benchmark]
        //public void Using_LinqSpecification_As_Func_Using_Specification_Twice()
        //{
        //    _ = Persons.Where((Func<Person, bool>) CriteriaLinq).ToList();
        //    _ = Persons.Where((Func<Person, bool>) CriteriaLinq).ToList();
        //}

        //// ================

        //[Benchmark]
        //public void Using_LinqSpecification_Calling_AsQueryable_Once()
        //{
        //    _ = Persons.AsQueryable().Where(CriteriaLinq).ToList();
        //}

        //[Benchmark]
        //public void Using_LinqSpecification_Calling_AsQueryable_Using_Specification_Twice()
        //{
        //    _ = Persons.AsQueryable().Where(CriteriaLinq).ToList();
        //    _ = Persons.AsQueryable().Where(CriteriaLinq).ToList();
        //}

        // ================
    }
}