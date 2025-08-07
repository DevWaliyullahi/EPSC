using EPSC.Infrastructure.Identity.Auth;
using Microsoft.AspNetCore.Identity;


namespace EPSC.Infrastructure.Identity.Auth
{
    public class EPSAuthUser : IdentityUser
    {
        public string? FullName { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
