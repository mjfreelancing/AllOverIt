using AllOverIt.Aws.Cdk.AppSync.Attributes;
using Amazon.CDK.AWS.AppSync;

namespace AllOverIt.Aws.Cdk.AppSync.Factories
{
    public interface IDataSourceFactory
    {
        BaseDataSource CreateDataSource(DataSourceAttribute attribute);
    }
}