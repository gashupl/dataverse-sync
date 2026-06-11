namespace Pg.DataverseSync.Domain.Services
{
    public class ServiceOperationResult
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
    }
}