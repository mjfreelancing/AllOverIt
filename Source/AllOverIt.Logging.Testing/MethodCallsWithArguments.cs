using AllOverIt.Extensions;

namespace AllOverIt.Logging.Testing
{
    public sealed class MethodCallsWithArguments : List<(IDictionary<string, object> State, Exception Exception)>
    {
        public IDictionary<string, object>[] States => this.SelectToArray(item => item.State);
        public Exception[] Exceptions => this.SelectToArray(item => item.Exception);
    }
}