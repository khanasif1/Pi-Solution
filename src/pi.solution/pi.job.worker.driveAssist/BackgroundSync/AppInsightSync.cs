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
            Console.WriteLine("Sync Started");

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

            }
            catch (Exception ex)
            {
                _logger.LogError("Error StartBackgroundSync");
                _logger.LogError(ex.Message);
                throw;
            }
            return true;

        }
        internal async Task<List<TrackingModel>> GetSyncData(ILogger<Worker> _logger)
        {
            try
            {
                string _sql = "SELECT * FROM DriveTable LIMIT 100 ; ";
                var _sqlInstance = SQLiteManage.Instance;
                return await _sqlInstance.GetDB(_sql, _logger);
            }
            catch (Exception ex)
            {

                _logger.LogError("Error sending telemetry");
                _logger.LogError(ex.Message);
                throw;
            }
        }
        internal async Task<bool> PostTelemetry(AppInsightPayload _payload, ILogger<Worker> _logger)
        {
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

                _logger.LogInformation("Telemetry send");
            }
            catch (Exception ex)
            {
                _logger.LogError("Error sending telemetry");
                _logger.LogError(ex.Message);
                throw;
            }

            _logger.LogInformation("End post telemetry");
            return true;
        }
    }
}
