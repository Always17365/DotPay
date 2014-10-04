using DotPay.Common;
using System.Web;
using System.Web.Optimization;

namespace DotPay.Web.Admin
{
    public class BundleConfig
    {
        // 有关 Bundling 的详细信息，请访问 http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery", "//lib.sinaapp.com/js/jquery/1.10.2/jquery-1.10.2.min.js").Include(
                        "~/Scripts/libs/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/angular", "//cdnjscn.b0.upaiyun.com/libs/angular.js/1.3.0-beta.3/angular.min.js")
               .Include("~/Scripts/libs/angular.js"));
            bundles.Add(new ScriptBundle("~/bundles/angular-route", "//cdnjscn.b0.upaiyun.com/libs/angular.js/1.3.0-beta.3/angular-route.min.js")
           .Include("~/Scripts/libs/angular-route.js"));
            bundles.Add(new ScriptBundle("~/bundles/angular-animate", "//cdnjscn.b0.upaiyun.com/libs/angular.js/1.3.0-beta.3/angular-animate.min.js")
                .Include("~/Scripts/libs/angular-animate.js"));

            bundles.Add(new StyleBundle("~/Content/authcss").Include("~/Content/auth.css"));
            bundles.Add(new StyleBundle("~/Content/css").Include("~/Content/site.css"));
            bundles.Add(new StyleBundle("~/Content/bootstrapcss", "//cdnjscn.b0.upaiyun.com/libs/twitter-bootstrap/3.1.1/css/bootstrap.min.css")
                         .Include("~/Content/css/bootstrap.css"));
            bundles.Add(new StyleBundle("~/Content/ueditor").Include("~/Content/ueditor/themes/iframe.css"));

            //bundles.Add(new ScriptBundle("~/bundles/angular-ui-bootstrap", @"//cdnjscn.b0.upaiyun.com/libs/angular-ui-bootstrap/0.10.0/ui-bootstrap.min.js")
            //    .Include("~/Scripts/libs/ui-bootstrap-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/angular-ui-bootstrap", @"//cdnjscn.b0.upaiyun.com/libs/angular-ui-bootstrap/0.10.0/ui-bootstrap-tpls.min.js")
                .Include("~/Scripts/libs/ui-bootstrap-tpls-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/angular-alerts")
                       .Include("~/Scripts/libs/angular-alertbox.js"));
            bundles.Add(new ScriptBundle("~/bundles/authctrl")
                     .Include("~/Scripts/controller/authcontroller.js"));

            bundles.Add(new ScriptBundle("~/bundles/adminctrl")
                       .Include("~/Scripts/app.js",
                                "~/Scripts/controller/controller.js",
                                "~/Scripts/controller/usercontroller.js",
                                "~/Scripts/controller/currencycontroller.js",
                                "~/Scripts/controller/withdrawcontroller.js",
                                "~/Scripts/controller/capitalAccountcontroller.js",
                                "~/Scripts/controller/depositcontroller.js",
                                "~/Scripts/controller/fundSourcecontroller.js",
                                "~/Scripts/controller/smsInterfacecontroller.js",
                                "~/Scripts/controller/marketcontroller.js",
                                "~/Scripts/controller/logscontroller.js",
                                "~/Scripts/controller/depositcodecontroller.js",
                                "~/Scripts/controller/depthmarketController.js",
                                "~/Scripts/controller/noticecontroller.js",
                                "~/Scripts/controller/vipsettingcontroller.js",
                                "~/Scripts/controller/checkpendingcontroller.js",
                                "~/Scripts/controller/bankoutletscontroller.js",
                                "~/Scripts/controller/markettransactionrecordscontroller.js",
                                "~/Scripts/controller/virtualcoinstatiscontroller.js",
                                "~/Scripts/controller/articlecontroller.js"));

            bundles.Add(new ScriptBundle("~/bundles/ueditorctrl")
           .Include("~/Content/ueditor/ueditor.config.js",
                    "~/Content/ueditor/ueditor.all.js",
                    "~/Content/ueditor/ueditor.config.js",
                    "~/Content/ueditor/lang/zh-cn/zh-cn.js",
                    "~/Content/ueditor/ueditor.parse.js"));

            bundles.Add(new ScriptBundle("~/bundles/timejsctrl")
            .Include("~/Content/My97DatePicker/WdatePicker.js"));

            BundleTable.EnableOptimizations = false;
            bundles.UseCdn = false;
        }
    }
}