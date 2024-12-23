﻿using System.Diagnostics;

namespace AllOverIt.EntityFrameworkCore.Diagrams.D2
{
    /// <summary>Provides D2 diagram export options.</summary>
    public sealed class D2ErdExportOptions
    {
        private const string DefaultLayoutEngine = "elk";

        /// <summary>The default them to use.</summary>
        public const Theme DefaultTheme = Theme.Neutral;

        /// <summary>The diagram text export filename.</summary>
        public required string DiagramFileName { get; init; }

        /// <summary>The diagram theme.</summary>
        public Theme Theme { get; init; } = DefaultTheme;

        /// <summary>The diagram layout engine.</summary>
        public string LayoutEngine { get; init; } = DefaultLayoutEngine;

        /// <summary>Specifies additional, optional, export formats.</summary>
        public ExportFormat[]? Formats { get; init; }     // Skipped if null / empty

        /// <summary>An optional callback to receive standard output generated by the D2 engine.</summary>
        public DataReceivedEventHandler? StandardOutputHandler { get; init; }

        /// <summary>An optional callback to receive error output generated by the D2 engine.</summary>
        public DataReceivedEventHandler? ErrorOutputHandler { get; init; }
    }
}