using Newtonsoft.Json;
using QuartzRedis.Buss;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuartzRedis.Common
{
    public class Global
    {
        public static void Startup()
        {
            DBHelp.ReloadConnectionString();
        }     

        public static int Interval
        {
            get
            {
                return Convert.ToInt32(Environment.GetEnvironmentVariable("Interval"));
            }
        }
        public static string DataSource
        {
            get
            {
                return Environment.GetEnvironmentVariable("DataSource");
            }
        }
        public static string InitialCatalog
        {
            get
            {
                return Environment.GetEnvironmentVariable("InitialCatalog");
            }
        }
        public static string UserID
        {
            get
            {
                return Environment.GetEnvironmentVariable("UserID");
            }
        }
        public static string Pwd
        {
            get
            {
                return Environment.GetEnvironmentVariable("Pwd");
            }
        }
        public static string AppId
        {
            get
            {
                return Environment.GetEnvironmentVariable("AppId");
            }
        }
        public static string AppSecret
        {
            get
            {
                return Environment.GetEnvironmentVariable("AppSecret");
            }
        }
        public static string PlaceHold
        {
            get
            {
                return Environment.GetEnvironmentVariable("PlaceHold");
            }
        }
        public static string PostUrl
        {
            get
            {
                return Environment.GetEnvironmentVariable("PostUrl");
            }
        }


    }
}
