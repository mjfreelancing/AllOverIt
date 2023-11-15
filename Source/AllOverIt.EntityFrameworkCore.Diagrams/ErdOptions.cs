using System;
using System.Collections.Generic;

namespace AllOverIt.EntityFrameworkCore.Diagrams
{
    /// <summary>Provides options that define how the entity relationship diagram will be created.</summary>
    public sealed class ErdOptions
    {
        /// <summary>Provides entity specific options that can be applied to a single entity, or globally.</summary>
        public abstract class EntityOptionsBase
        {
            /// <summary>Specifies how each entity column nullability is depicted on the generated diagram.</summary>
            public NullableColumn Nullable { get; } = new();

            /// <summary>Indicates if a column's maximum length should be depicted on the generated diagram.</summary>
            public bool ShowMaxLength { get; set; } = true;

            /// <summary>Provides styling options for an entity shape.</summary>
            public ShapeStyle ShapeStyle { get; internal set; } = new();
        }

        private const string DefaultOneToOneLabel = "ONE-TO-ONE";
        private const string DefaultOneToManyLabel = "ONE-TO-MANY";
        private const string DefaultIsNullLabel = "[NULL]";
        private const string DefaultNotNullLabel = "[NOT NULL]";

        private readonly Dictionary<Type, EntityOptions> _entityOptions = [];

        /// <summary>Provides options that specify how a column's nullability is depicted on the generated diagram.</summary>
        public sealed class NullableColumn
        {
            /// <summary>Indicates if the nullability of a column is visible on the diagram.</summary>
            public bool IsVisible { get; set; }

            /// <summary>Indicates if each column will be decorated as nullable or non-nullable.</summary>
            public NullableColumnMode Mode { get; set; }

            /// <summary>Specifies the text to decorate a nullable column with when the <see cref="Mode"/> is
            /// <see cref="NullableColumnMode.IsNull"/>.</summary>
            public string IsNullLabel { get; set; } = DefaultIsNullLabel;

            /// <summary>Specifies the text to decorate a non-nullable column with when the <see cref="Mode"/> is
            /// <see cref="NullableColumnMode.NotNull"/>.</summary>
            public string NotNullLabel { get; set; } = DefaultNotNullLabel;
        }

        /// <summary>Provides options for an individual entity that override the global <see cref="Entities"/> options.</summary>
        public sealed class EntityOptions : EntityOptionsBase
        {
        }

        /// <summary>Provides global options for all entities generated in the diagram.</summary>
        public sealed class EntityGlobalOptions : EntityOptionsBase
        {
        }

        /// <summary>Provides cardinality options for all relationships generated in the diagram.</summary>
        public sealed class CardinalityOptions
        {
            /// <summary>Indicates if entity relationships should be decorated with crows foot symbols.</summary>
            public bool ShowCrowsFoot { get; set; } = true;

            /// <summary>Provides the label styling for depicted relationships. To hide the label set its
            /// <see cref="LabelStyle.IsVisible"/> property to <see langword="false"/>.</summary>
            public LabelStyle LabelStyle { get; internal set; } = new();

            /// <summary>The label text for one-to-one relationships.</summary>
            public string OneToOneLabel { get; set; } = DefaultOneToOneLabel;

            /// <summary>The label text for one-to-many relationships.</summary>
            public string OneToManyLabel { get; set; } = DefaultOneToManyLabel;
        }

        /// <summary>Defines global options for all entities generated in the diagram.</summary>
        public EntityGlobalOptions Entities { get; } = new();

        /// <summary>Defines cardinality options for all relationships generated in the diagram.</summary>
        public CardinalityOptions Cardinality { get; } = new();

        /// <summary>Sets options for a single entity that overrides the global <see cref="Entities"/> options.</summary>
        /// <typeparam name="TEntity">The entity type to set option overrides.</typeparam>
        /// <returns>Options for an entity type that override the global <see cref="Entities"/> options.</returns>
        public EntityOptions Entity<TEntity>() where TEntity : class
        {
            return GetEntityOptions(typeof(TEntity));
        }

        /// <summary>Sets options for a single entity that overrides the global <see cref="Entities"/> options.</summary>
        /// <param name="entityType">The entity type to set option overrides.</param>
        /// <returns>Options for an entity type that override the global <see cref="Entities"/> options.</returns>
        public EntityOptions Entity(Type entityType)
        {
            return GetEntityOptions(entityType);
        }

        internal bool TryGetEntityOptions(Type entity, out EntityOptions options)
        {
            return _entityOptions.TryGetValue(entity, out options);
        }

        private EntityOptions GetEntityOptions(Type entity)
        {
            if (!_entityOptions.TryGetValue(entity, out var entityByNameOptions))
            {
                entityByNameOptions = new EntityOptions();
                _entityOptions[entity] = entityByNameOptions;
            }

            return entityByNameOptions;
        }
    }
}