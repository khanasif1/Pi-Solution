using Iot.Device.CpuTemperature;
using Iot.Device.Hcsr04;
using pi.job.worker.driveAssist.BackgroundSync;
using pi.job.worker.driveAssist.Common;
using pi.job.worker.driveAssist.DomainModel;
using pi.job.worker.driveAssist.SQLite;
using System.Data.SqlTypes;
using UnitsNet;
//using Iot.Device.Max7219;
//using System.Device.Spi;

//https://docs.microsoft.com/en-us/dotnet/api/system.transactions.transactionscope?view=net-6.0
//https://github.com/dotnet/iot/blob/main/src/devices/README.md

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

            CheckInternetStatus _internetStatus = new CheckInternetStatus();
            if (_internetStatus.IsInternetConnected()) ConfigSync.GetConfig();
            else ConfigSync.GetOfflineConfig();

        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine($"Start Main method");

            BuzzerAlert _alert = new BuzzerAlert();
            while (!stoppingToken.IsCancellationRequested)
            {

                try
                {
                    double _distance = GetDistance();
                    if (_distance > 140 && _distance <= 220)
                    {
                        Logger.LogMessage(LogType.info, "Between 140 ==> 220 cm, MINOR", ConfigManager.executionEnv);
                        await _alert.RaiseAlert(BuzzerAlert.AlertLevel.minor);
                    }
                    else if (_distance > 60 && _distance <= 140)
                    {
                        Logger.LogMessage(LogType.info, "Between 60 ==> 140 cm, INTERMEDIATE", ConfigManager.executionEnv);

                        await _alert.RaiseAlert(BuzzerAlert.AlertLevel.intermedidate);
                    }
                    else if (_distance > 20 && _distance <= 60)
                    {
                        Logger.LogMessage(LogType.info, "Between 140 ==> 220 cm, MAJOR", ConfigManager.executionEnv);
                        await _alert.RaiseAlert(BuzzerAlert.AlertLevel.major);
                    }
                    else if (_distance <= 20)
                    {
                        Logger.LogMessage(LogType.info, "Less than 20, ULTRA", ConfigManager.executionEnv);
                        await _alert.RaiseAlert(BuzzerAlert.AlertLevel.ultra);
                    }

                    if (_distance != double.MinValue)
                    {
                        Logger.LogMessage(LogType.info, "Local Persistance Started", ConfigManager.executionEnv);
                        await PersistData(_distance);
                    }
                    else
                    {
                        Logger.LogMessage(LogType.info, "Its Safe !! Object too far ", ConfigManager.executionEnv);
                    }
                    if (ConfigManager.enableBackgroundSync)
                    {
                        Logger.LogMessage(LogType.info, "Background Sync Started", ConfigManager.executionEnv);
                        await ManageBackgroundSync();
                    }
                    else
                    {
                        Logger.LogMessage(LogType.info, "Background Sync disabled", ConfigManager.executionEnv);
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
                Logger.LogMessage(LogType.info, $"In loop. Current count {BackgrounsSyncCounter}", ConfigManager.executionEnv);
                if (BackgrounsSyncCounter == 60)
                {
                    CheckInternetStatus _internetStatus = new CheckInternetStatus();
                    if (_internetStatus.IsInternetConnected())
                    {
                        Logger.LogMessage(LogType.info, "Internet is available sync started", ConfigManager.executionEnv);
                        AppInsightSync _appBackSync = new AppInsightSync();
                        bool result = await _appBackSync.StartBackgroundSync(_logger);
                        if (result) Logger.LogMessage(LogType.info, "Sync completed", ConfigManager.executionEnv);
                    }
                    BackgrounsSyncCounter = 0;
                }
            }
            catch (Exception ex)
            {
                Logger.LogMessage(LogType.error, "Error in ManageBackgroundSync", ConfigManager.executionEnv);
                Logger.LogMessage(LogType.error, ex.Message, ConfigManager.executionEnv);
                BackgrounsSyncCounter = 0;
                throw;
            }
            Logger.LogMessage(LogType.info, "End Worker --> ManageBackgroundSync", ConfigManager.executionEnv);
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
                        Logger.LogMessage(LogType.info, $"Distance: {Math.Round(distance.Centimeters, 2)} cm", ConfigManager.executionEnv);
                        _distance = Math.Round(distance.Centimeters, 2);
                    }
                    else
                    {
                        Logger.LogMessage(LogType.info, "Error reading sensor", ConfigManager.executionEnv);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogMessage(LogType.error, "Error in GetDistance", ConfigManager.executionEnv);
                Logger.LogMessage(LogType.error, ex.Message, ConfigManager.executionEnv);
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
                                (LogType.info, $"Temperature from {entry.Sensor.ToString()}: {entry.Temperature.DegreesCelsius} °C", ConfigManager.executionEnv);
                            _temp = entry.Temperature.DegreesCelsius;
                        }
                        else
                        {
                            Logger.LogMessage
                           (LogType.info, "Unable to read Temperature.", ConfigManager.executionEnv);
                        }
                    }
                }
                else
                {
                    Logger.LogMessage
                    (LogType.info, $"CPU temperature is not available", ConfigManager.executionEnv);
                }
            }
            catch (Exception ex)
            {
                Logger.LogMessage(LogType.error, "Error while getting temperature", ConfigManager.executionEnv);
                Logger.LogMessage(LogType.error, ex.Message, ConfigManager.executionEnv);
                throw;
            }
            return _temp;
        }
        private DateTime _correlationTimeId;


        private async Task<bool> PersistData(double? _distance)
        {
            Logger.LogMessage(LogType.info, "Start - Record saved locally", ConfigManager.executionEnv);
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
                Logger.LogMessage(LogType.error, "Error sending telemetry", ConfigManager.executionEnv);
                Logger.LogMessage(LogType.error, ex.Message, ConfigManager.executionEnv);
                throw;
            }
            Logger.LogMessage(LogType.info, " End - Record saved locally", ConfigManager.executionEnv);
            return true;
        }
    }

}