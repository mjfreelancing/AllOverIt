namespace AllOverIt.Aspects.Interceptor
{
    /// <summary>An abstract base class for state that can be provided to intercepted methods.</summary>
    public abstract class InterceptorState
    {
        private class Unit : InterceptorState
        {
        }

        /// <summary>An interceptor state with no value.</summary>
        public static InterceptorState None => new Unit();
    }
}
