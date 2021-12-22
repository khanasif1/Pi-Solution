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
        SQLiteManage(){}

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
            using (var connection = new SqliteConnection("Data Source=driveAssist.db"))
            {
                connection.Open();
                return connection;
            }
        }
        private static bool CloseConnection()
        {
            connection.Close();
            return true;
        }     
        private static bool InitDB()
        {
            try
            {
                SQLiteTransect _sqlTransection = new SQLiteTransect();
                _sqlTransection.InitDB(OpenConnection());
                CloseConnection();
            }
            catch (Exception)
            {
                CloseConnection();
                throw;
            }
            return true;
        }
        public bool InsertRecords(TrackingModel _model)
        {
            try
            {
                SQLiteTransect _sqlTransection = new SQLiteTransect();
                _sqlTransection.InsertDB(OpenConnection(),_model);
                CloseConnection();
            }
            catch (Exception)
            {

                throw;
            }
            return true;
        }

    }
}
