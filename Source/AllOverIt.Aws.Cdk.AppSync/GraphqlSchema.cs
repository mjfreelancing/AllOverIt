﻿using Cdklabs.AwsCdkAppsyncUtils;
using System;
using System.Collections.Generic;
using System.Text;

namespace AllOverIt.Aws.Cdk.AppSync
{
    internal sealed class GraphqlSchema : CodeFirstSchema
    {
        private ObjectType _query;
        private ObjectType _mutation;
        private ObjectType _subscription;

        private bool HasQuery => _query is not null;
        private bool HasMutation => _mutation is not null;
        private bool HasSubscription => _subscription is not null;
        private bool HasSchema => HasQuery || HasMutation || HasSubscription;

        internal ObjectType Query
        {
            get
            {
                if (_query is null)
                {
                    _query = new ObjectType(nameof(Query), new ObjectTypeOptions
                    {
                        Definition = new Dictionary<string, IField>()
                    });

                    AddType(_query);
                }

                return _query;
            }
        }

        internal ObjectType Mutation
        {
            get
            {
                if (_mutation is null)
                {
                    _mutation = new ObjectType(nameof(Mutation), new ObjectTypeOptions
                    {
                        Definition = new Dictionary<string, IField>()
                    });

                    AddType(_mutation);
                }

                return _mutation;
            }
        }

        internal ObjectType Subscription
        {
            get
            {
                if (_subscription is null)
                {
                    _subscription = new ObjectType(nameof(Subscription), new ObjectTypeOptions
                    {
                        Definition = new Dictionary<string, IField>()
                    });

                    AddType(_subscription);
                }

                return _subscription;
            }
        }

        public override string Definition
        {
            get
            {
                var builder = new StringBuilder();

                if (HasSchema)
                {
                    builder.Append("schema {\n");
                }

                if (HasQuery)
                {
                    builder.Append("  query: Query\n");
                }

                if (HasMutation)
                {
                    builder.Append("  mutation: Mutation\n");
                }

                if (HasSubscription)
                {
                    builder.Append("  subscription: Subscription\n");
                }

                if (HasSchema)
                {
                    builder.Append("}\n");
                }

                return builder.ToString();
            }
            set 
            {
                throw new InvalidOperationException("The schema definition cannot be set.");
            }
        }
    }
}