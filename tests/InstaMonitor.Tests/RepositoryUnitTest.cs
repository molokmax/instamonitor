using InstaMonitor.Engine;
using InstaMonitor.Model;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;
using System.IO;

namespace InstaMonitor.Tests
{
    [TestFixture]
    public class RepositoryUnitTest
    {
        private string DbFileName;

        private TestConfigurationBuilder IoC;
        private IServiceScope Scope;
        private IDataRepository Repo;

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
            string binDir = TestContext.CurrentContext.TestDirectory;
            string testDir = TestContext.CurrentContext.Test.Name;
            DbFileName = $"{binDir}/data/{testDir}/test_database_{Guid.NewGuid()}.db";
            Repo = new DataRepository(IoC.Configuration, DbFileName);
            InitTestData(Repo);
        }

        [TearDown]
        public void Cleanup()
        {
            Repo?.Dispose();
            if (File.Exists(DbFileName))
            {
                File.Delete(DbFileName);
            }
        }

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
        }
    }
}