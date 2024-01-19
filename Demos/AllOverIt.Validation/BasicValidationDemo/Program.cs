using BasicValidationDemo.Models;
using BasicValidationDemo.Validators;
using FluentValidation.Results;
using System;
using System.Linq;

namespace BasicValidationDemo
{
    class Program
    {
        static void Main()
        {
            var person = new Person();

            // Example 1
            var isValidPerson = new IsValidPersonValidator();
            var isValidPersonResult = isValidPerson.Validate(person);
            PrintResult(nameof(IsValidPersonValidator), isValidPersonResult);


            // Example 2
            var isValidContextPerson = new IsValidPersonContextValidator();

            var personContext = new PersonContext
            {
                MinimumAge = 18,
                LastNameIsOptional = false
            };

            // This method constructs the ValidationContext<Person> and attaches 'personContext' onto the RootDataContext.
            // The rule gets this context back by using the extension GetContextData() method.
            var isValidPersonContextResult = isValidContextPerson.Validate(person, personContext);

            PrintResult(nameof(IsValidPersonContextValidator), isValidPersonContextResult);


            // Example 3
            var personWithIdValidator = new PersonWithIdValidator();        // a validator that uses a tuple to allow a Guid to also be validated
            person.Age = 28;
            var personWithIdResult = personWithIdValidator.Validate((person, Guid.Empty));

            PrintResult(nameof(PersonWithIdValidator), personWithIdResult);


            Console.WriteLine();
            Console.WriteLine("All Over It.");
            Console.ReadKey();
        }

        private static void PrintResult(string message, ValidationResult result)
        {
            if (result.IsValid)
            {
                Console.WriteLine($"{message}: Is Valid");
            }
            else
            {
                var errorMessages = string.Join(Environment.NewLine, result.Errors.Select(item => item.ErrorMessage));
                Console.WriteLine($"{message}:{Environment.NewLine}{errorMessages}");
            }

            Console.WriteLine();
        }
    }
}
