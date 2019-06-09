using QuartzRedis.Buss;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuartzRedis.Common
{
    public class TaskJob
    {
        static TaskJobBuss taskJobBuss = new TaskJobBuss();

        static bool IsBusy = false;

        public static void Worker()
        {
            IsBusy = true;
            var redis = RedisManager.getRedisConn();
            var db = redis.GetDatabase(11);
            while (db.ListLength(Global.TASK_PREFIX + "." + Global.TASK_JOB) > 0)
            {
                RedisValue ids = db.ListRightPop(Global.TASK_PREFIX + "." + Global.TASK_JOB);
                if (!ids.IsNull)
                {
                    taskJobBuss.doWork(ids.ToString());
                }
            }
            IsBusy = false;
        }

        static void onMessageHandle(ChannelMessage channelMessage)
        {
            if (channelMessage.Message.ToString() == Global.TOPIC_MESSAGE)
            {
                Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm") + "> " + Global.TASK_TOPIC + "." + Global.TASK_JOB + "配置已更新");
                if (!IsBusy)
                {
                    Worker();
                }
                else
                {
                    Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm") + "> " + "系统正忙，忽略本次通知");
                }
            }
        }

        static Action<ChannelMessage> action = new Action<ChannelMessage>(onMessageHandle);

        public static void Subscribe()
        {
            var redis = RedisManager.getRedisConn();
            var queue = redis.GetSubscriber().Subscribe(Global.TASK_TOPIC + "." + Global.TASK_JOB);

            queue.OnMessage(action);
            Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm") + "> " + "已订阅" + Global.TASK_TOPIC + "." + Global.TASK_JOB + "配置更新");
        }

    }
}
