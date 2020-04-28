using InstaMonitor.Core.Engine;
using InstaMonitor.Core.Model;
using InstaMonitor.Tests.Mock;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace InstaMonitor.Tests
{
    [TestFixture]
    public class CheckExecutorUnitTest
    {
        private const string TimeFormat = "HH:mm:ss.fff";
        private TestConfigurationBuilder IoC;
        private ICheckExecutor Checker;
        private MockInstagramEngine Engine;
        private IDataRepository Repo;

        [Test]
        public void CreateAccountWithoutFollowersCheckTest()
        {
            DateTime startTime = TruncMilliseconds(DateTime.Now);
            string accountName = "test_user_name1";
            List<MonitorItem> monitorItems = BuildOneMonitorItem(accountName, false);
            Engine.Initialize().Wait();
            Checker.Check(monitorItems).Wait();
            DateTime endTime = TruncMilliseconds(DateTime.Now);


            Account account = Repo.GetAccount(accountName);
            Assert.IsNotNull(account);
            Assert.AreEqual(accountName, account.UserName);
            DateTime updateTime = TruncMilliseconds(account.LastUpdate);
            Assert.IsTrue(updateTime > startTime && updateTime < endTime, $"'{startTime.ToString(TimeFormat)}' <= {updateTime.ToString(TimeFormat)} <='{endTime.ToString(TimeFormat)}'");
            List<Follower> followers = Repo.GetFollowers(account);
            Assert.AreEqual(0, followers.Count);
            List<FollowerReport> followerReports = Repo.GetFollowerReports(account);
            Assert.AreEqual(0, followerReports.Count);
        }

        [Test]
        public void ZeroNewFollowersWhenWereZeroOldFollowersTest()
        {
            string accountName = "test_user_name2";
            List<MonitorItem> monitorItems = BuildOneMonitorItem(accountName, true);
            Engine.Initialize().Wait();
            Checker.Check(monitorItems).Wait();


            Account account = Repo.GetAccount(accountName);
            List<Follower> followers = Repo.GetFollowers(account);
            Assert.AreEqual(0, followers.Count);
            List<FollowerReport> followerReports = Repo.GetFollowerReports(account);
            Assert.AreEqual(0, followerReports.Count);
        }

        [Test]
        public void ZeroNewFollowersWhenWereOneOldFollowersTest()
        {
            string accountName = "test_user_name3";
            List<MonitorItem> monitorItems = BuildOneMonitorItem(accountName, true);

            string oldFollowerName = "old_follower_1";
            Engine.AddFollower(accountName, oldFollowerName);
            Account account = PrepareAccount(accountName);
            PrepareFollowers(account, new List<string>() { oldFollowerName });

            DateTime startTime = TruncMilliseconds(DateTime.Now);
            Engine.Initialize().Wait();
            Checker.Check(monitorItems).Wait();
            DateTime endTime = TruncMilliseconds(DateTime.Now);


            Account accountFromRepo = Repo.GetAccount(accountName);
            DateTime updateTime = TruncMilliseconds(accountFromRepo.LastUpdate);
            Assert.IsTrue(updateTime >= startTime && updateTime <= endTime, $"'{startTime.ToString(TimeFormat)}' <= {updateTime.ToString(TimeFormat)} <='{endTime.ToString(TimeFormat)}'");
            List<Follower> followers = Repo.GetFollowers(accountFromRepo);
            Assert.AreEqual(1, followers.Count);
            Follower oldFollower = followers.First();
            Assert.AreEqual(oldFollowerName, oldFollower.UserName);
            List<FollowerReport> followerReports = Repo.GetFollowerReports(accountFromRepo);
            Assert.AreEqual(0, followerReports.Count);
        }

        [Test]
        public void OneNewFollowersWhenWereZeroOldFollowersTest()
        {
            string accountName = "test_user_name4";
            List<MonitorItem> monitorItems = BuildOneMonitorItem(accountName, true);

            Engine.Initialize().Wait();
            Checker.Check(monitorItems).Wait();

            string newFollowerName = "new_follower_1";
            Engine.AddFollower(accountName, newFollowerName);

            DateTime startTime = TruncMilliseconds(DateTime.Now);
            Checker.Check(monitorItems).Wait();
            DateTime endTime = TruncMilliseconds(DateTime.Now);

            Account accountFromRepo = Repo.GetAccount(accountName);
            List<Follower> followers = Repo.GetFollowers(accountFromRepo);
            Assert.AreEqual(1, followers.Count);
            Follower newFollower = followers.First();
            Assert.AreEqual(newFollowerName, newFollower.UserName);
            List<FollowerReport> followerReports = Repo.GetFollowerReports(accountFromRepo);
            Assert.AreEqual(1, followerReports.Count);
            FollowerReport followerReport = followerReports.First();
            DateTime changeTime = TruncMilliseconds(followerReport.ChangeDate);
            Assert.IsTrue(changeTime >= startTime && changeTime <= endTime, $"'{startTime.ToString(TimeFormat)}' <= {changeTime.ToString(TimeFormat)} <='{endTime.ToString(TimeFormat)}'");
            Assert.AreEqual(1, followerReport.AddedFollowers.Count);
            Assert.AreEqual(newFollowerName, followerReport.AddedFollowers.First());
            Assert.AreEqual(0, followerReport.RemovedFollowers.Count);
        }

        [Test]
        public void OneNewFollowersWhileInitializingTest()
        {
            string accountName = "test_user_name5";
            List<MonitorItem> monitorItems = BuildOneMonitorItem(accountName, true);

            Engine.Initialize().Wait();
            Checker.Check(monitorItems).Wait();

            string newFollowerName = "new_follower_1";
            Engine.AddFollower(accountName, newFollowerName);

            DateTime startTime = TruncMilliseconds(DateTime.Now);
            Checker.Check(monitorItems).Wait();
            DateTime endTime = TruncMilliseconds(DateTime.Now);

            Account accountFromRepo = Repo.GetAccount(accountName);
            List<Follower> followers = Repo.GetFollowers(accountFromRepo);
            Assert.AreEqual(1, followers.Count);
            Follower newFollower = followers.First();
            Assert.AreEqual(newFollowerName, newFollower.UserName);
            List<FollowerReport> followerReports = Repo.GetFollowerReports(accountFromRepo);
            Assert.AreEqual(1, followerReports.Count);
            FollowerReport followerReport = followerReports.First();
            DateTime changeTime = TruncMilliseconds(followerReport.ChangeDate);
            Assert.IsTrue(changeTime >= startTime && changeTime <= endTime, $"'{startTime.ToString(TimeFormat)}' <= {changeTime.ToString(TimeFormat)} <='{endTime.ToString(TimeFormat)}'");
            Assert.AreEqual(1, followerReport.AddedFollowers.Count);
            Assert.AreEqual(newFollowerName, followerReport.AddedFollowers.First());
            Assert.AreEqual(0, followerReport.RemovedFollowers.Count);
        }

        [Test]
        public void OneNewFollowersWhenWereOneOldFollowersTest()
        {
            string accountName = "test_user_name6";
            List<MonitorItem> monitorItems = BuildOneMonitorItem(accountName, true);

            string oldFollowerName = "old_follower_1";
            Engine.AddFollower(accountName, oldFollowerName);
            Account account = PrepareAccount(accountName);
            PrepareFollowers(account, new List<string>() { oldFollowerName });

            Engine.Initialize().Wait();
            Checker.Check(monitorItems).Wait();

            string newFollowerName = "new_follower_1";
            Engine.AddFollower(accountName, newFollowerName);

            DateTime startTime = TruncMilliseconds(DateTime.Now);
            Checker.Check(monitorItems).Wait();
            DateTime endTime = TruncMilliseconds(DateTime.Now);

            Account accountFromRepo = Repo.GetAccount(accountName);
            List<Follower> followers = Repo.GetFollowers(accountFromRepo);
            Assert.AreEqual(2, followers.Count);
            Follower oldFollower = followers.FirstOrDefault(x => x.UserName == oldFollowerName);
            Assert.IsNotNull(oldFollower);
            Follower newFollower = followers.FirstOrDefault(x => x.UserName == newFollowerName);
            Assert.IsNotNull(newFollower);
            List<FollowerReport> followerReports = Repo.GetFollowerReports(accountFromRepo);
            Assert.AreEqual(1, followerReports.Count);
            FollowerReport followerReport = followerReports.First();
            DateTime changeTime = TruncMilliseconds(followerReport.ChangeDate);
            Assert.IsTrue(changeTime >= startTime && changeTime <= endTime, $"'{startTime.ToString("HH:mm:ss.fff")}' <= {changeTime.ToString("HH:mm:ss.fff")} <='{endTime.ToString("HH:mm:ss.fff")}'");
            Assert.AreEqual(1, followerReport.AddedFollowers.Count);
            Assert.AreEqual(newFollowerName, followerReport.AddedFollowers.First());
            Assert.AreEqual(0, followerReport.RemovedFollowers.Count);
        }

        [Test]
        public void OneUnsubscribeWhenWereOneOldFollowersTest()
        {
            string accountName = "test_user_name7";
            List<MonitorItem> monitorItems = BuildOneMonitorItem(accountName, true);

            string oldFollowerName = "old_follower_1";
            // Engine.AddFollower(accountName, oldFollowerName);
            Account account = PrepareAccount(accountName);
            PrepareFollowers(account, new List<string>() { oldFollowerName });

            DateTime startTime = TruncMilliseconds(DateTime.Now);
            Engine.Initialize().Wait();
            Checker.Check(monitorItems).Wait();
            DateTime endTime = TruncMilliseconds(DateTime.Now);

            Account accountFromRepo = Repo.GetAccount(accountName);
            List<Follower> followers = Repo.GetFollowers(accountFromRepo);
            Assert.AreEqual(0, followers.Count);
            List<FollowerReport> followerReports = Repo.GetFollowerReports(accountFromRepo);
            Assert.AreEqual(1, followerReports.Count);
            FollowerReport followerReport = followerReports.First();
            DateTime changeTime = TruncMilliseconds(followerReport.ChangeDate);
            Assert.IsTrue(changeTime >= startTime && changeTime <= endTime, $"'{startTime.ToString(TimeFormat)}' <= {changeTime.ToString(TimeFormat)} <='{endTime.ToString(TimeFormat)}'");
            Assert.AreEqual(0, followerReport.AddedFollowers.Count);
            Assert.AreEqual(1, followerReport.RemovedFollowers.Count);
            Assert.AreEqual(oldFollowerName, followerReport.RemovedFollowers.First());
        }


        [OneTimeSetUp]
        public void Init()
        {
            IoC = new TestConfigurationBuilder();
        }

        [OneTimeTearDown]
        public void Dispose()
        {
            string dataDir = this.GetType().Name;
            string binDir = TestContext.CurrentContext.TestDirectory;
            string dbDirName = $"{binDir}/{dataDir}";
            if (Directory.Exists(dbDirName))
            {
                Directory.Delete(dbDirName, true);
            }
        }

        [SetUp]
        public void SetUp()
        {
            Engine = new MockInstagramEngine(1000, 500);
            string dbFileName = GetDbFileName();
            Repo = new DataRepository(IoC.Configuration, dbFileName);
            Checker = new CheckExecutor(Engine, Repo);
        }

        [TearDown]
        public void Cleanup()
        {
            Repo?.Dispose();
            string dbFileName = GetDbFileName();
            if (File.Exists(dbFileName))
            {
                File.Delete(dbFileName);
            }
        }


        /// <summary>
        /// for each test we use new database
        /// </summary>
        /// <returns></returns>
        private string GetDbFileName()
        {
            string dataDir = this.GetType().Name;
            string binDir = TestContext.CurrentContext.TestDirectory;
            string testDir = TestContext.CurrentContext.Test.Name;
            string dbFileName = $"{binDir}/{dataDir}/{testDir}/test_database.db";
            return dbFileName;
        }

        private List<MonitorItem> BuildOneMonitorItem(string userName, bool checkFollowers)
        {
            List<MonitorItem> result = new List<MonitorItem>();
            result.Add(new MonitorItem()
            {
                UserName = userName,
                Followers = checkFollowers
            });
            return result;
        }

        private Account PrepareAccount(string userName)
        {
            Account account = new Account()
            {
                UserName = userName,
                LastUpdate = DateTime.Now
            };
            Repo.SaveAccount(account);
            return account;
        }


        private void PrepareFollowers(Account account, List<string> followers)
        {
            List<Follower> followerList = followers.Select(x => new Follower() {
                AccountId = account.AccountId,
                UserName = x
            }).ToList();
            Repo.AddFollowers(followerList);
        }

        private DateTime TruncMilliseconds(DateTime src)
        {
            long ticks = src.Ticks / 10000 * 10000;
            DateTime result = new DateTime(ticks);
            return result;
        }
    }
}
