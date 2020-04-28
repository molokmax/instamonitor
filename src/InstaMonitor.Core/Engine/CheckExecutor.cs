using InstaMonitor.Core.Model;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstaMonitor.Core.Engine
{
    /// <summary>
    /// Checks runner
    /// </summary>
    public class CheckExecutor : ICheckExecutor
    {
        private readonly IInstagramEngine Engine;
        private readonly IDataRepository Repo;

        private static ILogger reportLogger = LogManager.GetLogger(Consts.LogTypes.Report);

        public CheckExecutor(IInstagramEngine engine, IDataRepository repo)
        {
            Engine = engine;
            Repo = repo;
        }

        /// <summary>
        /// Run checks
        /// </summary>
        /// <param name="monitorItems"></param>
        /// <returns></returns>
        public async Task Check(List<MonitorItem> monitorItems)
        {

            foreach (MonitorItem item in monitorItems)
            {
                // Get record of instagram account. Create new if record is not exist
                Account account = Repo.GetAccount(item.UserName);
                if (account == null)
                {
                    account = new Account()
                    {
                        UserName = item.UserName
                    };
                }
                account.LastUpdate = DateTime.Now;
                Repo.SaveAccount(account);

                if (item.Followers)
                {
                    // Check followers. We should get actual list of followers from instagram. And previous list of followers from app database. Compare folowers list and save report record with new followers and unsabscribed followers
                    List<string> followerList = await Engine.GetFollowers(item.UserName);
                    ISet<string> followers = new HashSet<string>(followerList);

                    List<Follower> oldFollowers = Repo.GetFollowers(account);

                    ISet<string> oldFollowerNames = new HashSet<string>(oldFollowers.Select(x => x.UserName));
                    List<Follower> actualFollowers = followers.Select(x => new Follower() { UserName = x, AccountId = account.AccountId }).ToList();

                    List<Follower> followersToAdd = actualFollowers.Where(x => !oldFollowerNames.Contains(x.UserName)).ToList();
                    List<Follower> followersToDelete = oldFollowers.Where(x => !followers.Contains(x.UserName)).ToList();

                    Repo.DeleteFollowers(followersToDelete);
                    Repo.AddFollowers(followersToAdd);

                    if (followersToAdd.Any() || followersToDelete.Any())
                    {
                        FollowerReport report = new FollowerReport()
                        {
                            AccountId = account.AccountId,
                            ChangeDate = account.LastUpdate,
                            AddedFollowers = followersToAdd.Select(x => x.UserName).ToList(),
                            RemovedFollowers = followersToDelete.Select(x => x.UserName).ToList()
                        };
                        Repo.AddFollowerReport(report);
                    }

                    // TODO: remove
                    PrintFollowerReports(item, account, Repo);
                }
            }
        }


        private static void PrintFollowerReports(MonitorItem item, Account account, IDataRepository repo)
        {
            // Get reports for the account and print it
            List<FollowerReport> reports = repo.GetFollowerReports(account);

            StringBuilder reportText = new StringBuilder();
            string header = $"{item.UserName}'s reports:";
            reportText.AppendLine(header);
            reportText.AppendLine(new String('-', header.Length));
            reportText.AppendLine();
            foreach (FollowerReport report in reports)
            {
                reportText.AppendLine(String.Format("Change date: {0:yyyy-MM-dd}", report.ChangeDate));
                reportText.AppendLine();

                reportText.AppendLine("Subscribed followers:");
                reportText.AppendLine("---------------------");
                foreach (string i in report.AddedFollowers)
                {
                    reportText.AppendLine(i);
                }
                reportText.AppendLine();

                reportText.AppendLine("UnSubscribed followers:");
                reportText.AppendLine("-----------------------");
                foreach (string i in report.RemovedFollowers)
                {
                    reportText.AppendLine(i);
                }
                reportText.AppendLine();
            }
            reportText.AppendLine("=========");
            reportText.AppendLine();

            reportLogger.Info(reportText.ToString());
        }
    }
}
