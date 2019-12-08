using Dijing.Common.Core.DataStruct;
using Dijing.Common.Core.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WechatRobot.SDK.DataStruct;
using WechatRobot.SDK.DTO;

namespace WechatRobot.SDK.Infrastructure
{
    public sealed class WeChatEngine : IWeChatEngine
    {
        /*constructor*/
        public WeChatEngine(string webRootPath)
        {
            SystemInfo.WebRootPath = webRootPath;

            Init();
        }


        /*variable*/
        private CookieContainer _Cookie;
        private HttpClient _HttpClient;
        private IWeChatHttpClient _WeChatHttpClient;
        private IWeChatLoginClient _WeChatLoginClient;
        private IWeChatContactClient _WeChatContactClient;
        private IWeChatMessageClinet _WeChatMessageClient;

        /*attr*/
        public User User { get { return _WeChatLoginClient.User; } }
        public List<ContactUser> ContactList
        {
            get
            {
                return _WeChatContactClient.ContactList;
            }
        }
        public List<ContactUser> BatchContactList
        {
            get
            {
                return _WeChatContactClient.BatchContactList;
            }
        }



        /*public method*/
        public IResult<string> Login()
        {            
            var result = _WeChatLoginClient.Login();
            return result;
        }
        public IResult<User> WaitForLogin()
        {
            //扫码登陆
            var result = _WeChatLoginClient.WaitForLogin();

            if (result.Success)
            {
                //设置登录标志
                LogHelper.Default.LogDay("微信扫码登录成功");
                LogHelper.Default.LogPrint("微信扫码登录成功", 2);
                SystemInfo.IsLogin = true;

                //获取联系人
                GetContactList();

                //监听消息
                ListenMessage();
            }

            return result;
        }
        public IResult<SendMessageResponse> SendMessage(string message, string toUsername)
        {
            var result = _WeChatMessageClient.SendMessage(_WeChatLoginClient.LoginResponse, _WeChatLoginClient.Username, message, toUsername);
            return result;
        }

        /*private method*/
        private void Init()
        {
            if (_Cookie == null)
            {
                _Cookie = new CookieContainer();
            }
            if (_HttpClient == null)
            {
                _HttpClient = new HttpClient(new HttpClientHandler() { UseCookies = true, CookieContainer = _Cookie });
            }
            if (_WeChatHttpClient == null)
            {
                _WeChatHttpClient = new WeChatHttpClient(_HttpClient);
            }
            if (_WeChatContactClient == null)
            {
                _WeChatContactClient = new WeChatContactClient(_WeChatHttpClient);
            }
            if (_WeChatLoginClient == null)
            {
                _WeChatLoginClient = new WeChatLoginClient(_WeChatHttpClient, _WeChatContactClient);
            }
            if (_WeChatMessageClient == null)
            {
                _WeChatMessageClient = new WeChatMessageClinet(_WeChatHttpClient, _WeChatContactClient, _WeChatLoginClient);
            }
            

            if(!Directory.Exists(SystemInfo.WechatImagePath))
            {
                Directory.CreateDirectory(SystemInfo.WechatImagePath);
                LogHelper.Default.LogPrint($"微信图片文件夹创建成功", 2);
            }
        }
        private void GetContactList()
        {
            Task.Run(() =>
            {
                //微信联系人获取
                _WeChatContactClient.GetContactList(_WeChatLoginClient.LoginResponse);

                //微信群获取
                LogHelper.Default.LogDay("微信初始化后，主动进行群列表获取");
                LogHelper.Default.LogPrint("微信初始化后，主动进行群列表获取", 2);
                _WeChatContactClient.GetBatchContacts(_WeChatLoginClient.LoginResponse, _WeChatLoginClient.ChatSetArray);
            });
        }
        private void ListenMessage()
        {
            LogHelper.Default.LogDay("准备监听微信消息");
            LogHelper.Default.LogPrint("准备监听微信消息", 2);
            _WeChatMessageClient.BeginSyncMsg();
            Task.Run(() => _WeChatMessageClient.SyncCheck(_WeChatLoginClient));
        }
    }
}
