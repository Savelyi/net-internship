using IdentityModel;
using IdentityServer4.Models;
using static IdentityServer4.IdentityServerConstants;

namespace Entities.Utils
{
    public class IdentityConfig
    {
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>()
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new()
                {
                    Name = "roles",
                    DisplayName = "Roles",
                    UserClaims = {JwtClaimTypes.Role}
                },
                new()
                {
                    Name = "id",
                    DisplayName = "Id",
                    UserClaims = {JwtClaimTypes.Id}
                }
            };
        }

        public static IEnumerable<Client> GetClients()
        {
            return new[]
            {
                new Client
                {
                    ClientId = "api",
                    ClientName = "ClientApi",
                    AllowAccessTokensViaBrowser = true,
                    ClientSecrets = new[] {new Secret("clientsupersecret".Sha512())},
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    AllowedScopes = {StandardScopes.OpenId, "CatalogApi", "RentApi"}
                }
            };
        }

        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>()
            {
                new("CatalogApi", new[] {JwtClaimTypes.Id, JwtClaimTypes.Role})
                {
                    Scopes = new List<string> {"CatalogApi"},
                    ApiSecrets = new List<Secret> {new("catalogsupersecret".Sha256())}
                },
                new("RentApi", new[] {JwtClaimTypes.Id, JwtClaimTypes.Role})
                {
                    Scopes = new List<string> {"RentApi"},
                    ApiSecrets = new List<Secret> {new("rentsupersecret".Sha256())}
                }
            };
        }

        public static IEnumerable<ApiScope> GetApiScopes()
        {
            return new List<ApiScope>()
            {
                new("CatalogApi"),
                new("RentApi")
            };
        }
    }
}