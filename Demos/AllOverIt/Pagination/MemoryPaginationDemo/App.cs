using AllOverIt.Assertion;
using AllOverIt.Extensions;
using AllOverIt.GenericHost;
using AllOverIt.Pagination;
using AllOverIt.Pagination.Extensions;
using Bogus;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MemoryPaginationDemo
{
    public sealed class App : ConsoleAppBase
    {
        // Cannot use 'Person' as it contains fields - can only use properties in queries 
        private class PersonModel
        {
            public string FirstName { get; }
            public string LastName { get; }

            public PersonModel(Person person)
            {
                FirstName = person.FirstName;
                LastName = person.LastName;
            }
        }

        private class ContinuationTokens
        {
            public string Current { get; set; }
            public string Next { get; set; }
            public string Previous { get; set; }
        }

        private readonly IQueryPaginatorFactory _queryPaginatorFactory; 
        private readonly ILogger<App> _logger;

        public App(IQueryPaginatorFactory queryPaginatorFactory, ILogger<App> logger)
        {
            _queryPaginatorFactory = queryPaginatorFactory.WhenNotNull(nameof(queryPaginatorFactory));
            _logger = logger.WhenNotNull(nameof(logger));
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            const int DataSize = 100_000;
            const int PageSize = 25;

            Console.WriteLine("Adding data...");

            var faker = new Faker<Person>().CustomInstantiator(_ => new Person("en"));

            var persons = new List<PersonModel>(DataSize);

            for (var i = 1; i <= DataSize; i++)
            {
                var person = faker.Generate();
                var personModel = new PersonModel(person);

                persons.Add(personModel);
            }
            
            Console.WriteLine("Querying...");

            var query = from person in persons.AsQueryable()
                        select person;

            var queryPaginator = _queryPaginatorFactory
                  .CreatePaginator(query)
                  .ColumnAscending(person => person.LastName, item => item.FirstName);

            string continuationToken = default;
            char key = 'n';

            while (key != 'q')
            {
                var pageQuery = queryPaginator.BuildQuery(continuationToken, PageSize);
                var pageResults = pageQuery.ToList();

                pageResults.ForEach(person =>
                {
                    Console.WriteLine($"{person.LastName}, {person.FirstName}");
                });

                Console.WriteLine();
                Console.WriteLine("(N)ext, (P)revious, (Q)uit");
                Console.WriteLine();

                do
                {
                    key = Char.ToLower(Console.ReadKey(true).KeyChar);
                } while (key != 'n' && key != 'p' && key != 'q');

                if (pageResults.Any())
                {
                    if (key == 'n')
                    {
                        continuationToken = queryPaginator.CreateContinuationToken(ContinuationDirection.NextPage, pageResults);
                    }
                    else if (key == 'p')
                    {
                        continuationToken = queryPaginator.CreateContinuationToken(ContinuationDirection.PreviousPage, pageResults);
                    }
                }
            }

            ExitCode = 0;

            Console.WriteLine();
            Console.WriteLine("All Over It.");
            Console.ReadKey();
        }
    }
}