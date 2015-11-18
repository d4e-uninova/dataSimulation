using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Activation;
using System.Web;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;

namespace DataSimulation
{
    public class Global : System.Web.HttpApplication
    {
        public static Dictionary<SafeWaitHandle, string> fileNames = new Dictionary<SafeWaitHandle, string>();
        public static Dictionary<SafeWaitHandle, string> addresses = new Dictionary<SafeWaitHandle, string>();

        public static Logger logger;
        public static string serverRoot;

        protected void Application_Start(object sender, EventArgs e)
        {
            RouteTable.Routes.Add(new ServiceRoute("getfiles", new WebServiceHostFactory(), typeof(RestApi)));
            serverRoot = Server.MapPath("~");
            logger = new Logger("DataSimulation", @"C:\inetpub\logs\LogFiles\DataExchangeSolution");
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}