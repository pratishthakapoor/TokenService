using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.UI.WebControls;
using AdaptiveCards;
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
                                Activity reply = ((Activity)conversationUpdateActivity).CreateReply($"Support 24/7");
                                //await connector.Conversations.ReplyToActivityAsync(reply);
                                AdaptiveCard card = new AdaptiveCard();

                            //Add image to the card
                            card.Body.Add(new AdaptiveCards.Image()
                            {
                                Url = "https://ansiblergdiag813.blob.core.windows.net/chat-bot-images/support_bot_icon.png",
                                Size = ImageSize.Auto,
                                Style = ImageStyle.Normal,
                                HorizontalAlignment = HorizontalAlignment.Center
                            });

                            // Add text to the card
                            card.Body.Add(new TextBlock()
                            {
                                Text = "Welcome to Support 24/7",
                                Size = TextSize.Large,
                                Weight = TextWeight.Bolder
                            });

                            // Add text to the card
                            card.Body.Add(new TextBlock()
                            {
                                Text = "Please enter Hi to get started",
                                Size = TextSize.Large,
                                Weight = TextWeight.Bolder
                            });

                            //Create attachment
                            Attachment attachment = new Attachment()
                            {
                                ContentType = AdaptiveCard.ContentType,
                                Content = card
                            };
                            reply.Attachments.Add(attachment);

                            var replytoconverstaion = await connector.Conversations.SendToConversationAsync(reply);
                        }
                        }
                    }
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