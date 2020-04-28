using InstaMonitor.Core.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace InstaMonitor.Core.Engine
{
    /// <summary>
    /// Interface for working with report data
    /// </summary>
    public interface IReportService
    {
        /// <summary>
        /// Get list of report records for followers
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        Task<List<FollowerReport>> GetFollowerReports(string userName);
    }
}
