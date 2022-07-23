namespace PostCalendarAPI.Services.JWService.Models
{
    public class JWAnalysisException : Exception
    {
        public JWAnalysisException() : base() { }

        public JWAnalysisException(string message) : base(message) { }

        public JWAnalysisException(string message, Exception innerException) : base(message, innerException) { }

        public JWAnalysisException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }    
    }

}
