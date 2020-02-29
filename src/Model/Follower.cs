using LiteDB;
using System;
using System.Collections.Generic;
using System.Text;

namespace InstaMonitor.Model
{
    public class Follower
    {
        public ObjectId FollowerId { get; set; }
        public ObjectId AccountId { get; set; }
        public string UserName { get; set; }
    }
}
