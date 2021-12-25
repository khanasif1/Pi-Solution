using Iot.Device.CpuTemperature;
using Iot.Device.Hcsr04;
using pi.job.worker.driveAssist.BackgroundSync;
using pi.job.worker.driveAssist.Common;
using pi.job.worker.driveAssist.DomainModel;
using pi.job.worker.driveAssist.SQLite;
using UnitsNet;

namespace pi.job.worker.driveAssist
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private int BackgrounsSyncCounter = 0;
        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine($"Start Main method");
            while (!stoppingToken.IsCancellationRequested)
            {
                //_logger.LogInformation("Information - Worker running at: {time}", DateTimeOffset.Now);
                //_logger.LogWarning("Warning - Worker running at: {time}", DateTimeOffset.Now);
                //_logger.LogError("Error - Worker running at: {time}", DateTimeOffset.Now);
                //_logger.LogCritical("Critical - Worker running at: {time}", DateTimeOffset.Now);
                //_logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);     

                double _distance = GetDistance();
                if (_distance != double.MinValue)
                {
                    await PersistData(_distance);
                }
                if (ConfigManager.EnableBackgroundSync)
                    await ManageBackgroundSync();

                await Task.Delay(1000, stoppingToken);
            }
        }

        private async Task ManageBackgroundSync()
        {
            BackgrounsSyncCounter++;
            Console.WriteLine($"In loop. Current count {BackgrounsSyncCounter}");
            if (BackgrounsSyncCounter == 10)
            {
                CheckInternetStatus _internetStatus = new CheckInternetStatus();
                if (_internetStatus.IsInternetConnected())
                {
                    AppInsightSync _appBackSync = new AppInsightSync();
                    await _appBackSync.StartBackgroundSync(_logger);
                }
                BackgrounsSyncCounter = 0;
            }
        }

        private double GetDistance()
        {
            double _distance = double.MinValue;
            using (var sonar = new Hcsr04(4, 17))
            {
                if (sonar.TryGetDistance(out Length distance))
                {
                    Console.WriteLine($"Distance: {Math.Round(distance.Centimeters, 2)} cm");
                    _distance = Math.Round(distance.Centimeters, 2);

                }
                else
                {
                    Console.WriteLine("Error reading sensor");
                }
            }
            return _distance;
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


        private async Task<bool> PersistData(double? _distance)
        {
            _logger.LogInformation("Start - Record saved locally");
            try
            {
                TrackingModel _model = new TrackingModel
                {
                    Id = Guid.NewGuid().ToString(),
                    Sensor = "DistanceSensor",
                    Stamp = DateTime.UtcNow.ToString(),
                    Unit = "cms",
                    Value = _distance.HasValue ? _distance.Value : 0
                };
                var _sqlInstance = SQLiteManage.Instance;
                await _sqlInstance.InsertRecords(_model, _logger);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error sending telemetry");
                _logger.LogError(ex.Message);
                throw;
            }
            _logger.LogInformation(" End - Record saved locally");
            return true;
        }
    }
}