using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace Dotpay.Front.YiSheng
{
    /* 
     * 功能：易生接口公用函数类
     * 详细：该类是请求、通知返回两个文件所调用的公用函数核心处理文件，不需要修改
     * 修改日期：2012-02-14
     * 说明：
     * 以下代码只是为了方便商户测试而提供的样例代码，商户可以根据自己网站的需要，按照技术文档编写,并非一定要使用该代码。
     * 该代码仅供学习和研究易生支付接口使用，只是提供一个参考 
     */
    public class EasypayFunction
    {
        //生成签名结果
        public static string BuildMysign(Dictionary<string, string> dicArray, string key, string signType, string inputCharset)
        {
            string prestr = CreateLinkstring(dicArray);  //把数组所有元素，按照“参数=参数值”的模式用“&”字符拼接成字符串
            //去掉最後一個&字符
            int nLen = prestr.Length;
            prestr = prestr.Substring(0, nLen - 1);

            prestr = prestr + key;//把拼接后的字符串再与安全校验码直接连接起来
            string mysign = Sign(prestr, signType, inputCharset);    //把最终的字符串签名，获得签名结果

            return mysign;

        }

        // 把数组所有元素，按照“参数=参数值”的模式用“&”字符拼接成字符串
        public static string CreateLinkstring(Dictionary<string, string> dicArray)
        {
            StringBuilder prestr = new StringBuilder();
            foreach (KeyValuePair<string, string> temp in dicArray)
            {
                prestr.Append(temp.Key + "=" + temp.Value + "&");
            }

            return prestr.ToString();
        }

        // 除去数组中的空值和签名参数并以字母a到z的顺序排序
        public static Dictionary<string, string> ParaFilter(SortedDictionary<string, string> dicArrayPre)
        {
            Dictionary<string, string> dicArray = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> temp in dicArrayPre)
            {
                if (temp.Key.ToLower() != "sign" && temp.Key.ToLower() != "sign_type" && temp.Value != "")
                {
                    dicArray.Add(temp.Key.ToLower(), temp.Value);
                }
            }

            return dicArray;
        }

        //签名字符串,prestr--需要签名的字符串,sign_type--签名类型，_input_charset--编码格式
        private static string Sign(string prestr, string signType, string inputCharset)
        {
            StringBuilder sb = new StringBuilder(32);
            if (signType.ToUpper() == "MD5")
            {
                MD5 md5 = new MD5CryptoServiceProvider();
                byte[] t = md5.ComputeHash(Encoding.GetEncoding(inputCharset).GetBytes(prestr));
                for (int i = 0; i < t.Length; i++)
                {
                    sb.Append(t[i].ToString("x").PadLeft(2, '0'));
                }
            }
            return sb.ToString();
        }
    }
}