using Iot.Device.CpuTemperature;
using Iot.Device.Hcsr04;
using pi.job.worker.driveAssist.BackgroundSync;
using pi.job.worker.driveAssist.Common;
using pi.job.worker.driveAssist.DomainModel;
using pi.job.worker.driveAssist.SQLite;
using System.Data.SqlTypes;
using UnitsNet;

//https://docs.microsoft.com/en-us/dotnet/api/system.transactions.transactionscope?view=net-6.0

namespace pi.job.worker.driveAssist
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private int BackgrounsSyncCounter = 0;
        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
            Logger._logger = logger;
            SQLiteManage.InitDB();
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

                try
                {
                    double _distance = GetDistance();
                    if (_distance != double.MinValue)
                    {
                        Logger.LogMessage("Local Persistance Started", ConfigManager.executionEnv);
                        await PersistData(_distance);
                    }
                    if (ConfigManager.EnableBackgroundSync)
                    {
                        Logger.LogMessage("Background Sync Started", ConfigManager.executionEnv);
                        await ManageBackgroundSync();
                    }
                    else
                    {
                        Logger.LogMessage("Background Sync disabled", ConfigManager.executionEnv);
                    }
                }
                catch (Exception)
                {

                }

                await Task.Delay(1000, stoppingToken);
            }
        }

        private async Task<bool> ManageBackgroundSync()
        {
            Logger.LogMessage("Start Worker --> ManageBackgroundSync", ConfigManager.executionEnv);
            try
            {
                BackgrounsSyncCounter++;
                Logger.LogMessage($"In loop. Current count {BackgrounsSyncCounter}", ConfigManager.executionEnv);
                if (BackgrounsSyncCounter == 60)
                {
                    CheckInternetStatus _internetStatus = new CheckInternetStatus();
                    if (_internetStatus.IsInternetConnected())
                    {
                        Logger.LogMessage("Internet is available sync started", ConfigManager.executionEnv);
                        AppInsightSync _appBackSync = new AppInsightSync();
                        bool result = await _appBackSync.StartBackgroundSync(_logger);
                        if (result) Logger.LogMessage("Sync completed", ConfigManager.executionEnv);
                    }
                    BackgrounsSyncCounter = 0;
                }
            }
            catch (Exception ex)
            {
                Logger.LogMessage("Error in ManageBackgroundSync", ConfigManager.executionEnv);
                Logger.LogMessage(ex.Message, ConfigManager.executionEnv);
                BackgrounsSyncCounter = 0;
                throw;
            }
            Logger.LogMessage("End Worker --> ManageBackgroundSync", ConfigManager.executionEnv);
            return true;
        }

        private double GetDistance()
        {
            //double _distance = new Random().Next(10, 500);
            //Logger.LogMessage($"Distance: {Math.Round(_distance,2)} cm", ConfigManager.executionEnv);

            double _distance = double.MinValue;
            try
            {
                using (var sonar = new Hcsr04(4, 17))
                {
                    if (sonar.TryGetDistance(out Length distance))
                    {
                        Logger.LogMessage($"Distance: {Math.Round(distance.Centimeters, 2)} cm", ConfigManager.executionEnv);
                        _distance = Math.Round(distance.Centimeters, 2);
                    }
                    else
                    {
                        Logger.LogMessage("Error reading sensor", ConfigManager.executionEnv);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogMessage("Error in GetDistance", ConfigManager.executionEnv);
                Logger.LogMessage(ex.Message, ConfigManager.executionEnv);
                throw;
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
                            Logger.LogMessage
                                ($"Temperature from {entry.Sensor.ToString()}: {entry.Temperature.DegreesCelsius} °C", ConfigManager.executionEnv);
                            _temp = entry.Temperature.DegreesCelsius;
                        }
                        else
                        {
                            Logger.LogMessage
                           ("Unable to read Temperature.", ConfigManager.executionEnv);
                        }
                    }
                }
                else
                {
                    Logger.LogMessage
                    ($"CPU temperature is not available", ConfigManager.executionEnv);
                }
            }
            catch (Exception ex)
            {
                Logger.LogMessage("Error while getting temperature", ConfigManager.executionEnv);
                Logger.LogMessage(ex.Message, ConfigManager.executionEnv);
                throw;
            }
            return _temp;
        }
        private DateTime _correlationTimeId;


        private async Task<bool> PersistData(double? _distance)
        {
            Logger.LogMessage("Start - Record saved locally", ConfigManager.executionEnv);
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
                await SQLiteManage.InsertRecords(_model, _logger);
            }
            catch (Exception ex)
            {
                Logger.LogMessage("Error sending telemetry", ConfigManager.executionEnv);
                Logger.LogMessage(ex.Message, ConfigManager.executionEnv);
                throw;
            }
            Logger.LogMessage(" End - Record saved locally", ConfigManager.executionEnv);
            return true;
        }
    }
}