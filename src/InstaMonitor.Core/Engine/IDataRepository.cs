using InstaMonitor.Core.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace InstaMonitor.Core.Engine
{
    /// <summary>
    /// Interface of Data persist repository
    /// </summary>
    public interface IDataRepository : IDisposable
    {
        /// <summary>
        /// Get account model of account with passed login
        /// Returns null if not found
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        Account GetAccount(string userName);

        /// <summary>
        /// Add or update record of the account in database
        /// If primary key is empty record is add
        /// </summary>
        /// <param name="account"></param>
        void SaveAccount(Account account);

        /// <summary>
        /// Remove record of the account from database
        /// </summary>
        /// <param name="account"></param>
        void DeleteAccount(Account account);

        /// <summary>
        /// Get list of followers for passed account
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        List<Follower> GetFollowers(Account account);

        /// <summary>
        /// Remove passed followers from the database
        /// </summary>
        /// <param name="followers"></param>
        void DeleteFollowers(List<Follower> followers);

        /// <summary>
        /// Remove followers of passed account from the database
        /// </summary>
        /// <param name="account"></param>
        void DeleteFollowers(Account account);

        /// <summary>
        /// Add new records of followers to the database
        /// </summary>
        /// <param name="followers"></param>
        void AddFollowers(List<Follower> followers);

        /// <summary>
        /// Add report record about followers
        /// </summary>
        /// <param name="report"></param>
        void AddFollowerReport(FollowerReport report);

        /// <summary>
        /// Get from database list of followers by passed account
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        List<FollowerReport> GetFollowerReports(Account account);
    }
}
