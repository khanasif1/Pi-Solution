using Microsoft.Data.Sqlite;
using pi.job.worker.driveAssist.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pi.job.worker.driveAssist.SQLite
{
    public sealed class SQLiteManage
    {
        private static SQLiteManage? instance = null;
        private static readonly object padlock = new object();
        private static SqliteConnection? connection = null;
        SQLiteManage() { }

        public static SQLiteManage Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        InitDB();
                        instance = new SQLiteManage();
                    }
                    return instance;
                }
            }
        }
        private static SqliteConnection OpenConnection()
        {
            connection = new SqliteConnection("Data Source=driveAssist.db");
            connection.Open();
            return connection;

        }
        private static bool CloseConnection()
        {
            if (connection != null && connection.State == System.Data.ConnectionState.Open)
                connection.Close();
            return true;
        }
        private static bool InitDB()
        {
            try
            {
                SQLiteTransect _sqlTransection = new SQLiteTransect();
                SqliteConnection connection = OpenConnection();
                _sqlTransection.InitDB(connection);
                CloseConnection();
            }
            catch (Exception)
            {
                CloseConnection();
                throw;
            }
            return true;
        }
        public async Task<bool> InsertRecords(TrackingModel _model, ILogger<Worker> logger)
        {
            try
            {
                SQLiteTransect _sqlTransection = new SQLiteTransect();
                await _sqlTransection.InsertDB(OpenConnection(), _model, logger);
                CloseConnection();
            }
            catch (Exception ex)
            {
                logger.LogError("Error while getting temperature");

                logger.LogError(ex.Message);
                throw;
            }
            return true;
        }

    }
}
