using AllOverIt.Expressions.Strings;
using AllOverIt.Fixture;
using System.Collections.Generic;

namespace AllOverIt.Filtering.Tests.Operations
{
    public class OperationsFixtureBase : FixtureBase
    {
        internal sealed class DummyClass
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        internal DummyClass Model { get; }

        public OperationsFixtureBase()
        {
            Model = Create<DummyClass>();
        }

        protected static IEnumerable<object[]> FilterComparisonOptions()
        {
            return new List<object[]>
            {
                new object[] { false, StringComparisonMode.None },
                new object[] { true, StringComparisonMode.None },
                new object[] { false, StringComparisonMode.ToUpper },
                new object[] { true, StringComparisonMode.ToUpper  },
                new object[] { false, StringComparisonMode.ToLower },
                new object[] { true, StringComparisonMode.ToLower },
                new object[] { false, StringComparisonMode.InvariantCultureIgnoreCase },
                new object[] { true, StringComparisonMode.InvariantCultureIgnoreCase },
            };
        }
    }
}
