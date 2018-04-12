using Microsoft.Bot.Builder.FormFlow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Support24.Models
{
    [Serializable]
    public class DeletedFileModel
    {
        // Prompts shown as questions to be should to the user by the form flow

        [Prompt(new string[] { "Please enter your username for ODB account" })]
        public string UserName { get; set; }

        /*[Prompt(new string[] { "Please provide the password for the account"})]
        public string Password { get; set; }*/

        public static IForm<DeletedFileModel> BuildForm()
        {
            return new FormBuilder<DeletedFileModel>()
            .Field(nameof(UserName))
            //.Field(nameof(Password))
            .AddRemainingFields()
            .Build();
        }

    }
} 