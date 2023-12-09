namespace AllOverIt.Aspects
{
    /// <summary>Represents interceptor state that can be provided to method interceptors.</summary>
    public abstract class InterceptorState
    {
        private class Unit : InterceptorState
        {
        }

        /// <summary>An interceptor state with no value.</summary>
        public static InterceptorState None => new Unit();

        /// <summary>Indicates if the interceptor has handled the method intercept. If this has a value
        /// of <see langword="True"/> after the interceptor's <c>BeforeInvoke()</c> call, then the
        /// method on the decorated service will not be called. The interceptor's <c>AfterInvoke()</c>
        /// method will always be called.</summary>
        public bool IsHandled { get; set; }
    }
}
