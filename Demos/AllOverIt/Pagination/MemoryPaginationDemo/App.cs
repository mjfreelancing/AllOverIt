﻿using AllOverIt.Assertion;
using AllOverIt.Extensions;
using AllOverIt.GenericHost;
using AllOverIt.Pagination;
using AllOverIt.Pagination.Extensions;
using Bogus;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            const int DataSize = 205;
            const int PageSize = 20;

            var persons = GetData(DataSize);

            var query = from person in persons.AsQueryable()
                        select person;

            var queryPaginator = _queryPaginatorFactory
                  .CreatePaginator(query)
                  .ColumnAscending(person => person.LastName, item => item.FirstName);

            string continuationToken = default;
            char key = 'n';

            while (key != 'q')
            {
                Console.WriteLine();
                Console.WriteLine("Querying...");
                Console.WriteLine();

                var pageQuery = queryPaginator.BuildPageQuery(continuationToken, PageSize);

                var pageResults = pageQuery.ToList();

                var hasPrevious = pageResults.Any() ? queryPaginator.HasPreviousPage(pageResults.First()) : false;
                var hasNext = pageResults.Any() ? queryPaginator.HasNextPage(pageResults.Last()) : false;

                pageResults.ForEach(person =>
                {
                    Console.WriteLine($"{person.LastName}, {person.FirstName}");
                });

                key = GetUserInput(hasPrevious, hasNext);

                switch (key)
                {
                    case 'n':
                        continuationToken = queryPaginator.CreateContinuationToken(ContinuationDirection.NextPage, pageResults);
                        break;

                    case 'p':
                        continuationToken = queryPaginator.CreateContinuationToken(ContinuationDirection.PreviousPage, pageResults);
                        break;

                    case 'q':
                        Console.WriteLine();
                        Console.WriteLine("Done");
                        break;
                }
            }

            ExitCode = 0;

            Console.WriteLine();
            Console.WriteLine("All Over It.");
            Console.ReadKey();

            return Task.CompletedTask;
        }

        private static IReadOnlyCollection<PersonModel> GetData(int dataSize)
        {
            Console.WriteLine();
            Console.WriteLine("Adding data...");
            Console.WriteLine();

            var faker = new Faker<Person>().CustomInstantiator(_ => new Person("en"));

            var persons = new List<PersonModel>(dataSize);

            for (var i = 1; i <= dataSize; i++)
            {
                var person = faker.Generate();
                var personModel = new PersonModel(person);

                persons.Add(personModel);
            }

            return persons;
        }

        private static char GetUserInput(bool hasPrevious, bool hasNext)
        {
            Console.WriteLine();

            var sb = new StringBuilder();

            if (hasPrevious)
            {
                sb.Append("(P)revious, ");
            }

            if (hasNext)
            {
                sb.Append("(N)ext, ");
            }

            sb.Append("(Q)uit");

            Console.WriteLine();
            Console.WriteLine($"{sb}");
            Console.WriteLine();

            char key;

            do
            {
                key = char.ToLower(Console.ReadKey(true).KeyChar);
            } while ((key != 'p' || !hasPrevious) && (key != 'n' || !hasNext) && key != 'q');

            return key;
        }
    }
}