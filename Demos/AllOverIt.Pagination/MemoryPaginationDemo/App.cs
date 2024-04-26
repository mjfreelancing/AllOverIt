using AllOverIt.Assertion;
using AllOverIt.Extensions;
using AllOverIt.GenericHost;
using AllOverIt.Pagination;
using AllOverIt.Pagination.Extensions;
using Bogus;
using Bogus.DataSets;
using System.Diagnostics;
using System.Text;

namespace MemoryPaginationDemo
{
    public sealed class App : ConsoleAppBase
    {
        // Cannot use 'Bogus.Person' as it contains fields - can only use properties in queries 
        private class PersonModel
        {
            private static int _id;

            public int Id { get; }
            public string FirstName { get; }
            public string LastName { get; }
            public Name.Gender Gender { get; }

            public PersonModel(Person person)
            {
                Id = Interlocked.Increment(ref _id);
                FirstName = person.FirstName;
                LastName = person.LastName;
                Gender = person.Gender;
            }
        }

        private class ContinuationTokens
        {
            public string Current { get; set; }
            public string Next { get; set; }
            public string Previous { get; set; }
        }

        private readonly IQueryPaginatorFactory _queryPaginatorFactory;

        public App(IQueryPaginatorFactory queryPaginatorFactory)
        {
            _queryPaginatorFactory = queryPaginatorFactory.WhenNotNull(nameof(queryPaginatorFactory));
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            const int dataSize = 20005;
            const int pageSize = 20;

            var persons = GetData(dataSize);

            var query = from person in persons.AsQueryable()
                        select person;

            var paginatorConfig = new QueryPaginatorConfiguration
            {
                PageSize = pageSize,
                PaginationDirection = PaginationDirection.Forward,    // This is the default
                UseParameterizedQueries = false                         // Not required for memory based pagination
            };

            // Pagination requires a unique column on the end (Id in this example) just in case multiple records have the same lastname / firstname.
            // The 'Gender' item is only including for testing Enum's in the continuation token.
            var queryPaginator = _queryPaginatorFactory
                  .CreatePaginator(query, paginatorConfig)
                  .ColumnAscending(person => person.LastName, item => item.FirstName, item => item.Gender, item => item.Id);

            string continuationToken = default;
            var key = 'n';

            var stopwatch = Stopwatch.StartNew();

            while (key != 'q')
            {
                Console.WriteLine();
                Console.WriteLine("Querying...");
                Console.WriteLine();

                var lastCheckpoint = stopwatch.ElapsedMilliseconds;

                var pageResults = queryPaginator.GetPageResults(continuationToken);

                var elapsed = stopwatch.ElapsedMilliseconds;

                foreach (var person in pageResults.Results)
                {
                    Console.WriteLine($"{person.LastName}, {person.FirstName} ({person.Gender}, {person.Id})");
                };

                Console.WriteLine();
                Console.WriteLine($"Execution time: {elapsed - lastCheckpoint}ms");
                Console.WriteLine();

                key = GetUserInput(pageResults.PreviousToken.IsNotNullOrEmpty(), pageResults.NextToken.IsNotNullOrEmpty());

                lastCheckpoint = stopwatch.ElapsedMilliseconds;

                switch (key)
                {
                    case 'f':
                        continuationToken = queryPaginator.TokenEncoder.EncodeFirstPage();      // could also just set to null or string.Empty
                        break;

                    case 'p':
                        continuationToken = pageResults.PreviousToken;
                        break;

                    case 'n':
                        continuationToken = pageResults.NextToken;
                        break;

                    case 'l':
                        continuationToken = queryPaginator.TokenEncoder.EncodeLastPage();
                        break;

                    case 'q':
                        Console.WriteLine();
                        Console.WriteLine("Done");
                        break;
                }

                elapsed = stopwatch.ElapsedMilliseconds;

                if (continuationToken.IsNotNullOrEmpty())
                {
                    Console.WriteLine($"Continuation token generation time: {elapsed - lastCheckpoint}ms");
                    Console.WriteLine(continuationToken);
                }
            }

            ExitCode = 0;

            Console.WriteLine();
            Console.WriteLine("All Over It.");
            Console.ReadKey();

            return Task.CompletedTask;
        }

        private static List<PersonModel> GetData(int dataSize)
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

            sb.Append("(F)irst, ");

            if (hasPrevious)
            {
                sb.Append("(P)revious, ");
            }

            if (hasNext)
            {
                sb.Append("(N)ext, ");
            }

            sb.Append("(L)ast, ");

            sb.Append("(Q)uit");

            Console.WriteLine();
            Console.WriteLine($"{sb}");
            Console.WriteLine();

            char key;

            do
            {
                key = char.ToLower(Console.ReadKey(true).KeyChar);
            } while ((key != 'p' || !hasPrevious) && (key != 'n' || !hasNext) && key != 'f' && key != 'l' && key != 'q');

            return key;
        }
    }
}