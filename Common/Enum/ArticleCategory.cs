using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FC.Framework;

namespace DotPay.Common
{
    /// <summary>
    /// article category
    /// </summary>
    public enum ArticleCategory
    {
        [EnumDescription("新闻资讯")]
        Common = 1,
        [EnumDescription("业界动态")]
        Industry = 2,
        [EnumDescription("招贤纳士")]
        Recruitment = 3,
        [EnumDescription("关于我们")]
        AboutUs = 4,
        [EnumDescription("联系我们")]
        Contact = 5
    }
}
