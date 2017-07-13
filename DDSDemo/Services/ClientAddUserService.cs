using DDSDemo.Models;
using DDSDemoDAL;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using Microsoft.Owin;
using System.Threading.Tasks;

namespace DDSDemo.Services
{
    public class ClientAddUserService
    {
        public async Task<IdentityResult> AddUserClient(NewClientUserInputModel model, ApplicationUserManager UserManager)
        {
            var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
            var result = await UserManager.CreateAsync(user, "StaffFinder2017");
            if (result.Succeeded)
            {
                await UserManager.AddToRoleAsync(user.Id, "Client");
                await UserManager.AddClaimAsync(user.Id, new Claim("ClientID", model.ID.ToString()));

                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");
            }

            return result;
        }
    }
}