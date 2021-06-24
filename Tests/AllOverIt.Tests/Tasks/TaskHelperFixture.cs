using AllOverIt.Fixture;
using AllOverIt.Tasks;
using FluentAssertions;
using System.Threading.Tasks;
using Xunit;

namespace AllOverIt.Tests.Tasks
{
    public class TaskHelperFixture : FixtureBase
    {
        public class WhenAll : TaskHelperFixture
        {
            [Fact]
            public async Task Should_Wait_For_Two_Tasks()
            {
                var value1 = Create<bool>();
                var value2 = Create<int>();

                var task1 = Task.FromResult(value1);
                var task2 = Task.FromResult(value2);

                var (actual1, actual2) = await TaskHelper.WhenAll(task1, task2).ConfigureAwait(false);

                actual1.Should().Be(value1);
                actual2.Should().Be(value2);
            }

            [Fact]
            public async Task Should_Wait_For_Three_Tasks()
            {
                var value1 = Create<bool>();
                var value2 = Create<int>();
                var value3 = Create<string>();

                var task1 = Task.FromResult(value1);
                var task2 = Task.FromResult(value2);
                var task3 = Task.FromResult(value3);

                var (actual1, actual2, actual3) = await TaskHelper.WhenAll(task1, task2, task3).ConfigureAwait(false);

                actual1.Should().Be(value1);
                actual2.Should().Be(value2);
                actual3.Should().Be(value3);
            }

            [Fact]
            public async Task Should_Wait_For_For_Tasks()
            {
                var value1 = Create<bool>();
                var value2 = Create<int>();
                var value3 = Create<string>();
                var value4 = Create<double>();

                var task1 = Task.FromResult(value1);
                var task2 = Task.FromResult(value2);
                var task3 = Task.FromResult(value3);
                var task4 = Task.FromResult(value4);

                var (actual1, actual2, actual3, actual4) = await TaskHelper.WhenAll(task1, task2, task3, task4).ConfigureAwait(false);

                actual1.Should().Be(value1);
                actual2.Should().Be(value2);
                actual3.Should().Be(value3);
                actual4.Should().Be(value4);
            }

            [Fact]
            public async Task Should_Wait_For_Five_Tasks()
            {
                var value1 = Create<bool>();
                var value2 = Create<int>();
                var value3 = Create<string>();
                var value4 = Create<double>();
                var value5 = Create<short>();

                var task1 = Task.FromResult(value1);
                var task2 = Task.FromResult(value2);
                var task3 = Task.FromResult(value3);
                var task4 = Task.FromResult(value4);
                var task5 = Task.FromResult(value5);

                var (actual1, actual2, actual3, actual4, actual5) = await TaskHelper.WhenAll(task1, task2, task3, task4, task5).ConfigureAwait(false);

                actual1.Should().Be(value1);
                actual2.Should().Be(value2);
                actual3.Should().Be(value3);
                actual4.Should().Be(value4);
                actual5.Should().Be(value5);
            }

            [Fact]
            public async Task Should_Wait_For_Six_Tasks()
            {
                var value1 = Create<bool>();
                var value2 = Create<int>();
                var value3 = Create<string>();
                var value4 = Create<double>();
                var value5 = Create<short>();
                var value6 = Create<int>();

                var task1 = Task.FromResult(value1);
                var task2 = Task.FromResult(value2);
                var task3 = Task.FromResult(value3);
                var task4 = Task.FromResult(value4);
                var task5 = Task.FromResult(value5);
                var task6 = Task.FromResult(value6);

                var (actual1, actual2, actual3, actual4, actual5, actual6) =
                  await TaskHelper
                    .WhenAll(task1, task2, task3, task4, task5, task6)
                    .ConfigureAwait(false);

                actual1.Should().Be(value1);
                actual2.Should().Be(value2);
                actual3.Should().Be(value3);
                actual4.Should().Be(value4);
                actual5.Should().Be(value5);
                actual6.Should().Be(value6);
            }

            [Fact]
            public async Task Should_Wait_For_Seven_Tasks()
            {
                var value1 = Create<bool>();
                var value2 = Create<int>();
                var value3 = Create<string>();
                var value4 = Create<double>();
                var value5 = Create<short>();
                var value6 = Create<int>();
                var value7 = Create<long>();

                var task1 = Task.FromResult(value1);
                var task2 = Task.FromResult(value2);
                var task3 = Task.FromResult(value3);
                var task4 = Task.FromResult(value4);
                var task5 = Task.FromResult(value5);
                var task6 = Task.FromResult(value6);
                var task7 = Task.FromResult(value7);

                var (actual1, actual2, actual3, actual4, actual5, actual6, actual7)
                  = await TaskHelper
                    .WhenAll(task1, task2, task3, task4, task5, task6, task7)
                    .ConfigureAwait(false);

                actual1.Should().Be(value1);
                actual2.Should().Be(value2);
                actual3.Should().Be(value3);
                actual4.Should().Be(value4);
                actual5.Should().Be(value5);
                actual6.Should().Be(value6);
                actual7.Should().Be(value7);
            }

