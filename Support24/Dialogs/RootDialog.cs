using System;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Sample.ProactiveBot;
//using Microsoft.Bot.Sample.ProactiveBot;

namespace Support24.Dialogs
{
    [LuisModel(Constants.LUIS_APP_ID, Constants.LUIS_SUBSCRIPTION_ID)]
    [Serializable]
    public class RootDialog : LuisDialog<object>
    {
       [LuisIntent("Greetings")]
       public async Task Greetings(IDialogContext context, LuisResult result)
        {
            string response = null;
            EntityRecommendation rec;
            if (result.TryFindEntity("Wishes", out rec) || result.TryFindEntity("User", out rec)) response = rec.Entity;
            
            if(string.IsNullOrEmpty(response))
            {
                await context.PostAsync($"I didn't understand you");
            }
            else
            {
                //await context.PostAsync($"{response} i can raise an incident token on your behalf");
                PromptDialog.Text(
                    context : context,
                    resume : AfterResumeMessage,
                    prompt : response + " i am design to answer your queries.\n Do you wish to raise an incident token?",
                    retry : "Sorry didn't understnad that. Please try again."
                    );
            }
        }

        private async Task AfterResumeMessage(IDialogContext context, IAwaitable<string> result)
        {
            var argument = await result;
            Activity myActivity = (Activity)context.Activity;
            myActivity.Text = argument.ToString();
            await MessageReceived(context, Awaitable.FromItem(myActivity));
            
        }

        [LuisIntent("Token")]
        public async Task TokenRaiser(IDialogContext context, LuisResult result)
        {
            string tokenResponse = null;
            EntityRecommendation rec;
            if (result.TryFindEntity("TokenConfirmation", out rec)) tokenResponse = rec.Entity;

            if(string.IsNullOrEmpty(tokenResponse))
            {
                await context.PostAsync($"I didn't understand you");
            }
            else if(tokenResponse == "no")
            {
                await context.PostAsync($"Do you wish to rate us?");
            }
            else
            {
                await context.PostAsync("So please answer some question below to find a suitable solution for you");
                var tokenForm = new FormDialog<TokenModel>(new TokenModel(), TokenModel.BuildForm, FormOptions.PromptInStart);
                context.Call(tokenForm, getKeyPhrases);
            }
        }

        private async Task getKeyPhrases(IDialogContext context, IAwaitable<TokenModel> result)
        {
            var sentence = await result;
            string phrasesString = sentence.IssueDescription;
            var phrases = await KeyPhraseAnalytics.ExtractPhraseAsync(phrasesString);
            string phraseResult = String.Join(",", phrases.ToArray());

            await context.PostAsync($"The key phrases extracted are: {phraseResult}");

            /**
             * to call the the QnA maker knowledge base to get the appropraite response for the user queries
             * **/
            try
            {
                var subscriptionKey = ConfigurationManager.AppSettings["QnaSubscriptionkey"];
                var knowledgeBaseId = ConfigurationManager.AppSettings["QnaKnowledgebaseId"];

                var responseQuery = new QnAMakerDialog.QnAMakerDialog().GetQnAMakerResponse(phraseResult, subscriptionKey, knowledgeBaseId);

                var responseAnswers = responseQuery.answers.FirstOrDefault();

                //if-else-if condition for checking the knowledge base
                if(responseAnswers != null && responseAnswers.score >= double.Parse(ConfigurationManager.AppSettings["QnAScore"]))
                {
                    await context.PostAsync(responseAnswers.answer);
                }
                else if(responseAnswers !=null && responseAnswers.score < double.Parse(ConfigurationManager.AppSettings["QnAScore"]))
                {
                    //await context.PostAsync($"Please enter a more detailed description for the problem");
                    PromptDialog.Text(
                        context: context,
                        resume: getQnAResponse,
                        prompt: "Please enter a more detailed description for the problem",
                        retry: "i didn't understand that. Please try again"
                        );
                }
                else
                {
                    await context.PostAsync($"We could not find a solution for your problem. Please raise an incident ticket for this.");
                }

            }
            
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        private async Task getQnAResponse(IDialogContext context, IAwaitable<string> result)
        {
            var response = await result;
            var subscriptionKey = ConfigurationManager.AppSettings["QnaSubscriptionkey"];
            var knowledgeBaseId = ConfigurationManager.AppSettings["QnaKnowledgebaseId"];

            var responseQuery = new QnAMakerDialog.QnAMakerDialog().GetQnAMakerResponse(response, subscriptionKey, knowledgeBaseId);

            var responseAnswers = responseQuery.answers.FirstOrDefault();
            if (responseAnswers != null && responseAnswers.score >= double.Parse(ConfigurationManager.AppSettings["QnAScore"]))
            {
                await context.PostAsync(responseAnswers.answer);
            }
            //getKeyPhrases(responseAnswers)
        }
    }
}