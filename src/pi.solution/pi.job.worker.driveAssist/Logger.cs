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
    }
    public enum ExecutionEnv
    {
        dev,
        prod
    }
}
