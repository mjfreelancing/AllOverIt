namespace AllOverIt.Evaluator.Variables
{
    /// <summary>Provides the ability to look up referenced and referencing variables for a specified variable.</summary>
    public interface IVariableLookup
    {
        /// <summary>Get all variables referenced by the specified variable using the specified lookup mode.</summary>
        /// <param name="variable">The source variable to look up referenced variables for.</param>
        /// <param name="lookupMode">Specifies the lookup mode that determines which variables will be returned.</param>
        /// <returns>All variables referenced by the specified variable.</returns>
        IEnumerable<IVariable> GetReferencedVariables(IVariable variable, VariableLookupMode lookupMode);

        /// <summary>Get all variables referencing the specified variable using the specified lookup mode.</summary>
        /// <param name="variable">The source variable to look up referencing variables for.</param>
        /// <param name="lookupMode">Specifies the lookup mode that determines which variables will be returned.</param>
        /// <returns>All variables referencing the specified variable.</returns>
        IEnumerable<IVariable> GetReferencingVariables(IVariable variable, VariableLookupMode lookupMode);
    }
}