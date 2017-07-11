using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace DDSDemo.Infrastructure.Authorization
{
    public class ClaimsAccessAttribute : AuthorizeAttribute
    {
        public string ClaimType { get; set; }
        public string Value { get; set; }

        protected override bool AuthorizeCore(HttpContextBase context)
        {
            var result =  context.User.Identity.IsAuthenticated
                && context.User.Identity is ClaimsIdentity
                && ((ClaimsIdentity)context.User.Identity).HasClaim(x => x.Type == ClaimType);

            return result;
        }
    }
}