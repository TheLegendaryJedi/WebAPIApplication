using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net;
using Auth0.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Logging;
using WebAPIApplication;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder =>
        {
            builder
            .WithOrigins("http://localhost:8080")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
        });
});

var domain = $"https://{builder.Configuration["Auth0:Domain"]}/";
// builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
// .AddJwtBearer(options =>
// {
//     options.Authority = domain;
//     options.Audience = builder.Configuration["Auth0:Audience"];
// });

// Add Auth0 Auth, reading the app settings above
builder.Services.AddAuth0WebAppAuthentication(options =>
{
    options.Domain = builder.Configuration["Auth0:Domain"];
    options.ClientId = builder.Configuration["Auth0:ClientId"];
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("read:messages", policy => policy.Requirements.Add(new HasScopeRequirement("read:messages", domain)));
});

builder.Services.AddControllers();
builder.Services.AddSingleton<IAuthorizationHandler, HasScopeHandler>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();

}
else
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseHttpsRedirection();

app.UseRouting();

app.UseCors("AllowSpecificOrigin");
app.UseAuthentication();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();