﻿using AllOverIt.GenericHost;
using AllOverIt.Pagination.Extensions;
using MemoryPaginationDemo;
using Microsoft.Extensions.Hosting;

class Program
{
    static async Task Main(string[] args)
    {
        await GenericHost
            .CreateConsoleHostBuilder<App>(args)
            .ConfigureServices((hostContext, services) =>
            {
                services.AddQueryPagination();
            })
            .RunConsoleAsync(options => options.SuppressStatusMessages = true);
    }
}
