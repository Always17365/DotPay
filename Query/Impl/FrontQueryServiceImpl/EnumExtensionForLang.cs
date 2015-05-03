using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dotpay.Front.ViewModel;
using Dotpay.FrontQueryService;
using MongoDB.Bson;
using MongoDB.Driver;
using DFramework;

namespace Dotpay.FrontQueryServiceImpl
{
    public static class EnumExtension
    {
        public static string ToLangString(this Enum @enum)
        {
            return @enum.GetType().Name + @enum.ToString("F");
        }
    }

}
