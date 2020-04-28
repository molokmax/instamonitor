using LiteDB;
using System;
using System.Collections.Generic;
using System.Text;

namespace InstaMonitor.Core.Model
{
    /// <summary>
    /// Persist model of instagram follower
    /// </summary>
    public class Follower
    {
        /// <summary>
        /// Primary key of the follower
        /// </summary>
        public ObjectId FollowerId { get; set; }
        /// <summary>
        /// Key of follower's user
        /// </summary>
        public ObjectId AccountId { get; set; }
        /// <summary>
        /// Login of the follower
        /// </summary>
        public string UserName { get; set; }
    }
}
