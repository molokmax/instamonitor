using InstaMonitor.Core.Engine;
using InstaMonitor.Core.Model;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace InstaMonitor.Tests
{
    /// <summary>
    /// Tests for persist data repository
    /// </summary>
    [TestFixture]
    public class RepositoryUnitTest
    {

        private TestConfigurationBuilder IoC;
        private IDataRepository Repo;

        [Test]
        public void GetAccount()
        {
            string testUserName = "TestUserName2";
            DateTime testChangeDate = new DateTime(2020, 4, 5, 11, 15, 22, 122);
            Account rec = Repo.GetAccount("TestUserName2");
            Assert.IsNotNull(rec);
            Assert.IsNotNull(rec.AccountId);
            Assert.AreNotEqual(0, rec.AccountId);
            Assert.AreEqual(testUserName, rec.UserName);
            Assert.AreEqual(testChangeDate, rec.LastUpdate);
        }

        [Test]
        public void AddAccount()
        {
            string username = "TestUserName_2";
            DateTime changeDate = new DateTime(2020, 4, 6, 11, 15, 23, 123);
            Account account = new Account()
            {
                UserName = username,
                LastUpdate = changeDate
            };
            Repo.SaveAccount(account);
            Account rec = Repo.GetAccount(username);
            Assert.IsNotNull(rec);
            Assert.IsNotNull(rec.AccountId);
            Assert.AreNotEqual(0, rec.AccountId);
            Assert.AreEqual(username, rec.UserName);
            Assert.AreEqual(changeDate, rec.LastUpdate);
        }

        [Test]
        public void UpdateAccountName()
        {
            string username = "TestUserName1";
            string username2 = "TestUserName1_Rename";
            DateTime changeDate = new DateTime(2020, 4, 4, 10, 14, 21, 121);
            Account account = Repo.GetAccount(username);
            account.UserName = username2;
            Repo.SaveAccount(account);
            Account rec1 = Repo.GetAccount(username);
            Assert.IsNull(rec1);
            Account rec2 = Repo.GetAccount(username2);
            Assert.IsNotNull(rec2);
            Assert.IsNotNull(rec2.AccountId);
            Assert.AreNotEqual(0, rec2.AccountId);
            Assert.AreEqual(username2, rec2.UserName);
            Assert.AreEqual(changeDate, rec2.LastUpdate);
        }

        [Test]
        public void UpdateAccountChangeDate()
        {
            string username = "TestUserName1";
            DateTime changeDate = new DateTime(2020, 4, 5, 11, 16, 19, 212);
            Account account = Repo.GetAccount(username);
            account.LastUpdate = changeDate;
            Repo.SaveAccount(account);
            Account rec1 = Repo.GetAccount(username);
            Assert.IsNotNull(rec1);
            Assert.IsNotNull(rec1.AccountId);
            Assert.AreNotEqual(0, rec1.AccountId);
            Assert.AreEqual(username, rec1.UserName);
            Assert.AreEqual(changeDate, rec1.LastUpdate);
        }

        [Test]
        public void DeleteAccount()
        {
            string username = "TestUserName3";
            Account account = Repo.GetAccount(username);
            Repo.DeleteAccount(account);
            Account rec = Repo.GetAccount(username);
            Assert.IsNull(rec);
        }

        [Test]
        public void GetFollowers()
        {
            string username = "TestUserName3";
            Account account = Repo.GetAccount(username);
            List<Follower> followers = Repo.GetFollowers(account).OrderBy(x => x.UserName).ToList();
            Assert.AreEqual(2, followers.Count);
            Follower follower1 = followers[0];
            Assert.IsNotNull(follower1);
            Assert.AreEqual(account.AccountId, follower1.AccountId);
            Assert.IsNotNull(follower1.FollowerId);
            Assert.AreNotEqual(0, follower1.FollowerId);
            Assert.AreEqual("TestFollowerName3_1", follower1.UserName);
            Follower follower2 = followers[1];
            Assert.IsNotNull(follower2);
            Assert.AreEqual(account.AccountId, follower2.AccountId);
            Assert.IsNotNull(follower2.FollowerId);
            Assert.AreNotEqual(0, follower2.FollowerId);
            Assert.AreEqual("TestFollowerName3_2", follower2.UserName);
        }

        [Test]
        public void AddFollowers()
        {
            string username = "TestUserName1";
            Account account = Repo.GetAccount(username);

            List<Follower> followersToAdd = new List<Follower>()
            {
                new Follower()
                {
                    AccountId = account.AccountId,
                    UserName = "TestFollowerName1_2"
                }
            };
            Repo.AddFollowers(followersToAdd);

            List<Follower> followers = Repo.GetFollowers(account).OrderBy(x => x.UserName).ToList();
            Assert.AreEqual(2, followers.Count);
            Follower follower1 = followers[0];
            Assert.IsNotNull(follower1);
            Assert.AreEqual(account.AccountId, follower1.AccountId);
            Assert.IsNotNull(follower1.FollowerId);
            Assert.AreNotEqual(0, follower1.FollowerId);
            Assert.AreEqual("TestFollowerName1_1", follower1.UserName);
            Follower follower2 = followers[1];
            Assert.IsNotNull(follower2);
            Assert.AreEqual(account.AccountId, follower2.AccountId);
            Assert.IsNotNull(follower2.FollowerId);
            Assert.AreNotEqual(0, follower2.FollowerId);
            Assert.AreEqual("TestFollowerName1_2", follower2.UserName);
        }

        [Test]
        public void DeleteFollowers()
        {
            string username = "TestUserName3";
            Account account = Repo.GetAccount(username);
            List<Follower> followerToDelete = Repo.GetFollowers(account)
                .Where(x => x.UserName == "TestFollowerName3_1").ToList();
            Repo.DeleteFollowers(followerToDelete);
            List<Follower> followers = Repo.GetFollowers(account).OrderBy(x => x.UserName).ToList();
            Assert.AreEqual(1, followers.Count);
            Follower follower2 = followers[0];
            Assert.IsNotNull(follower2);
            Assert.AreEqual(account.AccountId, follower2.AccountId);
            Assert.IsNotNull(follower2.FollowerId);
            Assert.AreNotEqual(0, follower2.FollowerId);
            Assert.AreEqual("TestFollowerName3_2", follower2.UserName);
        }


        [Test]
        public void DeleteAccountFollowers()
        {
            string username = "TestUserName3";
            Account account = Repo.GetAccount(username);
            Repo.DeleteFollowers(account);
            List<Follower> followers = Repo.GetFollowers(account);
            Assert.AreEqual(0, followers.Count);
        }

        [Test]
        public void GetFollowerReports()
        {
            string username = "TestUserName2";
            Account account = Repo.GetAccount(username);
            List<FollowerReport> reports = Repo.GetFollowerReports(account)
                .OrderBy(x => x.ChangeDate).ToList();

            Assert.AreEqual(2, reports.Count);
            FollowerReport rec1 = reports[0];
            Assert.IsNotNull(rec1);
            Assert.AreEqual(account.AccountId, rec1.AccountId);
            Assert.IsNotNull(rec1.FollowerReportId);
            Assert.AreNotEqual(0, rec1.FollowerReportId);
            Assert.AreEqual("follower1,follower2", String.Join(",", rec1.AddedFollowers));
            Assert.AreEqual("follower3,follower4", String.Join(",", rec1.RemovedFollowers));
            Assert.AreEqual(new DateTime(2020, 3, 5, 3, 54, 34, 321), rec1.ChangeDate);
            FollowerReport rec2 = reports[1];
            Assert.IsNotNull(rec2);
            Assert.AreEqual(account.AccountId, rec2.AccountId);
            Assert.IsNotNull(rec2.FollowerReportId);
            Assert.AreNotEqual(0, rec2.FollowerReportId);
            Assert.AreEqual("follower5,follower6", String.Join(",", rec2.AddedFollowers));
            Assert.AreEqual("follower7,follower8", String.Join(",", rec2.RemovedFollowers));
            Assert.AreEqual(new DateTime(2020, 3, 6, 5, 53, 24, 221), rec2.ChangeDate);
        }

        [Test]
        public void AddFollowerReport()
        {
            string username = "TestUserName1";
            Account account = Repo.GetAccount(username);

            FollowerReport report = new FollowerReport()
            {
                AccountId = account.AccountId,
                ChangeDate = new DateTime(2020, 4, 7, 5, 53, 24, 221),
                AddedFollowers = new List<string>() { "follower13", "follower14" },
                RemovedFollowers = new List<string>() { "follower15", "follower16" }
            };
            Repo.AddFollowerReport(report);

            List<FollowerReport> reports = Repo.GetFollowerReports(account)
                .OrderBy(x => x.ChangeDate).ToList();

            Assert.AreEqual(2, reports.Count);
            FollowerReport rec2 = reports[1];
            Assert.IsNotNull(rec2);
            Assert.AreEqual(account.AccountId, rec2.AccountId);
            Assert.IsNotNull(rec2.FollowerReportId);
            Assert.AreNotEqual(0, rec2.FollowerReportId);
            Assert.AreEqual("follower13,follower14", String.Join(",", rec2.AddedFollowers));
            Assert.AreEqual("follower15,follower16", String.Join(",", rec2.RemovedFollowers));
            Assert.AreEqual(new DateTime(2020, 4, 7, 5, 53, 24, 221), rec2.ChangeDate);
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

            Account account2 = new Account()
            {
                UserName = "TestUserName2",
                LastUpdate = new DateTime(2020, 4, 5, 11, 15, 22, 122)
            };
            repo.SaveAccount(account2);

            Account account3 = new Account()
            {
                UserName = "TestUserName3",
                LastUpdate = new DateTime(2020, 4, 6, 12, 16, 23, 123)
            };
            repo.SaveAccount(account3);

            List<Follower> followers = new List<Follower>()
            {
                new Follower()
                {
                    AccountId = account1.AccountId,
                    UserName = "TestFollowerName1_1"
                },
                new Follower()
                {
                    AccountId = account3.AccountId,
                    UserName = "TestFollowerName3_1"
                },
                new Follower()
                {
                    AccountId = account3.AccountId,
                    UserName = "TestFollowerName3_2"
                },
            };
            repo.AddFollowers(followers);

            FollowerReport report1 = new FollowerReport()
            {
                AccountId = account1.AccountId,
                ChangeDate = new DateTime(2020, 3, 6, 3, 54, 34, 321),
                AddedFollowers = new List<string>() { "follower9", "follower10" },
                RemovedFollowers = new List<string>() { "follower11", "follower12" }
            };
            FollowerReport report2 = new FollowerReport()
            {
                AccountId = account2.AccountId,
                ChangeDate = new DateTime(2020, 3, 5, 3, 54, 34, 321),
                AddedFollowers = new List<string>() { "follower1", "follower2" },
                RemovedFollowers = new List<string>() { "follower3", "follower4" }
            };
            FollowerReport report3 = new FollowerReport()
            {
                AccountId = account2.AccountId,
                ChangeDate = new DateTime(2020, 3, 6, 5, 53, 24, 221),
                AddedFollowers = new List<string>() { "follower5", "follower6" },
                RemovedFollowers = new List<string>() { "follower7", "follower8" }
            };
            repo.AddFollowerReport(report1);
            repo.AddFollowerReport(report2);
            repo.AddFollowerReport(report3);
        }
    }
}