namespace JwglBackend.Models
{
    public class ErrorModel
    {
        public String Error { get; set; }

        public ErrorModel(string error)
        {
            Error = error;
        }
    }
}
