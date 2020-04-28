using LiteDB;
using System;
using System.Collections.Generic;
using System.Text;

namespace InstaMonitor.Core.Model
{
    /// <summary>
    /// Persist model of report about followers
    /// </summary>
    public class FollowerReport
    {
        /// <summary>
        /// Primary key of the record
        /// </summary>
        public ObjectId FollowerReportId { get; set; }
        /// <summary>
        /// Reference to instragram user
        /// </summary>
        public ObjectId AccountId { get; set; }
        /// <summary>
        /// Date of check and save the record
        /// </summary>
        public DateTime ChangeDate { get; set; }
        /// <summary>
        /// Array of new followers
        /// </summary>
        public List<string> AddedFollowers { get; set; }
        /// <summary>
        /// Array of unsubscribed followers
        /// </summary>
        public List<string> RemovedFollowers { get; set; }
    }
}
