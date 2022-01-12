using pi.job.worker.driveAssist.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pi.job.worker.driveAssist.Common
{
    internal static class ConfigSync
    {
        public static void GetConfig()
        {
            var builder = new ConfigurationBuilder();

            builder.AddAzureAppConfiguration("Endpoint=https://piconfig.azconfig.io;Id=btxc-lg-s0:mABNXOpscuv36p8rIWu0;Secret=m3TKxcYZsc5MV1PSneUFp0cqEgqYPZDy9oUkCHcvI3I=");

            var config = builder.Build();


            ConfigManager.enableBackgroundSync = !String.IsNullOrEmpty(config["enableBackgroundSync"]) ?
                Boolean.Parse(config["enableBackgroundSync"]) : false;
            ConfigManager.backgroundSyncRecordCount = !String.IsNullOrEmpty(config["backgroundSyncRecordCount"]) ?
                Int32.Parse(config["backgroundSyncRecordCount"]) : int.MinValue;

            if (!String.IsNullOrEmpty(config["executionEnv"]))
                Enum.TryParse(config["executionEnv"], out ConfigManager.executionEnv);
            else ConfigManager.executionEnv = ExecutionEnv.dev;

            ConfigManager.logAnalyticsTable = !String.IsNullOrEmpty(config["logAnalyticsTable"]) ?
                config["logAnalyticsTable"] : String.Empty;

            ConfigManager.customerId = !String.IsNullOrEmpty(config["customerId"]) ?
                config["customerId"] : String.Empty;

            ConfigManager.sharedKey = !String.IsNullOrEmpty(config["sharedKey"]) ?
                config["sharedKey"] : String.Empty;

        }
        public static void GetOfflineConfig()
        {
            ConfigManager.enableBackgroundSync = true;
            ConfigManager.backgroundSyncRecordCount = 1000;
            ConfigManager.executionEnv = ExecutionEnv.dev;
            ConfigManager.logAnalyticsTable = "AutoAssist";
            ConfigManager.customerId = "76c487b3-73ba-4a20-b18b-8de46ebcb135";
            ConfigManager.sharedKey = "EFxb2JDn+J3jWjwGB3bHqX1sLhEUi3smzjsaY5K3bpOFMg3LD40BpRpZUdcKONL3IwqWAriKv1kjTUb/nDbEGA==";

        }
    }
}
