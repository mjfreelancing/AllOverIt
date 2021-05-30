using AllOverIt.Expressions;
using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("AllOverIt.Tests")]

namespace AllOverIt.Reflection
{
    internal static class BindingOptionsHelper
    {
        internal static Func<MethodBase, bool> BuildBindingPredicate(BindingOptions binding)
        {
            // Must contain at least one from each group (groups are AND'd and items within group are OR'd):
            // * Static, Instance
            // * Abstract, Virtual, NonVirtual
            // * Public, Protected, Private, Internal

            // set up defaults for each group
            if ((binding & BindingOptions.AllScope) == 0)
            {
                binding |= BindingOptions.DefaultScope;
            }

            if ((binding & BindingOptions.AllAccessor) == 0)
            {
                binding |= BindingOptions.DefaultAccessor;
            }

            if ((binding & BindingOptions.AllVisibility) == 0)
            {
                binding |= BindingOptions.DefaultVisibility;
            }

            // calls such as binding.HasFlag(BindingOptions.Static) are slower than using bitwise operations (See Code Analysis warning RCS1096)
            var scopePredicate = OrBindProperty(null, () => (binding & BindingOptions.Static) != 0, info => info.IsStatic)
              .OrBindProperty(() => (binding & BindingOptions.Instance) != 0, info => !info.IsStatic);

            var accessorPredicate = OrBindProperty(null, () => (binding & BindingOptions.Abstract) != 0, info => info.IsAbstract)
              .OrBindProperty(() => (binding & BindingOptions.Virtual) != 0, info => info.IsVirtual)
              .OrBindProperty(() => (binding & BindingOptions.NonVirtual) != 0, info => !info.IsVirtual);

            var visibilityPredicate = OrBindProperty(null, () => (binding & BindingOptions.Public) != 0, info => info.IsPublic)
              .OrBindProperty(() => (binding & BindingOptions.Protected) != 0, info => info.IsFamily)
              .OrBindProperty(() => (binding & BindingOptions.Private) != 0, info => info.IsPrivate)
              .OrBindProperty(() => (binding & BindingOptions.Internal) != 0, info => info.IsAssembly);

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