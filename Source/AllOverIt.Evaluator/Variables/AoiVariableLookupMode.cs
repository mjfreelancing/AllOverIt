namespace AllOverIt.Evaluator.Variables
{
    public enum AoiVariableLookupMode
    {
        // The lookup process should only return explicitly referenced or referencing variables.
        Explicit,

        // The lookup process should return all (explicit and implicit) referenced or referencing variables.
        All
    }
}