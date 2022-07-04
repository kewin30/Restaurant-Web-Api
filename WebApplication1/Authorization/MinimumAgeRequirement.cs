using Microsoft.AspNetCore.Authorization;

namespace WebApplication1.Authorization
{
    public class MinimumAgeRequirement : IAuthorizationRequirement
    {
        public int MinimumAge { get;}

        public MinimumAgeRequirement(int minimumAge)
        {
            MinimumAge = minimumAge;
        }


    }
}
