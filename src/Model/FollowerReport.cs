using LiteDB;
using System;
using System.Collections.Generic;
using System.Text;

namespace InstaMonitor.Model
{
    public class FollowerReport
    {
        public ObjectId FollowerReportId { get; set; }
        public ObjectId AccountId { get; set; }
        public DateTime ChangeDate { get; set; }
        public List<string> AddedFollowers { get; set; }
        public List<string> RemovedFollowers { get; set; }
    }
}
