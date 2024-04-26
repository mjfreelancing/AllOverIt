using AllOverIt.Assertion;
using AllOverIt.Extensions;

namespace AllOverIt.Evaluator.Variables.Extensions
{
    /// <summary>Provides a variety of <see cref="IVariable"/> extensions.</summary>
    public static class VariableExtensions
    {
        /// <summary>Associates a variable with the specified variable registry if it inherits from VariableBase.</summary>
        /// <param name="variable">The variable to be associated with a variable registry.</param>
        /// <param name="variableRegistry">The variable registry to be associated with a variable.</param>
        /// <remarks>The variable registry replaces any other registry previously associated with the variable.</remarks>
        public static void SetVariableRegistry(this IVariable variable, IVariableRegistry variableRegistry)
        {
            _ = variable.WhenNotNull(nameof(variable));
            _ = variableRegistry.WhenNotNull(nameof(variableRegistry));

            if (variable is VariableBase variableBase)
            {
                variableBase.VariableRegistry = variableRegistry;
            }
        }

        /// <summary>Gets all variables referenced by the variable (explicit and implicit).</summary>
        public static IEnumerable<IVariable> GetAllReferencedVariables(this IVariable variable)
        {
            _ = variable.WhenNotNull(nameof(variable));

            var allVariables = new List<IVariable>();

            GetAllReferencedVariables(variable.ReferencedVariables, allVariables);

            return allVariables
              .Distinct()
              .AsReadOnlyCollection();
        }

        private static void GetAllReferencedVariables(IEnumerable<IVariable> referencedVariables, List<IVariable> allVariables)
        {
            var variables = referencedVariables.AsReadOnlyCollection();

            allVariables.AddRange(variables);

            foreach (var variable in variables)
            {
                GetAllReferencedVariables(variable.ReferencedVariables, allVariables);
            }
        }
    }
}