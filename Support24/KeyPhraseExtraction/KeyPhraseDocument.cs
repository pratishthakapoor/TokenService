using Newtonsoft.Json;

namespace Support24.Dialogs
{
    public class KeyPhraseDocument : Document, IDocument
    {
        /// <summary>
        /// Gets or sets the language the text is in
        /// </summary>
        /// <value>
        /// The language.
        /// </value>
        [JsonProperty("language")]
        public string Language
        {
            get;
            set;
        }
    }
}