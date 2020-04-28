using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InstaMonitor.Api.Model
{
    /// <summary>
    /// Data transfer object for follower report
    /// </summary>
    public class FollowerReportDTO
    {
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
