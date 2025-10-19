using AllOverIt.Assertion;
using AllOverIt.EntityFrameworkCore.Diagrams.D2.Extensions;
using AllOverIt.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Text;

namespace AllOverIt.EntityFrameworkCore.Diagrams.D2
{
    internal sealed class RelationshipNodeGenerator
    {
        private const string ConnectionWithCrowsFoot = "<->";
        private const string ConnectionOneToMany = "->";
        private const string ConnectionOneToOne = "--";

        private const string SourceArrowheadWithCrowsFoot =
            """
              source-arrowhead: {
                shape: cf-one-required
              }
            """;

        private const string TargetArrowHeadWithCrowsFootOneToMany =
            """
              target-arrowhead: {
                shape: cf-many
              }
            """;

        private const string TargetArrowHeadWithCrowsFootOneToOne =
            """
              target-arrowhead: {
                shape: cf-one
              }
            """;

        private readonly ErdOptions _options;
        private readonly IEntityType[] _dbContextEntityTypes;

        public RelationshipNodeGenerator(ErdOptions options, IEntityType[] dbContextEntityTypes)
        {
            _options = options.WhenNotNull();
            _dbContextEntityTypes = dbContextEntityTypes.WhenNotNullOrEmpty().AsArray();
        }

        public string CreateNode(PrincipalForeignKey foreignKey, string targetEntityName, string targetColumnName)
        {
            var entityName = foreignKey.EntityName;

            // For shadow entities (e.g., join tables), the Type is Dictionary<string, object> which won't match in the Groups
            // Groups. We need to find the actual IEntityType by table name and then look up its group by the ClrType.
            var principalEntityType = _dbContextEntityTypes.Single(entity => entity.GetTableName() == foreignKey.EntityName);

            var groupAlias = _options.Groups.GetAlias(principalEntityType.ClrType);

            if (groupAlias is not null)
            {
                entityName = $"{groupAlias}.{entityName}";
            }

            var sourceColumn = $"{entityName}.{foreignKey.ColumnName}";
            var targetColumn = $"{targetEntityName}.{targetColumnName}";
            var connection = GetConnection(foreignKey);
            var cardinalityNode = CreateCardinalityNode(foreignKey);

            return cardinalityNode.IsNullOrEmpty()
                ? $"{sourceColumn} {connection} {targetColumn}"
                : $"{sourceColumn} {connection} {targetColumn}: {cardinalityNode}";
        }

        private string GetConnection(PrincipalForeignKey foreignKey)
        {
            if (_options.Cardinality.ShowCrowsFoot)
            {
                return ConnectionWithCrowsFoot;
            }

            return foreignKey.IsOneToMany
                ? ConnectionOneToMany
                : ConnectionOneToOne;
        }

        private string CreateCardinalityNode(PrincipalForeignKey foreignKey)
        {
            var cardinality = string.Empty;

            if (_options.Cardinality.LabelStyle.IsVisible)
            {
                cardinality = foreignKey.IsOneToMany
                    ? _options.Cardinality.OneToManyLabel.D2EscapeString()
                    : _options.Cardinality.OneToOneLabel.D2EscapeString();
            }

            var sourceArrowHead = GetSourceArrowHead();
            var targetArrowHead = GetTargetArrowHead(foreignKey.IsOneToMany);
            var labelStyle = GetCardinalityLabelStyle();

            if (sourceArrowHead.IsNullOrEmpty() &&
                targetArrowHead.IsNullOrEmpty() &&
                labelStyle.IsNullOrEmpty())
            {
                return cardinality;
            }

            var sb = new StringBuilder();

            sb.AppendLine($"{cardinality} {{");

            if (sourceArrowHead.IsNotNullOrEmpty())
            {
                sb.AppendLine(sourceArrowHead);
            }

            if (targetArrowHead.IsNotNullOrEmpty())
            {
                sb.AppendLine(targetArrowHead);
            }

            if (labelStyle.IsNotNullOrEmpty())
            {
                sb.AppendLine(labelStyle);
            }

            sb.Append('}');

            return sb.ToString();
        }

        private string GetSourceArrowHead()
        {
            return _options.Cardinality.ShowCrowsFoot
                ? SourceArrowheadWithCrowsFoot
                : string.Empty;
        }

        private string GetTargetArrowHead(bool isOneToMany)
        {
            if (_options.Cardinality.ShowCrowsFoot)
            {
                return isOneToMany
                    ? TargetArrowHeadWithCrowsFootOneToMany
                    : TargetArrowHeadWithCrowsFootOneToOne;
            }

            return string.Empty;
        }

        private string GetCardinalityLabelStyle()
        {
            return _options.Cardinality.LabelStyle.AsText(2);
        }
    }
}