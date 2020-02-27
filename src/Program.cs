using InstaMonitor.Engine;
using InstaMonitor.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace InstaMonitor
{
    class Program
    {
        

        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Begin");

                InstagramMonitorConfigurationBuilder ioc = new InstagramMonitorConfigurationBuilder();
                List<MonitorItem> monitorItems = ioc.Configuration.GetSection("MonitorItems").Get<List<MonitorItem>>();

                if (monitorItems.Any())
                {
                    InstragramEngine engine = ioc.ServiceProvider.GetService<InstragramEngine>();
                    engine.Initialize().Wait();
                    
                    foreach (MonitorItem item in monitorItems)
                    {
                        if (item.Followers)
                        {
                            List<string> followers = engine.GetFollowers(item.UserName).GetAwaiter().GetResult();

                            Console.WriteLine($"{item.UserName}'s followers:");
                            foreach (string user in followers)
                            {
                                Console.WriteLine(user);
                            }
                            Console.WriteLine("=========");
                        }
                    }
                    // Console.Read();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error! {0}", e.ToString());
            }
            finally
            {
                Console.WriteLine("Finish");
            }
        }
    }
}
