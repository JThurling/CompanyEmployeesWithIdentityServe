// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4.Models;
using System.Collections.Generic;
using IdentityServer4;

namespace CompanyEmployees.IDP
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Address(),
                new IdentityResource("roles", "User role(s)", new List<string>{"role"}),
                new IdentityResource("country", "Your country", new List<string>{"country"})
            };

        //Configure our APIs and Clients
        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[]
            {
                new ApiScope("companyemployeeapi.scope", "CompanyEmployee API Scope"),
            };

        public static IEnumerable<ApiResource> Apis =>
            new ApiResource[]
            {
                new ApiResource("companyemployeeapi", "CompanyEmployee API")
                {
                    Scopes = {"companyemployeeapi.scope"}
                },
            };

        public static IEnumerable<Client> Clients =>
            new Client[]
            {
                new Client
                {
                    // Set up the redirection URL for client
                    ClientName = "CompanyEmployeeClient",
                    ClientId = "companyemployeeclient",
                    // Specifies the Authorization Code Flow
                    AllowedGrantTypes = GrantTypes.Code,
                    // The flow is redirection based so we sut up the URLS
                    RedirectUris = new List<string>{"https://localhost:5010/signin-oidc"},
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Address,
                        "roles",
                        "companyemployeeapi.scope",
                        "country"
                    },
                    // Hashed Client Secrect key
                    ClientSecrets = {new Secret("CompanyEmployeeClientSecret".Sha512())},
                    RequirePkce = true,
                    PostLogoutRedirectUris = new List<string>{"https://localhost:5010/signout-callback-oidc"},
                    RequireConsent = true,
                    AccessTokenLifetime = 120,
                    AllowOfflineAccess = true,
                    UpdateAccessTokenClaimsOnRefresh = true
                }
            };
    }
}
