using Iot.Device.CpuTemperature;
using Iot.Device.Hcsr04;
using pi.job.worker.driveAssist.DomainModel;
using pi.job.worker.driveAssist.SQLite;
using UnitsNet;

namespace pi.job.worker.driveAssist
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            
            //while (!stoppingToken.IsCancellationRequested)
            //{
            //_logger.LogInformation("Information - Worker running at: {time}", DateTimeOffset.Now);
            //_logger.LogWarning("Warning - Worker running at: {time}", DateTimeOffset.Now);
            //_logger.LogError("Error - Worker running at: {time}", DateTimeOffset.Now);
            //_logger.LogCritical("Critical - Worker running at: {time}", DateTimeOffset.Now);
            //_logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

            //    //double _CPUTemp = GetCPUTemp();
            //    //PostTelemetry(_CPUTemp);

            //    using (var sonar = new Hcsr04(4, 17))
            //    {
            //        if (sonar.TryGetDistance(out Length distance))
            //        {
            //            Console.WriteLine($"Distance: {Math.Round(distance.Centimeters, 2)} cm");
            //        }
            //        else
            //        {
            //            Console.WriteLine("Error reading sensor");
            //        }
            //    }


            await Task.Delay(1000, stoppingToken);
            //}
        }
        private double GetCPUTemp()
        {
            double _temp = 0;
            try
            {

                using CpuTemperature cpuTemperature = new CpuTemperature();

                if (cpuTemperature.IsAvailable)
                {
                    var temperature = cpuTemperature.ReadTemperatures();
                    foreach (var entry in temperature)
                    {
                        if (!double.IsNaN(entry.Temperature.DegreesCelsius))
                        {
                           _logger.LogInformation
                                ($"Temperature from {entry.Sensor.ToString()}: {entry.Temperature.DegreesCelsius} °C");
                            _temp = entry.Temperature.DegreesCelsius;
                        }
                        else
                        {                           
                            _logger.LogInformation
                            ("Unable to read Temperature.");
                        }
                    }
                }
                else
                {                    
                    _logger.LogInformation
                    ($"CPU temperature is not available");
                }
            }
            catch (Exception ex)
            {               
                _logger.LogError("Error while getting temperature");
                
                _logger.LogError(ex.Message);
                throw;
            }
            return _temp;
        }
        private DateTime _correlationTimeId;
        private void PostTelemetry(double _CPUTemp)
        {
            try
            {
                Console.WriteLine("Starting post telemetry");
                _correlationTimeId = DateTime.UtcNow;
                string _correlationId = Guid.NewGuid().ToString();
                string _traceMessage = $"Message from Pi temperature is {_CPUTemp.ToString()}";
                AppInsightHelper _insightHelper = new AppInsightHelper(
                    new AppInsightPayload
                    {
                        _operation = "Pi Telemetry",
                        _type = AppInsightLanguage.AppInsightTrace,
                        _payload = _traceMessage,
                        _correlationId = _correlationId,
                        _correlationTimeId = _correlationTimeId

                    });
                _insightHelper.AppInsightInit();
                                
                _logger.LogInformation
                ("Telemetry send");
            }
            catch (Exception ex)
            {
               _logger.LogError("Error sending telemetry");
                
                _logger.LogError
                (ex.Message);
                throw;
            }
            
            _logger.LogInformation
            ("End post telemetry");
        }

        private async Task<bool> PersistData(TrackingModel _model)
        {
            //await PersistData(new TrackingModel
            //{
            //    Id = Guid.NewGuid().ToString(),
            //    Sensor = "Distant Sensor",
            //    Stamp = DateTime.Now.ToString(),
            //    Value = i * 22.56,
            //    Unit = "cms"
            //}
            try
            {
                var _sqlInstance = SQLiteManage.Instance;
                await _sqlInstance.InsertRecords(_model, _logger);
            }
            catch (Exception)
            {

                throw;
            }
            return true;
        }
    }
}