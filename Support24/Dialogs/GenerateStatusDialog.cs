using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Support24.SnowLogger;

namespace Support24.Dialogs
{
    internal class GenerateStatusDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            PromptDialog.Text(
                context: context,
                resume: getTicketNo,
                prompt: "Please provide ticket number",
                retry: "Please try again later"
                );
        }

        private async Task getTicketNo(IDialogContext context, IAwaitable<string> result)
        {
            var response = await result;

            try
            {

                /**
                 * Method call to handle the user request to check ticket status from the SNOW service
                 * Read data from the SNOW incidence
                 **/

                /**
                 * statusDetails parameter stores the value return by RetrieveIncidentServiceNow method of the Snow Logger class
                 **/

                string statusDetails = Logger.RetrieveIncidentServiceNow(response);

                /**
                 * The if- else- if condition to match the state of the incident token returned by the RetrieveIncidentSerivceNow method
                 */

                if (statusDetails == "1")
                {
                    var status = "Your token is created and is under review by our team.";
                    string Notesresult = Logger.RetrieveIncidentWorkNotes(response);

                    var replyMessage = context.MakeMessage();
                    Attachment attachment = GetReplyMessage(Notesresult, response, status);
                    replyMessage.Attachments = new List<Attachment> { attachment };
                    await context.PostAsync(replyMessage);

                }

                else if (statusDetails == "2")
                {
                    var status = "Your ticket is in progress.";
                    string Notesresult = Logger.RetrieveIncidentWorkNotes(response);

                    var replyMessage = context.MakeMessage();
                    Attachment attachment = GetReplyMessage(Notesresult, response, status);
                    replyMessage.Attachments = new List<Attachment> { attachment };
                    await context.PostAsync(replyMessage);

                }

                else if (statusDetails == "3")
                {
                    await context.PostAsync("Your ticket is been kept on hold.");


                }

                else if (statusDetails == "6")
                {
                    await context.PostAsync("Your ticket is resolved.");

                    /**
                     * Retrieves the details from the resolve columns of SnowLogger class if the incident token is being resolved
                     **/

                    string resolveDetails = Logger.RetrieveIncidentResolveDetails(response);
                    await context.PostAsync("For the ticket id " + response + " solution fetched by our team is : " + resolveDetails);
                }


                else if (statusDetails == "7")
                {
                    await context.PostAsync("Your ticket has been closed by our team");

                    /**
                     * Retrieves the close_code from the SnowLogger class if the incident token is being closed
                     **/

                    string resolveDetails = Logger.RetrieveIncidentCloseDetails(response);
                    await context.PostAsync("Reasons for closing the ticket: " + resolveDetails);
                }

                else
                    await context.PostAsync("Our team cancelled your ticket");
            }

            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            context.Done(this);

        }

        private Attachment GetReplyMessage(string notesresult, string response, string status)
        {
            var heroCard = new HeroCard
            {
                //title for the given
                Title = "Progress details for the ticket " + response,
                // subtitle for the card
                Subtitle = status,
                //Detail text
                Text = "Latest work carried out on your raised ticket includes:\n\n" + notesresult,
                //in case for other channel use
                /**
                 * Text = "Latest work carried out on your raised ticket includes:\n\n" + notesresult, ex : Text = "More words <br> New line <br> New line <b><font color = \"#11b92f\>GREEN</font></b></br></br>
                 **/
                //list of buttons
                Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, "Need further details? ", value: "https://www.t-systems.hu/about-t-systems/customer-contact/service-desk"),
                    new CardAction(ActionTypes.OpenUrl, "Contact us at", value: "https://www.t-systems.com/de/en/contacts")}
            };
            return heroCard.ToAttachment();
        }
    }
}