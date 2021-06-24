using AllOverIt.Evaluator.Variables;
using AllOverIt.Evaluator.Variables.Extensions;
using System;
using System.Linq;

namespace VariableLookup
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new VariableFactory();
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

            var lookup = new AllOverIt.Evaluator.Variables.VariableLookup(registry);   // can be created before populating variables if required
            ReportVariables(lookup, a);
            ReportVariables(lookup, b);
            ReportVariables(lookup, c);
            ReportVariables(lookup, d);
            ReportVariables(lookup, e);
            ReportVariables(lookup, f);
            ReportVariables(lookup, g);

            Console.WriteLine("");
            Console.WriteLine("All Over It.");
            Console.ReadKey();
        }

        private static void ReportVariables(IVariableLookup lookup, IVariable variable)
        {
            var explicitReferences = lookup.GetReferencedVariables(variable, VariableLookupMode.Explicit);
            var allReferences = lookup.GetReferencedVariables(variable, VariableLookupMode.All);
            var explicitReferencing = lookup.GetReferencingVariables(variable, VariableLookupMode.Explicit);
            var allReferencing = lookup.GetReferencingVariables(variable, VariableLookupMode.All);

            Console.WriteLine($"Variable {variable.Name}");

            var explicitReferencesNames = string.Join(", ", explicitReferences.Select(referenced => referenced.Name).OrderBy(name => name));
            Console.WriteLine($"  Explicit References = {explicitReferencesNames}");

            var allReferencesNames = string.Join(", ", allReferences.Select(referenced => referenced.Name).OrderBy(name => name));
            Console.WriteLine($"  All References = {allReferencesNames}");

            var explicitReferencingNames = string.Join(", ", explicitReferencing.Select(referenced => referenced.Name).OrderBy(name => name));
            Console.WriteLine($"  Explicit Referencing = {explicitReferencingNames}");

            var allReferencingNames = string.Join(", ", allReferencing.Select(referenced => referenced.Name).OrderBy(name => name));
            Console.WriteLine($"  All Referencing = {allReferencingNames}");

            Console.WriteLine("");
        }
    }
}
