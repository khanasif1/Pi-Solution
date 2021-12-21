using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pi.job.worker.driveAssist
{
    public class AppInsightPayload
    {
        public string? _operation { get; set; }
        public AppInsightLanguage _type { get; set; }
        public string? _correlationId { get; set; }
        public DateTime _correlationTimeId { get; set; }
        public string? _payload { get; set; }
        public Exception? _ex { get; set; }
    }
}
