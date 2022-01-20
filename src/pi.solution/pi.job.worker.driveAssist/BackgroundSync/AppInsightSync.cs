using pi.job.worker.driveAssist.Common;
using pi.job.worker.driveAssist.DomainModel;
using pi.job.worker.driveAssist.Insights;
using pi.job.worker.driveAssist.SQLite;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pi.job.worker.driveAssist.BackgroundSync
{
    internal class AppInsightSync
    {
        internal async Task<bool> StartBackgroundSync(ILogger<Worker> _logger)
        {
            Logger.LogMessage(LogType.info,"Start Sync AppInsightSync --> StartBackgroundSync", ConfigManager.executionEnv);

            try
            {
                List<TrackingModel> _lstSyncData = await GetSyncData(_logger);

                string _correlationId = Guid.NewGuid().ToString();
                Logger.LogMessage(LogType.info, $"Start AppInsightSync --> PostTelemetry for {_lstSyncData.Count} records", ConfigManager.executionEnv);

                await PostTelemetry(_lstSyncData, _logger);

                Logger.LogMessage(LogType.info, $"Posting record created {_lstSyncData.Last().Stamp}", ConfigManager.executionEnv);
                await Task.Delay(500);
                //}
                Logger.LogMessage(LogType.info, $"End AppInsightSync --> PostTelemetry for {_lstSyncData.Count} records", ConfigManager.executionEnv);
                /*
                 * Clean records posted
                 */
                Logger.LogMessage(LogType.info,$"Start AppInsightSync --> Clean records posted ", ConfigManager.executionEnv);
                bool status = await SQLiteManage.Delete(_lstSyncData, _logger);
                Logger.LogMessage($"End AppInsightSync --> Clean records posted ", ConfigManager.executionEnv);
            }
            catch (Exception ex)
            {
                Logger.LogMessage(LogType.error,"Error StartBackgroundSync", ConfigManager.executionEnv);
                Logger.LogMessage(LogType.error,ex.Message, ConfigManager.executionEnv);
                throw;
            }
            Logger.LogMessage(LogType.info,"End Sync AppInsightSync --> StartBackgroundSync", ConfigManager.executionEnv);
            return true;

        }
        internal async Task<List<TrackingModel>> GetSyncData(ILogger<Worker> _logger)
        {
            Logger.LogMessage(LogType.info,"Start AppInsightSync -->  GetSyncData", ConfigManager.executionEnv);
            try
            {
                string _sql = $"SELECT * FROM DriveTable LIMIT {ConfigManager.backgroundSyncRecordCount} ; ";
                List<TrackingModel> _model = await SQLiteManage.GetDB(_sql, _logger);
                Logger.LogMessage(LogType.info,$"Got {_model.Count.ToString()} records to sync", ConfigManager.executionEnv);
                return _model;
            }
            catch (Exception ex)
            {

                Logger.LogMessage(LogType.error,"Error GetSyncData sending telemetry", ConfigManager.executionEnv);
                Logger.LogMessage(LogType.error,ex.Message, ConfigManager.executionEnv);
                throw;
            }
        }
        internal async Task<bool> PostTelemetry(List<TrackingModel> _payload, ILogger<Worker> _logger)
        {
            try
            {
                Logger.LogMessage(LogType.info,"In AppInsightSync --> PostTelemetry", ConfigManager.executionEnv);
                //AppInsightHelper _insightHelper = new AppInsightHelper(_payload);
                //await _insightHelper.AppInsightInit();
                //await AppInsightHelper.AppInsightInit(_payload);
                AppInsightAPIHelper.Post(_payload);
            }
            catch (Exception ex)
            {
                Logger.LogMessage(LogType.error,"Error PostTelemetry sending telemetry", ConfigManager.executionEnv);
                Logger.LogMessage(LogType.error,ex.Message, ConfigManager.executionEnv);
                throw;
            }
            return true;
        }
    }
}
