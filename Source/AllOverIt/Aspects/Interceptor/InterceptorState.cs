namespace AllOverIt.Aspects.Interceptor
{
    /// <summary>Represents interceptor state that can be provided to intercepted methods.</summary>
    public abstract class InterceptorState
    {
        private class Unit : InterceptorState
        {
        }

        /// <summary>An interceptor state with no value.</summary>
        public static InterceptorState None => new Unit();

        /// <summary>Indicates if the method interception has been handled. If this is set to <see langword="True"/>
        /// within <see cref="InterceptorBase{TServiceType}.BeforeInvoke(System.Reflection.MethodInfo, ref object[], ref object)"/>
        /// then the method of the decorated class will not be called. The
        /// <see cref="InterceptorBase{TServiceType}.AfterInvoke(System.Reflection.MethodInfo, object[], InterceptorState, ref object)"/>
        /// method will always be called.</summary>
        public bool Handled { get; set; }
    }
}
