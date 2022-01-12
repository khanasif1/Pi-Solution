using pi.job.worker.driveAssist.Common;
using pi.job.worker.driveAssist.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace pi.job.worker.driveAssist.Insights
{
    internal class AppInsightAPIHelper
    {

        //static string json = @"[{""DemoField1"":""Asif"",""DemoField2"":""24""},{""DemoField3"":""Khan"",""DemoField4"":""Sydney""}]";

        // Update customerId to your Log Analytics workspace ID
        static string customerId = ConfigManager.customerId;

        // For sharedKey, use either the primary or the secondary Connected Sources client authentication key   
        static string sharedKey = ConfigManager.sharedKey;

        // LogName is name of the event type that is being submitted to Azure Monitor
        static string LogName = ConfigManager.logAnalyticsTable;

        // You can use an optional field to specify the timestamp from the data. If the time field is not specified, Azure Monitor assumes the time is the message ingestion time
        static string TimeStampField = "";

        public static bool Post(List<TrackingModel> _lstpayload)
        {
            try
            {
                Logger.LogMessage("In AppInsightAPIHelper --> Post", ConfigManager.executionEnv);
                string json = BuildJson(_lstpayload);
                // Create a hash for the API signature
                var datestring = DateTime.UtcNow.ToString("r");
                var jsonBytes = Encoding.UTF8.GetBytes(json);
                string stringToHash = "POST\n" + jsonBytes.Length + "\napplication/json\n" + "x-ms-date:" + datestring + "\n/api/logs";
                string hashedString = BuildSignature(stringToHash, sharedKey);
                string signature = "SharedKey " + customerId + ":" + hashedString;
                Logger.LogMessage($"Start Posting {_lstpayload.Count.ToString()} records", ConfigManager.executionEnv);
                PostData(signature, datestring, json);
                Logger.LogMessage($"End Posting {_lstpayload.Count.ToString()} records", ConfigManager.executionEnv);
            }
            catch (Exception ex)
            {
                Logger.LogMessage("Error AppInsightAPIHelper --> Post sending telemetry", ConfigManager.executionEnv);
                Logger.LogMessage(ex.Message, ConfigManager.executionEnv);
                throw;                
            }
            return true;
        }

        internal static string BuildJson(List<TrackingModel> _lstpayload)
        {
            Logger.LogMessage($"Building Json paylod to post", ConfigManager.executionEnv);
            StringBuilder _jsonSb = new StringBuilder();
            try
            {
                TrackingModel last = _lstpayload.Last();
                _jsonSb.Append("[");
                foreach (var _payload in _lstpayload)
                {
                    _jsonSb.Append("{");
                    _jsonSb.Append(@$"""sensor"":""{ _payload.Sensor}"",");
                    _jsonSb.Append(@$"""id"":""{_payload.Id}"",");
                    _jsonSb.Append(@$"""stamp"":""{_payload.Stamp}"",");
                    _jsonSb.Append(@$"""value"":""{_payload.Value}"",");
                    _jsonSb.Append(@$"""unit"":""{_payload.Unit}""");
                    if (_payload.Equals(last))
                        _jsonSb.Append("}");
                    else
                        _jsonSb.Append("},");
                }
                _jsonSb.Append("]");
            }
            catch (Exception ex)
            {
                Logger.LogMessage("Error AppInsightAPIHelper --> BuildJson ", ConfigManager.executionEnv);
                Logger.LogMessage(ex.Message, ConfigManager.executionEnv);
                throw;
            }
            return _jsonSb.ToString();
        }
        // Build the API signature
        public static string BuildSignature(string message, string secret)
        {
            var encoding = new System.Text.ASCIIEncoding();
            byte[] keyByte = Convert.FromBase64String(secret);
            byte[] messageBytes = encoding.GetBytes(message);
            using (var hmacsha256 = new HMACSHA256(keyByte))
            {
                byte[] hash = hmacsha256.ComputeHash(messageBytes);
                return Convert.ToBase64String(hash);
            }
        }

        // Send a request to the POST API endpoint
        public async static void PostData(string signature, string date, string json)
        {
            try
            {
                string url = "https://" + customerId + ".ods.opinsights.azure.com/api/logs?api-version=2016-04-01";

                System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.DefaultRequestHeaders.Add("Log-Type", LogName);
                client.DefaultRequestHeaders.Add("Authorization", signature);
                client.DefaultRequestHeaders.Add("x-ms-date", date);
                client.DefaultRequestHeaders.Add("time-generated-field", TimeStampField);

                HttpContent httpContent = new StringContent(json, Encoding.UTF8);
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                HttpResponseMessage response = await client.PostAsync(new Uri(url), httpContent);
                if (response != null)
                {
                    if (response.IsSuccessStatusCode)
                    {
                        Logger.LogMessage($"Message posted to LogAnalytics with SUCCESS", ConfigManager.executionEnv);
                        Logger.LogMessage("Telemetry Posted with Success", ConfigManager.executionEnv);
                    }
                    else
                    {
                        Logger.LogMessage($"Message posted to LogAnalytics response NOT SUCCESS", ConfigManager.executionEnv);
                        Logger.LogMessage("Telemetry Post was not success", ConfigManager.executionEnv);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogMessage("Error AppInsightAPIHelper --> PostData ", ConfigManager.executionEnv);
                Logger.LogMessage(ex.Message, ConfigManager.executionEnv);
                throw;
            }
        }
    }
}
