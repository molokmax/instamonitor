using InstaMonitor.Engine;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Collections.Generic;

namespace InstaMonitor.Tests
{
    /// <summary>
    /// Tests for instagram api wrapper
    /// </summary>
    [TestFixture]
    public class InstagramEngineUnitTest
    {

        private TestConfigurationBuilder IoC;
        private IInstagramEngine Engine;

        [Test]
        public void InitializeTest()
        {
            Engine.Initialize().Wait();
        }


        [Test]
        public void GetFollowersTest()
        {
            Engine.Initialize().Wait();
            string accountName = IoC.Configuration["Test:TestAccountName"];
            List<string> followers = Engine.GetFollowers(accountName).GetAwaiter().GetResult();
            Assert.IsNotNull(followers);
        }




        [OneTimeSetUp]
        public void Init()
        {
            IoC = new TestConfigurationBuilder();
        }

        [OneTimeTearDown]
        public void Dispose()
        {
        }

        [SetUp]
        public void SetUp()
        {
            Engine = new InstagramEngine(IoC.Configuration);
        }

        [TearDown]
        public void Cleanup()
        {
        }
    }
}