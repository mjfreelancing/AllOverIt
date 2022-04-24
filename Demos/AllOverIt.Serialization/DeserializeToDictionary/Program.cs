#define USE_NEWTSONSOFT                 // Comment out to use System.Text
using System;
using System.Linq;
using AllOverIt.Extensions;
using AllOverIt.Serialization.JsonHelper.Extensions;

#if USE_NEWTSONSOFT
using AllOverIt.Serialization.NewtonsoftJson;
#else
using AllOverIt.Serialization.SystemTextJson;
#endif

namespace DeserializeToDictionary
{
    internal class Program
    {
        static void Main()
        {
            ProcessObject1();
            Console.WriteLine();

            ProcessObject2();
            Console.WriteLine();

            Console.WriteLine();
            Console.WriteLine("All Over It.");
            Console.ReadKey();
        }

        private static void ProcessObject1()
        {
            var anonymousObject = GetObject1ToProcess();

            var jsonHelper = new JsonHelper(anonymousObject);

            // Each of the queries below return the same result

            //var errorMessages = jsonHelper
            //    .GetArray("errors")
            //    .SelectMany(error => error.GetArray("errorInfo"))
            //    .SelectAsReadOnlyCollection(errorInfo => errorInfo.GetValue<string>("errorMessage"));

            //var errorMessages = jsonHelper
            //    .GetArray("errors")
            //    .SelectMany(error => error.GetArrayValues<string>("errorInfo", "errorMessage"))
            //    .AsReadOnlyCollection();

            //var errorMessages = jsonHelper
            //    .GetArray("errors")
            //    .GetChildArray("errorInfo")
            //    .SelectAsReadOnlyCollection(errorInfo => errorInfo.GetValue<string>("errorMessage"));

            //var errorMessages = jsonHelper
            //    .GetArray("errors")
            //    .GetChildArrayValues<string>(new[] { "errorInfo" }, "errorMessage")
            //    .AsReadOnlyCollection();

            //var errorMessages = jsonHelper
            //    .GetChildArray("errors", "errorInfo")
            //    .SelectAsReadOnlyCollection(errorInfo => errorInfo.GetValue<string>("errorMessage"));

            var errorMessages = jsonHelper.GetDescendantObjectArrayValues<string>(new[] { "errors", "errorInfo" }, "errorMessage");

            foreach (var error in errorMessages)
            {
                Console.WriteLine(error);
            }
        }

        private static void ProcessObject2()
        {
            var arrayData = GetObject2ToProcess();

            // Create a dummy anonymous object to process it
            var anonymousObject = new
            {
                errors = arrayData
            };

            var jsonHelper = new JsonHelper(anonymousObject);

            // This will get ALL 'errorMessage' properties
            // var errorMessages = jsonHelper.GetDescendantObjectArrayValues<string>(new[] { "errors", "errorInfo" }, "errorMessage");

            // This demonstrates how to query and drill down
            var objectArray = jsonHelper.GetDescendantObjectArray(new[] { "errors"});

            var errorMessages =
                from element in objectArray
                where element.GetValue("errorCode").As<int>() == 401
                select element.GetDescendantObjectArrayValues<string>(new[] {"errorInfo"}, "errorMessage");

            foreach (var error in errorMessages.SelectMany(item => item))
            {
                Console.WriteLine(error);
            }
        }

        private static object GetObject1ToProcess()
        {
            return new
            {
                data = new
                {
                    queryPerson = (object) null
                },
                errors = new[]
                {
                    new
                    {
                        path = new[]{ "queryPerson" },
                        data = new
                        {
                            fullName = (string)null,
                            childNames = (string[])null
                        },
                        errorInfo = new[]
                        {
                            new
                            {
                                field = "id",
                                attemptedValue = "000-001",
                                ErrorMessage = "The id format is invalid."          // <<== querying for this - note the name is UpperCamelCase here
                            },
                            new
                            {
                                field = "dob",
                                attemptedValue = "00-00-0000",
                                ErrorMessage = "Invalid date."                     // <<== querying for this - note the name is UpperCamelCase here
                            }
                        }
                    }
                }
            };
        }

        private static object GetObject2ToProcess()
        {
            // Note: This is an array - not an array property
            return new[]
            {
                new
                {
                    errorCode = 422,
                    errorInfo = new[]
                    {
                        new
                        {
                            field = "id",
                            attemptedValue = "000-001",
                            ErrorMessage = "The id format is invalid."  // <<== querying for this - note the name is UpperCamelCase here
                        },
                        new
                        {
                            field = "dob",
                            attemptedValue = "00-00-0000",
                            ErrorMessage = "Invalid date."              // <<== querying for this - note the name is UpperCamelCase here
                        }
                    }
                },
                new
                {
                    errorCode = 401,
                    errorInfo = new[]
                    {
                        new
                        {
                            field = "id",
                            attemptedValue = "000-001",
                            ErrorMessage = "A bad value #1"             // <<== querying for this - note the name is UpperCamelCase here
                        },
                        new
                        {
                            field = "dob",
                            attemptedValue = "00-00-0000",
                            ErrorMessage = "A bad value #2"             // <<== querying for this - note the name is UpperCamelCase here
                        }
                    }
                }
            };
        }
    }
}
