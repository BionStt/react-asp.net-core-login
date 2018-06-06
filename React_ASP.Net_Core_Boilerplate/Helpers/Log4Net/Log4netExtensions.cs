using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;
using log4net.Core;
using System.Xml;
using log4net;
using log4net.Repository;
using System.Reflection;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.IO;

namespace React_ASP.Net_Core_Boilerplate.Helpers{
    public static class Log4netExtensions
    {
        public static ILoggerFactory AddLog4Net(this ILoggerFactory factory, string log4NetConfigFile)
        {
            factory.AddProvider(new Log4NetProvider(log4NetConfigFile));
            return factory;
        }

        public static ILoggerFactory AddLog4Net(this ILoggerFactory factory)
        {
            factory.AddProvider(new Log4NetProvider("log4net.config"));
            return factory;
        }
    }
}

