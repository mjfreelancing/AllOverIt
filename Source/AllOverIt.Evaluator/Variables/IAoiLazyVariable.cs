namespace AllOverIt.Evaluator.Variables
{
    // Describes a read-only variable that obtains its value via a deferred delegate.
    public interface IAoiLazyVariable : IAoiVariable
    {
        // Resets the variable to force its value to be re-evaluated.
        void Reset();
    }
}