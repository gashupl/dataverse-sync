namespace Pg.DataverseSync.Domain.Services
{
    public class EndpointStepCreationResult
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
    }
}