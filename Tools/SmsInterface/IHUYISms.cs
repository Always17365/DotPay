using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using FC.Framework;
using FC.Framework.Log4net;
using FC.Framework.Utilities;
using System.Xml.Linq;
using System.Xml;

namespace DotPay.Tools.SmsInterface
{
    public class IHUYISms : ISms
    {
        private static string _account, _password;
        private static string _smsurl = "http://106.ihuyi.cn/webservice/sms.php?method=";
        private static string namespaceURI = "http://106.ihuyi.cn/";
        private static bool _inited = false;

        public static IHUYISms Initialize(string account, string password)
        {
            Check.Argument.IsNotEmpty(account, "account");
            Check.Argument.IsNotEmpty(password, "password");

            _account = account;
            _password = password;

            _inited = true;

            return new IHUYISms();
        }

        public void Send(string mobile, string content)
        {
            if (_inited)
            {
                Exception taskThrow = null;
                using (HttpClient client = new HttpClient())
                {
                    var url = _smsurl + "Submit&account=" + _account + "&password=" + _password + "&mobile=" + mobile + "&content=" + content;

                    try
                    {
                        var rep = client.GetAsync(url).Result;
                        var xmlmsg = rep.Content.ReadAsStringAsync().Result;
                        XmlDocument dom = new XmlDocument();
                        dom.LoadXml(xmlmsg);
                        XmlNamespaceManager nsmgr = new XmlNamespaceManager(dom.NameTable);
                        nsmgr.AddNamespace("ns", namespaceURI);
                        var code = Convert.ToInt32(dom.SelectSingleNode("//ns:code", nsmgr).InnerText);
                        var msg = dom.SelectSingleNode("//ns:msg", nsmgr).InnerText;
                        var smsid = dom.SelectSingleNode("//ns:smsid", nsmgr).InnerText;

                        if (code != 2) taskThrow = new SmsInterfaceException(code, msg);
                    }
                    catch (Exception ex)
                    {
                        Log.Error("发送短信验证码时出现异常", ex);
                        taskThrow = ex;
                    }


                    if (taskThrow != null) throw taskThrow;
                }
            }
        }

        public int Balance()
        {
            if (_inited)
            {
                var url = _smsurl + "GetNum&account=" + _account + "&password=" + _password;
                var balance = 0;
                Exception taskThrow = null;

                using (HttpClient client = new HttpClient())
                {
                    try
                    {
                        var rep = client.GetAsync(url).Result;
                        var xmlmsg = rep.Content.ReadAsStringAsync().Result;

                        XmlDocument dom = new XmlDocument();
                        dom.LoadXml(xmlmsg);
                        XmlNamespaceManager nsmgr = new XmlNamespaceManager(dom.NameTable);
                        nsmgr.AddNamespace("ns", namespaceURI);
                        var code = Convert.ToInt32(dom.SelectSingleNode("//ns:code", nsmgr).InnerText);
                        var msg = dom.SelectSingleNode("//ns:msg", nsmgr).InnerText;
                        var num = Convert.ToInt32(dom.SelectSingleNode("//ns:num", nsmgr).InnerText);
                        if (code == 2)
                        {
                            balance = num;
                        }
                        else
                        {
                            taskThrow = new SmsInterfaceException(code, msg);
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Warn("调用查看短信平台余额接口出现错误", ex);
                        taskThrow = new SmsInterfaceException(-1, ex.Message);
                    }

                    if (taskThrow != null) throw taskThrow;
                    else return balance;
                }
            }

            return 1;
        }
    }
}
