using Quartz;
using Quartz.Impl;
using QuartzRedis.Buss;
using QuartzRedis.Common;
using System;
using System.Collections.Specialized;
using System.Threading.Tasks;

namespace QuartzRedis
{
    class Program
    {
        static void Main(string[] args)
        {
            Global.Startup();
            StartAsync().GetAwaiter().GetResult();
            Console.ReadLine();
        }

        static async Task StartAsync()
        {
            NameValueCollection pros = new NameValueCollection();
            pros.Add("quartz.scheduler.instanceName", "System");
            pros.Add("quartz.threadPool.threadCount", "10");
            StdSchedulerFactory sf = new StdSchedulerFactory(pros);
            IScheduler sched = await sf.GetScheduler();

            await sched.Start();

            IJobDetail job = JobBuilder.Create<TaskJob>()
                .WithIdentity("RemoteTask", "System")
                .Build();
            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("RemoteTaskTrigger", "System")
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(Global.Interval)
                    .RepeatForever())
            .Build();

            await sched.ScheduleJob(job, trigger);
            Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm") + "> " + "已启动更新任务列表计划");
        }
    }
}
