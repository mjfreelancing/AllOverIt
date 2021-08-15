using AllOverIt.Evaluator;
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
            var compiler = new FormulaCompiler();
            var registry = new VariableRegistry();

            var a = registry.AddConstantVariable("a", 1);

            //                                      References                     Referencing
            //                   Value         Explicit        All             Explicit        All
            // a = 1               1           -               -               b, c, e         b, c, d, e, f, g
            // b = a + 2           3           a               a               c, e, f         c, d, e, f, g
            // c = a + b           4           a, b            a, b            d, e            d, e, g
            // d = c               4           c               a, b, c         -               -
            // e = a + b + c       8           a, b, c         a, b, c         g               g
            // f = b               3           b               a, b            -               -
            // g = e               8           e               a, b, c, e      -               -

            // showing the two steps
            var bCompiled = compiler.Compile("a + 2", registry);
            var b = registry.AddDelegateVariable("b", bCompiled);

            var c = registry.AddLazyVariable("c", compiler.Compile("a + b", registry));
            var d = registry.AddLazyVariable("d", compiler.Compile("c", registry));
            var e = registry.AddLazyVariable("e", compiler.Compile("a+b+c", registry));
            var f = registry.AddDelegateVariable("f", compiler.Compile("b", registry));
            var g = registry.AddDelegateVariable("g", compiler.Compile("e", registry));

            var lookup = new AllOverIt.Evaluator.Variables.VariableLookup(registry);
            ReportVariables(lookup, a);
            ReportVariables(lookup, b);
            ReportVariables(lookup, c);
            ReportVariables(lookup, d);
            ReportVariables(lookup, e);
            ReportVariables(lookup, f);
            ReportVariables(lookup, g);

            Console.WriteLine();
            Console.WriteLine("All Over It.");
            Console.ReadKey();
        }

        private static void ReportVariables(IVariableLookup lookup, IVariable variable)
        {
            var explicitReferences = lookup.GetReferencedVariables(variable, VariableLookupMode.Explicit);
            var allReferences = lookup.GetReferencedVariables(variable, VariableLookupMode.All);
            var explicitReferencing = lookup.GetReferencingVariables(variable, VariableLookupMode.Explicit);
            var allReferencing = lookup.GetReferencingVariables(variable, VariableLookupMode.All);

            Console.WriteLine($"Variable {variable.Name} = {variable.Value}");

            var explicitReferencesNames = string.Join(", ", explicitReferences.Select(referenced => referenced.Name).OrderBy(name => name));
            Console.WriteLine($"  Explicit References = {explicitReferencesNames}");

            var allReferencesNames = string.Join(", ", allReferences.Select(referenced => referenced.Name).OrderBy(name => name));
            Console.WriteLine($"  All References = {allReferencesNames}");

            var explicitReferencingNames = string.Join(", ", explicitReferencing.Select(referenced => referenced.Name).OrderBy(name => name));
            Console.WriteLine($"  Explicit Referencing = {explicitReferencingNames}");

            var allReferencingNames = string.Join(", ", allReferencing.Select(referenced => referenced.Name).OrderBy(name => name));
            Console.WriteLine($"  All Referencing = {allReferencingNames}");

            Console.WriteLine();
        }
    }
}
