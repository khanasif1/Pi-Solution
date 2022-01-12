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
        public static bool enableBackgroundSync;
        public static int backgroundSyncRecordCount = int.MinValue;
        public static ExecutionEnv executionEnv;
        public static string logAnalyticsTable = String.Empty;
        public static string customerId = String.Empty;
        public static string sharedKey = String.Empty;
    }
}
