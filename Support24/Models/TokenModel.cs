using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.FormFlow;

namespace Support24.Models
{
   [Serializable]
    public class TokenModel
    {
        // Prompts shown as questions to be should to the user by the form flow

        [Prompt(new string[] { "What is your name?" })]
        public string UserName { get; set; }

        [Prompt(new string[] { "Tell me about the issue you are facing" })]
        public string IssueDescription { get; set; }

        [Prompt(new string[] { "Please provide your email address"})]
        public string EmailID { get; set; }

        public static IForm<TokenModel> BuildForm()
        {
            return new FormBuilder<TokenModel>()
            .Field(nameof(UserName), validate: validateUser)
            .Field(nameof(IssueDescription))
            .Field(nameof(EmailID))
            .AddRemainingFields()
            //.OnCompletion(getKeyPhrases)
            .Build(); 
        }

        private static Task<ValidateResult> validateEmailId(TokenModel state, object value)
        {
            var result = new ValidateResult();
            string emailInfo = string.Empty;
            if(GetEmailAddress((string)value, out emailInfo))
            {
                result.IsValid = true;
                result.Value = emailInfo;
                result.Feedback = "You did not enter a valid email id";
            }
            else
            {
                result.IsValid = false;
                result.Feedback = "You did not enter a valid email id";
            }

            return Task.FromResult(result);
        }

        private static bool GetEmailAddress(string value, out string emailInfo)
        {
            emailInfo = string.Empty;
            var match = Regex.Match(value, @"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?");
            if (match.Success)
            {
                emailInfo = match.Value;
                return true;
            }
            return false;
        }

        private static Task<ValidateResult> validateUser(TokenModel state, object value)
        {
            var result = new ValidateResult();
            string usernameInfo = string.Empty;

            if (GetUsername((string)value, out usernameInfo))
            {
                result.IsValid = true;
                result.Value = usernameInfo;
            }
            else
            {
                result.IsValid = false;
                result.Feedback = "Please enter name a valid name";
            }
            return Task.FromResult(result);
        }

        private static bool GetUsername(string value, out string usernameInfo)
        {
            usernameInfo = string.Empty;

            //Matches wether the if the name should start with a letter, should contain smaller case letter and should be 3 to 11 character long

            var match = Regex.Match(value, "[a-zA-z][a-z]{3,11}");
            if (match.Success)
            {
                usernameInfo = match.Value;
                return true;
            }
            return false;
        }
    }
}