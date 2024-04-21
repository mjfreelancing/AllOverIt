using AllOverIt.Patterns.Result;
using AllOverIt.Patterns.Result.Extensions;
using System.Diagnostics;

namespace ResultOrErrorDemo.Errors;

internal class Program
{
    private static void Main(/*string[] args*/)
    {
        ShowSuccessWithNoResult();                  // EnrichedResult
        ShowSuccessWithNoTypedResult();             // EnrichedResult<int>
        ShowSuccessWithResults();                   // EnrichedResult<int?>
        ShowFailWithNoError();                      // EnrichedResult with Error = null
        ShowFailUsingFactoryBasedError();           // EnrichedResult with Error = EnrichedError<AppErrorType>
        ShowFailUsingStaticError();                 // EnrichedResult with Error = UnexpectedError (which is a EnrichedError<AppErrorType>)
        ShowFailUsingStronglyTypedError();          // EnrichedResult with Error = ValidationError (which is a EnrichedError<AppErrorType>)
        ShowFailWithAggregateError();
        ShowMatchWithResult();
        ShowMatchWithError();
    }

    private static void ShowSuccessWithNoResult()
    {
        var result = EnrichedResult.Success();

        if (result.IsSuccess)
        {
            Console.WriteLine("ShowSuccessWithNoResult - Passed");
        }
        else
        {
            throw new UnreachableException();
        }
    }

    private static void ShowSuccessWithNoTypedResult()
    {
        var result = EnrichedResult.Success<int>();

        if (result.IsSuccess)
        {
            Console.WriteLine("ShowSuccessWithNoTypedResult - Passed");
        }
        else
        {
            throw new UnreachableException();
        }
    }

    private static void ShowSuccessWithResults()
    {
        int? value = 42;

        var result1 = EnrichedResult.Success(value);

        if (result1.IsSuccess)
        {
            Console.WriteLine($"ShowSuccessWithResults - Passed with {result1.Value}");
        }
        else
        {
            throw new UnreachableException();
        }

        var result2 = EnrichedResult.Success("Bingo !");

        if (result2.IsSuccess)
        {
            Console.WriteLine($"ShowSuccessWithResults - Passed with {result2.Value}");
        }
        else
        {
            throw new UnreachableException();
        }
    }

    private static void ShowFailWithNoError()
    {
        var result = EnrichedResult.Fail();

        if (result.IsError)
        {
            Console.WriteLine("ShowFailWithNoError - Passed");
        }
        else
        {
            throw new UnreachableException();
        }
    }

    private static void ShowFailUsingFactoryBasedError()
    {
        // BadRequest() accepts a code and description
        var result = EnrichedResult.Fail(AppErrors.BadRequest());

        if (result.IsError)
        {
            switch (result.Error!.Type)
            {
                case "BadRequest":
                    Console.WriteLine($"ShowFailUsingFactoryBasedError - Passed with '{result.Error.Description}'");
                    break;

                default:
                    throw new UnreachableException();
            }
        }
        else
        {
            throw new UnreachableException();
        }
    }

    private static void ShowFailUsingStaticError()
    {
        // The Type, Code, and Description are all fixed
        var result = EnrichedResult.Fail(AppErrors.Unexpected);

        if (result.IsError)
        {
            var description = result.Error switch
            {
                UnexpectedError => result.Error.Description,
                _ => throw new UnreachableException()
            };

            Console.WriteLine($"ShowFailUsingStaticError - Passed with '{description}'");
        }
        else
        {
            throw new UnreachableException();
        }
    }

    private static void ShowFailUsingStronglyTypedError()
    {
        // The Type, Code, and Description are all fixed
        var result = EnrichedResult.Fail(AppErrors.Validation);

        if (result.IsError)
        {
            var errorType = (EnrichedError<AppErrorType>) result.Error!;

            var description = errorType.ErrorType switch
            {
                AppErrorType.Validation => result.Error!.Description,
                _ => throw new UnreachableException()
            };

            Console.WriteLine($"ShowFailUsingStronglyTypedError - Passed with '{description}'");
        }
        else
        {
            throw new UnreachableException();
        }
    }

    private static void ShowFailWithAggregateError()
    {
        var error1 = EnrichedResult.Fail(AppErrors.Unexpected).Error!;
        var error2 = EnrichedResult.Fail(AppErrors.Validation).Error!;
        var error3 = EnrichedResult.Fail(new EnrichedError("some type", "some description")).Error!;

        // There are additional overloads that allow a type, code, description to be given to the aggregate.
        var aggregate = EnrichedError.Aggregate(error1, error2, error3);

        var descriptions = aggregate.Errors.Select(error => error.Description);

        Console.WriteLine($"ShowFailWithAggregateError: {string.Join(", ", descriptions)}");
    }

    private static void ShowMatchWithResult()
    {
        var result = EnrichedResult.Success(100);

        // Showing that we can even return a different result type
        var matched = result.Match(
            result => EnrichedResult.Success($"ShowMatchWithResult - Passed with {result.Value}"),
            result => throw new UnreachableException());

        Console.WriteLine(matched.Value);
    }

    private static void ShowMatchWithError()
    {
        // Shows the creation of an EnrichedResult<int> with an EnrichedError<byte>
        var result = EnrichedResult.Fail<int, byte>(255, "Some random error");

        // Showing that we can even return a different result type
        result.Switch(
            result => throw new UnreachableException(),
            result => Console.WriteLine($"ShowMatchWithError - Passed with {result.Error!.Description}"));
    }


}