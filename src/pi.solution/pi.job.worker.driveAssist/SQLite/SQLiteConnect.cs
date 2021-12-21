using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pi.job.worker.driveAssist.SQLite
{
    internal class SQLiteConnect
    {
        internal void CreateConnection()
        {
            using (var connection = new SqliteConnection("Data Source=hello.db"))
            {
                connection.Open();               

                var command = connection.CreateCommand();

                CreateTable(command);

                command.CommandText = @"SELECT Name FROM user";
                //command.Parameters.AddWithValue("$id", 1);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var name = reader.GetString(0);

                        Console.WriteLine($"Hello, {name}!");
                    }
                }
            }
        }
        void CreateTable(SqliteCommand comm)
        {                     
            string Createsql = "CREATE TABLE User (Name VARCHAR(20), Age INT)";
            comm.CommandText = Createsql;
            comm.ExecuteNonQuery();

            string Insert1sql = "Insert Into User (Name,Age) VALUES ('ASIF', 40)";
            comm.CommandText = Insert1sql;
            comm.ExecuteNonQuery();

            string Insert2sql = "Insert Into User (Name,Age) VALUES ('Shaista', 43)";
            comm.CommandText = Insert2sql;
            comm.ExecuteNonQuery();

        }
    }
}
