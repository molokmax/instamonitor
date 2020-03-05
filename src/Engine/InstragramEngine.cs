using InstaSharper.API;
using InstaSharper.API.Builder;
using InstaSharper.Classes;
using InstaSharper.Logger;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstaMonitor.Engine
{
    /// <summary>
    /// Engine of instagram api. Wrapper for client of instagram api
    /// </summary>
    public class InstragramEngine
    {
        private IInstaApi api;
        private IConfiguration configuration;

        public InstragramEngine(IConfiguration config)
        {
            configuration = config;
        }

        /// <summary>
        /// Initialize api instagram
        /// Get user and pass from config and make sing in
        /// </summary>
        /// <returns></returns>
        public async Task Initialize()
        {
            UserSessionData user = new UserSessionData();
            user.UserName = configuration.GetValue<string>("Auth:UserName");
            user.Password = configuration.GetValue<string>("Auth:Password");

            api = BuildApi(user);

            await Login();
        }

        /// <summary>
        /// Build client of instagram api
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private IInstaApi BuildApi(UserSessionData user)
        {
            IInstaApi result = InstaApiBuilder.CreateBuilder()
                .SetUser(user)
                .UseLogger(new DebugLogger(LogLevel.Exceptions))
                .SetRequestDelay(RequestDelay.FromSeconds(6, 10))
                .Build();
            return result;
        }

        /// <summary>
        /// Make sign in
        /// </summary>
        /// <returns></returns>
        private async Task Login()
        {
            IResult<InstaLoginResult> request = await api.LoginAsync();
            if (request.Succeeded)
            {
                Console.WriteLine("Logged in!"); // TODO: log with NLog
            }
            else
            {
                Console.WriteLine("Error logging in!" + Environment.NewLine + request.Info.Message); // TODO: log with NLog
                // TODO: throw error
            }
        }
        
        /// <summary>
        /// Get list of followers of passed user
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public async Task<List<string>> GetFollowers(string userName)
        {
            var followersReq = await api.GetUserFollowersAsync(userName, PaginationParameters.Empty);
            if (followersReq.Succeeded)
            {
                var result = followersReq.Value.Select(x => x.UserName).ToList();
                return result;
            }
            else
            {
                Console.WriteLine("Error getting user followers!" + Environment.NewLine + followersReq.Info.Message); // TODO: log with NLog
                // TODO: throw error?
                return new List<string>();
            }
        }

    }
}
