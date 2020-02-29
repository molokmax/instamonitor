using InstaMonitor.Engine;
using InstaMonitor.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InstaMonitor
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                Console.WriteLine("Begin");

                InstagramMonitorConfigurationBuilder ioc = new InstagramMonitorConfigurationBuilder();
                List<MonitorItem> monitorItems = ioc.Configuration.GetSection("MonitorItems").Get<List<MonitorItem>>();

                if (monitorItems != null && monitorItems.Any())
                {
                    InstragramEngine engine = ioc.ServiceProvider.GetService<InstragramEngine>();
                    await engine.Initialize();

                    DataRepository repo = ioc.ServiceProvider.GetService<DataRepository>();

                    foreach (MonitorItem item in monitorItems)
                    {
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

                            List<FollowerReport> reports = repo.GetFollowerReports(account);

                            string header = $"{item.UserName}'s reports:";
                            Console.WriteLine(header);
                            Console.WriteLine(new String('-', header.Length));
                            Console.WriteLine();
                            foreach (FollowerReport report in reports)
                            {
                                Console.WriteLine("Change date: {0:yyyy-MM-dd}", report.ChangeDate);
                                Console.WriteLine();

                                Console.WriteLine("Subscribed followers:");
                                Console.WriteLine("---------------------");
                                foreach (string i in report.AddedFollowers)
                                {
                                    Console.WriteLine(i);
                                }
                                Console.WriteLine();

                                Console.WriteLine("UnSubscribed followers:");
                                Console.WriteLine("-----------------------");
                                foreach (string i in report.RemovedFollowers)
                                {
                                    Console.WriteLine(i);
                                }
                                Console.WriteLine();
                            }
                            Console.WriteLine("=========");
                            Console.WriteLine();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error! {0}", e.ToString());
            }
            finally
            {
                Console.WriteLine("Finish");
            }
        }
    }
}
