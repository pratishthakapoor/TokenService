using System;
using System.Runtime.Serialization;

namespace Support24.Dialogs
{
    [Serializable]
    internal class DocumentIdRequiredException : Exception
    {
        #region Constructor
        public DocumentIdRequiredException()
        {
        }

        public DocumentIdRequiredException(string message) : base(message)
        {
        }

        public DocumentIdRequiredException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected DocumentIdRequiredException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
        #endregion Constructor

        #region Properties
        public override string Message
        {
            get
            {
                return "A document's Id property is required.";
            }
        }
        #endregion
    }
}