namespace AllOverIt.Aws.Cdk.AppSync.Schema
{
    public interface ISchemaBuilder
    {
        ISchemaBuilder AddQuery<TType>() where TType : IQueryDefinition;
        ISchemaBuilder AddMutation<TType>() where TType : IMutationDefinition;
        ISchemaBuilder AddSubscription<TType>() where TType : ISubscriptionDefinition;
    }
}