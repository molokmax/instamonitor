using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace InstaMonitor.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) 
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            Directory.SetCurrentDirectory(baseDir);
            ArgsModel argModel = ParseArgs(args);
            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    // TODO configure root path
                    webBuilder
                        .UseContentRoot(baseDir)
                        .UseUrls(argModel.ListenUrls)
                        .UseStartup<Startup>();
                });
        }

        private static ArgsModel ParseArgs(string[] args)
        {
            ArgsModel result = new ArgsModel();
            if (args.Length > 0)
            {
                foreach (string item in args)
                {
                    if (!String.IsNullOrEmpty(item))
                    {
                        string[] itemArr = item.Split('=', 2);
                        string paramName = itemArr[0];
                        if (String.Equals(paramName, "urls", StringComparison.OrdinalIgnoreCase)) {
                            if (itemArr.Length > 1)
                            {
                                string val = itemArr[1];
                                string[] urls = val
                                    .Split(new char[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries)
                                    .Select(x => x.Trim()).Distinct().ToArray();
                                result.ListenUrls = urls;
                            }
                        }
                    }
                }
            }
            if (result.ListenUrls.Length == 0)
            {
                result.ListenUrls = new string[] { "https://localhost:5001", "http://localhost:5000" };
            }
            return result;
        }

        private class ArgsModel
        {
            public string[] ListenUrls = new string[0];
        }
    }
}
