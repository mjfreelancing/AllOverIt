using AllOverIt.Assertion;
using AllOverIt.EntityFrameworkCore.Diagrams.Exceptions;
using AllOverIt.Extensions;
using System.Collections;

namespace AllOverIt.EntityFrameworkCore.Diagrams
{
    /// <summary>Provides options that define how the entity relationship diagram will be created.</summary>
    public sealed class ErdOptions
    {
        private sealed class EntityGroups : IEntityGroups
        {
            private readonly Dictionary<string, EntityGroup> _aliasGroups = [];     // group to definition with collection of entities
            private readonly Dictionary<Type, string> _entityGroupAliases = [];     // entity type to group

            IEntityGroups IEntityGroups.Add(string alias, EntityGroup groupEntities)
            {
                if (_aliasGroups.ContainsKey(alias))
                {
                    throw new DiagramException($"The group alias '{alias}' already exists.");
                }

                _aliasGroups.Add(alias, groupEntities);

                foreach (var groupEntity in groupEntities.EntityTypes)
                {
                    if (_entityGroupAliases.TryGetValue(groupEntity, out var entityAlias))
                    {
                        throw new DiagramException($"The entity type '{groupEntity.GetFriendlyName()}' is already associated with group alias '{entityAlias}'.");
                    }

                    _entityGroupAliases.Add(groupEntity, alias);
                }

                return this;
            }

            string IEntityGroups.GetAlias(Type entityType)
            {
                if (_entityGroupAliases.TryGetValue(entityType, out var alias))
                {
                    return alias;
                }

                return null;
            }

            IEnumerator<KeyValuePair<string, EntityGroup>> IEnumerable<KeyValuePair<string, EntityGroup>>.GetEnumerator()
            {
                return _aliasGroups.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return ((IEnumerable<KeyValuePair<string, EntityGroup>>) this).GetEnumerator();
            }
        }

        /// <summary>Diagram direction options.</summary>
        public enum DiagramDirection
        {
            /// <summary>The direction flows towards left.</summary>
            Left,

            /// <summary>The direction flows towards right.</summary>
            Right,

            /// <summary>The direction flows towards up.</summary>
            Up,

            /// <summary>The direction flows towards down.</summary>
            Down
        }

        /// <summary>Provides entity specific options that can be applied to a single entity, or globally.</summary>
        public abstract class EntityOptionsBase
        {
            /// <summary>Determines whether the entity columns are exported in the same order as they are declared. Default is <see langword="True"/>.</summary>
            public bool PreserveColumnOrder { get; set; } = true;

            /// <summary>Specifies how each entity column nullability is depicted on the generated diagram.</summary>
            public NullableColumn Nullable { get; } = new();

            /// <summary>Indicates if a column's maximum length should be depicted on the generated diagram.</summary>
            public bool ShowMaxLength { get; set; } = true;

            /// <summary>Provides styling options for an entity shape.</summary>
            public ShapeStyle ShapeStyle { get; internal set; } = new();

            internal void CopyFrom(EntityOptionsBase source)
            {
                Nullable.CopyFrom(source.Nullable);
                ShowMaxLength = source.ShowMaxLength;
                ShapeStyle.CopyFrom(source.ShapeStyle);
                PreserveColumnOrder = source.PreserveColumnOrder;
            }
        }

        /// <summary>Represents a collection of entity groups, where each group contains one or more entities.</summary>
        public interface IEntityGroups : IEnumerable<KeyValuePair<string, EntityGroup>>
        {
            /// <summary>Adds another group of entities, associated with a unique alias.</summary>
            /// <param name="alias">The unique alias to associate with the group of entities.</param>
            /// <param name="entityGroup">Contains information about the group, including the collection of associated entities.</param>
            /// <returns>The same instance so additional calls can be chained.</returns>
            IEntityGroups Add(string alias, EntityGroup entityGroup);

            /// <summary>Gets the alias associated with a specified entity type.</summary>
            /// <param name="entityType">The entity type to get the alias for.</param>
            /// <returns>The alias associated with a specified entity type.</returns>
            string GetAlias(Type entityType);
        }

        /// <summary>Provides options that specify how a column's nullability is depicted on the generated diagram.</summary>
        public sealed class NullableColumn
        {
            /// <summary>Indicates if the nullability of a column is visible on the diagram.</summary>
            public bool IsVisible { get; set; } = true;

            /// <summary>Indicates if each column will be decorated as nullable or non-nullable.</summary>
            public NullableColumnMode Mode { get; set; } = NullableColumnMode.NotNull;

            /// <summary>Specifies the text to decorate a nullable column with when the <see cref="Mode"/> is
            /// <see cref="NullableColumnMode.IsNull"/>.</summary>
            public string IsNullLabel { get; set; } = DefaultIsNullLabel;

            /// <summary>Specifies the text to decorate a non-nullable column with when the <see cref="Mode"/> is
            /// <see cref="NullableColumnMode.NotNull"/>.</summary>
            public string NotNullLabel { get; set; } = DefaultNotNullLabel;

            internal void CopyFrom(NullableColumn source)
            {
                IsVisible = source.IsVisible;
                Mode = source.Mode;
                IsNullLabel = source.IsNullLabel;
                NotNullLabel = source.NotNullLabel;
            }
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

        /// <summary>Contains a group of entities and associated styling attributes for the diagram.</summary>
        public sealed class EntityGroup
        {
            private readonly List<Type> _entityTypes = [];

