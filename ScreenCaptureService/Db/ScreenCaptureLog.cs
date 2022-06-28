using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScreenCaptureService.Db
{
    public class ScreenCaptureLog
    {
        public int Id { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? StoppedAt { get; set; }
        public int? NumberOfScreenshots { get; set; }
    }
}
