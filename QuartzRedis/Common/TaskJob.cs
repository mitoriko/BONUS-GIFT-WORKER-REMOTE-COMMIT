using Quartz;
using QuartzRedis.Buss;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QuartzRedis.Common
{
    public class TaskJob : IJob
    {
        static TaskJobBuss taskJobBuss = new TaskJobBuss();

        public async Task Execute(IJobExecutionContext context)
        {
            await taskJobBuss.DoWork();
        }
    }
}
