using InstaMonitor.Core.Model;
using LiteDB;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace InstaMonitor.Core.Engine
{
    /// <summary>
    /// Repository for persist records
    /// </summary>
    public class DataRepository : IDataRepository
    {

        private LiteDatabase db;

        public DataRepository(IConfiguration config, string dbFileName = null)
        {
            ConnectionString connectionString = GetConnectionString(config);
            if (!String.IsNullOrEmpty(dbFileName))
            {
                connectionString.Filename = dbFileName;
            }
            string dir = Path.GetDirectoryName(connectionString.Filename);
            Directory.CreateDirectory(dir);
            db = new LiteDatabase(connectionString);
        }

        /// <summary>
        /// Get database connection string and parse it
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        private ConnectionString GetConnectionString(IConfiguration config)
        {
            string connectionString = config.GetConnectionString("LiteDB");
            ConnectionString result = new ConnectionString(connectionString);
            return result;
        }

        /// <summary>
        /// Get account model of account with passed login
        /// Returns null if not found
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public Account GetAccount(string userName)
        {
            var collection = db.GetCollection<Account>("accounts");
            collection.EnsureIndex(x => x.UserName);
            Account result = collection.FindOne(x => x.UserName == userName);
            return result;
        }

        /// <summary>
        /// Add or update record of the account in database
        /// If primary key is empty record is add
        /// </summary>
        /// <param name="account"></param>
        public void SaveAccount(Account account)
        {
            var collection = db.GetCollection<Account>("accounts");
            collection.Upsert(account);
        }

        /// <summary>
        /// Remove record of the account from database
        /// </summary>
        /// <param name="account"></param>
        public void DeleteAccount(Account account)
        {
            var collection = db.GetCollection<Account>("accounts");
            collection.Delete(account.AccountId);
        }

        /// <summary>
        /// Get list of followers for passed account
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public List<Follower> GetFollowers(Account account)
        {
            var collection = db.GetCollection<Follower>("followers");
            collection.EnsureIndex(x => x.AccountId);
            List<Follower> result = collection.Find(x => x.AccountId == account.AccountId).ToList();
            return result;
        }

        /// <summary>
        /// Remove passed followers from the database
        /// </summary>
        /// <param name="followers"></param>
        public void DeleteFollowers(List<Follower> followers)
        {
            var collection = db.GetCollection<Follower>("followers");
            List<ObjectId> ids = followers.Select(x => x.FollowerId).ToList();
            collection.DeleteMany(x => ids.Contains(x.FollowerId));
        }

        /// <summary>
        /// Remove followers of passed account from the database
        /// </summary>
        /// <param name="account"></param>
        public void DeleteFollowers(Account account)
        {
            var collection = db.GetCollection<Follower>("followers");
            collection.EnsureIndex(x => x.AccountId);
            collection.DeleteMany(x => x.AccountId == account.AccountId);
        }

        /// <summary>
        /// Add new records of followers to the database
        /// </summary>
        /// <param name="followers"></param>
        public void AddFollowers(List<Follower> followers)
        {
            var collection = db.GetCollection<Follower>("followers");
            collection.EnsureIndex(x => x.AccountId);
            collection.InsertBulk(followers);
        }

        /// <summary>
        /// Add report record about followers
        /// </summary>
        /// <param name="report"></param>
        public void AddFollowerReport(FollowerReport report)
        {
            var collection = db.GetCollection<FollowerReport>("followerReports");
            collection.EnsureIndex(x => x.AccountId);
            collection.Insert(report);
        }

        /// <summary>
        /// Get from database list of followers by passed account
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public List<FollowerReport> GetFollowerReports(Account account)
        {
            var collection = db.GetCollection<FollowerReport>("followerReports");
            collection.EnsureIndex(x => x.AccountId);
            List<FollowerReport> result = collection.Find(x => x.AccountId == account.AccountId).OrderBy(x => x.ChangeDate).ToList();
            return result;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (db != null)
                    {
                        db.Dispose();
                        db = null;
                    }
                }

                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }
        #endregion
    }
}
