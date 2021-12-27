using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static pi.job.worker.driveAssist.Logger;

namespace pi.job.worker.driveAssist.Common
{
    public static class ConfigManager
    {
        public const bool EnableBackgroundSync = false;
        public const ExecutionEnv executionEnv = ExecutionEnv.dev;
    }
}
