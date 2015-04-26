using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Routing; 
using System.Web.SessionState;
using System.Web.Http;
using DFramework;
using DFramework.Autofac;
using DFramework.Log4net;
using DFramework.Memcached;
using Dotpay.Common;
using Dotpay.Front.ViewModel;
using Dotpay.FrontQueryServiceImpl;
using Newtonsoft.Json;
using Orleans;

namespace Dotpay.Front
{
    public class Global : HttpApplication
    {
        void Application_BeginRequest()
        {
            string lang = "zh-cn";

            if (null != Request.Cookies["lanaguage"])
            {
                lang = Request.Cookies["lanaguage"].Value;
            }
            else
            {
                if (null != Request.UserLanguages)
                {
                    Response.Cookies.Add(new HttpCookie("lanaguage", Request.UserLanguages[0]));
                    lang = Request.UserLanguages[0];
                }
            }

            if (!lang.Equals(Thread.CurrentThread.CurrentCulture.Name, StringComparison.OrdinalIgnoreCase))
                SetCurrentLanaguage(lang);
        }
     
        void Application_Start(object sender, EventArgs e)
        { 
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            var assemblies = new List<Assembly>();

            var basePath = AppDomain.CurrentDomain.BaseDirectory;

            Directory.GetFiles(basePath, "*.dll", SearchOption.AllDirectories)
                .Where(f => f.IndexOf("Dotpay.CommandExecutor.dll", 0, StringComparison.OrdinalIgnoreCase) > -1)
                .ForEach(fp => assemblies.Add(Assembly.LoadFrom(fp)));


            DEnvironment.Initialize()
                        .UseAutofac()
                        .UseLog4net()
                        .UseMemcached("192.168.0.100")
                        .UseDefaultCommandBus(assemblies.ToArray())
                        .RegisterQueryService(DotpayConfig.DBConnectionString, DotpayConfig.DatabaseName);


            IoC.Register<IJsonSerializer, DotpayJsonSerializer>(LifeStyle.Singleton);

            GrainClient.Initialize(Path.Combine(basePath, "ClientConfiguration.xml"));
        }

        void Application_PreRequestHandlerExecute(Object sender, EventArgs e)
        {
            if (Context.Handler is IRequiresSessionState || Context.Handler is IReadOnlySessionState)
            {
                var sessionState = ConfigurationManager.GetSection("system.web/sessionState") as SessionStateSection;

                var cookieName = sessionState != null && !string.IsNullOrEmpty(sessionState.CookieName)
                  ? sessionState.CookieName
                  : "ASP.NET_SessionId";

                var timeout = sessionState != null ? sessionState.Timeout : TimeSpan.FromMinutes(20);

                if (this.Request.Cookies[cookieName] != null && Session.SessionID != null)
                {
                    Response.Cookies[cookieName].Value = Session.SessionID;
                    Response.Cookies[cookieName].Path = Request.ApplicationPath;
                    if (!DotpayConfig.Debug)
                        Response.Cookies[cookieName].Domain = DotpayConfig.Domain;
                }
            }
        }

        #region 
        private void SetCurrentLanaguage(string lang)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(lang);
            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;
        }
        #endregion


    }

    public class DotpayJsonSerializer : IJsonSerializer
    {
        JsonSerializerSettings settings;

        public DotpayJsonSerializer()
        {
            settings = new JsonSerializerSettings();
            settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            settings.Converters.Add(new UnixDateTimeConverter());
        }
        public string Serialize(object obj)
        {
            string jsonData = JsonConvert.SerializeObject(obj, Formatting.Indented, settings);

            return jsonData;
        }

        public object Deserialize(string value, Type type)
        {
            object obj = JsonConvert.DeserializeObject(value, type, settings);

            return obj;
        }

        public T Deserialize<T>(string value) where T : class
        {
            T obj = null;

            try
            {
                obj = (T)this.Deserialize(value, typeof(T));
                return obj;
            }
            catch
            {
                return null;
            }

        }
    }
}