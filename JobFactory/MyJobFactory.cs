using Quartz;
using Quartz.Spi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuartzConsoleAppTrail2.JobFactory
{
    public class MyJobFactory : IJobFactory
    {
        private readonly IServiceProvider service;
        public MyJobFactory(IServiceProvider serviceProvider)
        {
            service = serviceProvider;
        }
        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            var jobDetail = bundle.JobDetail;
            return (IJob)service.GetService(jobDetail.JobType);
        }

        public void ReturnJob(IJob job)
        {
            // Method intentionally left empty.
        }
    }
}
