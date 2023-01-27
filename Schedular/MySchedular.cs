using Microsoft.Extensions.Hosting;
using Quartz;
using Quartz.Spi;
using QuartzConsoleAppTrail2.Models;
using System.Collections.Specialized;

namespace QuartzConsoleAppTrail2.Schedular
{
    public class MySchedular : IHostedService
    {
        public IScheduler Scheduler { get; set; }
        private readonly IJobFactory jobFactory;
        private readonly List<JobMetadata> jobMetadatas;
        private readonly ISchedulerFactory schedulerFactory;

        public MySchedular(ISchedulerFactory schedulerFactory, List<JobMetadata> jobMetadatas, IJobFactory jobFactory)
        {
            this.jobFactory = jobFactory;
            this.schedulerFactory = schedulerFactory;
            this.jobMetadatas = jobMetadatas;
        }

        #region Private
        private ITrigger CreateTrigger(JobMetadata jobMetadata)
        {
            return TriggerBuilder.Create()
                .WithIdentity(jobMetadata.JobId.ToString())
                .WithCronSchedule(jobMetadata.CronExpression)
                .WithDescription(jobMetadata.JobName)
                .Build();
        }

        private IJobDetail CreateJob(JobMetadata jobMetadata)
        {
            return JobBuilder.Create(jobMetadata.JobType)
                .WithIdentity(jobMetadata.JobId.ToString())
                .WithDescription(jobMetadata.JobName)
                .Build();
        }
        #endregion

        #region Public
        public async Task StartAsync(CancellationToken cancellationToken)
        {

            // Create a connection string to the database
            string connectionString = "Server=ES-LAPTOP-953\\MSSQLSERVER2017;Database=QuartzTrail;User Id=SA;Password=Sa123456;Trusted_Connection=True;TrustServerCertificate=True;";

            NameValueCollection props = new NameValueCollection
            {
                    { "quartz.jobStore.type", "Quartz.Impl.AdoJobStore.JobStoreTX, Quartz" },
                    { "quartz.jobStore.dataSource", "default" },
                    { "quartz.jobStore.tablePrefix", "QRTZ_" },
                    { "quartz.dataSource.default.connectionString", connectionString },
                    { "quartz.dataSource.default.provider", "SqlServer" },
                    { "quartz.serializer.type", "binary" },
                    { "quartz.jobStore.performSchemaValidation", "false" }
            };

            //Creating Schdeular
            //Scheduler = new StdSchedulerFactory(props).GetScheduler().GetAwaiter().GetResult(); // Jobstore - props need to add for Jobstore

            Scheduler = await schedulerFactory.GetScheduler();
            Scheduler.JobFactory = jobFactory;

            //Suporrt for Multiple Jobs
            jobMetadatas?.ForEach(jobMetadata =>
            {
                //Create Job
                IJobDetail jobDetail = CreateJob(jobMetadata);
                //Create trigger
                ITrigger trigger = CreateTrigger(jobMetadata);
                //Schedule Job
                Scheduler.ScheduleJob(jobDetail, trigger, cancellationToken).GetAwaiter();
                //Start The Schedular
            });
            await Scheduler.Start(cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await Scheduler.Shutdown();
        }
        #endregion
    }
}
