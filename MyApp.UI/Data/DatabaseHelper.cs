using System;
using System.Data.SQLite;
using System.IO;

namespace MyApp.UI.Data
{
    public static class DatabaseHelper
    {
        // ✅ Database file path
        private static readonly string DbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "localdata.db");
        private static readonly string ConnectionString = $"Data Source={DbPath};Version=3;";

        public static void Initialize()
        {
            try
            {
                if (!File.Exists(DbPath))
                {
                    SQLiteConnection.CreateFile(DbPath);


                }
                else
                {
                    Console.WriteLine($"ℹ️ Using existing database at: {DbPath}");
                }
                using (var connection = new SQLiteConnection(ConnectionString))
                {
                    connection.Open();

                    // ✅ Create table if not exists
                    string sql = @"
                        CREATE TABLE IF NOT EXISTS SystemConfig (
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            Key TEXT UNIQUE,
                            Value TEXT,
                            LastUpdated DATETIME
                        );
                    ";

                    string sqlForPostSales = @"
                        CREATE TABLE IF NOT EXISTS PostSales (
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            Key TEXT UNIQUE,
                            Value TEXT,
                            LastUpdated DATETIME
                        );";

                    using (var cmd = new SQLiteCommand(sql, connection))
                    {
                        cmd.ExecuteNonQuery();
                    }

                    using (var cmd = new SQLiteCommand(sqlForPostSales, connection))
                    {
                        cmd.ExecuteNonQuery();
                       // MessageBox.Show(cmd.GetType().ToString());
                    }
                }

                Console.WriteLine("✅ Database initialized successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Database initialization failed: {ex.Message}");
            }
        }

        public static SQLiteConnection GetConnection()
        {
            return new SQLiteConnection(ConnectionString);
        }
    }
}
