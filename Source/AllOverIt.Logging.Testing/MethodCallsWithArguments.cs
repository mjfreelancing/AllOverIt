using AllOverIt.Extensions;
using Microsoft.Extensions.Logging;

namespace AllOverIt.Logging.Testing
{
    public sealed class MethodCallsWithArguments : List<MethodCallContext>
    {
        public LogLevel[] LogLevels => this.SelectToArray(item => item.LogLevel);
        public IDictionary<string, object>[] Entries => this.SelectToArray(item => item.Entries);
        public Exception[] Exceptions => this.SelectToArray(item => item.Exception);
    }
}