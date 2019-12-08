using Dijing.Common.Core.Utility;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WechatRobot.Common.DataStruct.Options;

namespace WechatRobot.Web
{
    public class LogService : IHostedService
    {
        /*constructor*/
        public LogService(IOptions<LogOption> logOption)
        {
            this._LogOption = logOption.Value;
        }


        /*variable*/
        private LogOption _LogOption { get; set; }


        /*public method*/
        public Task StartAsync(CancellationToken cancellationToken)
        {
            LogConfig();
            return Task.CompletedTask;
        }
        public Task StopAsync(CancellationToken cancellationToken)
        {
            LogHelper.Default.LogPrint($"Log Service Existing", 3);
            return Task.CompletedTask;
        }


        /*private method*/
        private void LogConfig()
        {
            var printLog = _LogOption.PrintLog;
            var logType = _LogOption.LogType;
            var supportEvent = _LogOption.SupportEvent;

            LogHelper.Default.LogConfig(printLog, logType, supportEvent);
            LogHelper.Default.ReceivingLogEvent += Default_ReceivingLogEvent;
            LogHelper.Default.LogPrint($"日志设置成功,printLog={printLog},logType={logType},supportEvent={supportEvent}", 2);
        }


        /*event*/
        private void Default_ReceivingLogEvent(object sender, LogContent e)
        {
            ConsoleColor printColor;
            switch (e.Type)
            {
                case 1:
                    printColor = ConsoleColor.White;
                    break;
                case 2:
                    printColor = ConsoleColor.Green;
                    break;
                case 3:
                    printColor = ConsoleColor.Yellow;
                    break;
                case 4:
                    printColor = ConsoleColor.Red;
                    break;
                case 5:
                    printColor = ConsoleColor.Magenta;
                    break;
                default:
                    printColor = ConsoleColor.White;
                    break;
            }
            Console.ForegroundColor = printColor;
            Console.WriteLine(e.Msg);
        }
    }
}
