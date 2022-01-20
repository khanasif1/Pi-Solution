using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pi.job.worker.driveAssist
{
    internal static class Logger
    {
        internal static ILogger<Worker> _logger;
        internal static void LogMessage(string? message, ExecutionEnv _env)
        {

            if (_env == ExecutionEnv.dev)
                Console.WriteLine(message);
            else
                _logger.LogInformation(message);
        }
        internal static void LogMessage(LogType _type, string? message, ExecutionEnv _env)
        {
            if (_env == ExecutionEnv.dev) Console.WriteLine(message);
            else
                {
                    switch (_type)
                    {
                        case LogType.info: _logger.LogInformation(message);
                           break;

                        case LogType.error: _logger.LogError(message);
                            break;

                        case LogType.warning: _logger.LogWarning(message);
                            break;

                    }

                }
            }
        }
        public enum ExecutionEnv
        {
            dev,
            prod
        }
        public enum LogType
        {
            info,
            error,
            warning
        }
    }
