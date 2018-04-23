using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.Internals.Fibers;
using Microsoft.Bot.Builder.Scorables.Internals;
using Microsoft.Bot.Connector;
using Support24.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Support24.Dialogs.ScorableDialogs
{
    public class SharepointandOneDriveScorable : ScorableBase<IActivity, string, double>
    {
        private readonly IDialogTask task;

        public SharepointandOneDriveScorable(IDialogTask task)
        {
            SetField.NotNull(out this.task, nameof(task), task);
        }

        protected override Task DoneAsync(IActivity item, string state, CancellationToken token)
        {
            return Task.CompletedTask;
        }

        protected override double GetScore(IActivity item, string state)
        {
            return 1.0;
        }

        protected override bool HasScore(IActivity item, string state)
        {
            return state != null;
        }

        protected override async Task PostAsync(IActivity item, string state, CancellationToken token)
        {
            var message = item as IMessageActivity;

            if (message != null)
            {

                var ticketForm = new FormDialog<TokenModel>(new TokenModel(), TokenModel.BuildForm, FormOptions.PromptInStart);

                var interruption = ticketForm.Void<object, IMessageActivity>();

                task.Call(interruption, null);

                await task.PollAsync(token);
            }
        }

        protected override async Task<string> PrepareAsync(IActivity item, CancellationToken token)
        {
            var message = item as IMessageActivity;
            if (message != null && !string.IsNullOrWhiteSpace(message.Text))
            {
                if (message.Text.Equals("Issue with SharePoint and OneDrive", StringComparison.InvariantCultureIgnoreCase) ||
                    message.Text.Equals("Issue with Sharepoint", StringComparison.InvariantCultureIgnoreCase) ||
                    message.Text.Equals("Issue with one drive", StringComparison.InvariantCultureIgnoreCase) || message.Text.Equals("restore files", StringComparison.InvariantCultureIgnoreCase))
                {
                    return message.Text;
                }
            }
            return null;
        }
    }
}