using InstaMonitor.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstaMonitor.Tests.Mock
{
    /// <summary>
    /// Mack implementation for Instagram engine
    /// </summary>
    public class MockInstagramEngine : IInstagramEngine
    {
        private IDictionary<string, HashSet<string>> followers = new Dictionary<string, HashSet<string>>();
        private int InitDelay;
        private int RequestDelay;

        /// <summary>
        /// Create instance of mock api wrapper
        /// </summary>
        /// <param name="initDelay">Make pause when initializing</param>
        /// <param name="requestDelay">Make pause when emulating request</param>
        public MockInstagramEngine(int initDelay = 4000, int requestDelay = 2000)
        {
            InitDelay = initDelay;
            RequestDelay = requestDelay;
        }

        /// <summary>
        /// Initialize api instagram
        /// Realy do nothing
        /// </summary>
        /// <returns></returns>
        public async Task Initialize()
        {
            await Task.Delay(InitDelay);
        }

        public async Task<List<string>> GetFollowers(string userName)
        {
            HashSet<string> list;
            if (!followers.TryGetValue(userName, out list))
            {
                list = new HashSet<string>();
                followers.Add(userName, list);
            }
            await Task.Delay(RequestDelay);
            return list.ToList();
        }


        /// <summary>
        /// Emulate 'follower' has subscribed to account 'userName'
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="follower"></param>
        public void AddFollower(string userName, string follower)
        {
            HashSet<string> list;
            if (!followers.TryGetValue(userName, out list))
            {
                list = new HashSet<string>();
                followers.Add(userName, list);
            }
            list.Add(follower);
        }
        /// <summary>
        /// Emulate 'follower' has unsubscribed from account 'userName'
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="follower"></param>
        public void DeleteFollower(string userName, string follower)
        {
            HashSet<string> list;
            if (!followers.TryGetValue(userName, out list))
            {
                list = new HashSet<string>();
                followers.Add(userName, list);
            }
            list.Remove(follower);
        }
    }
}
