using pi.job.worker.driveAssist.Common;
using pi.job.worker.driveAssist.DomainModel;
using pi.job.worker.driveAssist.SQLite;
using System;
using System.Collections.Generic;
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

                foreach (TrackingModel _model in _lstSyncData)
                {
                    await PostTelemetry(new AppInsightPayload
                    {
                        _operation = _model.Sensor,
                        _type = AppInsightLanguage.AppInsightTrace,
                        _payload = $"{ _model.Value.ToString()}{ _model.Unit}",
                        _correlationId = _correlationId,
                        _correlationTimeId = Convert.ToDateTime(_model.Stamp)
                    }, _logger);
                    if (true)
                    {

                    }
                }
                Logger.LogMessage("End Sync AppInsightSync --> StartBackgroundSync", ConfigManager.executionEnv);
            }
            catch (Exception ex)
            {
                Logger.LogMessage("Error StartBackgroundSync", ConfigManager.executionEnv);
                Logger.LogMessage(ex.Message, ConfigManager.executionEnv);
                throw;
            }
            return true;

        }
        internal async Task<List<TrackingModel>> GetSyncData(ILogger<Worker> _logger)
        {
            Logger.LogMessage("Start AppInsightSync -->  GetSyncData", ConfigManager.executionEnv);
            try
            {
                string _sql = "SELECT * FROM DriveTable LIMIT 100 ; ";             
                return await SQLiteManage.GetDB(_sql, _logger);
            }
            catch (Exception ex)
            {

                Logger.LogMessage("Error GetSyncData sending telemetry", ConfigManager.executionEnv);
                Logger.LogMessage(ex.Message, ConfigManager.executionEnv);
                throw;
            }
        }
        internal async Task<bool> PostTelemetry(AppInsightPayload _payload, ILogger<Worker> _logger)
        {
            Logger.LogMessage("Start AppInsightSync -->  PostTelemetry", ConfigManager.executionEnv);
            try
            {
                Console.WriteLine("Starting post telemetry");
                //_correlationTimeId = DateTime.UtcNow;
                //string _correlationId = Guid.NewGuid().ToString();
                //string _traceMessage = $"Message from Pi temperature is {_CPUTemp.ToString()}";

                AppInsightHelper _insightHelper = new AppInsightHelper(_payload

                    /*new AppInsightPayload
                    {
                        _operation = "Pi Telemetry",
                        _type = AppInsightLanguage.AppInsightTrace,
                        _payload = _traceMessage,
                        _correlationId = _correlationId,
                        _correlationTimeId = _correlationTimeId

                    }*/);
                await _insightHelper.AppInsightInit();

                Logger.LogMessage("End  AppInsightSync --> PostTelemetry Telemetry send", ConfigManager.executionEnv);
            }
            catch (Exception ex)
            {
                Logger.LogMessage("Error PostTelemetry sending telemetry", ConfigManager.executionEnv);
                Logger.LogMessage(ex.Message, ConfigManager.executionEnv);
                throw;
            }

            Logger.LogMessage("End post telemetry", ConfigManager.executionEnv);
            return true;
        }
    }
}
