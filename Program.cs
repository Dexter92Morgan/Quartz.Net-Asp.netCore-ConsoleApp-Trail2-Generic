using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using QuartzConsoleAppTrail2.JobFactory;
using QuartzConsoleAppTrail2.Jobs;
using QuartzConsoleAppTrail2.Models;
using QuartzConsoleAppTrail2.Schedular;

namespace QuartzConsoleAppTrail2
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton<IJobFactory, MyJobFactory>();
                    services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();

                    #region Adding JobType
                    services.AddSingleton<NotificationJob>();
                    services.AddSingleton<LoggerJob>();
                    #endregion

                    #region Adding Jobs 
                    List<JobMetadata> jobMetadatas = new List<JobMetadata>();
                    jobMetadatas.Add(new JobMetadata(Guid.NewGuid(), typeof(NotificationJob), "Notify Job", "0/10 * * * * ?"));
                    jobMetadatas.Add(new JobMetadata(Guid.NewGuid(), typeof(LoggerJob), "Log Job", "0/5 * * * * ?"));

                    services.AddSingleton(jobMetadatas);
                    #endregion

                    services.AddHostedService<MySchedular>();
                });
    }
}
