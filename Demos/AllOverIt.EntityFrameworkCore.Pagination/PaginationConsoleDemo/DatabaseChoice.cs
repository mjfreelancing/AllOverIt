namespace PaginationConsoleDemo
{
    public enum DatabaseChoice
    {
        Sqlite,

        // Disabled only because Pomelo.EntityFrameworkCore.MySql does not support new versions of EF Core
        //Mysql,

        PostgreSql
    }
}