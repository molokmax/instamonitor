using InstaMonitor.Core.Engine;
using InstaMonitor.Core.Model;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace InstaMonitor.Tests
{
    /// <summary>
    /// Tests for persist data repository
    /// </summary>
    [TestFixture]
    public class ReportServiceUnitTest
    {

        private TestConfigurationBuilder IoC;
        private IDataRepository Repo;
        private IReportService ReportService;

        [Test]
        public async Task GetFollowerReports()
        {
            string testUserName = "TestUserName1";
            DateTime testChangeDate = new DateTime(2020, 4, 5, 11, 15, 22, 122);

            List<FollowerReport> reports = await ReportService.GetFollowerReports(testUserName);

            Account account = Repo.GetAccount(testUserName);
            Assert.AreEqual(3, reports.Count);
            Assert.IsTrue(reports.All(x => x.AccountId == account.AccountId));
            Assert.IsTrue(reports.All(x => x.AddedFollowers.Count == 2));
            Assert.IsTrue(reports.All(x => x.AddedFollowers.All(f => f.StartsWith("follower"))));
            Assert.IsTrue(reports.All(x => x.RemovedFollowers.Count == 2));
            Assert.IsTrue(reports.All(x => x.RemovedFollowers.All(f => f.StartsWith("follower"))));
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
            string dbFileName = GetDbFileName();
            Repo = new DataRepository(IoC.Configuration, dbFileName);
            ReportService = new ReportService(Repo);
            InitTestData(Repo);
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


        /// <summary>
        /// Add test records to database
        /// </summary>
        /// <param name="repo"></param>
        private void InitTestData(IDataRepository repo)
        {
            Account account1 = new Account()
            {
                UserName = "TestUserName1",
                LastUpdate = new DateTime(2020, 4, 4, 10, 14, 21, 121)
            };
            repo.SaveAccount(account1);

            FollowerReport report1 = new FollowerReport()
            {
                AccountId = account1.AccountId,
                ChangeDate = new DateTime(2020, 3, 6, 3, 54, 34, 321),
                AddedFollowers = new List<string>() { "follower9", "follower10" },
                RemovedFollowers = new List<string>() { "follower11", "follower12" }
            };
            FollowerReport report2 = new FollowerReport()
            {
                AccountId = account1.AccountId,
                ChangeDate = new DateTime(2020, 3, 5, 3, 54, 34, 321),
                AddedFollowers = new List<string>() { "follower1", "follower2" },
                RemovedFollowers = new List<string>() { "follower3", "follower4" }
            };
            FollowerReport report3 = new FollowerReport()
            {
                AccountId = account1.AccountId,
                ChangeDate = new DateTime(2020, 3, 6, 5, 53, 24, 221),
                AddedFollowers = new List<string>() { "follower5", "follower6" },
                RemovedFollowers = new List<string>() { "follower7", "follower8" }
            };
            repo.AddFollowerReport(report1);
            repo.AddFollowerReport(report2);
            repo.AddFollowerReport(report3);

            Account account2 = new Account()
            {
                UserName = "TestUserName2",
                LastUpdate = new DateTime(2020, 4, 4, 10, 14, 21, 121)
            };
            repo.SaveAccount(account2);
            FollowerReport report4 = new FollowerReport()
            {
                AccountId = account2.AccountId,
                ChangeDate = new DateTime(2020, 3, 6, 5, 53, 24, 221),
                AddedFollowers = new List<string>() { "follower5", "follower6" },
                RemovedFollowers = new List<string>() { "follower7", "follower8" }
            };
            repo.AddFollowerReport(report4);

        }
    }
}