using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Auth0.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPIApplication.Controllers;

public class AccountController : ControllerBase
{
    [Route("/account/signup")]
    public async Task Signup(string returnUrl = "/private")
    {
        var authenticationProperties = new LoginAuthenticationPropertiesBuilder()
            .WithParameter("screen_hint", "signup")
            .WithRedirectUri(returnUrl)
            .Build();

        await HttpContext.ChallengeAsync(Auth0Constants.AuthenticationScheme, authenticationProperties);
    }
    [Route("/account/login")]
    public async Task Login(string returnUrl = "/home")
    {
        var authenticationProperties = new LoginAuthenticationPropertiesBuilder()
            // Indicate here where Auth0 should redirect the user after a login.
            // Note that the resulting absolute Uri must be added to the
            // **Allowed Callback URLs** settings for the app.
            .WithRedirectUri(returnUrl)
            .Build();

        await HttpContext.ChallengeAsync(Auth0Constants.AuthenticationScheme, authenticationProperties);
    }
    
    [Authorize]
    [Route("/account/logout")]
    public async Task Logout()
    {
        var authenticationProperties = new LogoutAuthenticationPropertiesBuilder()
            // Indicate here where Auth0 should redirect the user after a logout.
            // Note that the resulting absolute Uri must be added to the
            // **Allowed Logout URLs** settings for the app.
            .WithRedirectUri("/home")
            .Build();

        await HttpContext.SignOutAsync(Auth0Constants.AuthenticationScheme, authenticationProperties);
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        Response.Cookies.Delete(".AspNetCore.Cookies");
    }

    [Authorize]
    [Route("/home")]
    public IActionResult Home()
    {
        return Ok(new
        {
            Message = "Hello from a private endpoint! You need to be authenticated to see this."
        });
    }
}