namespace RyderX_Server.Authentication
{
    public class ExternalAuthModel
    {
        public string Provider { get; set; } = string.Empty;   
        public string IdToken { get; set; } = string.Empty;
    }
}
