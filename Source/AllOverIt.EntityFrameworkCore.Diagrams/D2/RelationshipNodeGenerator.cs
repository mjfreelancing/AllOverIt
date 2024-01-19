using AllOverIt.Assertion;
using AllOverIt.EntityFrameworkCore.Diagrams.D2.Extensions;
using AllOverIt.Extensions;
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

        public RelationshipNodeGenerator(ErdOptions options)
        {
            _options = options.WhenNotNull(nameof(options));
        }

        public string CreateNode(PrincipalForeignKey foreignKey, string targetEntityName, string targetColumnName)
        {
            var sourceColumn = $"{foreignKey.EntityName}.{foreignKey.ColumnName}";
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

            if (sourceArrowHead is null &&
                targetArrowHead is null &&
                labelStyle is null)
            {
                return cardinality;
            }

            var sb = new StringBuilder();

            sb.AppendLine($"{cardinality} {{");

            if (sourceArrowHead is not null)
            {
                sb.AppendLine(sourceArrowHead);
            }

            if (targetArrowHead is not null)
            {
                sb.AppendLine(targetArrowHead);
            }

            if (labelStyle is not null)
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
                : null;
        }

        private string GetTargetArrowHead(bool isOneToMany)
        {
            if (_options.Cardinality.ShowCrowsFoot)
            {
                return isOneToMany
                    ? TargetArrowHeadWithCrowsFootOneToMany
                    : TargetArrowHeadWithCrowsFootOneToOne;
            }

            return null;
        }

        private string GetCardinalityLabelStyle()
        {
            return _options.Cardinality.LabelStyle.IsDefault()
                ? null
                : _options.Cardinality.LabelStyle.AsText(2);
        }
    }
}