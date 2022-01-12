using Microsoft.Data.Sqlite;
using pi.job.worker.driveAssist.Common;
using pi.job.worker.driveAssist.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pi.job.worker.driveAssist.SQLite
{
    public static class SQLiteManage
    {
        private static readonly object padlock = new object();
        private static SqliteConnection? connection = null;


        public static SqliteConnection OpenConnection()
        {
            Logger.LogMessage("Start  SQLiteManage --> OpenConnection", ConfigManager.executionEnv);
            connection = new SqliteConnection("Data Source=driveAssist.db");
            connection.Open();
            Logger.LogMessage("End SQLiteManage --> OpenConnection", ConfigManager.executionEnv);
            return connection;

        }
        public static bool CloseConnection()
        {
            Logger.LogMessage("Start SQLiteManage --> CloseConnection", ConfigManager.executionEnv);
            if (connection != null && connection.State == System.Data.ConnectionState.Open)
                connection.Close();
            return true;
        }
        public static bool InitDB()
        {
            Logger.LogMessage("Start SQLiteManage --> InitDB", ConfigManager.executionEnv);
            try
            {                
                {
                    SQLiteTransect _sqlTransection = new SQLiteTransect();
                    SqliteConnection connection = OpenConnection();
                    _sqlTransection.InitDB(connection);
                    CloseConnection();
                }             
            }
            catch (Exception)
            {
                CloseConnection();
                throw;
            }
            Logger.LogMessage("End SQLiteManage --> InitDB", ConfigManager.executionEnv);
            return true;
        }
        public async static Task<bool> InsertRecords(TrackingModel _model, ILogger<Worker> logger)
        {
            Logger.LogMessage("Start SQLiteManage --> InsertRecords", ConfigManager.executionEnv);
            try
            {
                SQLiteTransect _sqlTransection = new SQLiteTransect();
                await _sqlTransection.InsertDB(OpenConnection(), _model, logger);
                CloseConnection();
            }
            catch (Exception ex)
            {
                Logger.LogMessage("Error while inserting temperature", ConfigManager.executionEnv);

                Logger.LogMessage(ex.Message, ConfigManager.executionEnv);
                throw;
            }
            Logger.LogMessage("End SQLiteManage --> InsertRecords", ConfigManager.executionEnv);
            return true;
        }
        
        public async static Task<List<TrackingModel>> GetDB(string? _sql, ILogger<Worker> logger)
        {
            Logger.LogMessage("Start SQLiteManage --> GetDB", ConfigManager.executionEnv);
            try
            {
                SQLiteTransect _sqlTransection = new SQLiteTransect();
                List<TrackingModel> _response = await _sqlTransection.GetDB(OpenConnection(), _sql, logger);
                CloseConnection();
                Logger.LogMessage("End SQLiteManage --> GetDB", ConfigManager.executionEnv);
                return _response;
            }
            catch (Exception ex)
            {
                Logger.LogMessage("Error while getting temperature", ConfigManager.executionEnv);

                Logger.LogMessage(ex.Message, ConfigManager.executionEnv);
                throw;
            }
        }
        public async static Task<bool> Delete(List<TrackingModel> _lstTrackPublished, ILogger<Worker> logger)
        {
            Logger.LogMessage($"Start SQLiteManage --> Deleteing record count {_lstTrackPublished.Count}", ConfigManager.executionEnv);
            try
            {
                string _deleteId = String.Join(" ,", _lstTrackPublished.Select(_track => "'" + _track.Id + "'"));
                string _deleteCoreSql = $" DELETE FROM DriveTable WHERE ID IN ( {_deleteId} ); ";
                SQLiteTransect _sqlTransection = new SQLiteTransect();
                await _sqlTransection.Delete(OpenConnection(), _deleteCoreSql, logger);
                CloseConnection();
            }
            catch (Exception ex)
            {
                Logger.LogMessage("Error while Delete", ConfigManager.executionEnv);
                Logger.LogMessage(ex.Message, ConfigManager.executionEnv);
                throw;
            }
            Logger.LogMessage("End SQLiteManage --> Delete", ConfigManager.executionEnv);
            return true;
        }    
    }
}
