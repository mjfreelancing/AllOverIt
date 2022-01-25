namespace AllOverIt.Csv
{
    public sealed record HeaderIdentifier<THeaderId>
    {
        public THeaderId Id { get; init; }      // Could be the item's index within a collection or a key in a dictionary
        public string Name { get; init; }
    }
}
