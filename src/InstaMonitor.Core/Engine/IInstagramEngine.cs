using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace InstaMonitor.Core.Engine
{
    /// <summary>
    /// Interface of instagram-api wrapper
    /// </summary>
    public interface IInstagramEngine
    {
        /// <summary>
        /// Initialize api instagram
        /// Get user and pass from config and make sing in
        /// </summary>
        /// <returns></returns>
        Task Initialize();

        /// <summary>
        /// Get list of followers of passed user
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        Task<List<string>> GetFollowers(string userName);

    }
}
