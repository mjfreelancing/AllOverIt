using AllOverIt.Assertion;
using AllOverIt.EntityFrameworkCore.Diagrams.D2.Extensions;
using AllOverIt.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Text;

namespace AllOverIt.EntityFrameworkCore.Diagrams.D2
{
    /// <summary>Implements an entity relationship diagram generator for the D2 format.</summary>
    public sealed class D2ErdGenerator : ErdGeneratorBase
    {
        private readonly ErdOptions _options;

        /// <summary>Constructor.</summary>
        /// <param name="options">The entity relationship diagram generator options.</param>
        public D2ErdGenerator(ErdOptions options)
        {
            _options = options.WhenNotNull(nameof(options));
        }

        /// <inheritdoc/>
        public override string GenerateDiagram(DbContext dbContext, EntityColumns entityColumns)
        {
            var sb = new StringBuilder();
            var relationships = new List<string>();

            var dbContextEntityTypes = dbContext.Model.GetEntityTypes().AsReadOnlyCollection();

            var defaultShapeStyle = !_options.Entities.ShapeStyle.IsDefault()
                ? _options.Entities.ShapeStyle.AsText(2)
                : default;

            sb.AppendLine($"direction: {_options.Direction}".ToLowerInvariant());
            sb.AppendLine();

            // Process all groups
            _options.Groups.ForEach((entityGroup, _) =>
            {
                var alias = entityGroup.Key;
                var config = entityGroup.Value;

                var style = config.ShapeStyle.AsText(2);

                sb.AppendLine($"{alias}: {config.Title} {{");
                sb.AppendLine(style);
                sb.AppendLine("}");
                sb.AppendLine();
            });

            // Process all entities
            var entityNodeGenerator = new EntityNodeGenerator(_options, dbContextEntityTypes, defaultShapeStyle);

            foreach (var (entityIdentifier, columns) in entityColumns)
            {
                var entityNode = entityNodeGenerator.CreateNode(entityIdentifier, columns, relationships.Add);

                sb.AppendLine(entityNode);
                sb.AppendLine();
            }

            foreach (var relationship in relationships)
            {
                sb.AppendLine(relationship);
                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}