namespace Pg.DataverseSync.Api.Models.Auth
{
    public class LogoutRequest
    {
        public string? RefreshToken { get; internal set; }
    }
}