using AllOverIt.Patterns.Result;
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
            switch (result.Error.Type)
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
}