using Microsoft.Data.Sqlite;
using System;
using System.IO;

class FixDatabase
{
    static void Main()
    {
        var connectionString = "Data Source=cms.db";
        var sqlScript = File.ReadAllText("fix-database.sql");

        try
        {
            using var connection = new SqliteConnection(connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = sqlScript;
            command.ExecuteNonQuery();

            Console.WriteLine("Database fixed successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}