namespace AllOverIt.Logging
{
    /// <summary>A console logger capable of generating output in color.</summary>
    public interface IColorConsoleLogger
    {
        /// <summary>Writes <paramref name="formattedText"/> to the console at the current position.</summary>
        /// <param name="formattedText">The text, formatted with foreground and/or background color tags, to be sent to the console.</param>
        /// <returns>This <see cref="IColorConsoleLogger"/> instance so additional method calls can be chained.</returns>
        IColorConsoleLogger Write(string formattedText);

        /// <summary>Writes <paramref name="text"/> to the console at the current position using the specified <paramref name="foreColor"/>.</summary>
        /// <param name="foreColor">The foreground color.</param>
        /// <param name="text">The text to write to the console.</param>
        /// <returns>This <see cref="IColorConsoleLogger"/> instance so additional method calls can be chained.</returns>
        IColorConsoleLogger Write(ConsoleColor foreColor, string text);

        /// <summary>Writes <paramref name="text"/> to the console at the current position using the specified <paramref name="foreColor"/>
        /// and <paramref name="backColor"/>.</summary>
        /// <param name="foreColor">The foreground color.</param>
        /// <param name="backColor">The background color.</param>
        /// <param name="text">The text to write to the console.</param>
        /// <returns>This <see cref="IColorConsoleLogger"/> instance so additional method calls can be chained.</returns>
        IColorConsoleLogger Write(ConsoleColor foreColor, ConsoleColor backColor, string text);

        /// <summary>Writes <paramref name="formattedText"/> to the console at the current position appended with a newline.</summary>
        /// <param name="formattedText">The text, formatted with foreground and/or background color tags, to be sent to the console.</param>
        /// <returns>This <see cref="IColorConsoleLogger"/> instance so additional method calls can be chained.</returns>
        IColorConsoleLogger WriteLine(string formattedText);

        /// <summary>Writes <paramref name="text"/> to the console at the current position appended with a newline using the specified
        /// <paramref name="foreColor"/>.</summary>
        /// <param name="foreColor">The foreground color.</param>
        /// <param name="text">The text to write to the console.</param>
        /// <returns>This <see cref="IColorConsoleLogger"/> instance so additional method calls can be chained.</returns>
        IColorConsoleLogger WriteLine(ConsoleColor foreColor, string text);

        /// <summary>Writes <paramref name="text"/> to the console at the current position appended with a newline using the specified
        /// <paramref name="foreColor"/> and <paramref name="backColor"/>.</summary>
        /// <param name="foreColor">The foreground color.</param>
        /// <param name="backColor">The background color.</param>
        /// <param name="text">The text to write to the console.</param>
        /// <returns>This <see cref="IColorConsoleLogger"/> instance so additional method calls can be chained.</returns>
        IColorConsoleLogger WriteLine(ConsoleColor foreColor, ConsoleColor backColor, string text);

        /// <summary>Appends a newline at the current console location.</summary>
        /// <returns>This <see cref="IColorConsoleLogger"/> instance so additional method calls can be chained.</returns>
        IColorConsoleLogger WriteLine();
    }
}