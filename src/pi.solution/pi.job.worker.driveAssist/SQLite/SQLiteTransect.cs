using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using pi.job.worker.driveAssist.DomainModel;

namespace pi.job.worker.driveAssist.SQLite
{
    internal class SQLiteTransect
    {
        public bool InitDB(SqliteConnection connection)
        {
            using (var command = connection.CreateCommand())
            {
                try
                {

                    string Createsql = "CREATE TABLE DriveTable (" +
                        "Id VARCHAR(20), " +
                        "Stamp VARCHAR(500), " +
                        "Sensor VARCHAR(200), " +
                        "Value DOUBLE, " +
                        "Unit VARCHAR(20)) ;";
                    command.CommandText = Createsql;
                    command.ExecuteNonQuery();
                }
                catch (Exception)
                {
                    command.Dispose();
                    throw;
                }
            }

            return true;
        }
        public bool InsertDB(SqliteConnection connection, TrackingModel _model)
        {
            using (var command = connection.CreateCommand())
            {
                try
                {
                    string Insertsql = " Insert Into DriveTable  " +
                        "(Id, " +
                        "Stamp, " +
                        "Sensor, " +
                        "Value, " +
                        "Unit) " +
                        "VALUES " +
                        $"({_model.Id}, " +
                        $"'{_model.Stamp}', " +
                        $"'{_model.Sensor}'," +
                        $"{_model.Value}" +
                        $"'{_model.Unit}') ;";
                    command.CommandText = Insertsql;
                    command.ExecuteNonQuery();

                }
                catch (Exception)
                {
                    command.Dispose();
                    throw;
                }
            }

            return true;
        }

        public void GetDB(SqliteConnection connection)
        {
            using (var command = connection.CreateCommand())
            {
                try
                {
                    command.CommandText = @"SELECT Name FROM user";

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var name = reader.GetString(0);

                            Console.WriteLine($"Hello, {name}!");
                        }
                    }
                }
                catch (Exception)
                {
                    command.Dispose();
                    throw;
                }
            }

        }
    }
}
