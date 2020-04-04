﻿using Facebook;
using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace BooksApi.Security
{
    public class SimpleAuthTokenProvider : OAuthAuthorizationServerProvider

    {

        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)

        {

            context.Validated();

        }



        public override async Task GrantCustomExtension(OAuthGrantCustomExtensionContext context)

        {

            if (context.GrantType.ToLower() == "facebook")

            {

                var fbClient = new FacebookClient(context.Parameters.Get("accesstoken"));



                dynamic response = await fbClient.GetTaskAsync("me", new { fields = "email, first_name, last_name" });



                string id = response.id;

                string email = response.email;

                string firstname = response.first_name;

                string lastname = response.last_name;

                // place your own logic to lookup and/or create users....



                // your choice of claims...

                var identity = new ClaimsIdentity(context.Options.AuthenticationType);

                identity.AddClaim(new Claim("sub", $"{firstname} {lastname}"));

                identity.AddClaim(new Claim("role", id));



                await base.GrantCustomExtension(context);

                context.Validated(identity);



            }

            return;

        }

        private void SetContextHeaders(IOwinContext context)
        {
            context.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });
            context.Response.Headers.Add("Access-Control-Allow-Methods", new[] { "GET, PUT, DELETE, POST, OPTIONS" });
            context.Response.Headers.Add("Access-Control-Allow-Headers", new[] { "Content-Type, Accept, Authorization" });
            context.Response.Headers.Add("Access-Control-Max-Age", new[] { "1728000" });
        }

    }
}