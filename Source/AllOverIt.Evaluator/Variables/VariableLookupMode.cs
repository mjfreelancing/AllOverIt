namespace AllOverIt.Evaluator.Variables
{
    public enum VariableLookupMode
    {
        // The lookup process should only return explicitly referenced or referencing variables.
        Explicit,

        // The lookup process should return all (explicit and implicit) referenced or referencing variables.
        All
    }
}