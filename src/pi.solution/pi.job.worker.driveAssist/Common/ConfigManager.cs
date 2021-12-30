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
        public const bool EnableBackgroundSync = true;
        public const ExecutionEnv executionEnv = ExecutionEnv.dev;
        public const string LogAnalyticsTable= "AutoAssist";
        public const string customerId = "76c487b3-73ba-4a20-b18b-8de46ebcb135";          
        public const string sharedKey = "EFxb2JDn+J3jWjwGB3bHqX1sLhEUi3smzjsaY5K3bpOFMg3LD40BpRpZUdcKONL3IwqWAriKv1kjTUb/nDbEGA==";
    }
}
