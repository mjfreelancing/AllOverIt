namespace AllOverIt.GenericHost
{
    public interface IConsoleApp : IGenericApp
    {
        public int? ExitCode { get; }
    }
}