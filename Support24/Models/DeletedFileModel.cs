using Microsoft.Bot.Builder.FormFlow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Support24.Models
{
    public class DeletedFileModel
    {
        // Prompts shown as questions to be should to the user by the form flow

        [Prompt(new string[] { "What is your name?" })]
        public string UserName { get; set; }

        [Prompt(new string[] { "Tell me about the issue you are facing" })]
        public string IssueDescription { get; set; }

        [Prompt(new string[] { "Please provide your email address" })]
        public string EmailID { get; set; }

        public static IForm<DeletedFileModel> BuildForm()
        {
            return new FormBuilder<DeletedFileModel>()
            .Field(nameof(UserName))
            .Field(nameof(IssueDescription))
            .Field(nameof(EmailID))
            .AddRemainingFields()
            .Build();
        }

    }
}