using Dijing.Common.Core.DataStruct;
using Dijing.Common.Core.Utility;
using Dijing.Weixin.Common;
using Newtonsoft.Json;
using QRCoder;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using WechatRobot.SDK.Common;
using WechatRobot.SDK.DataStruct;
using WechatRobot.SDK.DTO;

namespace WechatRobot.SDK.Infrastructure
{
    public class WeChatHttpClient : IWeChatHttpClient
    {
        /*constructor*/
        public WeChatHttpClient(HttpClient httpClient)
        {
            _HttpClient = httpClient;
        }


        /*variable*/
        private HttpClient _HttpClient;


        /*public method*/
        public IResult<string> GetUuid()
        {
            var result = new Result<string>();

            try
            {
                var getUUIDUrl = string.Format(UrlEndPoints.GetUUIDUrl, DateTimeHelper.Default.GetTimestamp(13));
                var respage = _HttpClient.GetStringAsync(getUUIDUrl).Result;

                Match match = Regex.Match(respage, "window.QRLogin.code = (\\d+); window.QRLogin.uuid = \"(\\S+?)\"");
                if (match.Success && "200" == match.Groups[1].Value)
                {
                    var uuid = match.Groups[2].Value;
                    LogHelper.Default.LogDay($"UUID获取成功，uuid={uuid}");
                    LogHelper.Default.LogPrint($"UUID获取成功，uuid={uuid}", 2);

                    result.SetSuccess();
                    result.SetData(uuid);
                    return result;                    
                }
                else
                {
                    LogHelper.Default.LogDay($"GetUuid失败，{respage}");
                    LogHelper.Default.LogPrint($"GetUuid失败，{respage}", 3);

                    result.SetFailed();
                    result.SetDesc("UUID获取失败");
                    return result;                    
                }
            }
            catch (Exception ex)
            {
                LogHelper.Default.LogDay($"GetUuid异常，{ex}");
                LogHelper.Default.LogPrint($"GetUuid异常，{ex.Message}", 4);
                result.SetFailed();
                result.SetDesc("UUID获取异常，" + ex.Message);
                return result;
            }
        }
        public IResult<string> GetQRCodeImage(string uuid)
        {
            var result = new Result<string>();

            try
            {
                var qrCodeUrl = string.Format(UrlEndPoints.GetQRCodeUrl, uuid);
                var qrGenerator = new QRCodeGenerator();
                var qrCodeData = qrGenerator.CreateQrCode(qrCodeUrl, QRCodeGenerator.ECCLevel.Q);
                var qrCode = new QRCode(qrCodeData);                
                var qrCodeImage = qrCode.GetGraphic(20);
                using (var ms = new MemoryStream())
                {
                    qrCodeImage.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                    var qrcodeImageBuff = ms.ToArray();
                    var qrcodeImageStr = Convert.ToBase64String(qrcodeImageBuff);

                    LogHelper.Default.LogDay("登陆二维码获取成功");
                    LogHelper.Default.LogPrint("登陆二维码获取成功", 2);

                    result.SetSuccess();
                    result.SetData(qrcodeImageStr);
                    return result;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Default.LogDay($"登陆二维码获取异常，{ex}");
                LogHelper.Default.LogPrint($"登陆二维码获取异常，{ex.Message}", 4);

                result.SetFailed();
                result.SetDesc("登陆二维码获取异常，"+ex.Message);
                return result;
            }
        }
        public IResult<WaitLoginResponse> WaitForScanQRCode(string uuid)
        {
            //tip=1, 等待用户扫描二维码,
            //       201: scaned
            //       408: timeout
            //tip=0, 等待用户确认登录,
            //       200: confirmed
            //       408: timeout

            string tip = "1";
            int tryTimes = 0;
            int tryTimesLimit = 2;
            var result = new Result<WaitLoginResponse>();

            try
            {
                while (true)
                {
                    tryTimes++;

                    if (tip == "1")
                    {
                        LogHelper.Default.LogDay($"等待用户扫码，尝试次数={tryTimes}");
                        LogHelper.Default.LogPrint($"等待用户扫码，尝试次数={tryTimes}", 2);
                    }                       
                    else
                    {
                        LogHelper.Default.LogDay($"等待用户登陆确认，尝试次数={tryTimes}");
                        LogHelper.Default.LogPrint($"等待用户登陆确认，尝试次数={tryTimes}", 2);
                    }

                    var loginUrl = string.Format(UrlEndPoints.WaitLoginUrl, tip, uuid, DateTimeHelper.Default.GetTimestamp(13));
                    var respage = _HttpClient.GetStringAsync(loginUrl).Result;
                    var code = Regex.Match(respage, "(?<=window.code=)\\d+").ToString();

                    if (code == "201")
                    {
                        LogHelper.Default.LogDay("用户已经扫码完成");
                        LogHelper.Default.LogPrint("用户已经扫码完成", 2);
                        tip = "0";
                    }
                    else if (code == "200")
                    {
                        LogHelper.Default.LogDay("用户扫码后确认登陆成功");
                        LogHelper.Default.LogPrint($"用户扫码后确认登陆成功", 2);
                        var redirectUrl = Regex.Match(respage, "(?<=window.redirect_uri=\")[^\"]+").ToString();
                        if (!string.IsNullOrEmpty(redirectUrl))
                        {
                            redirectUrl += "&fun=new";
                            var baseUrl = redirectUrl.Substring(0, redirectUrl.LastIndexOf('/'));
                            var host = baseUrl.Substring(8);

                            var response = new WaitLoginResponse()
                            {
                                RedirectUri = redirectUrl,
                                BaseUri = baseUrl,
                                BaseHost = host
                            };

                            result.SetSuccess();
                            result.SetData(response);
                            return result;
                        }
                        else
                        {
                            LogHelper.Default.LogDay("登陆后的跳转地址解析失败");
                            LogHelper.Default.LogPrint("登陆后的跳转地址解析失败", 3);

                            result.SetFailed();
                            result.SetDesc("登陆后的跳转地址解析失败");
                            break;
                        }
                    }
                    else if (code == "400")
                    {
                        LogHelper.Default.LogDay($"登陆异常，code={code}");
                        LogHelper.Default.LogPrint($"登陆异常，code={code}", 3);

                        result.SetFailed();
                        result.SetDesc($"登陆异常，code={code}");
                        break;
                    }
                    else if (code == "408")
                    {
                        //TODO:在未扫码的情况下，需要测试几次等待后超时
                        LogHelper.Default.LogDay($"登陆等待超时，tip={tip}");
                        LogHelper.Default.LogPrint($"登陆等待超时，tip={tip}", 3);
                        tip = "1";


                        result.SetFailed();
                        result.SetDesc($"登陆等待超时,请刷新页面重新扫码登录");
                        break;

                        //if (tryTimes >= tryTimesLimit)
                        //{
                        //    result.SetFailed();
                        //    result.SetDesc($"登陆等待超时，tip={tip}");
                        //    break;
                        //}
                        //else
                        //{
                        //    tip = "1";
                        //}
                    }
                    else
                    {
                        LogHelper.Default.LogDay($"登录过程异常未解析的Code={code}");
                        LogHelper.Default.LogPrint($"登录过程异常未解析的Code={code}",3);
                    }
                }

                LogHelper.Default.LogDay("等待扫码登录循环检测退出");
                LogHelper.Default.LogPrint($"等待扫码登录循环检测退出", 3);
                return result;
            }
            catch (Exception ex)
            {
                LogHelper.Default.LogDay($"登陆异常，{ex}");
                LogHelper.Default.LogPrint($"登陆异常，{ex.Message}", 4);

                result.SetFailed();
                result.SetDesc($"登陆异常，{ex.Message}");
                return result;
            }
        }
        public IResult<LoginResponse> LoginRedirect(string redirectUrl)
        {
            var result = new Result<LoginResponse>();

            try
            {
                var respage = _HttpClient.GetStringAsync(redirectUrl).Result;
                var loginResponse = Utility.XmlDeserialize<LoginResponse>(respage, x => x.PassTicket = x.PassTicket.UrlDecode());

                LogHelper.Default.LogDay("登录跳转完成");
                LogHelper.Default.LogPrint("登录跳转完成");

                result.SetSuccess();
                result.SetData(loginResponse);
                return result;
            }
            catch (Exception ex)
            {
                LogHelper.Default.LogDay($"登陆跳转异常，{ex}");
                LogHelper.Default.LogPrint($"登陆跳转异常，{ex.Message}", 4);

                result.SetFailed();
                result.SetDesc($"登陆跳转异常，{ex.Message}");
                return result;
            }
        }
        public IResult<WeChatInitResponse> WeChatInit(LoginResponse loginResponse)
        {
            var result = new Result<WeChatInitResponse>();
            string respage = string.Empty;
             
            try
            {
                var baseRequest = Utility.MapperToBaseRequest(loginResponse, SystemInfo.DeviceId);
                var body = new { BaseRequest = baseRequest };
                var postStr = JsonConvert.SerializeObject(body);
                var content = new StringContent(postStr);
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");
                var weChatInitUrl = string.Format(UrlEndPoints.WeChatInitUrl, DateTimeHelper.Default.GetTimestamp(13), loginResponse.PassTicket);
                var response = _HttpClient.PostAsync(weChatInitUrl, content).Result;
                respage = response.Content.ReadAsStringAsync().Result;
                var weChatInitResponse = JsonConvert.DeserializeObject<WeChatInitResponse>(respage);

                LogHelper.Default.LogDay("微信初始化成功");
                LogHelper.Default.LogPrint("微信初始化成功", 2);

                result.SetSuccess();
                result.SetData(weChatInitResponse);
                return result;
            }
            catch (Exception ex)
            {
                LogHelper.Default.LogDay($"微信初始化异常，{ex}\r\n{respage}");
                LogHelper.Default.LogPrint($"微信初始化异常，{ex.Message}", 4);

                result.SetFailed();
                result.SetDesc($"微信初始化异常，{ex.Message}");
                return result;
            }
        }
        public IResult<string> GetHeadPhoto(string headPhototUrl)
        {
            var result = new Result<string>();
            try
            {
                var url = UrlEndPoints.Domain + headPhototUrl;
                var buff = _HttpClient.GetByteArrayAsync(url).Result;
                var headPhotoBase64Str = Convert.ToBase64String(buff);

                //LogHelper.Default.LogDay("用户头像获取成功");
                LogHelper.Default.LogPrint("用户头像获取成功", 2);

                result.SetSuccess();
                result.SetData(headPhotoBase64Str);
                return result;
            }
            catch (Exception ex)
            {
                LogHelper.Default.LogDay($"当前登陆用户头像获取异常，{ex}");
                LogHelper.Default.LogPrint($"当前登陆用户头像获取异常，{ex.Message}", 4);

                result.SetFailed();
                result.SetDesc($"当前登陆用户头像获取异常，{ex.Message}");
                return result;
            }
        }
        public IResult<ContactResponse> GetContactList(LoginResponse loginResponse)
        {
            var result = new Result<ContactResponse>();
            try
            {
                var getContactsUrl = string.Format(UrlEndPoints.GetContactsUrl, loginResponse.PassTicket, loginResponse.Skey, DateTimeHelper.Default.GetTimestamp(13));
                var respage = _HttpClient.GetStringAsync(getContactsUrl).Result;
                var contactList = JsonConvert.DeserializeObject<ContactResponse>(respage);

                LogHelper.Default.LogDay("联系人列表获取成功");
                LogHelper.Default.LogPrint("联系人列表获取成功", 2);

                result.SetSuccess();
                result.SetData(contactList);
                return result;
            }
            catch (Exception ex)
            {
                LogHelper.Default.LogDay($"联系人列表获取异常，{ex}");
                LogHelper.Default.LogPrint($"联系人列表获取异常，{ex.Message}", 4);

                result.SetFailed();
                result.SetDesc($"联系人列表获取异常，{ex.Message}");
                return result;
            }
        }
        public IResult<BatchContactResponse> GetBatchContactList(LoginResponse loginResponse,string[] userNameArray)
        {
            var result = new Result<BatchContactResponse>();
            try
            {
                var list = new List<object>();
                foreach(var v in userNameArray)
                {
                    var chatRoom = new
                    {
                        EncryChatRoomId = "",
                        UserName = v
                    };
                    list.Add(chatRoom);
                }

                BaseRequest baseRequest = Utility.MapperToBaseRequest(loginResponse, SystemInfo.DeviceId);
                var body = new
                {
                    BaseRequest = baseRequest,
                    Count = list.Count,
                    List = list
                };
                var postStr = JsonConvert.SerializeObject(body);
                var content = new StringContent(postStr);
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

                var getBatchContactsUrl = string.Format(UrlEndPoints.GetBatchContactsUrl, DateTimeHelper.Default.GetTimestamp(13), loginResponse.PassTicket);
                var response = _HttpClient.PostAsync(getBatchContactsUrl, content).Result;
                var respage = response.Content.ReadAsStringAsync().Result;
                var contactList = JsonConvert.DeserializeObject<BatchContactResponse>(respage);

                LogHelper.Default.LogDay("微信群列表获取成功");
                LogHelper.Default.LogPrint("微信群列表获取成功", 2);

                result.SetSuccess();
                result.SetData(contactList);
                return result;
            }
            catch (Exception ex)
            {
                LogHelper.Default.LogDay($"微信群列表获取异常，{ex}");
                LogHelper.Default.LogPrint($"微信群列表获取异常，{ex.Message}", 4);

                result.SetFailed();
                result.SetDesc($"微信群列表获取异常，{ex.Message}");
                return result;
            }
        }
        public IResult<SyncCheckResponse> SyncCheck(LoginResponse loginResponse, string syncKey)
        {
            var result = new Result<SyncCheckResponse>();
            string respage = string.Empty;

            try
            {
                var timeStamp = DateTimeHelper.Default.GetTimestamp(13);
                var syncCheckUrl = string.Format(UrlEndPoints.SyncCheckUrl,
                    loginResponse.WxSid,
                    loginResponse.WxUin,
                    syncKey,
                    timeStamp,
                    loginResponse.Skey.Replace("@", "%40"),
                    SystemInfo.DeviceId,
                    timeStamp);
                respage = _HttpClient.GetStringAsync(syncCheckUrl).Result;
                var match = Regex.Match(respage, "retcode:\"(\\d+)\",selector:\"(\\d+)\"");
                if (match.Success)
                {
                    var syncCheckResponse = new SyncCheckResponse()
                    {
                        RetCode = match.Groups[1].Value,
                        Selector = match.Groups[2].Value
                    };

                    result.SetSuccess();
                    result.SetData(syncCheckResponse);
                    return result;
                }
                else
                {
                    LogHelper.Default.LogDay($"同步消息检测失败，{respage}");
                    LogHelper.Default.LogPrint($"同步消息检测失败，{respage}", 3);

                    result.SetFailed();
                    result.SetDesc("同步消息检测失败");
                    return result;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Default.LogDay($"同步消息检测异常，{ex},rspage={respage}");
                LogHelper.Default.LogPrint($"同步消息检测异常，{ex.Message}", 4);

                result.SetFailed();
                result.SetDesc("同步消息检测异常," + ex.Message);
                return result;
            }
        }
        public IResult<FetchMessageResponse> FetchMessage(LoginResponse loginResponse,SyncKey syncKey)
        {
            var result = new Result<FetchMessageResponse>();
            try
            {
                var fetchMessageUrl = string.Format(UrlEndPoints.FetchMessageUrl, loginResponse.WxSid, loginResponse.Skey, loginResponse.PassTicket);
                var request = new FetchMessageRequest()
                {
                    BaseRequest = new BaseRequest()
                    {
                        DeviceID = SystemInfo.DeviceId,
                        Sid = loginResponse.WxSid,
                        Skey = loginResponse.Skey,
                        Uin = loginResponse.WxUin.ToString()
                    },
                    SyncKey = syncKey,
                    rr = DateTimeHelper.Default.GetTimestamp(13)
                };
                var postStr = JsonConvert.SerializeObject(request);
                var content = new StringContent(postStr);
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");
                var response = _HttpClient.PostAsync(fetchMessageUrl, content).Result;
                var respage = response.Content.ReadAsStringAsync().Result;
                var fetchMessageResponse = JsonConvert.DeserializeObject<FetchMessageResponse>(respage);

                result.SetSuccess();
                result.SetData(fetchMessageResponse);
                return result;
            }
            catch (Exception ex)
            {
                LogHelper.Default.LogDay($"消息拉取异常，{ex}");
                LogHelper.Default.LogPrint($"消息拉取异常，{ex.Message}", 4);

                result.SetFailed();
                result.SetDesc($"消息拉取异常，{ex.Message}");
                return result;
            }
        }
        public IResult<byte[]> FetchImageMessage(string msgId,string type, LoginResponse loginResponse)
        {
            var result = new Result<byte[]>();
            try
            {
                var fetchImageMessageUrl = string.Format(UrlEndPoints.FetchImageMessageUrl,
                    msgId,
                    loginResponse.Skey.Replace("@", "%40"),
                    type);
                var buff = _HttpClient.GetByteArrayAsync(fetchImageMessageUrl).Result;
                result.SetSuccess();
                result.SetData(buff);
                return result;
            }
            catch (Exception ex)
            {
                LogHelper.Default.LogDay($"图片下载异常，{ex}");
                LogHelper.Default.LogPrint($"图片下载异常，{ex.Message}", 4);

                result.SetFailed();
                result.SetDesc($"图片下载异常，{ex.Message}");
                return result;
            }
        }
        public IResult<byte[]> FetchVideoMessage(string msgId, LoginResponse loginResponse)
        {
            var result = new Result<byte[]>();
            try
            {
                var fetchVideoMessageUrl = string.Format(UrlEndPoints.FetchVideoMessageUrl,
                    msgId,
                    loginResponse.Skey.Replace("@", "%40"));
                _HttpClient.DefaultRequestHeaders.Add("Range", "bytes=0-");

                var buff = _HttpClient.GetByteArrayAsync(fetchVideoMessageUrl).Result;

                _HttpClient.DefaultRequestHeaders.Remove("Range");
                result.SetSuccess();
                result.SetData(buff);
                return result;
            }
            catch (Exception ex)
            {
                LogHelper.Default.LogDay($"小视频下载异常，{ex}");
                LogHelper.Default.LogPrint($"小视频下载异常，{ex.Message}", 4);

                result.SetFailed();
                result.SetDesc($"小视频下载异常，{ex.Message}");
                return result;
            }
        }
        public IResult<byte[]> FetchAudioMessage(string msgId, LoginResponse loginResponse)
        {
            var result = new Result<byte[]>();
            try
            {
                var fetchAudioMessageUrl = string.Format(UrlEndPoints.FetchAudioMessageUrl,
                    msgId,
                    loginResponse.Skey);
                _HttpClient.DefaultRequestHeaders.Add("Range", "bytes=0-");

                var buff = _HttpClient.GetByteArrayAsync(fetchAudioMessageUrl).Result;

                _HttpClient.DefaultRequestHeaders.Remove("Range");
                result.SetSuccess();
                result.SetData(buff);
                return result;
            }
            catch (Exception ex)
            {
                LogHelper.Default.LogDay($"语音下载异常，{ex}");
                LogHelper.Default.LogPrint($"语音下载异常，{ex.Message}", 4);

                result.SetFailed();
                result.SetDesc($"语音下载异常，{ex.Message}");
                return result;
            }
        }
        public IResult<byte[]> FetchFileMessage(string sender,string mediaid,string encryfilename, LoginResponse loginResponse)
        {
            var result = new Result<byte[]>();
            try
            {
                var fetchFileMessageUrl = string.Format(UrlEndPoints.FetchFileMessageUrl,
                    sender,
                    mediaid,
                    encryfilename,
                    loginResponse.PassTicket);

                var buff = _HttpClient.GetByteArrayAsync(fetchFileMessageUrl).Result;
                result.SetSuccess();
                result.SetData(buff);
                return result;
            }
            catch (Exception ex)
            {
                LogHelper.Default.LogDay($"文件下载异常，{ex}");
                LogHelper.Default.LogPrint($"文件下载异常，{ex.Message}", 4);

                result.SetFailed();
                result.SetDesc($"文件下载异常，{ex.Message}");
                return result;
            }
        }
        public IResult<SendMessageResponse> SendMessage(LoginResponse loginResponse, string fromUsername, string message, string toUsername)
        {
            var result = new Result<SendMessageResponse>();
            try
            {
                var sendMessageUrl = string.Format(UrlEndPoints.SendMessageUrl, loginResponse.PassTicket);
                var request = new SendMessageRequest()
                {
                    BaseRequest = Utility.MapperToBaseRequest(loginResponse, SystemInfo.DeviceId),
                    Msg = new SendMessageBody()
                    {
                        LocalID = Utility.GetClientMsgId(),
                        Content = message,
                        FromUserName = fromUsername,
                        ToUserName = toUsername,
                        Type = 1
                    }
                };
                var postStr = JsonConvert.SerializeObject(request);
                var content = new StringContent(postStr);
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                var response = _HttpClient.PostAsync(sendMessageUrl, content).Result;
                var respage = response.Content.ReadAsStringAsync().Result;
                var sendMessageResponse = JsonConvert.DeserializeObject<SendMessageResponse>(respage);

                result.SetSuccess();
                result.SetData(sendMessageResponse);
                return result;
            }
            catch (Exception ex)
            {
                LogHelper.Default.LogDay($"消息发送异常，{ex}");
                LogHelper.Default.LogPrint($"消息发送异常，{ex.Message}", 4);

                result.SetFailed();
                result.SetDesc($"消息发送异常，{ex.Message}");
                return result;
            }
        }
    }
}
