#nullable enable

using System.Diagnostics.CodeAnalysis;

namespace AllOverIt.Patterns.Result
{
    /// <summary>Represents a void result type.</summary>
    [ExcludeFromCodeCoverage]
    public readonly struct VoidType
    {
        /// <summary>A static instance of <see cref="VoidType"/>.</summary>
        public static readonly VoidType Default = new();
    }
}
