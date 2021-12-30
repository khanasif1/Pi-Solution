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
            Logger.LogMessage("Start Sync AppInsightSync --> StartBackgroundSync", ConfigManager.executionEnv);

            try
            {
                List<TrackingModel> _lstSyncData = await GetSyncData(_logger);

                string _correlationId = Guid.NewGuid().ToString();
                Logger.LogMessage($"Start AppInsightSync --> PostTelemetry for {_lstSyncData.Count} records", ConfigManager.executionEnv);
                //foreach (TrackingModel _model in _lstSyncData)
                //{
                await PostTelemetry(_lstSyncData, _logger);
                //await PostTelemetry(new AppInsightPayload
                //{
                //    _operation = _model.Sensor,
                //    _type = AppInsightLanguage.AppInsightTrace,
                //    _payload = $"{ _model.Value.ToString()}{ _model.Unit}",
                //    _correlationId = _correlationId,
                //    //_correlationTimeId = DateTime.Parse(_model.Stamp)
                //}, _logger);
                Logger.LogMessage($"Posting record created {_lstSyncData.Last().Stamp}", ConfigManager.executionEnv);
                await Task.Delay(500);
                //}
                Logger.LogMessage($"End AppInsightSync --> PostTelemetry for {_lstSyncData.Count} records", ConfigManager.executionEnv);
                /*
                 * Clean records posted
                 */
                Logger.LogMessage($"Start AppInsightSync --> Clean records posted ", ConfigManager.executionEnv);
                bool status = await SQLiteManage.Delete(_lstSyncData, _logger);
                Logger.LogMessage($"End AppInsightSync --> Clean records posted ", ConfigManager.executionEnv);
            }
            catch (Exception ex)
            {
                Logger.LogMessage("Error StartBackgroundSync", ConfigManager.executionEnv);
                Logger.LogMessage(ex.Message, ConfigManager.executionEnv);
                throw;
            }
            Logger.LogMessage("End Sync AppInsightSync --> StartBackgroundSync", ConfigManager.executionEnv);
            return true;

        }
        internal async Task<List<TrackingModel>> GetSyncData(ILogger<Worker> _logger)
        {
            Logger.LogMessage("Start AppInsightSync -->  GetSyncData", ConfigManager.executionEnv);
            try
            {
                string _sql = "SELECT * FROM DriveTable LIMIT 100 ; ";
                List<TrackingModel> _model = await SQLiteManage.GetDB(_sql, _logger);
                Logger.LogMessage($"Got {_model.Count.ToString()} records to sync", ConfigManager.executionEnv);
                return _model;
            }
            catch (Exception ex)
            {

                Logger.LogMessage("Error GetSyncData sending telemetry", ConfigManager.executionEnv);
                Logger.LogMessage(ex.Message, ConfigManager.executionEnv);
                throw;
            }
        }
        internal async Task<bool> PostTelemetry(List<TrackingModel> _payload, ILogger<Worker> _logger)
        {
            try
            {
                Logger.LogMessage("In AppInsightSync --> PostTelemetry", ConfigManager.executionEnv);
                //AppInsightHelper _insightHelper = new AppInsightHelper(_payload);
                //await _insightHelper.AppInsightInit();
                //await AppInsightHelper.AppInsightInit(_payload);
                AppInsightAPIHelper.Post(_payload);
            }
            catch (Exception ex)
            {
                Logger.LogMessage("Error PostTelemetry sending telemetry", ConfigManager.executionEnv);
                Logger.LogMessage(ex.Message, ConfigManager.executionEnv);
                throw;
            }
            return true;
        }
    }
}
