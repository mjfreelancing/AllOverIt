namespace AllOverIt.Aspects
{
    /// <summary>Contains the state of a class or method level interceptor.</summary>
    public class InterceptorState
    {
        private object _result;

        /// <summary>Indicates if the interceptor has handled the intercepted method. If the interceptor's
        /// <c>BeforeInvoke()</c> call returns a state instance with this property set to <see langword="True"/>,
        /// then the method on the decorated service will not be called. The interceptor's <c>AfterInvoke()</c>
        /// method will always be called.</summary>
        public bool IsHandled { get; set; }

        /// <summary>Gets the current result.</summary>
        public object GetResult() => _result;

        /// <summary>Gets the current result cast to a <typeparamref name="TResult"/>.</summary>
        public TResult GetResult<TResult>() => (TResult) _result;

        /// <summary>Sets the current result.</summary>
        /// <typeparam name="TResult">The result type.</typeparam>
        /// <param name="result">The result value.</param>
        public void SetResult<TResult>(TResult result)
        {
            _result = result;
        }
    }

    /// <summary>Contains the state of a class or method level interceptor.</summary>
    /// <typeparam name="TResult">The state's result type.</typeparam>
    public class InterceptorState<TResult> : InterceptorState
    {
        /// <summary>Contains the state's current result.</summary>
        public TResult Result
        {
            get => GetResult<TResult>();
            set => SetResult(value);
        }
    }
}
