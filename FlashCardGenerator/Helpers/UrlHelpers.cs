using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace FlashCardGenerator.Helpers
{
    public static class UrlHelpers
    {
        public static dynamic Javascript(this UrlHelper helper, string path)
        {
            return helper.Content("~/Content/Scripts/" + path);
        }

        public static dynamic Css(this UrlHelper helper, string path)
        {
            return helper.Content("~/Content/" + path);
        }

        public static dynamic Image(this UrlHelper helper, string path)
        {
            return helper.Content("~/Content/Images/" + path);
        }
    }
}
