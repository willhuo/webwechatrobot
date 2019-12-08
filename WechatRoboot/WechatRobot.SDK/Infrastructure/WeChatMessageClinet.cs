using Dijing.Common.Core.DataStruct;
using Dijing.Common.Core.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WechatRobot.SDK.DataStruct;
using WechatRobot.SDK.DTO;

namespace WechatRobot.SDK.Infrastructure
{
    public class WeChatMessageClinet : IWeChatMessageClinet
    {
        /*constructor*/
        public WeChatMessageClinet(IWeChatHttpClient weChatHttpClient, IWeChatContactClient weChatContactClient,IWeChatLoginClient weChatLoginClient)
        {
            _WeChatHttpClient = weChatHttpClient;
            _WeChatContactClient = weChatContactClient;
            _WeChatLoginClient = weChatLoginClient;

            Init();
        }


        /*variable*/
        private IWeChatHttpClient _WeChatHttpClient;
        private IWeChatContactClient _WeChatContactClient;
        private IWeChatLoginClient _WeChatLoginClient;
        private System.Timers.Timer _SyncCheckTimer;                    //同步消息线程监控Timer
        private int _SyncCheckMaxInterval = 60000;                      //最大循环间隔60S
        private DateTime _LastSyncCheckTime = DateTime.MinValue;        //最后一次同步消息的时间
        private int _NullCyclelingSyncCheckTimes = 0;                   //获取空消息次数
        private int __NullCyclelingSyncCheckTimesLimit = 3;             //获取空消息次数限制
        private int _StartNewTaskForSyncCheckByFreeSecondsLimit = 60;   //空闲时间，超出该值，则启动新线程同步消息
        public bool _Logout = false;                                    //退出标志，仅供内部循环使用
        public int _LogoutTimes = 0;                                    //登出次数
        public int _LogoutTimesLimit = 3;                               //登出次数限制

        /*attr*/
          

        /*public method*/
        public void BeginSyncMsg()
        {
            _Logout = false;
        }
        public void SyncCheck(IWeChatLoginClient weChatLoginClient)
        {
            //检测程序是否退出
            if(_Logout)
            {
                SystemInfo.IsLogin = false;

                LogHelper.Default.LogDay("微信离线，消息同步线程退出");
                LogHelper.Default.LogPrint("微信离线，消息同步线程退出", 3);
                return;
            }

            //更新同步消息时间
            _LastSyncCheckTime = DateTime.Now;

            //同步检查是否有新消息
            var resultSyncCheckResponse = _WeChatHttpClient.SyncCheck(weChatLoginClient.LoginResponse, weChatLoginClient.SyncKey.ToString());
            if (!resultSyncCheckResponse.Success)
            {
                //同步消息异常，直接进入下个循环
                //LogHelper.Default.LogPrint("", 3);
                Task.Run(() => SyncCheck(weChatLoginClient));
            }
            else
            {
                //同步消息结果解析
                SyncCheckResponseParser(resultSyncCheckResponse.Data, weChatLoginClient);

                //进入下一个循环接收消息
                Task.Run(() => SyncCheck(weChatLoginClient));
            }
        }
        public IResult<SendMessageResponse> SendMessage(LoginResponse loginResponse, string fromUsername, string message, string toUsername)
        {
            var result = _WeChatHttpClient.SendMessage(loginResponse, fromUsername, message, toUsername);
            return result;
        }


