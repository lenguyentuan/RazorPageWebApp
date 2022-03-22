using Microsoft.AspNetCore.Authorization;

namespace RazorWebApp.Security.Requirements
{
    public class GenzRequirement : IAuthorizationRequirement
    {
        public GenzRequirement(int fromYear = 1997, int toYear = 2012)
        {
            FromYear = fromYear;
            ToYear = toYear;
        }

        public int FromYear { set; get; }

        public int ToYear { set; get; }
    }
}