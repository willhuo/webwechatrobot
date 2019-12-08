using Dijing.Common.Core.DataStruct;
using System;
using System.Collections.Generic;
using System.Text;
using WechatRobot.SDK.DTO;

namespace WechatRobot.SDK.Infrastructure
{
    public interface IWeChatHttpClient
    {
        IResult<string> GetUuid();
        IResult<string> GetQRCodeImage(string uuid);
        IResult<WaitLoginResponse> WaitForScanQRCode(string uuid);
        IResult<LoginResponse> LoginRedirect(string redirectUrl);
        IResult<WeChatInitResponse> WeChatInit(LoginResponse loginResponse);
        IResult<string> GetHeadPhoto(string headPhototUrl);
        IResult<ContactResponse> GetContactList(LoginResponse loginResponse);
        IResult<BatchContactResponse> GetBatchContactList(LoginResponse loginResponse, string[] userNameArray);
        IResult<SyncCheckResponse> SyncCheck(LoginResponse loginResponse, string syncKey);
        IResult<FetchMessageResponse> FetchMessage(LoginResponse loginResponse, SyncKey syncKey);
        IResult<byte[]> FetchImageMessage(string msgId, string type, LoginResponse loginResponse);
        IResult<byte[]> FetchVideoMessage(string msgId, LoginResponse loginResponse);
        IResult<byte[]> FetchAudioMessage(string msgId, LoginResponse loginResponse);
        IResult<byte[]> FetchFileMessage(string sender, string mediaid, string encryfilename, LoginResponse loginResponse);
        IResult<SendMessageResponse> SendMessage(LoginResponse loginResponse, string fromUsername, string message, string toUsername);
    }
}
