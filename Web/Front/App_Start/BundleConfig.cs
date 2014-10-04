using DotPay.Common;
using System.Web;
using System.Web.Optimization;

namespace DotPay.Web
{
    public class BundleConfig
    {
        // 有关 Bundling 的详细信息，请访问 http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            var cdn = Config.CDN;
            var jquery = new ScriptBundle("~/bundles/jquery",  cdn + "/Scripts/jquery-1.11.1.min.js").Include("~/Scripts/jquery-1.11.1.min.js");
            jquery.CdnFallbackExpression = "window.jQuery";
            var ko = new ScriptBundle("~/bundles/knockout", cdn + "/Scripts/knockout-3.1.0.js").Include("~/Scripts/knockout-3.1.0.js");
            ko.CdnFallbackExpression = "window.ko";
            var komapping = new ScriptBundle("~/bundles/knockoutmap", cdn + "/Scripts/knockout.mapping-latest.js").Include("~/Scripts/knockout.mapping-latest.js");
            komapping.CdnFallbackExpression = "window.ko.mapping";
            var highcharts = new ScriptBundle("~/bundles/highstock", cdn + "/Scripts/Highstock/highstock.js").Include("~/Scripts/Highstock/highstock.js");
            highcharts.CdnFallbackExpression = "window.Highcharts";
            var mCustomScrollbar = new ScriptBundle("~/bundles/mCustomScrollbar", cdn + "/Scripts/jquery.mCustomScrollbar.min.js").Include("~/Scripts/jquery.mCustomScrollbar.min.js");
            mCustomScrollbar.CdnFallbackExpression = "$.mCustomScrollbar";
            bundles.Add(jquery);
            bundles.Add(new ScriptBundle("~/bundles/jqueryval", cdn + "/Scripts/jquery.unobtrusive-ajax.min.js").Include("~/Scripts/jquery.unobtrusive-ajax.min.js"));
            bundles.Add(ko);
            bundles.Add(komapping);
            bundles.Add(new ScriptBundle("~/bundles/fullcoin", cdn + "/Scripts/common.js").Include("~/Scripts/common.js"));
            bundles.Add(mCustomScrollbar);
            bundles.Add(new ScriptBundle("~/bundles/trade", cdn + "/Scripts/trade.js").Include("~/Scripts/trade.js"));
            bundles.Add(new ScriptBundle("~/bundles/site").Include("~/Scripts/jquery.Slide.js", "~/Scripts/Selectyze.jquery.min.js", "~/Scripts/style.js"));
            bundles.Add(new ScriptBundle("~/bundles/mCustomScrollbar", cdn + "/Scripts/jquery.mCustomScrollbar.min.js").Include("~/Scripts/jquery.mCustomScrollbar.min.js"));
            bundles.Add(new ScriptBundle("~/bundles/noty", cdn + "/Scripts/jquery.noty.packaged.min.js").Include("~/Scripts/jquery.noty.packaged.min.js"));
            bundles.Add(highcharts);

            bundles.Add(new StyleBundle("~/Content/bootstrapcss").Include("~/Content/bootstrap.css"));
            bundles.Add(new StyleBundle("~/Content/css", cdn + "/Content/css.css").Include("~/Content/css.css"));
            bundles.Add(new StyleBundle("~/Content/indexcss", cdn + "/Content/index.css").Include("~/Content/index.css"));
            bundles.Add(new StyleBundle("~/Content/scrollcss", cdn + "/Content/jquery.mCustomScrollbar.css").Include("~/Content/jquery.mCustomScrollbar.css"));


            if (!Config.Debug)
            {
                BundleTable.EnableOptimizations = true;
                if (!string.IsNullOrEmpty(Config.CDN)) bundles.UseCdn = true;
            }
        }
    }
}