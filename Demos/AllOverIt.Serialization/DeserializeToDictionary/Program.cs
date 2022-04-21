#define USE_NEWTSONSOFT                 // Comment out to use System.Text
using System;

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
            var anonymousObject = GetObjectToProcess();

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

            var errorMessages = jsonHelper.GetChildArrayValues<string>(new[] { "errors", "errorInfo" }, "errorMessage");

            foreach (var error in errorMessages)
            {
                Console.WriteLine(error);
            }

            Console.WriteLine();
            Console.WriteLine("All Over It.");
            Console.ReadKey();
        }

        private static object GetObjectToProcess()
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
                                errorMessage = "The id format is invalid."          // <<== querying for this
                            },
                            new
                            {
                                field = "dob",
                                attemptedValue = "00-00-0000",
                                errorMessage = "Invalid date."                     // <<== querying for this
                            }
                        }
                    }
                }
            };
        }
    }
}
