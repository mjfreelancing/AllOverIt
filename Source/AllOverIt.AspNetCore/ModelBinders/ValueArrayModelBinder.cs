using AllOverIt.AspNetCore.ValueArray;
using AllOverIt.Extensions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AllOverIt.AspNetCore.ModelBinders
{
    /// <summary>Provides a model binder for any <see cref="ValuesArray{TType}"/>.</summary>
    /// <typeparam name="TArray">The <see cref="ValuesArray{TType}"/> type to bind.</typeparam>
    /// <typeparam name="TType">The type within the array.</typeparam>
    /// <remarks>Only supports arrays of values within a QueryString. The expected format is [Value1,Value2,Value3], with each value quoted if required.</remarks>
    public class ValueArrayModelBinder<TArray, TType> : IModelBinder
        where TArray : ValueArray<TType>, new()
    {
        private static readonly Regex _regex = new(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))", RegexOptions.Compiled);

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var request = bindingContext.HttpContext.Request;

            // only expecting to support use within query strings
            if (request.QueryString.HasValue)
            {
                if (request.Query.TryGetValue(bindingContext.ModelName, out var value))
                {
                    try
                    {
                        var array = new TArray
                        {
                            // split all values by comma, taking into account quoted values
                            Values = _regex.Split(value[0]).SelectAsReadOnlyCollection(item => item.As<TType>())
                        };

                        bindingContext.Result = ModelBindingResult.Success(array);
                    }
                    catch (Exception exception)
                    {
                        bindingContext.ModelState.TryAddModelException(bindingContext.ModelName, exception);
                    }
                }
            }

            return Task.CompletedTask;
        }
    }
}
