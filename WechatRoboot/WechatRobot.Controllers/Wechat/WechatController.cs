using Dijing.Common.Core.DataStruct;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WechatRobot.BusinessLogic.Wechat;
using WechatRobot.Controllers.Base;
using WechatRobot.Model.Search;
using WechatRobot.SDK.Infrastructure;

namespace WechatRobot.Controllers.Wechat
{
    public class WechatController:BaseController
    {
        #region Contructor
        public WechatController(IWeChatEngine weChatEngine,IWechatLogic wechatLogic)
        {
            this._WeChatEngine = weChatEngine;
            this._WechatLogic = wechatLogic;
        }
        private IWeChatEngine _WeChatEngine { get; set; }
        private IWechatLogic _WechatLogic { get; set; }
        #endregion

        #region VIEW
        /*微信登录界面*/
        [HttpGet]
        public IActionResult Index()
        {
            if (WechatRobot.SDK.DataStruct.SystemInfo.IsLogin)
            {
                ViewBag.IsLogin = true;
                return View(_WeChatEngine.User);
            }
            else
            {
                ViewBag.IsLogin = false;
                var result = _WeChatEngine.Login();
                if (result.Success)
                {
                    ViewBag.QRCodeFlag = true;
                    ViewBag.QRCodeBase64 = result.GetData();

                    Task.Run(() => _WeChatEngine.WaitForLogin());
                }
                else
                {
                    ViewBag.QRCodeFlag = false;
                    ViewBag.ErrorMsg = result.Desc;
                }
                return View();
            }
        }

        /*微信好友列表界面*/
        [HttpGet]
        public IActionResult ContactList()
        {
            return View();
        }
        #endregion

        #region AJAX
        [HttpGet]
        public JsonResult GetLoginFlag()
        {
            var result = new Result<bool>();
            result.SetSuccess();
            result.SetData(WechatRobot.SDK.DataStruct.SystemInfo.IsLogin);
            return Json(result);
        }

        [HttpPost]
        public JsonResult GetContactList(GetContactsSearch search)
        {
            var result = _WechatLogic.GetContactList(search);
            return Json(result);
        }
        #endregion
    }
}
