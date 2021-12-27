using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using pi.job.worker.driveAssist.Common;
using pi.job.worker.driveAssist.DomainModel;

namespace pi.job.worker.driveAssist.SQLite
{
    internal class SQLiteTransect
    {
        public bool InitDB(SqliteConnection connection)
        {
            Logger.LogMessage("Start SQLiteTransect --> InitDB", ConfigManager.executionEnv);
            bool IsTableExist = false;

            Logger.LogMessage("SQLiteTransect --> InitDB Check if Table Exist", ConfigManager.executionEnv);

            string _sqltablecheck = "SELECT name FROM sqlite_master WHERE type='table' AND name='DriveTable'; ";
            using (var commandCheck = connection.CreateCommand())
            {
                try
                {
                    string tableName = String.Empty;
                    commandCheck.CommandText = _sqltablecheck;
                    using (var reader = commandCheck.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tableName = reader.GetString(0);
                        }
                    }
                    if (!string.IsNullOrEmpty(tableName))
                    {
                        Logger.LogMessage("SQLiteTransect --> InitDB Table Exists", ConfigManager.executionEnv);
                        IsTableExist = true;
                    }
                    else
                    {
                        Logger.LogMessage("SQLiteTransect --> InitDB Table Missing Create One", ConfigManager.executionEnv);
                    }
                }
                catch (Exception ex)
                {
                    commandCheck.Dispose();
                    throw;
                }
            }

            if (!IsTableExist)
            {
                using (var command = connection.CreateCommand())
                {
                    Logger.LogMessage("SQLiteTransect --> InitDB Start Creating Table", ConfigManager.executionEnv);
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
            }
            Logger.LogMessage("End SQLiteTransect --> InitDB", ConfigManager.executionEnv);
            return true;
        }
        public async Task<bool> InsertDB(SqliteConnection connection, TrackingModel _model, ILogger<Worker> logger)
        {
            Logger.LogMessage("Start SQLiteTransect --> InsertDB", ConfigManager.executionEnv);
            using (var command = connection.CreateCommand())
            {
                try
                {
                    //List<TrackingModel> _lsttrackModel= await GetDB(connection, "Select * from [DriveTable];", logger);
                    //if(_lsttrackModel == null )
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
            Logger.LogMessage("End SQLiteTransect --> InsertDB", ConfigManager.executionEnv);
            return true;
        }

        public async Task<List<TrackingModel>> GetDB(SqliteConnection connection, string? _sql, ILogger<Worker> logger)
        {
            Logger.LogMessage("Start SQLiteTransect --> GetDB", ConfigManager.executionEnv);
            List<TrackingModel> _lsttrackingModel = new List<TrackingModel>();
            using (var command = connection.CreateCommand())
            {
                try
                {
                    command.CommandText = _sql;

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (reader.Read())
                        {
                            _lsttrackingModel.Add(new TrackingModel
                            {
                                Id = reader.GetString(0),
                                Sensor = reader.GetString(1),
                                Stamp = reader.GetString(2),
                                Unit = reader.GetString(3),
                                Value = reader.GetDouble(4)
                            });
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
            Logger.LogMessage("End SQLiteTransect --> GetDB", ConfigManager.executionEnv);
            return _lsttrackingModel;

        }

        public async Task<bool> Delete(SqliteConnection connection, string? _deleteCoreSql, ILogger<Worker> logger)
        {
            Logger.LogMessage("Start SQLiteTransect --> Delete", ConfigManager.executionEnv);
            using (var command = connection.CreateCommand())
            {
                try
                {
                    command.CommandText = _deleteCoreSql;
                    await command.ExecuteNonQueryAsync();
                }
                catch (Exception ex)
                {
                    logger.LogError("Error while Delete");
                    logger.LogError(ex.Message);
                    command.Dispose();
                    throw;
                }
                Logger.LogMessage("End SQLiteTransect --> Delete", ConfigManager.executionEnv);
                return true;
            }
        }
    }
}
