using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Sample.ProactiveBot;
using Support24.Models;
//using Microsoft.Bot.Sample.ProactiveBot;

namespace Support24.Dialogs
{
    [LuisModel(Constants.LUIS_APP_ID, Constants.LUIS_SUBSCRIPTION_ID)]
    [Serializable]
    public class RootDialog : LuisDialog<object>
    {
        string JobId;
        private readonly IDictionary<string, string> UserOptions = new Dictionary<string, string>
        {
            {"1", "Recurring issue with SharePoint and OneDrive" },
            {"2", "Restore ODB deleted files" },
        };

       [LuisIntent("None")]
       [LuisIntent("")]
       public async Task EmptyResponse(IDialogContext context, LuisResult result)
        {
            string emptyResponse = null;
            EntityRecommendation rec;
            if (result.TryFindEntity("Null", out rec)) emptyResponse = rec.Entity;

            if(string.IsNullOrEmpty(emptyResponse))
            {
                await context.PostAsync($"I didn't understand you");
            }
            else
            {
                await context.PostAsync($"Please enter valid response");
            }
        }
       
       [LuisIntent("Request")]
       public async Task UserHelp(IDialogContext context, LuisResult result)
        {
            string request = null;
            EntityRecommendation rec;
            if (result.TryFindEntity("Help", out rec)) request = rec.Entity;

            if(string.IsNullOrEmpty(request))
            {
                await context.PostAsync($"I didn't undestand you");
            }
            else
            {
                await context.PostAsync(Responses.HelpMessage);
            }
        }
       
        [LuisIntent("Response")]
        public async Task UserResponse(IDialogContext context, LuisResult result)
        {
            string userResponse = null;
            EntityRecommendation rec;
            if (result.TryFindEntity("Message", out rec)) userResponse = rec.Entity;

            if(string.IsNullOrEmpty(userResponse))
            {
                await context.PostAsync($"I didn't inderstand you");
            }
            else
            {
                await context.PostAsync($"Want to restart the chat again?");
            }
        }

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
            else if(response == "good night" || response == "bye")
            {
                await context.PostAsync($"{response}");
            }
            else
            {
               await context.PostAsync($"{response}i am design to answer your queries. ");
                PromptDialog.Choice<string>(
                    /*context : context,
                    resume : AfterResumeMessage,
                    prompt : response + " i am design to answer your queries.\n Do you wish to raise an incident token? \n Do you want to restore the deleted files from OneDrive for business",
                    retry : "Sorry didn't understnad that. Please try again."*/
                    context,
                    this.AfterResumeMessage,
                    this.UserOptions.Values,
                    "Do you wish to?",
                    "Oops, some problem occured. Please try again.",
                    2,
                    PromptStyle.Auto,
                    this.UserOptions.Values
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
            else if(tokenResponse == "no" || tokenResponse == "not")
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

        [LuisIntent("Files")]
        public async Task RetrieveFiles(IDialogContext context, LuisResult result)
        {
            string tokenResponse = null;
            EntityRecommendation rec;
            if (result.TryFindEntity("RetrieveConfirmation", out rec)) tokenResponse = rec.Entity;

            if (string.IsNullOrEmpty(tokenResponse))
            {
                await context.PostAsync($"I didn't understand you");
            }
            else if (tokenResponse == "no" || tokenResponse == "not")
            {
               await context.PostAsync($"Do you wish to rate us?");

            }
            else
            {

                //await context.PostAsync("So please answer some question below to find a suitable solution for you");
                await context.PostAsync("So please answer some question below to find a suitable solution for you");
                var fileForm = new FormDialog<DeletedFileModel>(new DeletedFileModel(), DeletedFileModel.BuildForm, FormOptions.PromptInStart);
                //context.Call(fileForm, getDeletedFileDetails);
                context.Call(fileForm, getDeletedFileDetails);
                await context.PostAsync("Fine, connecting to the ODB server");
            }
        }

        private async Task getDeletedFileDetails(IDialogContext context, IAwaitable<DeletedFileModel> result)
        {
            try
            {
                string Uri = "https://s13events.azure-automation.net/webhooks?token=KABhb3NEJCq22z7v0a3%2fMR4rw0P8Qplg61B3mDMrgSk%3d";

                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(Uri);

                //string data = string.Empty;
                string data = "akumar25@agileconsulting.onmicrosoft.com";
                request.Method = "POST";
                request.ContentType = "text/plain;charset=utf-8";
                System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
                byte[] bytes = encoding.GetBytes(data);

                request.ContentLength = bytes.Length;
                using (Stream requestStream = request.GetRequestStream())
                {
                    requestStream.Write(bytes, 0, bytes.Length);
                }

                request.BeginGetResponse((x) =>
                {
                    using (HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(x))
                    {
                        using (Stream stream = response.GetResponseStream())
                        {
                            StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                            String responseString = reader.ReadToEnd();
                            //Console.WriteLine("Script Triggered" + System.DateTime.Now + "\n Job details" + responseString);
                            JobId = responseString;
                        }
                    }
                }, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            if (JobId != null)
            {
                await context.PostAsync("All deleted files are successfully retrieved");
            }
            else
            {
                await context.PostAsync("Please contact our helpdesk for further assistance");
            }
        }

        private async Task getKeyPhrases(IDialogContext context, IAwaitable<TokenModel> result)
        {
            var sentence = await result;
            string phrasesString = sentence.IssueDescription;
            var phrases = await KeyPhraseAnalytics.ExtractPhraseAsync(phrasesString);
            string phraseResult = String.Join(",", phrases.ToArray());

            //await context.PostAsync($"The key phrases extracted are: {phraseResult}");
            await context.PostAsync($"Thanks for sharing the details");
            await context.PostAsync($"Let me check for a appropriate solution for your problem");

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
            else
            {
                await context.PostAsync($"We could not find a solution for your problem. Please raise an incident ticket for this.");
            }
            //getKeyPhrases(responseAnswers)
        }
    }
}