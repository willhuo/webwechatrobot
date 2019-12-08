using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Dijing.Common.Core.Utility;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using WechatRobot.BusinessLogic.Wechat;
using WechatRobot.Common.DataStruct.Options;
using WechatRobot.SDK.Infrastructure;

namespace WechatRobot.Web
{
    public class Program
    {
        /*main func*/
        static void Main(string[] args)
        {
            //忽略ctr+c退出程序
            Console.CancelKeyPress += Console_CancelKeyPress;

            //中文支持
            EncodingReg();

            //全局异常捕获
            ErrorHandle();

            //标题设置
            InitUI();

            //运行自宿主服务
            HostRun(args);

            //循环等待
            LogHelper.Default.LogPrint($"WechatRobot启动成功", 2);
            Cycling();
        }

        /*private method*/
        private static void EncodingReg()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Console.OutputEncoding = Encoding.UTF8;
        }
        private static void ErrorHandle()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
        }
        private static void InitUI()
        {
            var curAssembly = System.Reflection.Assembly.GetExecutingAssembly();
            var attributes = curAssembly.GetCustomAttributes(typeof(System.Reflection.AssemblyFileVersionAttribute), false);
            var fileVersionAttribute = (System.Reflection.AssemblyFileVersionAttribute)attributes.First();
            var version = fileVersionAttribute.Version;
            Console.Title = $"WechatRobot[{version}]";

            RemoveCloseButton();
        }
        private static void RemoveCloseButton()
        {
            EnvironmentHelper.Default.RemoveCurrentConsoleWindowMenu();
        }
        private static void Cycling()
        {
            while (true)
            {
                Task.Delay(5000).Wait();
            }
        }
        private static void HostRun(string[] args)
        {
            RegisterHelper.Default.CheckLicense("5bd223e6654368182ab0284c1d4032e8", true);
            var webHost = new WebHostBuilder()
                .UseKestrel()
                .ConfigureKestrel(options =>
                {

                    var webserverOption = options.ApplicationServices.GetService<IOptions<WebserverOption>>()?.Value;
                    options.ListenLocalhost(webserverOption.ListenPort);
#if DEBUG
                    options.Listen(IPAddress.Parse(webserverOption.DebugListenHost), webserverOption.ListenPort);
#elif RELEASE
                    options.Listen(IPAddress.Parse(webserverOption.ReleaseListenHost), webserverOption.ListenPort);
#endif                    
                })
                .ConfigureAppConfiguration(config =>
                {
                    config.SetBasePath(Directory.GetCurrentDirectory());
                    config.AddJsonFile("appsettings.json", false, reloadOnChange: true);
                    config.AddEnvironmentVariables();
                })
                .ConfigureServices((hostContext, services) =>
                {
                    //注册Cookie认证服务
                    services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();
                    services.AddSession(x => { x.IdleTimeout = TimeSpan.FromMinutes(20); });
                    services.AddMvc(options =>
                    {
                        options.RespectBrowserAcceptHeader = false;
                        options.OutputFormatters.Add(new XmlSerializerOutputFormatter());
                        options.OutputFormatters.Add(new XmlDataContractSerializerOutputFormatter());

                    }).AddJsonOptions(options =>
                    {
                        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                        options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver();
                        options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
                    });

                    //从反向代理获取真是的用户访问IP地址
                    services.Configure<ForwardedHeadersOptions>(options =>
                    {
                        options.ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor | Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto;
                    });

                    services.AddCors(options =>
                    {
                        options.AddPolicy("chatHub",
                            policy => policy.AllowAnyOrigin()
                                            .AllowAnyHeader()
                                            .AllowAnyMethod());
                    });

                    //添加signalIR服务
                    services.AddSignalR();

                    //参数配置
                    services.AddOptions();
                    services.Configure<LogOption>(hostContext.Configuration.GetSection("Log"));
                    services.Configure<WebserverOption>(hostContext.Configuration.GetSection("WebServer"));

                    //接口注入
                    services.AddSingleton<IWeChatEngine>(new WeChatEngine(hostContext.HostingEnvironment.WebRootPath));
                    services.AddScoped<IWechatLogic, WechatLogic>();
                    services.AddHostedService<LogService>();
                })
                .Configure(app =>
                {
                    app.UseDeveloperExceptionPage();

                    //app.UseStaticFiles();
                    var provider = new FileExtensionContentTypeProvider();
                    provider.Mappings.Add(".apk", "application/vnd.android.package-archive");
                    app.UseStaticFiles(new StaticFileOptions
                    {
                        ServeUnknownFileTypes = true,
                        ContentTypeProvider = provider
                    });
                    app.UseAuthentication();
                    app.UseSession();
                    //app.UseSignalR(routes =>
                    //{
                    //    routes.MapHub<ChatHub>("/chatHub");
                    //});
                    app.UseWebSockets();

                    app.UseMvc(routes =>
                    {
                        routes.MapRoute(
                            name: "Default",
                            template: "{controller=Home}/{action=Index}/{id?}");
                    });
                })
                .Build();
            webHost.RunAsync();
        }


        /*event*/
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            LogHelper.Default.LogDay($"CurrentDomain_UnhandledException,{e}");
        }
        private static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            LogHelper.Default.LogDay($"CurrentDomain_ProcessExit,{e}");
        }
        private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            LogHelper.Default.LogPrint("Ctrl+C不再支持退出操作，请通过任务管理结束进程", 3);
            return;
        }
    }
}
