using AllOverIt.AspNetCore.ValueArray;
using AllOverIt.Extensions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AllOverIt.AspNetCore.ModelBinders
{
    /// <summary>Base class for ValueArrayModelBinder.</summary>
    /// <remarks>Required so a static Regex can be defined in a non-generic class.</remarks>
    public abstract class ValueArrayModelBinderBase
    {
        /// <summary>Splits all values by comma, taking into account quoted values.</summary>
#if !NETSTANDARD2_1
        [System.Diagnostics.CodeAnalysis.SuppressMessage("GeneratedRegex", "SYSLIB1045:Convert to 'GeneratedRegexAttribute'.")]
#endif
        protected static readonly Regex SplitRegex = new(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))", RegexOptions.Compiled);
    }

    /// <summary>Provides a model binder for any <see cref="ValueArray{TType}"/>.</summary>
    /// <typeparam name="TArray">The <see cref="ValueArray{TType}"/> type to bind.</typeparam>
    /// <typeparam name="TType">The type within the array. Must be convertible from a string via <seealso cref="StringExtensions.As{TType}"/>.</typeparam>
    /// <remarks>Only supports arrays of values within a QueryString. The expected format is paramName=Value1,Value2,Value3 with each value quoted if required.</remarks>
    public class ValueArrayModelBinder<TArray, TType> : ValueArrayModelBinderBase, IModelBinder
        where TArray : ValueArray<TType>, new()
    {
        /// <inheritdoc />
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var request = bindingContext.HttpContext.Request;

            // only expecting to support use within query strings
            if (request.QueryString.HasValue && request.Query.TryGetValue(bindingContext.ModelName, out var value))
            {
                try
                {
                    var array = new TArray
                    {
                        Values = SplitRegex.Split(value[0]).SelectAsReadOnlyCollection(item => item.As<TType>())
                    };

                    bindingContext.Result = ModelBindingResult.Success(array);
                }
                catch (Exception exception)
                {
                    bindingContext.ModelState.TryAddModelException(bindingContext.ModelName, exception);
                }
            }

            return Task.CompletedTask;
        }
    }
}
