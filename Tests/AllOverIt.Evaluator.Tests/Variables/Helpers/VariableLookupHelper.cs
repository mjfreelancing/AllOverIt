using AllOverIt.Evaluator.Variables;
using AllOverIt.Evaluator.Variables.Extensions;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AllOverIt.Evaluator.Tests.Variables.Helpers
{
    internal static class VariableLookupHelper
    {
        public static IAoiVariableRegistry GetKnownVariableRegistry()
        {
            var factory = new AoiVariableFactory();
            var registry = factory.CreateVariableRegistry();

            //                                 References                     Referencing
            //                            Explicit        All             Explicit        All
            // a = 1                      -               -               b, c, e         b, c, d, e, f, g
            // b = a + 2                  a               a               c, e, f         c, d, e, f, g
            // c = a + b                  a, b            a, b            d, e            d, e, g
            // d = c                      c               a, b, c         -               -
            // e = a + b + c              a, b, c         a, b, c         g               g
            // f = b                      b               a, b            -               -
            // g = e                      e               a, b, c, e      -               -

            var a = factory.CreateConstantVariable("a");
            var b = factory.CreateDelegateVariable("b", () => 0.0d, new[] { "a" });
            var c = factory.CreateDelegateVariable("c", () => 0.0d, new[] { "a", "b" });
            var d = factory.CreateLazyVariable("d", () => 0.0d, new[] { "c" });
            var e = factory.CreateLazyVariable("e", () => 0.0d, new[] { "a", "b", "c" });
            var f = factory.CreateDelegateVariable("f", () => 0.0d, new[] { "b" });
            var g = factory.CreateDelegateVariable("g", () => 0.0d, new[] { "e" });

            registry.Add(a, b, c, d, e, f, g);

            return registry;
        }

        public static IDictionary<IAoiVariable, IList<IAoiVariable>> GetExpectedReferencedExplicit(IAoiVariableRegistry registry)
        {
            var a = registry.Variables.Single(kv => kv.Key == "a").Value;
            var b = registry.Variables.Single(kv => kv.Key == "b").Value;
            var c = registry.Variables.Single(kv => kv.Key == "c").Value;
            var d = registry.Variables.Single(kv => kv.Key == "d").Value;
            var e = registry.Variables.Single(kv => kv.Key == "e").Value;
            var f = registry.Variables.Single(kv => kv.Key == "f").Value;
            var g = registry.Variables.Single(kv => kv.Key == "g").Value;

            return new Dictionary<IAoiVariable, IList<IAoiVariable>>
            {
                [a] = new List<IAoiVariable>(),
                [b] = new List<IAoiVariable>(new[] { a }),
                [c] = new List<IAoiVariable>(new[] { a, b }),
                [d] = new List<IAoiVariable>(new[] { c }),
                [e] = new List<IAoiVariable>(new[] { a, b, c }),
                [f] = new List<IAoiVariable>(new[] { b }),
                [g] = new List<IAoiVariable>(new[] { e })
            };
        }

        public static IDictionary<IAoiVariable, IList<IAoiVariable>> GetExpectedReferencedAll(IAoiVariableRegistry registry)
        {
            var a = registry.Variables.Single(kv => kv.Key == "a").Value;
            var b = registry.Variables.Single(kv => kv.Key == "b").Value;
            var c = registry.Variables.Single(kv => kv.Key == "c").Value;
            var d = registry.Variables.Single(kv => kv.Key == "d").Value;
            var e = registry.Variables.Single(kv => kv.Key == "e").Value;
            var f = registry.Variables.Single(kv => kv.Key == "f").Value;
            var g = registry.Variables.Single(kv => kv.Key == "g").Value;

            return new Dictionary<IAoiVariable, IList<IAoiVariable>>
            {
                [a] = new List<IAoiVariable>(),
                [b] = new List<IAoiVariable>(new[] { a }),
                [c] = new List<IAoiVariable>(new[] { a, b }),
                [d] = new List<IAoiVariable>(new[] { a, b, c }),
                [e] = new List<IAoiVariable>(new[] { a, b, c }),
                [f] = new List<IAoiVariable>(new[] { a, b }),
                [g] = new List<IAoiVariable>(new[] { a, b, c, e })
            };
        }

        public static IDictionary<IAoiVariable, IList<IAoiVariable>> GetExpectedReferencingExplicit(IAoiVariableRegistry registry)
        {
            var a = registry.Variables.Single(kv => kv.Key == "a").Value;
            var b = registry.Variables.Single(kv => kv.Key == "b").Value;
            var c = registry.Variables.Single(kv => kv.Key == "c").Value;
            var d = registry.Variables.Single(kv => kv.Key == "d").Value;
            var e = registry.Variables.Single(kv => kv.Key == "e").Value;
            var f = registry.Variables.Single(kv => kv.Key == "f").Value;
            var g = registry.Variables.Single(kv => kv.Key == "g").Value;

            return new Dictionary<IAoiVariable, IList<IAoiVariable>>
            {
                [a] = new List<IAoiVariable>(new[] { b, c, e }),
                [b] = new List<IAoiVariable>(new[] { c, e, f }),
                [c] = new List<IAoiVariable>(new[] { d, e }),
                [d] = new List<IAoiVariable>(),
                [e] = new List<IAoiVariable>(new[] { g }),
                [f] = new List<IAoiVariable>(),
                [g] = new List<IAoiVariable>()
            };
        }

        public static IDictionary<IAoiVariable, IList<IAoiVariable>> GetExpectedReferencingAll(IAoiVariableRegistry registry)
        {
            var a = registry.Variables.Single(kv => kv.Key == "a").Value;
            var b = registry.Variables.Single(kv => kv.Key == "b").Value;
            var c = registry.Variables.Single(kv => kv.Key == "c").Value;
            var d = registry.Variables.Single(kv => kv.Key == "d").Value;
            var e = registry.Variables.Single(kv => kv.Key == "e").Value;
            var f = registry.Variables.Single(kv => kv.Key == "f").Value;
            var g = registry.Variables.Single(kv => kv.Key == "g").Value;

            return new Dictionary<IAoiVariable, IList<IAoiVariable>>
            {
                [a] = new List<IAoiVariable>(new[] { b, c, d, e, f, g }),
                [b] = new List<IAoiVariable>(new[] { c, d, e, f, g }),
                [c] = new List<IAoiVariable>(new[] { d, e, g }),
                [d] = new List<IAoiVariable>(),
                [e] = new List<IAoiVariable>(new[] { g }),
                [f] = new List<IAoiVariable>(),
                [g] = new List<IAoiVariable>()
            };
        }

        public static void AssertExpectedLookup(IDictionary<IAoiVariable, IList<IAoiVariable>> testCases,
          Func<IAoiVariable, IEnumerable<IAoiVariable>> actualResults)
        {
            foreach (var variable in testCases.Keys)
            {
                var actual = actualResults.Invoke(variable);
                var expected = testCases[variable];

                actual.Should().BeEquivalentTo(expected);
            }
        }
    }
}