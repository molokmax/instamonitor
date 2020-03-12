using InstaMonitor.Engine;
using InstaMonitor.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstaMonitor
{
    class Program
    {
        private static ILogger commonLogger = LogManager.GetLogger(Consts.LogTypes.Common);
        private static ILogger reportLogger = LogManager.GetLogger(Consts.LogTypes.Report);

        static async Task Main(string[] args)
        {
            try
            {
                commonLogger.Info("InstaMonitor started");
 
                InstagramMonitorConfigurationBuilder ioc = new InstagramMonitorConfigurationBuilder();
                using (IServiceScope scope = ioc.CreateServiceScope())
                {
                    // Parse configuration. Get list of objects for monitoring
                    List<MonitorItem> monitorItems = ioc.Configuration.GetSection("MonitorItems").Get<List<MonitorItem>>();

                    if (monitorItems != null && monitorItems.Any())
                    {
                        InstragramEngine engine = scope.ServiceProvider.GetService<InstragramEngine>();
                        await engine.Initialize();

                        DataRepository repo = scope.ServiceProvider.GetService<DataRepository>();

                        foreach (MonitorItem item in monitorItems)
                        {
                            // Get record of instagram account. Create new if record is not exist
                            Account account = repo.GetAccount(item.UserName);
                            if (account == null)
                            {
                                account = new Account()
                                {
                                    UserName = item.UserName
                                };
                            }
                            account.LastUpdate = DateTime.Now;
                            repo.SaveAccount(account);

                            if (item.Followers)
                            {
                                // Check followers. We should get actual list of followers from instagram. And previous list of followers from app database. Compare folowers list and save report record with new followers and unsabscribed followers
                                List<string> followerList = await engine.GetFollowers(item.UserName);
                                ISet<string> followers = new HashSet<string>(followerList);

                                List<Follower> oldFollowers = repo.GetFollowers(account);

                                ISet<string> oldFollowerNames = new HashSet<string>(oldFollowers.Select(x => x.UserName));
                                List<Follower> actualFollowers = followers.Select(x => new Follower() { UserName = x, AccountId = account.AccountId }).ToList();

                                List<Follower> followersToAdd = actualFollowers.Where(x => !oldFollowerNames.Contains(x.UserName)).ToList();
                                List<Follower> followersToDelete = oldFollowers.Where(x => !followers.Contains(x.UserName)).ToList();

                                repo.DeleteFollowers(followersToDelete);
                                repo.AddFollowers(followersToAdd);

                                if (followersToAdd.Any() || followersToDelete.Any())
                                {
                                    FollowerReport report = new FollowerReport()
                                    {
                                        AccountId = account.AccountId,
                                        ChangeDate = account.LastUpdate,
                                        AddedFollowers = followersToAdd.Select(x => x.UserName).ToList(),
                                        RemovedFollowers = followersToDelete.Select(x => x.UserName).ToList()
                                    };
                                    repo.AddFollowerReport(report);
                                }

                                PrintFollowerReports(item, account, repo);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                commonLogger.Error(e);
            }
            finally
            {
                commonLogger.Info("InstaMonitor finished");
            }
        }

        private static void PrintFollowerReports(MonitorItem item, Account account, DataRepository repo)
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
