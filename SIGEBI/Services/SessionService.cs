using SIGEBI.Services;

namespace SIGEBI.Services
{
    public static class SessionService
    {
        public static string? Token { get; set; }
        public static ApiService? ApiService { get; set; }
    }
}
