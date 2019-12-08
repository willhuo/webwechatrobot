using Dijing.Common.Core.DataStruct;
using Dijing.Common.Core.Utility;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using WechatRobot.SDK.DTO;

namespace WechatRobot.SDK.Infrastructure
{
    public class WeChatLoginClient : IWeChatLoginClient
    {
        /*constructor*/
        public WeChatLoginClient(IWeChatHttpClient weChatHttpClient, IWeChatContactClient weChatContactClient)
        {
            _WeChatHttpClient = weChatHttpClient;
            _WeChatContactClient = weChatContactClient;
        }

        /*variable*/        
        private IWeChatHttpClient _WeChatHttpClient;
        private IWeChatContactClient _WeChatContactClient;
        private WaitLoginResponse _WaitLoginResponse;
        private LoginResponse _LoginResponse;
        private WeChatInitResponse _WeChatInitResponse;
        private string _UUID = string.Empty;


        /*attribute*/
        public User User => _WeChatInitResponse.User;                       //当前登录用户信息
        public string Username => _WeChatInitResponse.User.UserName;        //当前用户名     
        public LoginResponse LoginResponse => _LoginResponse;               //登录response
        public string[] ChatSetArray => _WeChatInitResponse.ChatSetArray;   //微信初始化过程中所附带的群列表提示信息
        public SyncKey SyncKey
        {
            get => _WeChatInitResponse.SyncKey;
            set => _WeChatInitResponse.SyncKey = value;
        }




        /*public method*/
        public IResult<string> Login()
        {
            LogHelper.Default.LogDay("准备获取UUID");
            LogHelper.Default.LogPrint("准备获取UUID", 2);
            var resultUUID = _WeChatHttpClient.GetUuid();
            if(!resultUUID.Success)
            {
                return resultUUID;
            }
            else
            {
                _UUID = resultUUID.Data;
            }

            LogHelper.Default.LogDay("准备获取登录用的二维码");
            LogHelper.Default.LogPrint("准备获取登录用的二维码", 2);
            var resultQRCodeImage = _WeChatHttpClient.GetQRCodeImage(resultUUID.Data);
            return resultQRCodeImage;
        }
        public IResult<User> WaitForLogin()
        {
            var result = new Result<User>();

            //扫码登陆
            var resultWaitLoginResponse = _WeChatHttpClient.WaitForScanQRCode(_UUID);
            if(!resultWaitLoginResponse.Success)
            {
                result.SetFailed();
                result.SetDesc(resultWaitLoginResponse.Desc);
                return result;
            }
            else
            {
                _WaitLoginResponse = resultWaitLoginResponse.GetData();
            }

            //登陆跳转
            var resultLoginResponse = _WeChatHttpClient.LoginRedirect(resultWaitLoginResponse.Data.RedirectUri);
            if(!resultLoginResponse.Success)
            {
                result.SetFailed();
                result.SetDesc(resultLoginResponse.Desc);
                return result;
            }
            else
            {
                _LoginResponse = resultLoginResponse.GetData();
            }

            //微信初始化
            var resultWeChatInitResponse = _WeChatHttpClient.WeChatInit(resultLoginResponse.Data);
            if(!resultWeChatInitResponse.Success)
            {
                result.SetFailed();
                result.SetDesc(resultWeChatInitResponse.Desc);
                return result;
            }
            else
            {
                _WeChatInitResponse = resultWeChatInitResponse.GetData();
            }           

            //获取当前用户头像
            var resultHeadPhoto = _WeChatHttpClient.GetHeadPhoto(resultWeChatInitResponse.Data.User.HeadImgUrl);
            if(resultHeadPhoto.Success)
            {
                resultWeChatInitResponse.Data.User.HeadImgBase64 = resultHeadPhoto.GetData();
            }

            result.SetSuccess();
            result.SetData(resultWeChatInitResponse.Data.User);
            return result;
        }
    }
}
