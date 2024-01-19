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
        public static IVariableRegistry GetKnownVariableRegistry()
        {
            var compiler = new FormulaCompiler();
            var registry = new VariableRegistry();

            //                                      References                     Referencing
            //                   Value         Explicit        All             Explicit        All
            // a = 1               1           -               -               b, c, e         b, c, d, e, f, g
            // b = a + 2           3           a               a               c, e, f         c, d, e, f, g
            // c = a + b           4           a, b            a, b            d, e            d, e, g
            // d = c               4           c               a, b, c         -               -
            // e = a + b + c       8           a, b, c         a, b, c         g               g
            // f = b               3           b               a, b            -               -
            // g = e               8           e               a, b, c, e      -               -

            registry.AddConstantVariable("a", 1);
            registry.AddDelegateVariable("b", compiler.Compile("a + 2", registry));
            registry.AddLazyVariable("c", compiler.Compile("a + b", registry));
            registry.AddLazyVariable("d", compiler.Compile("c", registry));
            registry.AddLazyVariable("e", compiler.Compile("a+b+c", registry));
            registry.AddDelegateVariable("f", compiler.Compile("b", registry));
            registry.AddDelegateVariable("g", compiler.Compile("e", registry));
            
            return registry;
        }

        public static IDictionary<IVariable, IList<IVariable>> GetExpectedReferencedExplicit(IVariableRegistry registry)
        {
            var a = registry.Single(kv => kv.Key == "a").Value;
            var b = registry.Single(kv => kv.Key == "b").Value;
            var c = registry.Single(kv => kv.Key == "c").Value;
            var d = registry.Single(kv => kv.Key == "d").Value;
            var e = registry.Single(kv => kv.Key == "e").Value;
            var f = registry.Single(kv => kv.Key == "f").Value;
            var g = registry.Single(kv => kv.Key == "g").Value;

            return new Dictionary<IVariable, IList<IVariable>>
            {
                [a] = new List<IVariable>(),
                [b] = new List<IVariable>(new[] { a }),
                [c] = new List<IVariable>(new[] { a, b }),
                [d] = new List<IVariable>(new[] { c }),
                [e] = new List<IVariable>(new[] { a, b, c }),
                [f] = new List<IVariable>(new[] { b }),
                [g] = new List<IVariable>(new[] { e })
            };
        }

        public static IDictionary<IVariable, IList<IVariable>> GetExpectedReferencedAll(IVariableRegistry registry)
        {
            var a = registry.Single(kv => kv.Key == "a").Value;
            var b = registry.Single(kv => kv.Key == "b").Value;
            var c = registry.Single(kv => kv.Key == "c").Value;
            var d = registry.Single(kv => kv.Key == "d").Value;
            var e = registry.Single(kv => kv.Key == "e").Value;
            var f = registry.Single(kv => kv.Key == "f").Value;
            var g = registry.Single(kv => kv.Key == "g").Value;

            return new Dictionary<IVariable, IList<IVariable>>
            {
                [a] = new List<IVariable>(),
                [b] = new List<IVariable>(new[] { a }),
                [c] = new List<IVariable>(new[] { a, b }),
                [d] = new List<IVariable>(new[] { a, b, c }),
                [e] = new List<IVariable>(new[] { a, b, c }),
                [f] = new List<IVariable>(new[] { a, b }),
                [g] = new List<IVariable>(new[] { a, b, c, e })
            };
        }

        public static IDictionary<IVariable, IList<IVariable>> GetExpectedReferencingExplicit(IVariableRegistry registry)
        {
            var a = registry.Single(kv => kv.Key == "a").Value;
            var b = registry.Single(kv => kv.Key == "b").Value;
            var c = registry.Single(kv => kv.Key == "c").Value;
            var d = registry.Single(kv => kv.Key == "d").Value;
            var e = registry.Single(kv => kv.Key == "e").Value;
            var f = registry.Single(kv => kv.Key == "f").Value;
            var g = registry.Single(kv => kv.Key == "g").Value;

            return new Dictionary<IVariable, IList<IVariable>>
            {
                [a] = new List<IVariable>(new[] { b, c, e }),
                [b] = new List<IVariable>(new[] { c, e, f }),
                [c] = new List<IVariable>(new[] { d, e }),
                [d] = new List<IVariable>(),
                [e] = new List<IVariable>(new[] { g }),
                [f] = new List<IVariable>(),
                [g] = new List<IVariable>()
            };
        }

        public static IDictionary<IVariable, IList<IVariable>> GetExpectedReferencingAll(IVariableRegistry registry)
        {
            var a = registry.Single(kv => kv.Key == "a").Value;
            var b = registry.Single(kv => kv.Key == "b").Value;
            var c = registry.Single(kv => kv.Key == "c").Value;
            var d = registry.Single(kv => kv.Key == "d").Value;
            var e = registry.Single(kv => kv.Key == "e").Value;
            var f = registry.Single(kv => kv.Key == "f").Value;
            var g = registry.Single(kv => kv.Key == "g").Value;

            return new Dictionary<IVariable, IList<IVariable>>
            {
                [a] = new List<IVariable>(new[] { b, c, d, e, f, g }),
                [b] = new List<IVariable>(new[] { c, d, e, f, g }),
                [c] = new List<IVariable>(new[] { d, e, g }),
                [d] = new List<IVariable>(),
                [e] = new List<IVariable>(new[] { g }),
                [f] = new List<IVariable>(),
                [g] = new List<IVariable>()
            };
        }

        public static void AssertExpectedLookup(IDictionary<IVariable, IList<IVariable>> testCases,
          Func<IVariable, IEnumerable<IVariable>> actualResults)
        {
            foreach (var variable in testCases.Keys)
            {
                var actual = actualResults.Invoke(variable);
                var expected = testCases[variable];

                expected.Should().BeEquivalentTo(actual);
            }
        }
    }
}