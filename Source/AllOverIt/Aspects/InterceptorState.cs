namespace AllOverIt.Aspects
{
    /// <summary>Represents class and method level interceptor state.</summary>
    public class InterceptorState
    {
        private object _result;

        /// <summary>Indicates if the interceptor has handled the intercepted method. If the interceptor's
        /// <c>BeforeInvoke()</c> call returns a state instance with this property set to <see langword="True"/>,
        /// then the method on the decorated service will not be called. The interceptor's <c>AfterInvoke()</c>
        /// method will always be called.</summary>
        public bool IsHandled { get; set; }

        public object GetResult() => _result;

        public TResult GetResult<TResult>() => (TResult) _result;

        public void SetResult<TResult>(TResult result)
        {
            _result = result;
        }
    }

    public class InterceptorState<TResult> : InterceptorState
    {
        public TResult Result
        {
            get => GetResult<TResult>();
            set => SetResult(value);
        }
    }
}
