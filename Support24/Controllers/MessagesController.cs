using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.UI.WebControls;
using Autofac;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Connector;

namespace Support24
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.Type == ActivityTypes.Message)
            {
               
                await Conversation.SendAsync(activity, () => new Dialogs.RootDialog());
            }
            else
            {
                HandleSystemMessageAsync(activity);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        private async Task<Activity> HandleSystemMessageAsync(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channel
                    IConversationUpdateActivity conversationUpdateActivity = message as IConversationUpdateActivity;
                    if(conversationUpdateActivity != null)
                    {
                        ConnectorClient connector = new ConnectorClient(new System.Uri(message.ServiceUrl));
                        foreach(var member in conversationUpdateActivity.MembersAdded ?? System.Array.Empty<ChannelAccount>())
                        {
                            if(member.Id == conversationUpdateActivity.Recipient.Id)
                            {
                                var reply = ((Activity)conversationUpdateActivity).CreateReply($"Welcome to Support 24/7. Please enter Hi to get started");
                                await connector.Conversations.ReplyToActivityAsync(reply);
                            }
                        }
                    }

                /*IConversationUpdateActivity update = message;
                using (var scope = DialogModule.BeginLifetimeScope(Conversation.Container, message))
                {
                    //var client = scope.Resolve();
                    ConnectorClient connector = new ConnectorClient(new Uri(message.ServiceUrl));
                    if (update.MembersAdded.Any())
                    {
                        var replyNow = message.CreateReply();
                        foreach (var newMember in update.MembersAdded)
                        {
                            if (newMember.Id != message.Recipient.Id)
                            {
                                List<CardImage> cardImages = new List<CardImage>();
                                string strCurrentURL = this.Url.Request.RequestUri.AbsoluteUri.Replace(@"/api/messages", "");
                                string imageURL = String.Format(@"{0}/{1}", strCurrentURL, "Images/icon_Hamilton_1.png");
                                cardImages.Add(new CardImage(url: imageURL));
                                string subtitle = "";
                                subtitle = @"Welcome!";
                                HeroCard plCard = new HeroCard()
                                {
                                    Title = "HHHHH",
                                    Subtitle = subtitle,
                                    Images = cardImages,
                                    Buttons = null
                                };
                                Attachment plAttachment = plCard.ToAttachment();
                                List<Attachment> alist = new List<Attachment>();
                                alist.Add(plAttachment);
                                replyNow.Attachments = alist;
                                connector.Conversations.ReplyToActivityAsync(replyNow);
                            }
                        }
                    }
                }*/

            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }
    }
}