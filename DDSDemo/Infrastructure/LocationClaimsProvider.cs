using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace DDSDemo.Infrastructure
{
    public static class LocationClaimsProvider
    {
        public static IEnumerable<Claim> GetClaims(ClaimsIdentity user)
        {
            List<Claim> claims = new List<Claim>();
            foreach(var claim in user.Claims)
            {
                if(claim.Type == "EmployerID" || claim.Type == "ClientID")
                {
                    claims.Add(new Claim(claim.Type, claim.Value, ClaimValueTypes.String, "Test"));
                }
            }
            return claims;
        }
    }
}