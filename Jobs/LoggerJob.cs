using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuartzConsoleAppTrail2.Jobs
{
    public class LoggerJob : IJob
    {
        private readonly ILogger<LoggerJob> _logger;
        public LoggerJob(ILogger<LoggerJob> logger)
        {
            this._logger = logger;
        }
        public Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation($"Log Job: at {DateTime.Now} and Jobtype: {context.JobDetail.JobType}");
            return Task.CompletedTask;
        }
    }
}
