using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GardifyModels.Models
{
    public class MaintenanceSetting
    {
        
        public int Id { get; set; }
        public string MaintenanceNote { get; set; }
        public DateTime MaintenanceNoticeStart { get; set; }
        public DateTime MaintenanceStart { get; set; }
        public bool IsRunning { get; set; }
    }

    public class MaintenanceView
    {
        public bool IsNoticeExist { get; set; }
        public string Message { get; set; }
    }
}