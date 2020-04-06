using InstaMonitor.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace InstaMonitor.Engine
{
    public interface ICheckExecutor
    {
        Task Check(List<MonitorItem> monitorItems);
    }
}