            [Fact]
            public async Task Should_Wait_For_Eigth_Tasks()
            {
                var value1 = Create<bool>();
                var value2 = Create<int>();
                var value3 = Create<string>();
                var value4 = Create<double>();
                var value5 = Create<short>();
                var value6 = Create<int>();
                var value7 = Create<long>();
                var value8 = Create<int>();

                var task1 = Task.FromResult(value1);
                var task2 = Task.FromResult(value2);
                var task3 = Task.FromResult(value3);
                var task4 = Task.FromResult(value4);
                var task5 = Task.FromResult(value5);
                var task6 = Task.FromResult(value6);
                var task7 = Task.FromResult(value7);
                var task8 = Task.FromResult(value8);

                var (actual1, actual2, actual3, actual4, actual5, actual6, actual7, actual8)
                  = await TaskHelper
                    .WhenAll(task1, task2, task3, task4, task5, task6, task7, task8)
                    .ConfigureAwait(false);

                actual1.Should().Be(value1);
                actual2.Should().Be(value2);
                actual3.Should().Be(value3);
                actual4.Should().Be(value4);
                actual5.Should().Be(value5);
                actual6.Should().Be(value6);
                actual7.Should().Be(value7);
                actual8.Should().Be(value8);
            }

            [Fact]
            public async Task Should_Wait_For_Nine_Tasks()
            {
                var value1 = Create<bool>();
                var value2 = Create<int>();
                var value3 = Create<string>();
                var value4 = Create<double>();
                var value5 = Create<short>();
                var value6 = Create<int>();
                var value7 = Create<long>();
                var value8 = Create<int>();
                var value9 = Create<string>();

                var task1 = Task.FromResult(value1);
                var task2 = Task.FromResult(value2);
                var task3 = Task.FromResult(value3);
                var task4 = Task.FromResult(value4);
                var task5 = Task.FromResult(value5);
                var task6 = Task.FromResult(value6);
                var task7 = Task.FromResult(value7);
                var task8 = Task.FromResult(value8);
                var task9 = Task.FromResult(value9);

                var (actual1, actual2, actual3, actual4, actual5, actual6, actual7, actual8, actual9)
                  = await TaskHelper
                    .WhenAll(task1, task2, task3, task4, task5, task6, task7, task8, task9)
                    .ConfigureAwait(false);

                actual1.Should().Be(value1);
                actual2.Should().Be(value2);
                actual3.Should().Be(value3);
                actual4.Should().Be(value4);
                actual5.Should().Be(value5);
                actual6.Should().Be(value6);
                actual7.Should().Be(value7);
                actual8.Should().Be(value8);
                actual9.Should().Be(value9);
            }

            [Fact]
            public async Task Should_Wait_For_Ten_Tasks()
            {
                var value1 = Create<bool>();
                var value2 = Create<int>();
                var value3 = Create<string>();
                var value4 = Create<double>();
                var value5 = Create<short>();
                var value6 = Create<int>();
                var value7 = Create<long>();
                var value8 = Create<int>();
                var value9 = Create<string>();
                var value10 = Create<string>();

                var task1 = Task.FromResult(value1);
                var task2 = Task.FromResult(value2);
                var task3 = Task.FromResult(value3);
                var task4 = Task.FromResult(value4);
                var task5 = Task.FromResult(value5);
                var task6 = Task.FromResult(value6);
                var task7 = Task.FromResult(value7);
                var task8 = Task.FromResult(value8);
                var task9 = Task.FromResult(value9);
                var task10 = Task.FromResult(value10);

                var (actual1, actual2, actual3, actual4, actual5, actual6, actual7, actual8, actual9, actual10)
                  = await TaskHelper
                    .WhenAll(task1, task2, task3, task4, task5, task6, task7, task8, task9, task10)
                    .ConfigureAwait(false);

                actual1.Should().Be(value1);
                actual2.Should().Be(value2);
                actual3.Should().Be(value3);
                actual4.Should().Be(value4);
                actual5.Should().Be(value5);
                actual6.Should().Be(value6);
                actual7.Should().Be(value7);
                actual8.Should().Be(value8);
                actual9.Should().Be(value9);
                actual10.Should().Be(value10);
            }
        }
    }
}