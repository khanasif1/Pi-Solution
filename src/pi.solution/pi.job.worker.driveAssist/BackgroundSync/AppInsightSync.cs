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
            return true;
            //return await Task.Run(() =>
            //PostTelemetry(new AppInsightPayload(), _logger)
            //);
        }
        internal async Task<bool> GetSyncData(ILogger<Worker> _logger)
        {
            return true;
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
