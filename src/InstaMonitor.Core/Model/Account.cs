using LiteDB;
using System;
using System.Collections.Generic;
using System.Text;

namespace InstaMonitor.Core.Model
{
    /// <summary>
    /// Persist Model for instragram user
    /// </summary>
    public class Account
    {
        /// <summary>
        /// Primary key of the record
        /// </summary>
        public ObjectId AccountId { get; set; }
        /// <summary>
        /// Login of the user
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// Date of last check
        /// </summary>
        public DateTime LastUpdate { get; set; }
    }
}
