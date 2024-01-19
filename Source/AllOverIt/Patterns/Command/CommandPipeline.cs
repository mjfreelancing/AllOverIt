using AllOverIt.Assertion;
using AllOverIt.Patterns.Command.Exceptions;
using System.Collections.Generic;
using System.Linq;

namespace AllOverIt.Patterns.Command
{
    /// <summary>Implements a pipeline that allows multiple commands to be executed in turn where the output of each command
    /// is provided as the input to the next. The input and output are of the same type.</summary>
    /// <typeparam name="TInput">The input type to be provided to the command.</typeparam>
    public class CommandPipeline<TInput> : CommandPipeline<TInput, TInput>
    {
    }

    /// <summary>Implements a pipeline that allows multiple commands to be executed in turn where the output of each command
    /// is provided as the input to the next. The output type must be the same as, or inherit from, the input type.</summary>
    /// <typeparam name="TInput">The input type to be provided to the command.</typeparam>
    /// <typeparam name="TOutput">The output type to be returned from the command.</typeparam>
    public class CommandPipeline<TInput, TOutput> where TOutput : TInput
    {
        private readonly List<ICommand<TInput, TOutput>> _commands = [];

        /// <summary>Constructor.</summary>
        public CommandPipeline()
        {
        }

        /// <summary>Constructor.</summary>
        /// <param name="commands">Commands to be appended to the pipeline.</param>
        public CommandPipeline(params ICommand<TInput, TOutput>[] commands)
        {
            Append(commands);
        }

        /// <summary>Appends the specified command to the pipeline.</summary>
        /// <param name="commands">One or more commands to be appended to the pipeline.</param>
        /// <returns>The pipeline instance, allowing for a fluent syntax.</returns>
        public CommandPipeline<TInput, TOutput> Append(params ICommand<TInput, TOutput>[] commands)
        {
            _ = commands.WhenNotNullOrEmpty(nameof(commands));

            _commands.AddRange(commands);
            return this;
        }

        /// <summary>Processes a specified input by passing it to the first command in the pipeline and sequentially passing the
        /// output to the next command in the sequence.</summary>
        /// <param name="input">The input value provided to the command.</param>
        /// <returns>The output from the last command in the pipeline sequence.</returns>
        /// <exception cref="CommandException">Thrown when there are no commands to execute.</exception>
        public TOutput Execute(TInput input)
        {
            if (_commands.Count == 0)
            {
                throw new CommandException("There are no commands to execute.");
            }

            var output = _commands.First().Execute(input);

            foreach (var command in _commands.Skip(1))
            {
                output = command.Execute(output);
            }

            return output;
        }
    }
}
