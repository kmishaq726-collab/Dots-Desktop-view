using System;
using System.Data.SQLite;
using System.Text.Json;

namespace MyApp.UI.Data
{
    public class SystemConfigRepository
    {
        /// <summary>
        /// Saves or updates a configuration key-value pair in the SystemConfig table.
        /// The value is stored as a JSON string.
        /// </summary>
        public static void SaveConfig(string key, object data)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Key cannot be null or empty.", nameof(key));

            string json = JsonSerializer.Serialize(data);

            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();

                // ✅ Simplified: Use INSERT OR REPLACE (instead of delete + insert)
                string sql = @"
                    INSERT OR REPLACE INTO SystemConfig (Id, Key, Value, LastUpdated)
                    VALUES (
                        (SELECT Id FROM SystemConfig WHERE Key = @key),
                        @key,
                        @value,
                        @date
                    );
                ";

                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@key", key);
                    cmd.Parameters.AddWithValue("@value", json);
                    cmd.Parameters.AddWithValue("@date", DateTime.Now);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        //For salePost data
        public static void SavePostSale(string key, object data)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Key cannot be null or empty.", nameof(key));

            string json = JsonSerializer.Serialize(data);

            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();

                // ✅ Simplified: Use INSERT OR REPLACE (instead of delete + insert)
                string sql = @"
                    INSERT OR REPLACE INTO PostSales (Id, Key, Value, LastUpdated)
                    VALUES (
                        (SELECT Id FROM PostSales WHERE Key = @key),
                        @key,
                        @value,
                        @date
                    );
                ";

                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@key", key);
                    cmd.Parameters.AddWithValue("@value", json);
                    cmd.Parameters.AddWithValue("@date", DateTime.Now);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Retrieves the JSON string value for a given key.
        /// Returns null if not found.
        /// </summary>
        public static string? GetConfig(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Key cannot be null or empty.", nameof(key));

            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();

                string sql = "SELECT Value FROM SystemConfig WHERE Key = @key LIMIT 1;";
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@key", key);
                    var result = cmd.ExecuteScalar();
                    return result?.ToString();
                }
            }
        }

        public static string? GetPostSale(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Key cannot be null or empty.", nameof(key));

            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();

                string sql = "SELECT Value FROM PostSales WHERE Key = @key LIMIT 1;";
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@key", key);
                    var result = cmd.ExecuteScalar();
                    return result?.ToString();
                }
            }
        }


        public static int GetNotSyncedTotal()
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();

                string sql = "SELECT COUNT(*) FROM PostSales;";
                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    var result = cmd.ExecuteScalar();
                    return Convert.ToInt32(result);
                }
            }
        }


        /// <summary>
        /// Retrieves a strongly-typed configuration object.
        /// Example: var api = SystemConfigRepository.GetConfig&lt;ApiSettings&gt;("api_settings");
        /// </summary>
        public static T? GetConfig<T>(string key)
        {
            string? json = GetConfig(key);
            return json is null ? default : JsonSerializer.Deserialize<T>(json);
        }

        /// <summary>
        /// Deletes a specific configuration entry.
        /// </summary>
        public static void DeleteConfig(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Key cannot be null or empty.", nameof(key));

            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                using (var cmd = new SQLiteCommand("DELETE FROM SystemConfig WHERE Key = @key;", conn))
                {
                    cmd.Parameters.AddWithValue("@key", key);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void DeletePostSale(int id)
        {
            if (id != null && id <= 0)
                throw new ArgumentException("Key cannot be null or empty.", nameof(id));

            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                using (var cmd = new SQLiteCommand("DELETE FROM PostSales WHERE id = @id;", conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Clears all configuration data from SystemConfig.
        /// </summary>
        public static void ClearAll()
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();
                using (var cmd = new SQLiteCommand("DELETE FROM SystemConfig;", conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public static PostSaleRecord? GetFirstPostSale()
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();

                string sql = @"
            SELECT Id, Key, Value, LastUpdated
            FROM PostSales
            ORDER BY Id ASC
            LIMIT 1;
        ";

                using (var cmd = new SQLiteCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new PostSaleRecord
                        {
                            Id = reader.GetInt32(0),
                            Key = reader.GetString(1),
                            Value = reader.GetString(2),
                            LastUpdated = reader.GetDateTime(3)
                        };
                    }
                }
            }

            return null;
        }


        public static void DeletePostSaleById(int id)
        {
            using (var conn = DatabaseHelper.GetConnection())
            {
                conn.Open();

                string sql = "DELETE FROM PostSales WHERE Id = @Id;";

                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }



    }
}
