using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.DependencyCollector;
using Microsoft.ApplicationInsights.Extensibility;

namespace pi.job.worker.driveAssist
{
    public class AppInsightHelper
    {
        private AppInsightPayload _payload;
        public AppInsightHelper(AppInsightPayload payload) 
        {
            this._payload= payload;
        }
    
        public void AppInsightInit()
        {           
            TelemetryConfiguration configuration = TelemetryConfiguration.CreateDefault();

            configuration.InstrumentationKey = "d48146d0-e4f8-4d31-8be1-4bce7e1be59a";
            configuration.TelemetryInitializers.Add(new HttpDependenciesParsingTelemetryInitializer());

            var telemetryClient = new TelemetryClient(configuration);

            if (_payload._type.CompareTo(AppInsightLanguage.AppInsightEvent) == 0)
            {
                EventTelemetry _evt = new EventTelemetry();
                _evt.Context.Operation.Id = _payload._correlationId;
                _evt.Name = _payload._payload;
                telemetryClient.TrackEvent(_payload._payload);
            }
            if (_payload._type.CompareTo(AppInsightLanguage.AppInsightTrace) == 0)
            {
                TraceTelemetry _trct = new TraceTelemetry();
                _trct.Message = _payload._payload;
                _trct.Context.Operation.Id = _payload._correlationId;

                telemetryClient.TrackTrace(_trct);
            }
            if (_payload._type.CompareTo(AppInsightLanguage.AppInsightException) == 0)
            {
                ExceptionTelemetry _et = new ExceptionTelemetry();
                _et.Exception = _payload._ex;
                _et.Context.Operation.Id = _payload._correlationId;
                telemetryClient.TrackException(_et);
            }

            telemetryClient.Flush();

        }
    }
    public enum AppInsightLanguage
    {
        AppInsightEvent,
        AppInsightTrace,
        AppInsightException
    }
}
