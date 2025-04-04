﻿using AllOverIt.Aws.Cdk.AppSync.Attributes.Resolvers;
using AllOverIt.Aws.Cdk.AppSync.Attributes.Types;
using AllOverIt.Aws.Cdk.AppSync.DataSources;
using AllOverIt.Aws.Cdk.AppSync.Factories;
using AllOverIt.Aws.Cdk.AppSync.Resolvers;
using SystemType = System.Type;

namespace AllOverIt.Aws.Cdk.AppSync
{
    /// <summary>Contains options for an AppSync GraphQL API.</summary>
    public interface IAppGraphqlProps
    {
        /// <summary>Provides a list of all DataSources, each having a <see cref="GraphqlDataSourceBase.DataSourceName"/>. This unique name
        /// is referenced by a resolver attribute, such as <see cref="UnitResolverAttribute"/>, to associate it with a field.</summary>
        GraphqlDataSourceBase[] DataSources { get; }

        /// <summary>Provides name overrides for types discovered without a <see cref="SchemaTypeBaseAttribute"/>,
        /// such as <see cref="SchemaEnumAttribute"/>. This would normally be used with other types, such as enumerations,
        /// defined in a shared assembly.</summary>
        IReadOnlyDictionary<SystemType, string> TypeNameOverrides { get; }

        /// <summary>Contains all configured or discovered resolvers. This can be <see langword="null"/> if all field resolvers
        /// are discoverable via a resolver attribute, such as <see cref="UnitResolverAttribute"/>. All resolvers must implement
        /// <see cref="IVtlRuntime"/> or <see cref="IJsRuntime"/>, both of which inherit <see cref="IResolverRuntime"/>.</summary>
        ResolverRegistry ResolverRegistry { get; }

        /// <summary>Contains factory registrations for resolver types that typically do not have a default constructor
        /// because arguments need to be provided at runtime.</summary>
        ResolverFactory ResolverFactory { get; }
    }
}