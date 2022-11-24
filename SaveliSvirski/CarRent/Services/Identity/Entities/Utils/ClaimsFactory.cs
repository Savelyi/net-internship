using System.Security.Claims;
using IdentityModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Entities.Utils
{
    public class ClaimsFactory : UserClaimsPrincipalFactory<IdentityUser>
    {
        private readonly UserManager<IdentityUser> userManager;

        public ClaimsFactory(
            UserManager<IdentityUser> userManager,
            IOptions<IdentityOptions> optionsAccessor) : base(userManager, optionsAccessor)
        {
            this.userManager = userManager;
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(IdentityUser user)
        {
            var identity = await base.GenerateClaimsAsync(user);
            var roles = await userManager.GetRolesAsync(user);

            identity.AddClaims(roles.Select(role => new Claim(JwtClaimTypes.Role, role)));
            identity.AddClaim(new Claim(JwtClaimTypes.Id, user.Id));

            return identity;
        }
    }
}