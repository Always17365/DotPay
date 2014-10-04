using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace DotPay.Web.Admin
{
    public static class EnumExtensions
    {
        public static IHtmlString SelectForEnum(this HtmlHelper html, Type enumType, object htmlAttributes, params Enum[] filter)
        {
            var enumValueList = new Dictionary<string, string>();

            foreach (Enum crtEnum in Enum.GetValues(enumType))
            {
                //如果过滤器中包含该枚举值，则跳过此次循环
                if (filter != null && filter.Contains(crtEnum)) continue;

                enumValueList.Add(crtEnum.ToString("D"), crtEnum.ToString());

            }

            return html.DropDownList(enumType.Name, new SelectList(enumValueList, "Key", "Value"), htmlAttributes);
        }

        public static IHtmlString SelectForEnum(this HtmlHelper html, Type enumType, string optionLabel, object htmlAttributes, params Enum[] filter)
        {
            var enumValueList = new Dictionary<string, string>();

            foreach (Enum crtEnum in Enum.GetValues(enumType))
            {
                //如果过滤器中包含该枚举值，则跳过此次循环
                if (filter != null && filter.Contains(crtEnum)) continue;

                enumValueList.Add(crtEnum.ToString("D"), crtEnum.ToString());

            }

            return html.DropDownList(enumType.Name, new SelectList(enumValueList, "Key", "Value"), optionLabel, htmlAttributes);
        }

        public static IHtmlString SelectForEnum(this HtmlHelper html, Type enumType, IDictionary<string, object> htmlAttributes, params Enum[] filter)
        {
            var enumValueList = new Dictionary<string, string>();

            foreach (Enum crtEnum in Enum.GetValues(enumType))
            {
                //如果过滤器中包含该枚举值，则跳过此次循环
                if (filter != null && filter.Contains(crtEnum)) continue;

                enumValueList.Add(crtEnum.ToString("D"), crtEnum.ToString());

            }

            return html.DropDownList(enumType.Name, new SelectList(enumValueList, "Key", "Value"), htmlAttributes);
        }

        public static IHtmlString SelectForEnum(this HtmlHelper html, Type enumType, string optionLabel, IDictionary<string, object> htmlAttributes, params Enum[] filter)
        {
            var enumValueList = new Dictionary<string, string>();

            foreach (Enum crtEnum in Enum.GetValues(enumType))
            {
                //如果过滤器中包含该枚举值，则跳过此次循环
                if (filter != null && filter.Contains(crtEnum)) continue;

                enumValueList.Add(crtEnum.ToString("D"), crtEnum.ToString());

            }

            return html.DropDownList(enumType.Name, new SelectList(enumValueList, "Key", "Value"), optionLabel, htmlAttributes);
        }
    }
}