using Autofac;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Builder.Scorables;
using Microsoft.Bot.Connector;
using Support24.Dialogs.ScorableDialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Support24
{
    public class GloabalHandleeMessage : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            /**
             * registering builder for CancelScrolable
             **/

            builder
                .Register(c => new CancelScorable(c.Resolve<IDialogTask>()))
                .As<IScorable<IActivity, double>>()
                .InstancePerLifetimeScope();

            /**
             * Registering builder for RestoredScroable
             **/

            builder
                .Register(c => new RestoredFileScorable(c.Resolve<IDialogTask>()))
                .As<IScorable<IActivity, double>>()
                .InstancePerLifetimeScope();

            /**
             * Registering builder for Sharepoint and one drive scorable
             **/

            builder
                .Register(c => new SharepointandOneDriveScorable(c.Resolve<IDialogTask>()))
                .As<IScorable<IActivity, double>>()
                .InstancePerLifetimeScope();

        }
    }
}