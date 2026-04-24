namespace Pg.DataverseSync.Api.Application.Results
{
    public class CreateUserResult
    {
        public bool Success { get; set; }
        public int? UserId { get; set; }
        public string? ErrorMessage { get; set; }
    }
}