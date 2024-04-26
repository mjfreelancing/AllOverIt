namespace AllOverIt.Async
{
    /// <summary>Provides a variety of helper methods for <see cref="Task{T}"/>.</summary>
    public static class TaskHelper
    {
        #region WhenAll

        // The following link shows an alternative that provides GetAwaiter() / ConfigureAwait() extensions to await a tuple
        // https://github.com/buvinghausen/TaskTupleAwaiter/blob/a568b46692c317b851e3df2ccb517ae2160b7a15/src/TaskTupleAwaiter/TaskTupleExtensions.cs
        //
        // More info on making anything awaitable: // https://devblogs.microsoft.com/pfxteam/await-anything/

        /// <summary>Waits for all the tasks to complete and returns their results.</summary>
        /// <typeparam name="T1">The result type returned by <paramref name="task1"/></typeparam>
        /// <typeparam name="T2">The result type returned by <paramref name="task2"/></typeparam>
        /// <param name="task1">The first task</param>
        /// <param name="task2">The second task</param>
        /// <returns>The results of all tasks when they are all completed.</returns>
        public static async Task<(T1, T2)> WhenAll<T1, T2>(Task<T1> task1, Task<T2> task2)
        {
            // await them all to ensure exceptions are handled correctly
            await Task.WhenAll(task1, task2).ConfigureAwait(false);

            return (task1.Result, task2.Result);
        }

        /// <summary>Waits for all the tasks to complete and returns their results.</summary>
        /// <typeparam name="T1">The result type returned by <paramref name="task1"/></typeparam>
        /// <typeparam name="T2">The result type returned by <paramref name="task2"/></typeparam>
        /// <typeparam name="T3">The result type returned by <paramref name="task3"/></typeparam>
        /// <param name="task1">The first task</param>
        /// <param name="task2">The second task</param>
        /// <param name="task3">The third task</param>
        /// <returns>The results of all tasks when they are all completed.</returns>
        public static async Task<(T1, T2, T3)> WhenAll<T1, T2, T3>(Task<T1> task1, Task<T2> task2, Task<T3> task3)
        {
            // await them all to ensure exceptions are handled correctly
            await Task.WhenAll(task1, task2, task3).ConfigureAwait(false);

            return (task1.Result, task2.Result, task3.Result);
        }

        /// <summary>Waits for all the tasks to complete and returns their results.</summary>
        /// <typeparam name="T1">The result type returned by <paramref name="task1"/></typeparam>
        /// <typeparam name="T2">The result type returned by <paramref name="task2"/></typeparam>
        /// <typeparam name="T3">The result type returned by <paramref name="task3"/></typeparam>
        /// <typeparam name="T4">The result type returned by <paramref name="task4"/></typeparam>
        /// <param name="task1">The first task</param>
        /// <param name="task2">The second task</param>
        /// <param name="task3">The third task</param>
        /// <param name="task4">The fourth task</param>
        /// <returns>The results of all tasks when they are all completed.</returns>
        public static async Task<(T1, T2, T3, T4)> WhenAll<T1, T2, T3, T4>(Task<T1> task1, Task<T2> task2, Task<T3> task3,
          Task<T4> task4)
        {
            // await them all to ensure exceptions are handled correctly
            await Task.WhenAll(task1, task2, task3, task4).ConfigureAwait(false);

            return (task1.Result, task2.Result, task3.Result, task4.Result);
        }

        /// <summary>Waits for all the tasks to complete and returns their results.</summary>
        /// <typeparam name="T1">The result type returned by <paramref name="task1"/></typeparam>
        /// <typeparam name="T2">The result type returned by <paramref name="task2"/></typeparam>
        /// <typeparam name="T3">The result type returned by <paramref name="task3"/></typeparam>
        /// <typeparam name="T4">The result type returned by <paramref name="task4"/></typeparam>
        /// <typeparam name="T5">The result type returned by <paramref name="task5"/></typeparam>
        /// <param name="task1">The first task</param>
        /// <param name="task2">The second task</param>
        /// <param name="task3">The third task</param>
        /// <param name="task4">The fourth task</param>
        /// <param name="task5">The fifth task</param>
        /// <returns>The results of all tasks when they are all completed.</returns>
        public static async Task<(T1, T2, T3, T4, T5)> WhenAll<T1, T2, T3, T4, T5>(Task<T1> task1, Task<T2> task2, Task<T3> task3,
          Task<T4> task4, Task<T5> task5)
        {
            // await them all to ensure exceptions are handled correctly
            await Task.WhenAll(task1, task2, task3, task4, task5).ConfigureAwait(false);

            return (task1.Result, task2.Result, task3.Result, task4.Result, task5.Result);
        }

        /// <summary>Waits for all the tasks to complete and returns their results.</summary>
        /// <typeparam name="T1">The result type returned by <paramref name="task1"/></typeparam>
        /// <typeparam name="T2">The result type returned by <paramref name="task2"/></typeparam>
        /// <typeparam name="T3">The result type returned by <paramref name="task3"/></typeparam>
        /// <typeparam name="T4">The result type returned by <paramref name="task4"/></typeparam>
        /// <typeparam name="T5">The result type returned by <paramref name="task5"/></typeparam>
        /// <typeparam name="T6">The result type returned by <paramref name="task6"/></typeparam>
        /// <param name="task1">The first task</param>
        /// <param name="task2">The second task</param>
        /// <param name="task3">The third task</param>
        /// <param name="task4">The fourth task</param>
        /// <param name="task5">The fifth task</param>
        /// <param name="task6">The sixth task</param>
        /// <returns>The results of all tasks when they are all completed.</returns>
        public static async Task<(T1, T2, T3, T4, T5, T6)> WhenAll<T1, T2, T3, T4, T5, T6>(Task<T1> task1, Task<T2> task2, Task<T3> task3,
          Task<T4> task4, Task<T5> task5, Task<T6> task6)
        {
            // await them all to ensure exceptions are handled correctly
            await Task.WhenAll(task1, task2, task3, task4, task5, task6).ConfigureAwait(false);

            return (task1.Result, task2.Result, task3.Result, task4.Result, task5.Result, task6.Result);
        }

        /// <summary>Waits for all the tasks to complete and returns their results.</summary>
        /// <typeparam name="T1">The result type returned by <paramref name="task1"/></typeparam>
        /// <typeparam name="T2">The result type returned by <paramref name="task2"/></typeparam>
        /// <typeparam name="T3">The result type returned by <paramref name="task3"/></typeparam>
        /// <typeparam name="T4">The result type returned by <paramref name="task4"/></typeparam>
        /// <typeparam name="T5">The result type returned by <paramref name="task5"/></typeparam>
        /// <typeparam name="T6">The result type returned by <paramref name="task6"/></typeparam>
        /// <typeparam name="T7">The result type returned by <paramref name="task7"/></typeparam>
        /// <param name="task1">The first task</param>
        /// <param name="task2">The second task</param>
        /// <param name="task3">The third task</param>
        /// <param name="task4">The fourth task</param>
        /// <param name="task5">The fifth task</param>
        /// <param name="task6">The sixth task</param>
        /// <param name="task7">The seventh task</param>
        /// <returns>The results of all tasks when they are all completed.</returns>
        public static async Task<(T1, T2, T3, T4, T5, T6, T7)> WhenAll<T1, T2, T3, T4, T5, T6, T7>(Task<T1> task1, Task<T2> task2, Task<T3> task3,
          Task<T4> task4, Task<T5> task5, Task<T6> task6, Task<T7> task7)
        {
            // await them all to ensure exceptions are handled correctly
            await Task.WhenAll(task1, task2, task3, task4, task5, task6, task7).ConfigureAwait(false);

            return (task1.Result, task2.Result, task3.Result, task4.Result, task5.Result, task6.Result, task7.Result);
        }

        /// <summary>Waits for all the tasks to complete and returns their results.</summary>
        /// <typeparam name="T1">The result type returned by <paramref name="task1"/></typeparam>
        /// <typeparam name="T2">The result type returned by <paramref name="task2"/></typeparam>
        /// <typeparam name="T3">The result type returned by <paramref name="task3"/></typeparam>
        /// <typeparam name="T4">The result type returned by <paramref name="task4"/></typeparam>
        /// <typeparam name="T5">The result type returned by <paramref name="task5"/></typeparam>
        /// <typeparam name="T6">The result type returned by <paramref name="task6"/></typeparam>
        /// <typeparam name="T7">The result type returned by <paramref name="task7"/></typeparam>
        /// <typeparam name="T8">The result type returned by <paramref name="task8"/></typeparam>
        /// <param name="task1">The first task</param>
        /// <param name="task2">The second task</param>
        /// <param name="task3">The third task</param>
        /// <param name="task4">The fourth task</param>
        /// <param name="task5">The fifth task</param>
        /// <param name="task6">The sixth task</param>
        /// <param name="task7">The seventh task</param>
        /// <param name="task8">The eighth task</param>
        /// <returns>The results of all tasks when they are all completed.</returns>
        public static async Task<(T1, T2, T3, T4, T5, T6, T7, T8)> WhenAll<T1, T2, T3, T4, T5, T6, T7, T8>(Task<T1> task1, Task<T2> task2,
          Task<T3> task3, Task<T4> task4, Task<T5> task5, Task<T6> task6, Task<T7> task7, Task<T8> task8)
        {
            // await them all to ensure exceptions are handled correctly
            await Task.WhenAll(task1, task2, task3, task4, task5, task6, task7, task8).ConfigureAwait(false);

            return (task1.Result, task2.Result, task3.Result, task4.Result, task5.Result, task6.Result, task7.Result, task8.Result);
        }

        /// <summary>Waits for all the tasks to complete and returns their results.</summary>
        /// <typeparam name="T1">The result type returned by <paramref name="task1"/></typeparam>
        /// <typeparam name="T2">The result type returned by <paramref name="task2"/></typeparam>
        /// <typeparam name="T3">The result type returned by <paramref name="task3"/></typeparam>
        /// <typeparam name="T4">The result type returned by <paramref name="task4"/></typeparam>
        /// <typeparam name="T5">The result type returned by <paramref name="task5"/></typeparam>
        /// <typeparam name="T6">The result type returned by <paramref name="task6"/></typeparam>
        /// <typeparam name="T7">The result type returned by <paramref name="task7"/></typeparam>
        /// <typeparam name="T8">The result type returned by <paramref name="task8"/></typeparam>
        /// <typeparam name="T9">The result type returned by <paramref name="task9"/></typeparam>
        /// <param name="task1">The first task</param>
        /// <param name="task2">The second task</param>
        /// <param name="task3">The third task</param>
        /// <param name="task4">The fourth task</param>
        /// <param name="task5">The fifth task</param>
        /// <param name="task6">The sixth task</param>
        /// <param name="task7">The seventh task</param>
        /// <param name="task8">The eighth task</param>
        /// <param name="task9">The ninth task</param>
        /// <returns>The results of all tasks when they are all completed.</returns>
        public static async Task<(T1, T2, T3, T4, T5, T6, T7, T8, T9)> WhenAll<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Task<T1> task1, Task<T2> task2,
          Task<T3> task3, Task<T4> task4, Task<T5> task5, Task<T6> task6, Task<T7> task7, Task<T8> task8, Task<T9> task9)
        {
            // await them all to ensure exceptions are handled correctly
            await Task.WhenAll(task1, task2, task3, task4, task5, task6, task7, task8, task9).ConfigureAwait(false);

            return (task1.Result, task2.Result, task3.Result, task4.Result, task5.Result, task6.Result, task7.Result, task8.Result, task9.Result);
        }

        /// <summary>Waits for all the tasks to complete and returns their results.</summary>
        /// <typeparam name="T1">The result type returned by <paramref name="task1"/></typeparam>
        /// <typeparam name="T2">The result type returned by <paramref name="task2"/></typeparam>
        /// <typeparam name="T3">The result type returned by <paramref name="task3"/></typeparam>
        /// <typeparam name="T4">The result type returned by <paramref name="task4"/></typeparam>
        /// <typeparam name="T5">The result type returned by <paramref name="task5"/></typeparam>
        /// <typeparam name="T6">The result type returned by <paramref name="task6"/></typeparam>
        /// <typeparam name="T7">The result type returned by <paramref name="task7"/></typeparam>
        /// <typeparam name="T8">The result type returned by <paramref name="task8"/></typeparam>
        /// <typeparam name="T9">The result type returned by <paramref name="task9"/></typeparam>
        /// <typeparam name="T10">The result type returned by <paramref name="task10"/></typeparam>
        /// <param name="task1">The first task</param>
        /// <param name="task2">The second task</param>
        /// <param name="task3">The third task</param>
        /// <param name="task4">The fourth task</param>
        /// <param name="task5">The fifth task</param>
        /// <param name="task6">The sixth task</param>
        /// <param name="task7">The seventh task</param>
        /// <param name="task8">The eighth task</param>
        /// <param name="task9">The ninth task</param>
        /// <param name="task10">The tenth task</param>
        /// <returns>The results of all tasks when they are all completed.</returns>
        public static async Task<(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10)> WhenAll<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(
          Task<T1> task1, Task<T2> task2, Task<T3> task3, Task<T4> task4, Task<T5> task5, Task<T6> task6, Task<T7> task7,
          Task<T8> task8, Task<T9> task9, Task<T10> task10)
        {
            // await them all to ensure exceptions are handled correctly
            await Task.WhenAll(task1, task2, task3, task4, task5, task6, task7, task8, task9, task10).ConfigureAwait(false);

            return (task1.Result, task2.Result, task3.Result, task4.Result, task5.Result, task6.Result, task7.Result, task8.Result,
              task9.Result, task10.Result);
        }

        #endregion
    }
}