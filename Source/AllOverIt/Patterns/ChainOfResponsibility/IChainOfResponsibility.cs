namespace AllOverIt.Patterns.ChainOfResponsibility
{
    public interface IChainOfResponsibility<TInput, TOutput>
    {
        IChainOfResponsibility<TInput, TOutput> SetNext(IChainOfResponsibility<TInput, TOutput> handler);
        TOutput Handle(TInput request);
    }
}