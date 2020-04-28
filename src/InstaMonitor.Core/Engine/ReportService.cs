using InstaMonitor.Core.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace InstaMonitor.Core.Engine
{
    /// <summary>
    /// Service for working with report data
    /// </summary>
    public class ReportService : IReportService
    {
        private readonly IDataRepository Repo;

        public ReportService(IDataRepository repo)
        {
            Repo = repo;
        }

        /// <summary>
        /// Get list of report records for followers
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public Task<List<FollowerReport>> GetFollowerReports(string userName)
        {
            Account account = Repo.GetAccount(userName);
            if (account == null)
            {
                return Task.FromResult(new List<FollowerReport>());
            } 
            else
            {
                List<FollowerReport> result = Repo.GetFollowerReports(account);
                return Task.FromResult(result);
            }
        }
    }
}
