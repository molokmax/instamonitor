using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InstaMonitor.Core.Engine;
using InstaMonitor.Core.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace InstaMonitor.Api.Controllers
{
    /// <summary>
    /// Controller for running checks
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class CheckController : ControllerBase
    {

        private readonly IInstagramEngine Engine;
        private readonly IDataRepository Repo;
        private readonly IConfiguration Configuration;

        public CheckController(IInstagramEngine engine, IDataRepository repo, IConfiguration config)
        {
            Engine = engine;
            Repo = repo;
            Configuration = config;
        }

        /// <summary>
        /// TODO: make POST method and return info about check
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<bool> Get()
        {
            // TODO: add logging
            // Parse configuration. Get list of objects for monitoring
            List<MonitorItem> monitorItems = Configuration.GetSection("MonitorItems").Get<List<MonitorItem>>();

            if (monitorItems != null && monitorItems.Any())
            {
                await Engine.Initialize();

                ICheckExecutor checker = new CheckExecutor(Engine, Repo);
                await checker.Check(monitorItems);
            }
            return true;
        }
    }
}
