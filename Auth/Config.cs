// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4.Models;
using System.Collections.Generic;

namespace Api
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Address(),
                new IdentityResource()
                {
                  Name = "roles",
                  DisplayName = "Your role(s)",
                  UserClaims = new List<string> { "role" }
                },
                new IdentityResource()
                {
                  Name = "subscription",
                  DisplayName = "Your subscription(s)",
                  UserClaims = new List<string> { "subscription" }
                },
                new IdentityResource()
                {
                  Name = "country",
                  DisplayName = "Your country",
                  UserClaims = new List<string> { "country" }
                }
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[]
            {
                new ApiScope("scope1"),
                new ApiScope("api", "Api")
                {
                  UserClaims = new List<string> { "role", "subscription", "country" }
                }
            };

        public static IEnumerable<Client> Clients =>
            new Client[]
            {
                // m2m client credentials flow client
                new Client
                {
                    ClientId = "m2m.client",
                    ClientName = "Client Credentials Client",

                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets = { new Secret("511536EF-F270-4058-80CA-1C89C192F69A".Sha256()) },

                    AllowedScopes = { "scope1" }
                },

                // interactive client using code flow + pkce
                new Client
                {
                    ClientId = "mvc",
                    ClientName = "Mvc",
                    ClientSecrets = { new Secret("49C1A7E1-0C79-4A89-A3D6-A37998FB86B0".Sha256()) },
                    
                    AllowedGrantTypes = GrantTypes.Code,
                    UpdateAccessTokenClaimsOnRefresh = true,

                    AlwaysIncludeUserClaimsInIdToken = true, // Because of Internet Explorer

                    RedirectUris = { "https://localhost:44341/signin-oidc" },
                    FrontChannelLogoutUri = "https://localhost:44341/signout-oidc",
                    PostLogoutRedirectUris = { "https://localhost:44341/signout-callback-oidc" },

                    AllowOfflineAccess = true,
                    AllowedScopes = { "openid", "profile", "address", "roles", "api", "subscription", "country" }
                },
            };
    }
}