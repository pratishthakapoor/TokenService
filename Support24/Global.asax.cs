using Autofac;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Internals.Fibers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;

namespace Support24
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);

            this.RegisterBotModules();

        }

        private void RegisterBotModules()
        {
            Conversation.UpdateContainer(
               builder =>
               {
                   builder.RegisterModule(new ReflectionSurrogateModule());
                   builder.RegisterModule<GloabalHandleeMessage>();
               });
        }
    }
}