            /// <summary>The group's title. Set to <see langword="null"/> if not required.</summary>
            public string Title { get; }

            /// <summary>Contains styling options to use for the group in the generated diagram.</summary>
            public ShapeStyle ShapeStyle { get; }

            /// <summary>The entity types associated with the group.</summary>
            public IReadOnlyCollection<Type> EntityTypes => _entityTypes;

            /// <summary>Constructor.</summary>
            /// <param name="title">The group's title. Set to <see langword="null"/> if not required.</param>
            /// <param name="shapeStyle">Contains styling options to use for the group in the generated diagram.</param>
            public EntityGroup(string title, ShapeStyle shapeStyle)
            {
                Title = title.WhenNotNullOrEmpty();
                ShapeStyle = shapeStyle.WhenNotNull();
            }

            /// <summary>Adds an entity type to the group.</summary>
            /// <typeparam name="TEntity">The entity type to add to the group.</typeparam>
            /// <returns>The entity group to allow for chained calls.</returns>
            public EntityGroup Add<TEntity>() where TEntity : class
            {
                _entityTypes.Add(typeof(TEntity));

                return this;
            }
        }

        private const string DefaultOneToOneLabel = "ONE-TO-ONE";
        private const string DefaultOneToManyLabel = "ONE-TO-MANY";
        private const string DefaultIsNullLabel = "[NULL]";
        private const string DefaultNotNullLabel = "[NOT NULL]";

        private readonly Dictionary<Type, EntityOptions> _entityOptions = [];
        private readonly EntityGroups _groupEntities = new();

        /// <summary>Specifies the direction the diagram flows towards.</summary>
        public DiagramDirection Direction { get; set; } = DiagramDirection.Left;

        /// <summary>Defines global options for all entities generated in the diagram.</summary>
        public EntityGlobalOptions Entities { get; } = new();

        /// <summary>Defines cardinality options for all relationships generated in the diagram.</summary>
        public CardinalityOptions Cardinality { get; } = new();

        /// <summary>The groups of entities.</summary>
        public IEntityGroups Groups => _groupEntities;

        /// <summary>Creates a new grouping of entity types using a default style.</summary>
        /// <param name="alias">The alias to use in the diagram file for the group.</param>
        /// <param name="title">The group title. Optional.</param>
        /// <param name="entities">An action that adds the required entities to the group.</param>
        public void Group(string alias, string title, Action<EntityGroup> entities)
        {
            // Not using ShapeStyle.Default because the caller can change properties
            Group(alias, title, new ShapeStyle(), entities);
        }

        /// <summary>Creates a new grouping of entity types.</summary>
        /// <param name="alias">The alias to use in the diagram file for the group.</param>
        /// <param name="title">The group title. Optional.</param>
        /// <param name="shapeStyle">The styling options for the group shape.</param>
        /// <param name="entities">An action that adds the required entities to the group.</param>
        public void Group(string alias, string title, ShapeStyle shapeStyle, Action<EntityGroup> entities)
        {
            _ = alias.WhenNotNullOrEmpty();
            _ = entities.WhenNotNull();

            if (title.IsNullOrEmpty())
            {
                title = "\"\"";
            }

            var groupEntities = new EntityGroup(title, shapeStyle);

            entities.Invoke(groupEntities);

            Groups.Add(alias, groupEntities);
        }

        /// <summary>Sets options for a single entity that overrides the global <see cref="Entities"/> options.</summary>
        /// <typeparam name="TEntity">The entity type to set option overrides.</typeparam>
        /// <param name="copyGlobal">If <see langword="True"/> at the time of the initial call for the specified <typeparamref name="TEntity"/>,
        /// the <see cref="EntityOptions"/> returned will be pre-configured with the same options as currently defined on the global
        /// <see cref="Entities"/> property. Default is <see langword="True"/>.</param>
        /// <returns>Options for an entity type that override the global <see cref="Entities"/> options.</returns>
        public EntityOptions Entity<TEntity>(bool copyGlobal = true) where TEntity : class
        {
            return GetEntityOptions(typeof(TEntity), copyGlobal);
        }

        /// <summary>Sets options for a single entity that overrides the global <see cref="Entities"/> options.</summary>
        /// <param name="entityType">The entity type to set option overrides.</param>
        /// <param name="copyGlobal">If <see langword="True"/> at the time of the initial call for the specified <paramref name="entityType"/>,
        /// the <see cref="EntityOptions"/> returned will be pre-configured with the same options as currently defined on the global
        /// <see cref="Entities"/> property. Default is <see langword="True"/>.</param>
        /// <returns>Options for an entity type that override the global <see cref="Entities"/> options.</returns>
        public EntityOptions Entity(Type entityType, bool copyGlobal = true)
        {
            return GetEntityOptions(entityType, copyGlobal);
        }

        internal bool TryGetEntityOptions(Type entity, out EntityOptions options)
        {
            return _entityOptions.TryGetValue(entity, out options);
        }

        private EntityOptions GetEntityOptions(Type entity, bool copyGlobal)
        {
            if (!_entityOptions.TryGetValue(entity, out var entityByNameOptions))
            {
                entityByNameOptions = new EntityOptions();

                if (copyGlobal)
                {
                    entityByNameOptions.CopyFrom(Entities);
                }

                _entityOptions[entity] = entityByNameOptions;
            }

            return entityByNameOptions;
        }
    }
}