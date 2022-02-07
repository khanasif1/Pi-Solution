using Iot.Device.Buzzer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pi.job.worker.driveAssist.Common
{
    internal class BuzzerAlert
    {
        public async Task<bool> RaiseAlert(AlertLevel _level)
        {
            Logger.LogMessage(LogType.info, $"Start Alert Level {_level.ToString()}", ConfigManager.executionEnv);
            using (Buzzer _buzzer = new Buzzer(21))
            {
                if(_level == AlertLevel.minor)
                {
                    Logger.LogMessage(LogType.info, "Minor Alert", ConfigManager.executionEnv);
                    _buzzer.StartPlaying(100);
                    Thread.Sleep(1000);
                    _buzzer.StopPlaying();
                }
                else if(_level == AlertLevel.intermedidate)
                {
                    Logger.LogMessage(LogType.info, "Intermediate Alert", ConfigManager.executionEnv);
                    _buzzer.StartPlaying(200);
                    Thread.Sleep(1000);
                    _buzzer.StopPlaying();
                }
                else if (_level == AlertLevel.major)
                {
                    Logger.LogMessage(LogType.info, "Major Alert", ConfigManager.executionEnv);
                    _buzzer.StartPlaying(300);
                    Thread.Sleep(1000);
                    _buzzer.StopPlaying();
                }
                else if (_level == AlertLevel.ultra)
                {
                    Logger.LogMessage(LogType.info, "Ultra Alert", ConfigManager.executionEnv);
                    _buzzer.StartPlaying(400);
                    Thread.Sleep(1000);
                    _buzzer.StopPlaying();
                }     
            }           
            Logger.LogMessage(LogType.info, $"End Alert Level {_level.ToString()}", ConfigManager.executionEnv);
            return true;
        }
        public enum AlertLevel
        {
            minor,
            intermedidate,
            major,
            ultra
        }

    }
}
