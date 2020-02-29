﻿using InstaMonitor.Model;
using LiteDB;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace InstaMonitor.Engine
{
    public class DataRepository : IDisposable
    {

        private LiteDatabase db;

        public DataRepository(IConfiguration config)
        {
            ConnectionString connectionString = GetConnectionString(config);
            string dir = Path.GetDirectoryName(connectionString.Filename);
            Directory.CreateDirectory(dir);
            db = new LiteDatabase(connectionString);
        }

        private ConnectionString GetConnectionString(IConfiguration config)
        {
            string connectionString = config.GetConnectionString("LiteDB");
            ConnectionString result = new ConnectionString(connectionString);
            return result;
        }

        public Account GetAccount(string userName)
        {
            var collection = db.GetCollection<Account>("accounts");
            collection.EnsureIndex(x => x.UserName);
            Account result = collection.FindOne(x => x.UserName == userName);
            return result;
        }

        public void SaveAccount(Account account)
        {
            var collection = db.GetCollection<Account>("accounts");
            collection.Upsert(account);
        }

        public void DeleteAccount(Account account)
        {
            var collection = db.GetCollection<Account>("accounts");
            collection.Delete(account.AccountId);
        }

        public List<Follower> GetFollowers(Account account)
        {
            var collection = db.GetCollection<Follower>("followers");
            collection.EnsureIndex(x => x.AccountId);
            List<Follower> result = collection.Find(x => x.AccountId == account.AccountId).ToList();
            return result;
        }

        public void DeleteFollowers(List<Follower> followers)
        {
            var collection = db.GetCollection<Follower>("followers");
            List<ObjectId> ids = followers.Select(x => x.FollowerId).ToList();
            collection.DeleteMany(x => ids.Contains(x.FollowerId));
        }

        public void DeleteFollowers(Account account)
        {
            var collection = db.GetCollection<Follower>("followers");
            collection.EnsureIndex(x => x.AccountId);
            collection.DeleteMany(x => x.AccountId == account.AccountId);
        }

        public void AddFollowers(List<Follower> followers)
        {
            var collection = db.GetCollection<Follower>("followers");
            collection.EnsureIndex(x => x.AccountId);
            collection.InsertBulk(followers);
        }

        public void AddFollowerReport(FollowerReport report)
        {
            var collection = db.GetCollection<FollowerReport>("followerReports");
            collection.EnsureIndex(x => x.AccountId);
            collection.Insert(report);
        }

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
