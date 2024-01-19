namespace AllOverIt.Wpf.Threading
{
    /// <summary>An awaitable providing a <see cref="GetAwaiter"/> that returns a <see cref="UIThreadAwaiter"/>.</summary>
    public readonly struct UIThreadAwaitable
    {
        /// <summary>Gets an awaiter for the UI thread that is of type <see cref="UIThreadAwaiter"/>.</summary>
        /// <returns>An awaiter for the UI thread that is of type <see cref="UIThreadAwaiter"/>.</returns>
        public UIThreadAwaiter GetAwaiter()
        {
            return new UIThreadAwaiter();
        }
    }
}