using System;
using System.IO;
using log4net;
using log4net.Config;
using log4net.Repository;

namespace Sikiro.SMS.Toolkits
{
    /// <summary>
    /// 日志帮助类
    /// </summary>
    public static class LoggerHelper
    {
        private static readonly ILoggerRepository Repository = LogManager.CreateRepository("NETCoreRepository");
        public static readonly ILog Log = LogManager.GetLogger(Repository.Name, typeof(LoggerHelper));

        static LoggerHelper()
        {
            XmlConfigurator.Configure(Repository, new FileInfo("log4net.config"));
        }

        #region 文本日志

        /// <summary>
        /// 文本日志
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        public static void WriteToFile(this Exception ex, string message = null)
        {
            if (string.IsNullOrEmpty(message))
                message = ex.Message;

            Log.Error(message, ex);
        }
        #endregion
    }
}
