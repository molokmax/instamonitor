using LiteDB;
using System;
using System.Collections.Generic;
using System.Text;

namespace InstaMonitor.Model
{
    public class Account
    {
        public ObjectId AccountId { get; set; }
        public string UserName { get; set; }
        public DateTime LastUpdate { get; set; }
    }
}
