using Microsoft.Bot.Builder.Dialogs;
using Support24.SnowLogger;
using System;
using System.Threading.Tasks;

namespace Support24.Dialogs
{
    internal class DetailFormDialog : IDialog<object>
    {
        private string phrasesString;

        string user_category;
        int no_of_user_impacted;
        string impact_number;

        public DetailFormDialog(string phrasesString)
        {
            this.phrasesString = phrasesString;
        }

        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync("So please answer some question below to a raise an icident ticket for you");

            PromptDialog.Text(
                context: context,
                resume: getCategoryDetail,
                prompt: "Please select a category(Inquiry / Help, Software, Hadware, Network, Database, None)",
                retry: "Oops, I didn't get that. Please try again."
                );
        }

        private async Task getCategoryDetail(IDialogContext context, IAwaitable<string> category)
        {
            var response = await category;

            user_category = response;

            string shortDescription = phrasesString;

            /**
             * Connection string for SnowIncident ticket creation.
             **/

            String incidentNo = string.Empty;

            incidentNo = Logger.CreateIncidentServiceNow(shortDescription, user_category);

            Console.WriteLine(incidentNo);
            await context.PostAsync("Your ticket has been raised successfully, " + incidentNo + " your token id for the raised ticket");
            await context.PostAsync("Please keep the note of above token number. as it would be used for future references");
            context.Done(this);


        }

        /*public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync("So please answer some question below to a raise an icident ticket for you");

            PromptDialog.Text(
                context,
                resume: getServerDetails,
                prompt: "Which servers do you use? Please specify None if not used by you",
                retry: "Oops, I didn't get that. Please try again"
                );
        }

        private async Task getServerDetails(IDialogContext context, IAwaitable<string> ServerDetails)
        {
            var response = await ServerDetails;

            PromptDialog.Text(
                context,
                resume: getMiddlewareDetails,
                prompt: "Which middleware service do you use? Please specify None if not used by you",
                retry: "Oops, I didn't get that. Please try again"
                );
        }

        private async Task getMiddlewareDetails(IDialogContext context, IAwaitable<string> MiddlewareDetails)
        {
            var response = await MiddlewareDetails;

            PromptDialog.Text(
                context,
                resume: getDatabaseDetails,
                prompt: "Which databases are used by you? Please specify None if not used by you",
                retry: "Oops, I didn't get that. Please try again"
                );
        }

        private async Task getDatabaseDetails(IDialogContext context, IAwaitable<string> DatabaseDetails)
        {
            var response = await DatabaseDetails;
            PromptDialog.Text(
               context,
               resume: getCategory,
               prompt: "Please select a category(Inquiry/ Help, Software, Hadware, Network, Database)",
               retry: "Oops, I didn't get that. Please try again"
               );
        }

        private async Task getCategory(IDialogContext context, IAwaitable<string> category)
        {
        }*/
        }
}