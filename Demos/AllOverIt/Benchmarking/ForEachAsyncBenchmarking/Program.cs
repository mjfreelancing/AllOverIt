﻿using BenchmarkDotNet.Running;
using System;

namespace ForEachAsyncBenchmarking
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<AsyncCalculations>();
        }
    }
}
