namespace Dotpay.Front.YiSheng
{
    public class EasypayConfig
    {
        private string input_charset = "";
        private string key = "";
        private string notify_url = "";
        // 功能：设置帐户有关信息及返回路径（基础配置页面）
        // 日期：2012-02-14
        // 说明：
        // 以下代码只是为了方便商户测试而提供的样例代码，商户可以根据自己网站的需要，按照技术文档编写,并非一定要使用该代码。
        // 该代码仅供学习和研究易生支付接口使用，只是提供一个参考。

        //定义变量（无需改动）
        private string _partner = "";
        private string _paymentType = "";
        private string _returnUrl = "";
        private string _sellerEmail = "";
        private string _service = "";
        private string _signType = "";
        private string _transport = "";

        public EasypayConfig()
        {
            // 合作身份者ID，由纯数字组成的字符串
            this._partner = "100000000001264";
            //交易安全检验码，由数字和字母组成的32位字符串
            key = "ada466g1d5907cg51aedb2011412f4fd50a73d4ge561d8ga5549ec0107d83457";
            //签约易生支付账号或卖家收款易生支付帐户
            this._sellerEmail = "59098759@qq.com";
            //notify_url 交易过程中服务器通知的页面 要用 http://格式的完整路径，不允许加?id=123这类自定义参数
            notify_url = "http://10.68.76.135/TestEasy/notify_url.aspx";

            //付完款后跳转的页面 要用 http://格式的完整路径，不允许加?id=123这类自定义参数
            this._returnUrl = "http://10.68.76.135/TestEasy/return_url.aspx";

            //字符编码格式 目前支持 gbk 或 utf-8
            input_charset = "utf-8";
            //签名方式 不需修改
            this._signType = "MD5";
            //访问模式,根据自己的服务器是否支持ssl访问，若支持请选择https；若不支持请选择http
            this._transport = "http";
            //接口服务名称，目前固定值：create_direct_pay_by_user（网上支付）和PRECARD_1.0（预付费卡支付）
            this._service = "create_direct_pay_by_user";
            //支付类型，目前固定值：1
            this._paymentType = "1";
        }

        /// 获取或设置合作者身份ID
        public string Partner
        {
            get { return this._partner; }
            set { this._partner = value; }
        }

        //获取或设置交易安全检验码
        public string Key
        {
            get { return key; }
            set { key = value; }
        }

        //获取或设置签约易生账号

        public string SellerEmail
        {
            get { return this._sellerEmail; }
            set { this._sellerEmail = value; }
        }

        //获取或设置付完款后跳转的页面路径

        public string ReturnUrl
        {
            get { return this._returnUrl; }
            set { this._returnUrl = value; }
        }

        //获取或设置服务器异步通知页面路径
        public string NotifyUrl
        {
            get { return notify_url; }
            set { notify_url = value; }
        }

        //获取或设置字符编码格式
        public string InputCharset
        {
            get { return input_charset; }
            set { input_charset = value; }
        }

        //获取或设置签名方式
        public string SignType
        {
            get { return this._signType; }
            set { this._signType = value; }
        }

        //获取或设置访问模式
        public string Transport
        {
            get { return this._transport; }
            set { this._transport = value; }
        }

        //获取或设置支付类型
        public string PaymentType
        {
            get { return this._paymentType; }
            set { this._paymentType = value; }
        }

        //获取或设置接口服务名称
        public string Service
        {
            get { return this._service; }
            set { this._service = value; }
        }
    }
}