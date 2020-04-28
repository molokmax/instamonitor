using InstaMonitor.Core.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace InstaMonitor.Core.Engine
{
    /// <summary>
    /// Interface for running checks
    /// </summary>
    public interface ICheckExecutor
    {
        /// <summary>
        /// Run checks
        /// </summary>
        /// <param name="monitorItems"></param>
        /// <returns></returns>
        Task Check(List<MonitorItem> monitorItems);
    }
}
