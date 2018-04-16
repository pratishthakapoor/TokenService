using AdaptiveCards;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Support24.Dialogs
{
    [Serializable]
    public class FeedbackFormDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync("Please fill the form below: ");

            var replyMessage = context.MakeMessage();
            //Attachment attachment = null;
            replyMessage.Attachments = new List<Attachment> { CreateFeedBackForm()};

            context.Wait(this.MessageRecieved);

            await context.PostAsync(replyMessage);

            //context.Done(this);
        }

        private async Task MessageRecieved(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;
            if(string.IsNullOrEmpty(message.Text))
            {
                dynamic value = message.Value;
                if (value == null)
                {
                    await context.PostAsync("Feedback not recorded");
                }
                else
                {
                    await context.PostAsync("Your response was sucessfully recorded. Thanks for your feedback");
                }
            }
            context.Done(this);
        }

        private Attachment CreateFeedBackForm()
        {
            var card = new AdaptiveCard()
            {
                Body = new List<CardElement>()
                {
                    new TextBlock()
                    { Text = "Was your problem resolved to your satisfaction"},

                    new TextInput()
                    {
                        Id = "ResolveQueries",
                        Speak = "<s> Please enter Yes/No",
                        Placeholder = "Please enter Yes/No",
                        Style = TextInputStyle.Text
                    },

                    new TextBlock()
                    {Text = "Was your problem resolved within an adequate timescale" },

                    new TextInput()
                    {
                         Id = "TimeScale",
                        Speak = "<s> Please enter Yes/No",
                        Placeholder = "Please enter Yes/No",
                        Style = TextInputStyle.Text
                    },

                    new TextBlock()
                    {Text = "How do you rate the service you recieve on a scale of 10" },

                    new NumberInput()
                    {
                        Id = "Rating",
                        Min = 1,
                        Max = 10,
                    },

                },
               Actions = new List<ActionBase>()
                {
                    new SubmitAction()
                    {
                        Title = "Submit",
                        Speak = "<s>Submit</s>",
                        DataJson = "{\"Submit\" : \"Submit\"}"
                    }
                }
            };

            Attachment attachment = new Attachment()
            {
                ContentType = AdaptiveCard.ContentType,
                Content = card
            };

            return attachment;
        }
    }
}