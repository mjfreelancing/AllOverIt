using AllOverIt.Caching;
using AllOverIt.Expressions;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace AllOverIt.Reflection
{
    internal static class BindingOptionsHelper
    {
        internal static Func<MethodBase, bool> BuildBindingPredicate(BindingOptions bindingOptions)
        {
            var key = new GenericCacheKey<IReflectionCacheKey<BindingOptions>, BindingOptions>(null, bindingOptions);

            return GenericCache.Default.GetOrAdd(key, cacheKey =>
            {
                var (_, options) = (GenericCacheKey<IReflectionCacheKey<BindingOptions>, BindingOptions>) cacheKey;

                // set up defaults for each group
                if ((options & BindingOptions.AllScope) == 0)
                {
                    options |= BindingOptions.DefaultScope;
                }

                if ((options & BindingOptions.AllAccessor) == 0)
                {
                    options |= BindingOptions.DefaultAccessor;
                }

                if ((options & BindingOptions.AllVisibility) == 0)
                {
                    options |= BindingOptions.DefaultVisibility;
                }

                // calls such as bindingOptions.HasFlag(BindingOptions.Static) are slower than using bitwise operations (See Code Analysis warning RCS1096)
                var scopePredicate = OrBindProperty(null, () => (options & BindingOptions.Static) != 0, info => info.IsStatic)
                    .OrBindProperty(() => (options & BindingOptions.Instance) != 0, info => !info.IsStatic);

                var accessorPredicate = OrBindProperty(null, () => (options & BindingOptions.Abstract) != 0, info => info.IsAbstract)
                    .OrBindProperty(() => (options & BindingOptions.Virtual) != 0, info => info.IsVirtual)
                    .OrBindProperty(() => (options & BindingOptions.NonVirtual) != 0, info => !info.IsVirtual);

                var visibilityPredicate = OrBindProperty(null, () => (options & BindingOptions.Public) != 0, info => info.IsPublic)
                    .OrBindProperty(() => (options & BindingOptions.Protected) != 0, info => info.IsFamily)
                    .OrBindProperty(() => (options & BindingOptions.Private) != 0, info => info.IsPrivate)
                    .OrBindProperty(() => (options & BindingOptions.Internal) != 0, info => info.IsAssembly);

                return scopePredicate.And(accessorPredicate).And(visibilityPredicate).Compile();
            });
        }

        private static Expression<Func<MethodBase, bool>> OrBindProperty(this Expression<Func<MethodBase, bool>> expression,
            Func<bool> predicate, Expression<Func<MethodBase, bool>> creator)
        {
            if (!predicate.Invoke())
            {
                return expression;
            }

            return expression == null
              ? PredicateBuilder.Where(creator)
              : expression.Or(creator);
        }
    }
}