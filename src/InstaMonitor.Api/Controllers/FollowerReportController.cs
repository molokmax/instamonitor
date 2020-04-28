using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InstaMonitor.Api.Model;
using InstaMonitor.Core.Engine;
using InstaMonitor.Core.Model;
using Microsoft.AspNetCore.Mvc;

namespace InstaMonitor.Api.Controllers
{
    /// <summary>
    /// Controller for working with reports
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class FollowerReportController : ControllerBase
    {

        private readonly IReportService ReportService;

        public FollowerReportController(IReportService reportService)
        {
            ReportService = reportService;
        }

        /// <summary>
        /// Get ino about follower reports
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IEnumerable<FollowerReportDTO>> Get([FromQuery]string login)
        {
            // TODO: add logging
            List<FollowerReport> list = await ReportService.GetFollowerReports(login);
            List<FollowerReportDTO> result = list.Select(x => new FollowerReportDTO()
            {
                ChangeDate = x.ChangeDate,
                AddedFollowers = x.AddedFollowers,
                RemovedFollowers = x.RemovedFollowers
            }).ToList();
            return result;
        }
    }
}
