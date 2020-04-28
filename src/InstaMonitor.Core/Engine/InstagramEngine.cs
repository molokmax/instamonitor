using InstaSharper.API;
using InstaSharper.API.Builder;
using InstaSharper.Classes;
using InstaSharper.Logger;
using Microsoft.Extensions.Configuration;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstaMonitor.Core.Engine
{
    /// <summary>
    /// Engine of instagram api. Wrapper for client of instagram api
    /// </summary>
    public class InstagramEngine : IInstagramEngine
    {
        private static ILogger commonLogger = LogManager.GetLogger(Consts.LogTypes.Common);

        private IInstaApi api;
        private IConfiguration configuration;

        public InstagramEngine(IConfiguration config)
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
                .UseLogger(new DebugLogger(InstaSharper.Logger.LogLevel.Exceptions))
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
                commonLogger.Debug("Login successful");
            }
            else
            {
                string errorInfo = "Login failed. " + request.Info.Message + Environment.NewLine + 
                    "Raw response: " + request.Info.ResponseRaw;
                commonLogger.Error(request.Info.Exception, errorInfo);
                throw new ApplicationException($"Login failed. {request.Info.Message}. See log for details");
            }
        }
        
        /// <summary>
        /// Get list of followers of passed user
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public async Task<List<string>> GetFollowers(string userName)
        {
            var request = await api.GetUserFollowersAsync(userName, PaginationParameters.Empty);
            if (request.Succeeded)
            {
                var result = request.Value.Select(x => x.UserName).ToList();
                return result;
            }
            else
            {
                string errorInfo = "Getting user followers failed. " + request.Info.Message + Environment.NewLine +
                    "Raw response: " + request.Info.ResponseRaw;
                commonLogger.Error(request.Info.Exception, errorInfo);
                throw new ApplicationException($"Getting user followers failed. {request.Info.Message}. See log for details");
            }
        }

    }
}
