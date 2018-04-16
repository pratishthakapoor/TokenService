using Microsoft.Bot.Builder.FormFlow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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
            .Field(nameof(UserName), validate: ValidateUsername)
            //.Field(nameof(Password))
            .AddRemainingFields()
            .Build();
        }

        private static Task<ValidateResult> ValidateUsername(DeletedFileModel state, object value)
        {
            var result = new ValidateResult();
            string UsernameInfo = string.Empty;
            if(GetUsername((string)value, out UsernameInfo))
            {
                result.IsValid = true;
                result.Value = UsernameInfo;
            }
            else
            {
                result.IsValid = false;
                result.Feedback = "Please enter a valid username";
            }

            return Task.FromResult(result);
        }

        private static bool GetUsername(string value, out string usernameInfo)
        {
            usernameInfo = string.Empty;
            var match = Regex.Match(value, @"^[0-9A-Z]([-.\w]*[0-9A-Z]*$");

            if(match.Success)
            {
                usernameInfo = match.Value;
                return true;
            }

            return false;
        }
    }
} 