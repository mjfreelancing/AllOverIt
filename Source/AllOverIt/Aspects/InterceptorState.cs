namespace AllOverIt.Aspects
{


    /// <summary>Represents interceptor state that can be provided to method interceptors.</summary>
    public /*abstract*/ class InterceptorState
    {
        //private class Unit : InterceptorState
        //{
        //}

        ///// <summary>An interceptor state with no value.</summary>
        //public static InterceptorState None => new Unit();

        private object _result;

        /// <summary>Indicates if the interceptor has handled the method intercept. If this has a value
        /// of <see langword="True"/> after the interceptor's <c>BeforeInvoke()</c> call, then the
        /// method on the decorated service will not be called. The interceptor's <c>AfterInvoke()</c>
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
        //internal InterceptorState State { get; private set; }

        //public bool IsHandled
        //{
        //    get => State.IsHandled;
        //    set => State.IsHandled = value;
        //}

        public TResult Result
        {
            get => GetResult<TResult>();
            set => SetResult(value);
        }

        //public InterceptorState()
        //    : this(new InterceptorState())
        //{
        //}

        //public InterceptorState(InterceptorState state)
        //{
        //    State = state.WhenNotNull(nameof(state));
        //}
    }
}
