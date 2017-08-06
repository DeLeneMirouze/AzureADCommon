#region using
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using CommonLibrary;
using CommonLibrary.Repositories;
#endregion

namespace DemoCommonCore
{
    public class Startup
    {
        #region Constructor
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see https://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets<Startup>();
            }
            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        } 
        #endregion

        public IConfigurationRoot Configuration { get; }

        #region ConfigureServices
        /// <summary>
        ///  This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();

            services.AddAuthentication(
                SharedOptions => SharedOptions.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme);

            services.AddSingleton<IConfiguration>(Configuration);
            services.AddTransient(typeof(ITokenHelper), typeof(TokenHelper));
            services.AddTransient(typeof(IAzureRepositories), typeof(AzureRepositories));
        }
        #endregion

        #region Configure
        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        /// <param name="loggerFactory"></param>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseCookieAuthentication();

            //https://stackoverflow.com/questions/43644657/using-onauthorizationcodereceived-to-retrieve-azure-graphapi-accesstoken

            app.UseOpenIdConnectAuthentication(new OpenIdConnectOptions()
            {
                ClientId = Configuration["Authentication:AzureAd:ClientId"],
                Authority = Configuration["Authentication:AzureAd:AADInstance"] + "Common",
                CallbackPath = Configuration["Authentication:AzureAd:CallbackPath"],
                ClientSecret = Configuration["Authentication:AzureAd:Secret"],

                ResponseType = OpenIdConnectResponseType.CodeIdToken,

                TokenValidationParameters = new TokenValidationParameters
                {
                    // Instead of using the default validation (validating against a single issuer value, as we do in line of business apps),
                    // we inject our own multitenant validation logic
                    ValidateIssuer = false,

                    // If the app is meant to be accessed by entire organizations, add your issuer validation logic here.
                    //IssuerValidator = (issuer, securityToken, validationParameters) => {
                    //    if (myIssuerValidationLogic(issuer)) return issuer;
                    //}
                },
                Events = new OpenIdConnectEvents
                {
                    OnTicketReceived = (context) =>
                    {
                        // If your authentication logic is based on users then add your logic here
                        return Task.FromResult(0);
                    },
                    OnAuthenticationFailed = (context) =>
                    {
                        context.Response.Redirect("/Home/Error");
                        context.HandleResponse(); // Suppress the exception
                        return Task.FromResult(0);
                    },

                    OnAuthorizationCodeReceived = async (context) =>
                    {
                        // get Access token and push it to default ADAL cache
                        // OK, OK. It is a bad cache in a web related scenario
                        // but I am a little bit lazy for this demo...

                        ClientCredential credential = new ClientCredential(
                            context.TokenEndpointRequest.ClientId,
                            context.TokenEndpointRequest.ClientSecret);

                        string resourceUri = Constantes.Endpoints.ArmEndpoint;
                        AuthenticationContext authContext = new AuthenticationContext(context.Options.Authority);

                        AuthenticationResult result = await authContext.AcquireTokenByAuthorizationCodeAsync(
                            context.TokenEndpointRequest.Code,
                            new Uri(context.TokenEndpointRequest.RedirectUri),
                            credential, resourceUri);

                        context.HandleCodeRedemption(result.AccessToken, result.IdToken);
                    }
                    // If your application needs to do authenticate single users, add your user validation below.
                    //OnTokenValidated = (context) =>
                    //{
                    //    return myUserValidationLogic(context.Ticket.Principal);
                    //}
                }
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        } 
        #endregion
    }
}
