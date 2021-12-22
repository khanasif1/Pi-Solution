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
        public async Task<bool> InsertDB(SqliteConnection connection, TrackingModel _model, ILogger<Worker> logger)
        {
            using (var command = connection.CreateCommand())
            {
                try
                {
                    TrackingModel _trackModel= GetDB(connection, "Select * from [DriveTable];", logger);
                    if(_trackModel == null)
                    {
                        string Insertsql = " Insert Into DriveTable  " +
                                                "(Id, " +
                                                "Stamp, " +
                                                "Sensor, " +
                                                "Value, " +
                                                "Unit) " +
                                                "VALUES " +
                                                $"('{_model.Id}', " +
                                                $"'{_model.Stamp}', " +
                                                $"'{_model.Sensor}'," +
                                                $"{_model.Value} ," +
                                                $"'{_model.Unit}') ;";
                        command.CommandText = Insertsql;
                        await command.ExecuteNonQueryAsync();
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError("Error while InsertDB");
                    logger.LogError(ex.Message);
                    command.Dispose();
                    throw;
                }
            }
            return true;
        }

        public TrackingModel GetDB(SqliteConnection connection, string? _sql, ILogger<Worker> logger)
        {
            TrackingModel _trackingModel = null;
            using (var command = connection.CreateCommand())
            {
                try
                {
                    command.CommandText = _sql;

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            return _trackingModel = new TrackingModel
                            {
                                Id = reader.GetString(0),
                                Sensor = reader.GetString(1),
                                Stamp = reader.GetString(2),
                                Unit = reader.GetString(3),
                                Value = reader.GetDouble(4)
                            };
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError("Error while GetDB");
                    logger.LogError(ex.Message);
                    command.Dispose();
                    throw;
                }
            }
            return _trackingModel;

        }
    }
}
