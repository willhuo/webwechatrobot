using Dijing.Common.Core.DataStruct;
using System;
using System.Collections.Generic;
using System.Text;
using WechatRobot.SDK.DTO;

namespace WechatRobot.SDK.Infrastructure
{
    public interface IWeChatContactClient
    {
        /*attr*/
        List<ContactUser> ContactList { get; }
        List<ContactUser> BatchContactList { get; }
        List<ContactUser> AllMemberList { get; }


        /*public method*/
        IResult<ContactResponse> GetContactList(LoginResponse loginResponse);
        IResult<BatchContactResponse> GetBatchContacts(LoginResponse loginResponse, string[] userNameArray);
    }
}
