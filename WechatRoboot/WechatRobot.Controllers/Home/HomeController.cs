using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using WechatRobot.Controllers.Base;

namespace WechatRobot.Controllers.Home
{
    public class HomeController:BaseController
    {
        #region VIEW
        public IActionResult Index()
        {
            return View();
        }
        #endregion

        #region AJAX
        #endregion
    }
}
