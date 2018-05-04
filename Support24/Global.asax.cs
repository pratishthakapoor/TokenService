﻿using Autofac;
using Microsoft.Bot.Builder.Azure;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Builder.Internals.Fibers;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Configuration;
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

                   /*var store = new TableBotDataStore(ConfigurationManager.AppSettings["AzureWebJobsStorage"]);

                   builder.Register(c => store)
                           .Keyed<IBotDataStore<BotData>>(AzureModule.Key_DataStore)
                           .AsSelf()
                           .SingleInstance();

                   builder.Register(c => new CachingBotDataStore(store, CachingBotDataStoreConsistencyPolicy.ETagBasedConsistency))
                           .As<IBotDataStore<BotData>>()
                           .AsSelf()
                           .InstancePerLifetimeScope();*/

               });
        }
    }
}
