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

            builder.AddAzureAppConfiguration("Endpoint=https://piconfigure.azconfig.io;Id=5bBV-lg-s0:6cXg/tLM/niXOsV5juba;Secret=8f7zENyNZwt8P0/fOK3HCvrYOBku2y8BtGou+bTn2vo=");

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
            ConfigManager.customerId = "30a28886-3c3b-47eb-8b9a-9a80d1d2d22e";
            ConfigManager.sharedKey = "UpreDRkNjpMWbaC0ib55tyDFmC8M/w8gnxU8NtcIc9bK9oAa+7JwCZzvXzceRWKdpsGka4W5E1SlmtpcIBj3gw==";

        }
    }
}
