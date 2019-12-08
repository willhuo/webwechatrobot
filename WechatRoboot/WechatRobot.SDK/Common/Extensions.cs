using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Dijing.Weixin.Common
{
    public static class Extensions
    {
        public static string UrlDecode(this string text)
        {
            return WebUtility.UrlDecode(text);
        }
    }
}
