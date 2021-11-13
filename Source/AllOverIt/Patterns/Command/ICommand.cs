namespace AllOverIt.Patterns.Command
{
    /// <summary>Represents a command with no input or output.</summary>
    public interface ICommand
    {
        /// <summary>Executes the command.</summary>
        void Execute();
    }

    /// <summary>Represents a command with a specified input type and no output type.</summary>
    /// <typeparam name="TInput">The input type to be provided to the command.</typeparam>
    public interface ICommand<TInput>
    {
        /// <summary>Executes the command with a specified input.</summary>
        /// <param name="input">The input instance to be provided to the command.</param>
        void Execute(TInput input);
    }

    /// <summary>Represents a command with a specified input and output type.</summary>
    /// <typeparam name="TInput">The input type to be provided to the command.</typeparam>
    /// <typeparam name="TOutput">The output type to be returned from the command.</typeparam>
    public interface ICommand<TInput, TOutput>
    {
        /// <summary>Executes the command with a specified input and returns an output.</summary>
        /// <param name="input">The input instance to be provided to the command.</param>
        /// <returns>The output result from the command.</returns>
        TOutput Execute(TInput input);
    }
}
