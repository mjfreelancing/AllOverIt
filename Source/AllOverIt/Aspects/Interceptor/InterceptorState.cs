namespace AllOverIt.Aspects.Interceptor
{
    /// <summary>Represents interceptor state that can be provided to method interceptors.</summary>
    public abstract class InterceptorState
    {
        private class Unit : InterceptorState
        {
        }

        /// <summary>An interceptor state with no value.</summary>
        public static InterceptorState None => new Unit();
    }
}
