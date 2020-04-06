using InstaMonitor.Engine;
using InstaMonitor.Model;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace InstaMonitor.Tests
{
    [TestFixture]
    public class RepositoryUnitTest
    {

        private TestConfigurationBuilder IoC;
        private IServiceScope Scope;
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
        public void SaveAccount()
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


        [OneTimeSetUp]
        public void Init()
        {
            IoC = new TestConfigurationBuilder();
            Scope = IoC.CreateServiceScope();
        }

        [OneTimeTearDown]
        public void Dispose()
        {
            Scope?.Dispose();
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



        private string GetDbFileName()
        {
            string binDir = TestContext.CurrentContext.TestDirectory;
            string testDir = TestContext.CurrentContext.Test.Name;
            string dbFileName = $"{binDir}/data_test/{testDir}/test_database.db";
            return dbFileName;
        }

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
        }
    }
}