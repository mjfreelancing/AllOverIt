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
            // set up defaults for each group
            if ((bindingOptions & BindingOptions.AllScope) == 0)
            {
                bindingOptions |= BindingOptions.DefaultScope;
            }

            if ((bindingOptions & BindingOptions.AllAccessor) == 0)
            {
                bindingOptions |= BindingOptions.DefaultAccessor;
            }

            if ((bindingOptions & BindingOptions.AllVisibility) == 0)
            {
                bindingOptions |= BindingOptions.DefaultVisibility;
            }

            // calls such as bindingOptions.HasFlag(BindingOptions.Static) are slower than using bitwise operations (See Code Analysis warning RCS1096)
            var scopePredicate = OrBindProperty(null, () => (bindingOptions & BindingOptions.Static) != 0, info => info.IsStatic)
                .OrBindProperty(() => (bindingOptions & BindingOptions.Instance) != 0, info => !info.IsStatic);

            var accessorPredicate = OrBindProperty(null, () => (bindingOptions & BindingOptions.Abstract) != 0, info => info.IsAbstract)
                .OrBindProperty(() => (bindingOptions & BindingOptions.Virtual) != 0, info => info.IsVirtual)
                .OrBindProperty(() => (bindingOptions & BindingOptions.NonVirtual) != 0, info => !info.IsVirtual);

            var visibilityPredicate = OrBindProperty(null, () => (bindingOptions & BindingOptions.Public) != 0, info => info.IsPublic)
                .OrBindProperty(() => (bindingOptions & BindingOptions.Protected) != 0, info => info.IsFamily)
                .OrBindProperty(() => (bindingOptions & BindingOptions.Private) != 0, info => info.IsPrivate)
                .OrBindProperty(() => (bindingOptions & BindingOptions.Internal) != 0, info => info.IsAssembly);

            return scopePredicate.And(accessorPredicate).And(visibilityPredicate).Compile();
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