        /*private method*/
        private void Init()
        {
            _SyncCheckTimer = new System.Timers.Timer(_SyncCheckMaxInterval);
            _SyncCheckTimer.Elapsed += _SyncCheckTimer_Elapsed;
        }
        private void SyncCheckResponseParser(SyncCheckResponse syncCheckResponse, IWeChatLoginClient weChatLoginClient)
        {
            try
            {
                if (syncCheckResponse.RetCode == "0")
                {
                    if (syncCheckResponse.Selector == "0")
                    {
                        //没有任何消息
                        LogHelper.Default.LogPrint($"没有新消息，空循环", 1);
                    }
                    else if (syncCheckResponse.Selector == "2")
                    {
                        LogHelper.Default.LogPrint("接收到新消息", 2);

                        //拉取消息
                        var resultFetchMessageResponse = _WeChatHttpClient.FetchMessage(weChatLoginClient.LoginResponse, weChatLoginClient.SyncKey);
                        if (resultFetchMessageResponse.Success)
                        {
                            //更新synckey
                            weChatLoginClient.SyncKey = resultFetchMessageResponse.Data.SyncKey;

                            //提取消息
                            GetMessages(resultFetchMessageResponse.Data, weChatLoginClient);
                        }
                        else
                        {
                            LogHelper.Default.LogDay("消息拉取失败");
                            LogHelper.Default.LogPrint("消息拉取失败", 3);
                        }
                    }
                    else if (syncCheckResponse.Selector == "4")
                    {
                        LogHelper.Default.LogPrint("通讯列表更新", 3);

                        //拉取通讯录更新消息
                        var resultFetchMessageResponse = _WeChatHttpClient.FetchMessage(weChatLoginClient.LoginResponse, weChatLoginClient.SyncKey);
                        if (resultFetchMessageResponse.Success)
                        {
                            //更新synckey
                            weChatLoginClient.SyncKey = resultFetchMessageResponse.Data.SyncKey;

                            //提取通讯录更新消息
                            GetMessages(resultFetchMessageResponse.Data, weChatLoginClient);
                        }
                        else
                        {
                            LogHelper.Default.LogDay("通讯列表更新消息拉取失败");
                            LogHelper.Default.LogPrint("通讯列表更新消息拉取失败", 3);
                        }
                    }
                    else if (syncCheckResponse.Selector == "6")
                    {
                        LogHelper.Default.LogDay("0-6:暂停5S");
                        LogHelper.Default.LogPrint("0-6:暂停5S", 1);
                        Task.Delay(5000).Wait();
                    }
                    else if (syncCheckResponse.Selector == "7")
                    {
                        LogHelper.Default.LogDay("0-7:暂停5S");
                        LogHelper.Default.LogPrint("0-7:暂停5S", 1);
                        Task.Delay(5000).Wait();
                    }
                    else
                    {
                        LogHelper.Default.LogDay($"未知情况,RetCode={syncCheckResponse.RetCode},Selector={syncCheckResponse.Selector}");
                        LogHelper.Default.LogPrint($"未知情况,RetCode={syncCheckResponse.RetCode},Selector={syncCheckResponse.Selector}", 3);
                    }

                    _LogoutTimes = 0;
                }
                else if (syncCheckResponse.RetCode == "1101")
                {
                    _LogoutTimes++;
                    if (_LogoutTimes > _LogoutTimesLimit)
                    {
                        _Logout = true;
                    }

                    LogHelper.Default.LogDay($"从微信客户端上登出,{_LogoutTimes}/{_LogoutTimesLimit}");
                    LogHelper.Default.LogPrint($"从微信客户端上登出,{_LogoutTimes}/{_LogoutTimesLimit}", 3);
                }
                else
                {
                    LogHelper.Default.LogDay($"未解析的同步消息,RetCode={syncCheckResponse.RetCode},Selector={syncCheckResponse.Selector}");
                    LogHelper.Default.LogPrint($"未解析的同步消息,RetCode={syncCheckResponse.RetCode},Selector={syncCheckResponse.Selector}", 3);
                    Task.Delay(5000).Wait();
                }
            }
            catch(Exception ex)
            {
                LogHelper.Default.LogDay($"消息类型解析异常，{ex}");
                LogHelper.Default.LogPrint($"消息类型解析异常，{ex.Message}", 4);
            }
        }
        private void GetMessages(FetchMessageResponse fetchMessageResponse, IWeChatLoginClient weChatLoginClient)
        {
            try
            {
                LogHelper.Default.LogPrint($"当前接收消息条数：{fetchMessageResponse.AddMsgCount}", 2);

                if (fetchMessageResponse.AddMsgList != null && fetchMessageResponse.AddMsgList.Count > 0)
                {
                    //消息解析
                    foreach (var v in fetchMessageResponse.AddMsgList)
                    {
                        var fromUserName = v.FromUserName;
                        var toUserName = v.ToUserName;
                        var fromNickName = _WeChatContactClient.AllMemberList?.Where(x => x.UserName == fromUserName).Select(x => x.NickName).FirstOrDefault();
                        var toNickName = _WeChatContactClient.AllMemberList?.Where(x => x.UserName == toUserName).Select(x => x.NickName).FirstOrDefault();

                        if (v.MsgType == 1 && v.ImgStatus == 1 && v.Status == 3)
                        {
                            //文字【确认】
                            //腾讯新闻检测规则(?<=;name[^\[]+)\[CDATA\[[^\]]+]](?=\S+/name)
                            //腾讯新闻列表提取规则(?<=;title[^\[]+)\[CDATA\[[^\]]+]](?=\S+/title)
                            var name = Regex.Match(v.Content, "(?<=;name[^\\[]+)\\[CDATA\\[[^\\]]+]](?=\\S+/name)").ToString();
                            if (!string.IsNullOrEmpty(name))
                            {
                                var itemList = new List<string>();
                                var mas = Regex.Matches(v.Content, "(?<=;title[^\\[]+)\\[CDATA\\[[^\\]]+]](?=\\S+/title)");
                                foreach (Match ma in mas)
                                {
                                    itemList.Add(ma.ToString());
                                }
                                LogHelper.Default.LogPrint($"{name}\r\n{string.Join("\r\n", itemList)}");
                            }
                            else
                            {
                                LogHelper.Default.LogPrint($"文本消息[{fromNickName}==>{toNickName}]:{v.Content}", 2);
                            }
                        }
                        else if (v.MsgType == 3 && v.ImgStatus == 2 && v.Status == 3)
                        {
                            //图片【确认】
                            LogHelper.Default.LogPrint($"图片[{fromNickName}==>{toNickName}]:MsgId={v.MsgId}", 2);
                            DownloadImage(v.MsgId, "big", weChatLoginClient, ".jpg");
                        }
                        else if (v.MsgType == 34 && v.ImgStatus == 1 && v.Status == 3)
                        {
                            //语音消息【确认】
                            LogHelper.Default.LogPrint($"语音[{fromNickName}==>{toNickName}]", 2);

                            //语音下载
                            DownloadAudio(v.MsgId, weChatLoginClient);
                        }
                        else if (v.MsgType == 43 && v.ImgStatus == 1 && v.Status == 3)
                        {
                            //小视频【确定】
                            LogHelper.Default.LogPrint($"小视频[{fromNickName}==>{toNickName}]", 2);

                            //视频略缩图下载
                            DownloadImage(v.MsgId, "slave", weChatLoginClient, ".jpg");

                            //小视频下载
                            DownloadVideo(v.MsgId, weChatLoginClient);
                        }
                        else if (v.MsgType == 47 && v.ImgStatus == 2 && v.Status == 3)
                        {
                            //动画【确认】
                            //动画类型提取 1:系统内置 2：可以使GIF
                            var type = Regex.Match(v.Content, "(?<=type=\")[^\"]+").ToString().Trim();
                            if (type == "1")
                            {
                                var ma = Regex.Match(v.Content, "gameext type=\"([^\"]+)\"\\s+content=\"([^\"]+)");
                                LogHelper.Default.LogPrint($"内置动画[{fromNickName}==>{toNickName}]:MsgId={v.MsgId},gameext type={ma.Groups[1]},content={ma.Groups[2]}", 2);
                                DownloadImage(v.MsgId, "big", weChatLoginClient, ".jpg");
                            }
                            else if (type == "2")
                            {
                                LogHelper.Default.LogPrint($"GIF图片[{fromNickName}==>{toNickName}]:MsgId={v.MsgId}", 2);
                                DownloadImage(v.MsgId, "big", weChatLoginClient, ".gif");
                            }
                            else if (v.HasProductId == 1)
                            {
                                LogHelper.Default.LogDay($"只能在手机查看的表情[{fromNickName}==>{toNickName}]:MsgId={v.MsgId}");
                            }
                            else
                            {
                                LogHelper.Default.LogDay($"未知图片[{fromNickName}==>{toNickName}]:MsgId={v.MsgId}");
                                LogHelper.Default.LogPrint($"未知图片[{fromNickName}==>{toNickName}]:MsgId={v.MsgId}", 2);
                            }
                        }
                        else if (v.MsgType == 49 && v.ImgStatus == 1 && v.Status == 3)
                        {
                            if (v.AppMsgType == 6)
                            {
                                //文件接收
                                var fileName = v.FileName;
                                LogHelper.Default.LogPrint($"接收文件[{fromNickName}==>{toNickName}]：{fileName}", 2);

                                //文件下载
                                DownloadFile(v.MsgId, toUserName, v.MediaId, v.FileName, v.EncryFileName, weChatLoginClient);
                            }
                            else if (v.AppMsgType == 2000)
                            {
                                //微信转账
                                var fileName = v.FileName;
                                var desc = Regex.Match(v.Content, "(?<=;des[^\\[]+)\\[CDATA\\[[^\\]]+]](?=\\S+/des)").ToString();
                                LogHelper.Default.LogPrint($"{fileName}[{fromNickName}==>{toNickName}]:{desc}", 2);
                            }
                            else
                            {
                                LogHelper.Default.LogDay($"49-1-3内置类型未解析，{JsonConvert.SerializeObject(v)}");
                                LogHelper.Default.LogPrint($"49-1-3内置类型未解析，{JsonConvert.SerializeObject(v)}", 3);
                            }


                            ////订阅号消息【确认】
                            //var stepCountType = Regex.Match(v.Content, "\\[CDATA\\[夺得\\d+月\\d+日排行榜冠军\\]\\]").ToString();
                            //if (!string.IsNullOrEmpty(stepCountType))
                            //{
                            //    //微信运动
                            //    LogHelper.Default.LogDay($"微信运动订阅号消息[{fromNickName}==>{toNickName}]");
                            //    LogHelper.Default.LogPrint($"微信运动订阅号消息[{fromNickName}==>{toNickName}]", 2);
                            //}
                            //else
                            //{
                                
                            //    //(?<=;title[^\[]+)\[CDATA\[[^\]]+]](?=\S+/title)                                
                            //    var mas = Regex.Matches(v.Content, "(?<=;title[^\\[]+)\\[CDATA\\[[^\\]]+]](?=\\S+/title)");
                            //    if(mas.Count==1)
                            //    {
                            //        //转账消息，也有可能是别的
                            //        //消息识别规则(?<=;des[^\[]+)\[CDATA\[[^\]]+]](?=\S+/des)
                            //        var desc = Regex.Match(v.Content, "(?<=;des[^\\[]+)\\[CDATA\\[[^\\]]+]](?=\\S+/des)").ToString();
                            //        LogHelper.Default.LogDay($"[{fromNickName}==>{toNickName}]:title={mas[0].ToString()},des={desc}");
                            //        LogHelper.Default.LogPrint($"[{fromNickName}==>{toNickName}]:title={mas[0].ToString()},des={desc}", 2);
                            //    }
                            //    else
                            //    {
                            //        //普通订阅号消息
                            //        var itemList = new List<string>();
                            //        foreach (Match ma in mas)
                            //        {
                            //            itemList.Add(ma.ToString());
                            //        }

                            //        LogHelper.Default.LogDay($"订阅号消息[{fromNickName}==>{toNickName}]\r\n{string.Join("\r\n", itemList)}");
                            //        LogHelper.Default.LogPrint($"订阅号消息[{fromNickName}==>{toNickName}]\r\n{string.Join("\r\n", itemList)}", 2);
                            //    }                               
                            //}
                        }
                        else if (v.MsgType == 49 && v.ImgStatus == 2 && v.Status == 3)
                        {
                            //群内的新闻链接【确定】
                            var title = Regex.Match(v.Content, "(?<=title)\\S+(?=/title)").ToString().Replace("&gt;", "");
                            var url = Regex.Match(v.Content, "(?<=url)\\S+(?=/url)").ToString().Replace("&gt;", "");
                            LogHelper.Default.LogPrint($"外链消息[{fromNickName}==>{toNickName}]\r\n" +
                                $"title={title}\r\n" +
                                $"url={url}", 2);
                        }
                        else if (v.MsgType == 51 && v.ImgStatus == 1 && v.Status == 3)
                        {
                            //跟通信列表有关系，单个联系人更新或者群列表同志【确认】
                            if (v.StatusNotifyUserNameArray != null && v.StatusNotifyUserNameArray.Length > 1)
                            {
                                LogHelper.Default.LogDay("消息解析过程中，遇到微信群列表通知，进行群列表获取");
                                LogHelper.Default.LogPrint("消息解析过程中，遇到微信群列表通知，进行群列表获取", 2);
                                GetBatchContacts(v.StatusNotifyUserNameArray);
                            }
                            else
                            {
                                //LogHelper.Default.LogDay($"通信列表更新,[{fromNickName}==>{toNickName}]:{JsonConvert.SerializeObject(fetchMessageResponse)}");
                                LogHelper.Default.LogPrint($"通信列表更新[{fromNickName}==>{toNickName}]", 2);
                            }
                        }
                        else if (v.MsgType == 10000 && v.ImgStatus == 1 && v.Status == 3)
                        {
                            //收到红包【确定】
                            LogHelper.Default.LogPrint($"[{fromNickName}==>{toNickName}]:{v.Content}", 2);
                        }
                        else if (v.MsgType == 10000 && v.ImgStatus == 1 && v.Status == 4)
                        {
                            //群内邀请通知【确定】
                            LogHelper.Default.LogPrint($"群内邀请通知[{fromNickName}==>{toNickName}]:{v.Content}", 2);
                        }
                        else if (v.MsgType == 10002 && v.ImgStatus == 1 && v.Status == 4)
                        {
                            //消息撤回【确认】
                            var oldMgId = Regex.Match(v.Content, "(?<=;oldmsgid[^\\d]+)\\d+(?=[^\\d+]+oldmsg)").ToString();
                            var msgId = Regex.Match(v.Content, "(?<=;msgid[^\\d]+)\\d+(?=[^\\d+]+msg)").ToString();
                            var msg = Regex.Match(v.Content, "\\[[^\\]]+]]").ToString();
                            LogHelper.Default.LogPrint($"消息撤回[{fromNickName}==>{toNickName}]:{msg},oldMsgId={oldMgId},msgId={msgId}", 2);
                        }
                        else
                        {
                            LogHelper.Default.LogDay($"未解析类型消息：{v.MsgType}-{v.ImgStatus}-{v.Status}-{v.Content}");
                            LogHelper.Default.LogPrint($"未解析类型消息：{v.MsgType}-{v.ImgStatus}-{v.Status}-{v.Content}", 3);
                        }
                    }

                    _NullCyclelingSyncCheckTimes = 0;
                }
                else if (fetchMessageResponse.ModContactCount > 0)
                {
                    //通信列表更新消息解析
                    foreach (var v in fetchMessageResponse.ModContactList)
                    {
                        LogHelper.Default.LogDay($"通信列表更新，NickName={v.NickName},UserName={v.UserName},City={v.City},Signature={v.Signature}");
                        LogHelper.Default.LogPrint($"通信列表更新，NickName={v.NickName},UserName={v.UserName},City={v.City},Signature={v.Signature}", 2);
                    }

                    _NullCyclelingSyncCheckTimes = 0;
                }
                else
                {
                    //LogHelper.Default.LogDay($"接收到空的消息体,{JsonConvert.SerializeObject(fetchMessageResponse)}");
                    //LogHelper.Default.LogPrint($"接收到空的消息体,{JsonConvert.SerializeObject(fetchMessageResponse)}", 3);
                    LogHelper.Default.LogDay($"接收到空的消息体");
                    LogHelper.Default.LogPrint($"接收到空的消息体", 3);

                    //高频率空循环延时检测
                    _NullCyclelingSyncCheckTimes++;
                    if (_NullCyclelingSyncCheckTimes >= __NullCyclelingSyncCheckTimesLimit)
                    {
                        _NullCyclelingSyncCheckTimes = 0;
                        Task.Delay(5000).Wait();
                    }
                }
            }
            catch(Exception ex)
            {
                LogHelper.Default.LogDay($"消息解析异常，{ex}");
                LogHelper.Default.LogPrint($"消息解析异常，{ex.Message}", 4);
            }
        }
        private void DownloadImage(string msgId, string type, IWeChatLoginClient weChatLoginClient, string ext = ".jpg")
        {
            var result = _WeChatHttpClient.FetchImageMessage(msgId, type, weChatLoginClient.LoginResponse);
            if (result.Success)
            {
                var fullFileName = SystemInfo.WechatImagePath + msgId + ext;
                File.WriteAllBytes(fullFileName, result.GetData());
                LogHelper.Default.LogPrint($"图片下载完成", 2);
            }
        }
        private void DownloadVideo(string msgId, IWeChatLoginClient weChatLoginClient)
        {
            var result = _WeChatHttpClient.FetchVideoMessage(msgId, weChatLoginClient.LoginResponse);
            if (result.Success)
            {
                var fullFileName = SystemInfo.WechatImagePath + msgId + ".mp4";
                File.WriteAllBytes(fullFileName, result.GetData());
                LogHelper.Default.LogPrint($"小视频下载完成", 2);
            }
        }
        private void DownloadAudio(string msgId, IWeChatLoginClient weChatLoginClient)
        {
            var result = _WeChatHttpClient.FetchAudioMessage(msgId, weChatLoginClient.LoginResponse);
            if (result.Success)
            {
                var fullFileName = SystemInfo.WechatImagePath + msgId + ".mp3";
                File.WriteAllBytes(fullFileName, result.GetData());
                LogHelper.Default.LogPrint($"语音下载完成", 2);
            }
        }
        private void DownloadFile(string msgId, string sender, string mediaid,string fileName, string encryfilename, IWeChatLoginClient weChatLoginClient)
        {
            var result = _WeChatHttpClient.FetchFileMessage(sender,mediaid,encryfilename, weChatLoginClient.LoginResponse);
            if (result.Success)
            {
                var fullFileName = SystemInfo.WechatImagePath + msgId + fileName;
                File.WriteAllBytes(fullFileName, result.GetData());
                LogHelper.Default.LogPrint($"文件下载完成", 2);
            }
        }
        private void GetBatchContacts(string[] userNameArray)
        {
            var result = _WeChatContactClient.GetBatchContacts(_WeChatLoginClient.LoginResponse, userNameArray);
        }

        /*event*/
        private void _SyncCheckTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (_Logout == false && SystemInfo.IsLogin)
            {
                var sp = (DateTime.Now - _LastSyncCheckTime).TotalSeconds;
                if(sp> _StartNewTaskForSyncCheckByFreeSecondsLimit)
                {
                    Task.Run(() => SyncCheck(_WeChatLoginClient));
                    LogHelper.Default.LogDay($"启动新的消息同步线程");
                    LogHelper.Default.LogPrint($"启动新的消息同步线程", 3);
                }
            }
        }
    }
}
 