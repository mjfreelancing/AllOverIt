using AllOverIt.Assertion;
using AllOverIt.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AllOverIt.Patterns.Command
{
    /// <summary>Implements a pipeline that allows multiple commands to be executed in turn where the output of each command
    /// is provided as the input to the next. The input and output are of the same type.</summary>
    /// <typeparam name="TInput">The input type to be provided to the command.</typeparam>
    public class AsyncCommandPipeline<TInput> : AsyncCommandPipeline<TInput, TInput>
    {
    }

    /// <summary>Implements a pipeline that allows multiple commands to be executed in turn where the output of each command
    /// is provided as the input to the next. The output type must be the same as, or inherit from, the input type.</summary>
    /// <typeparam name="TInput">The input type to be provided to the command.</typeparam>
    /// <typeparam name="TOutput">The output type to be returned from the command.</typeparam>
    public class AsyncCommandPipeline<TInput, TOutput> where TOutput : TInput
    {
        private readonly List<IAsyncCommand<TInput, TOutput>> _commands = new ();

        /// <summary>Constructor.</summary>
        public AsyncCommandPipeline()
        {
        }

        /// <summary>Constructor.</summary>
        /// <param name="commands">Commands to be appended to the pipeline.</param>
        public AsyncCommandPipeline(params IAsyncCommand<TInput, TOutput>[] commands)
        {
            Append(commands);
        }

        /// <summary>Appends the specified command to the pipeline.</summary>
        /// <param name="commands">One or more commands to be appended to the pipeline.</param>
        /// <returns>The pipeline instance, allowing for a fluent syntax.</returns>
        public AsyncCommandPipeline<TInput, TOutput> Append(params IAsyncCommand<TInput, TOutput>[] commands)
        {
            _ = commands.WhenNotNullOrEmpty(nameof(commands));

            _commands.AddRange(commands);
            return this;
        }

        /// <summary>Asynchronously processes a specified input by passing it to the first command in the pipeline and
        /// sequentially passing the output to the next command in the sequence.</summary>
        /// <typeparam name="TInput">The input type to be provided to the command.</typeparam>
        /// <returns>The output from the last command in the pipeline sequence.</returns>
        public async Task<TOutput> ExecuteAsync(TInput input)
        {
            if (!_commands.Any())
            {
                throw new CommandException("There are no commands to execute.");
            }

            var output = await _commands.First().ExecuteAsync(input).ConfigureAwait(false);

            foreach (var command in _commands.Skip(1))
            {
                output = await command.ExecuteAsync(output).ConfigureAwait(false);
            }

            return output;
        }
    }
}
