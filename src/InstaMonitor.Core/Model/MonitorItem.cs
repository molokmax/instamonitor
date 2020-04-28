using System;
using System.Collections.Generic;
using System.Text;

namespace InstaMonitor.Core.Model
{
    /// <summary>
    /// Config model of object for monitoring
    /// </summary>
    public class MonitorItem
    {
        /// <summary>
        /// Instagram login
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// Need check followers
        /// </summary>
        public bool Followers { get; set; }
    }
}
