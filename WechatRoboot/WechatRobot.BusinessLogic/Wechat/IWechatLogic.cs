using Dijing.Common.Core.DataStruct;
using System;
using System.Collections.Generic;
using System.Text;
using WechatRobot.Model.Search;
using WechatRobot.SDK.DTO;

namespace WechatRobot.BusinessLogic.Wechat
{
    public interface IWechatLogic
    {
        IResult<List<ContactUser>> GetContactList(GetContactsSearch search);
    }
}
