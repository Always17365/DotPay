using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Security.Cryptography;
using Dotpay.Front.YiSheng;

namespace Dotpay.Front.YiSheng
{
    public class EasypayService
    {
        private const string GATEWAY = "http://cashier.bhecard.com/portal?"; //网关地址
        private string _key = "";                    //交易安全校验码
        private string _inputCharset = "";           //编码格式
        private string _signType = "";               //签名方式
        private string _mysign = "";                 //签名结果
        private Dictionary<string, string> _sPara = new Dictionary<string, string>();//要签名的字符串

       
        public EasypayService()
        { 
        }
        public EasypayService(string partner, string sellerEmail, string returnUrl, string notifyUrl, string outTradeNo,
                               string subject, string body, string totalFee, string paymethod, string defaultbank,
                               string buyerEmail, string key, string inputCharset, string signType)
        {
           
            _key = key.Trim();
            this._inputCharset = inputCharset.ToLower();
            this._signType = signType.ToUpper();
            SortedDictionary<string, string> sParaTemp = new SortedDictionary<string, string>
            {
                {"service", "create_direct_pay_by_user"},
                {"payment_type", "1"},
                {"partner", partner},
                {"seller_email", sellerEmail},
                {"return_url", returnUrl},
                {"notify_url", notifyUrl},
                {"_input_charset", this._inputCharset},
                {"out_trade_no", outTradeNo},
                {"subject", subject},
                {"body", body},
                {"total_fee", totalFee},
                {"paymethod", paymethod},
                {"defaultbank", defaultbank},
                {"buyer_email", buyerEmail}
            };

            //构造签名参数数组
            this._sPara = EasypayFunction.ParaFilter(sParaTemp);
            //获得签名结果
            this._mysign = EasypayFunction.BuildMysign(this._sPara, _key, this._signType, this._inputCharset);

           
        }


        //计算查询签名
        public string Easypay_query(string partner, string out_trade_no,string key, string input_charset, string sign_type)
        {
            _key = key.Trim();
            this._inputCharset = input_charset.ToLower();
            this._signType = sign_type.ToUpper();
            SortedDictionary<string, string> sParaTemp = new SortedDictionary<string, string>();

            //构造签名参数数组
           
            sParaTemp.Add("partner", partner);
            sParaTemp.Add("_input_charset", this._inputCharset);
            sParaTemp.Add("out_trade_no", out_trade_no);
        

            this._sPara = EasypayFunction.ParaFilter(sParaTemp);
            //获得签名结果
            this._mysign = EasypayFunction.BuildMysign(this._sPara, _key, this._signType, this._inputCharset);

            return this._mysign;
        }



        //计算退款签名
        public string EasypayRefund(string partner, string origOutTradeNo, string outTradeNo, string subject, string amount, string key, string inputCharset, string signType)
        {
            _key = key.Trim();
            this._inputCharset = inputCharset.ToLower();
            this._signType = signType.ToUpper();
            var sParaTemp = new SortedDictionary<string, string>
            {
                {"partner", partner},
                {"_input_charset", this._inputCharset},
                {"out_trade_no", outTradeNo},
                {"orig_out_trade_no", origOutTradeNo},
                {"amount", amount},
                {"subject", subject}
            };


            this._sPara = EasypayFunction.ParaFilter(sParaTemp);
            this._mysign = EasypayFunction.BuildMysign(this._sPara, _key, this._signType, this._inputCharset);

            return this._mysign;
        }



        /// <summary>
        /// 构造表单提交HTML
        /// </summary>
        /// <returns>输出 表单提交HTML文本</returns>
        public string BuildForm()
        {
            StringBuilder sbHtml = new StringBuilder();

            //GET方式传递
            sbHtml.Append("<form id=\"easypaysubmit\" name=\"easypaysubmit\" action=\"" + GATEWAY + "_input_charset=" + this._inputCharset + "\" method=\"get\">");

            //POST方式传递（GET与POST二必选一）
            //sbHtml.Append("<form id=\"easypaysubmit\" name=\"easypaysubmit\" action=\"" + gateway + "_input_charset=" + _input_charset + "\" method=\"post\">");

            foreach (KeyValuePair<string, string> temp in this._sPara)
            {
                sbHtml.Append("<input type=\"hidden\" name=\"" + temp.Key + "\" value=\"" + temp.Value + "\"/>");
            }

            sbHtml.Append("<input type=\"hidden\" name=\"sign\" value=\"" + this._mysign + "\"/>");
            sbHtml.Append("<input type=\"hidden\" name=\"sign_type\" value=\"" + this._signType + "\"/>");

            //submit按钮控件请不要含有name属性
            sbHtml.Append("<input type=\"submit\" value=\"易生支付确认付款\"></form>");

            sbHtml.Append("<script>document.forms['easypaysubmit'].submit();</script>");

            return sbHtml.ToString();
        }
    }
}