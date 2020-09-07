using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using CompanyEmployees.Client.Handlers;
using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Net.Http.Headers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace CompanyEmployees.Client
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient("APIClient", client =>
            {
                client.BaseAddress = new Uri("https://localhost:5001/");
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
            }).AddHttpMessageHandler<BearerTokenHandler>();
            services.AddHttpClient("IDPClient", client =>
            {
                client.BaseAddress = new Uri("https://localhost:5005/");
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
            });
            services.AddAuthentication(options =>
                {
                    // We register the auth service and populate our schemes
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
                    //We register the cookie based authentication for our default scheme
                }).AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
                {
                    options.AccessDeniedPath = "/Auth/AccessDenied";
                })
                // OpenId Connect set to default for the auth actions
                .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
                {
                    // Same as our defaultScheme
                    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    // Has the URI for our IDP server
                    options.Authority = "https://localhost:5005";
                    // Client ID and Client Secret needs to be the same as the ID and secret from the Config
                    options.ClientId = "companyemployeeclient";
                    // We set it to the specific flow - returns to the /authorization endpoint
                    options.ResponseType = OpenIdConnectResponseType.Code;
                    // Saves the token after the authorization has been successfull
                    options.SaveTokens = true;
                    options.ClientSecret = "CompanyEmployeeClientSecret";
                    options.GetClaimsFromUserInfoEndpoint = true;
                    options.ClaimActions.DeleteClaim("sid");
                    options.ClaimActions.DeleteClaim("idp");
                    options.Scope.Add("address");
                    options.Scope.Add("roles");
                    options.ClaimActions.MapUniqueJsonKey("role", "role");
                    options.Scope.Add("country");
                    options.ClaimActions.MapUniqueJsonKey("country", "country");

                    options.Scope.Add("companyemployeeapi.scope");
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        RoleClaimType = JwtClaimTypes.Role
                    };
                    options.Scope.Add("offline_access");
                });
            services.AddAuthorization(options =>
            {
                options.AddPolicy("CanCreateAndModifyData", builder =>
                {
                    builder.RequireAuthenticatedUser();
                    builder.RequireRole("role", "Administrator");
                    builder.RequireClaim("country", "USA");
                });
            });
            services.AddControllersWithViews();
            services.AddHttpContextAccessor();
            services.AddTransient<BearerTokenHandler>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
