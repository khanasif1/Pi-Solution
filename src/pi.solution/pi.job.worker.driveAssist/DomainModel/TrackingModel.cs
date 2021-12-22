using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pi.job.worker.driveAssist.DomainModel
{
    public class TrackingModel
    {
        public string? Id { get; set; }
        public string? Stamp { get; set; }
        public string? Sensor { get; set; }
        public double? Value { get; set; }
        public string? Unit { get; set; }

    }
}
