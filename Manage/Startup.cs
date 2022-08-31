using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using BioTech.Models;
using Microsoft.AspNetCore.Identity;
//using BioTech.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Newtonsoft.Json.Serialization;
using Microsoft.AspNetCore.Identity.UI.Services;
using BioTech.Services;
using AutoMapper;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Routing;
using Manage.Services;
using Manage.Helpers;

namespace BioTech
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddDataProtection()
                .PersistKeysToFileSystem(new System.IO.DirectoryInfo("bin/"))
                //.ProtectKeysWithCertificate(new X509Certificate2());
                .SetDefaultKeyLifetime(TimeSpan.FromDays(90));

            // configure password strength restrictions
            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequiredLength = 6;
                options.Password.RequireDigit = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
            });

            // add the customized identity db context
            services.AddDbContext<BioTechIdentityContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("BioTechContextConnection")));

            // add BioTech DbContext
            services.AddDbContext<BioTechContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("BioTechContextConnection")));

            // add identity 
            services.AddIdentity<ApplicationUser, ApplicationRole>(options => {
                options.Stores.MaxLengthForKeys = 128;
                options.SignIn.RequireConfirmedEmail = false;
            })
                .AddEntityFrameworkStores<BioTechIdentityContext>()
                .AddDefaultUI()
                .AddDefaultTokenProviders();

            services.AddAutoMapper(typeof(Startup));

            // configure mvc routing and require requests to have authentication unless they override with [AllowAnonymous]
            // resource: require authenticated users https://docs.microsoft.com/en-us/aspnet/core/security/authorization/secure-data?view=aspnetcore-2.1#require-authenticated-users
            services.AddMvc(config => {
                var policy = new AuthorizationPolicyBuilder()
                         .RequireAuthenticatedUser()
                         .Build();
                config.Filters.Add(new AuthorizeFilter(policy));
            })
            .AddJsonOptions(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver()) // asp.net core now returns json as camelCase where Kendo looks for PascalCase field names
            .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => false;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            services.AddDistributedMemoryCache(); // Adds a default in-memory implementation of IDistributedCache
            services.AddSession();

            //Setting Form length and count to maximum value, sometimes is required to send list of objects in a request, this prevent an Exception
            //to be thrown because of Form values limit
            services.Configure<FormOptions>(x =>
            {
                x.ValueLengthLimit = int.MaxValue;
                x.MultipartBodyLengthLimit = int.MaxValue;
                x.KeyLengthLimit = int.MaxValue;
                x.ValueLengthLimit = int.MaxValue;
                x.ValueCountLimit = int.MaxValue;
            });

            //Register services
            services.AddScoped<ICompanyService, CompanyService>();
            services.AddScoped<ITouchLogService, TouchLogService>();
            services.AddScoped<IEmailSender, EmailSender>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IContactService, ContactService>();
            services.AddScoped<IBillingRoleService, BillingRoleService>();
            services.AddScoped<ITaskService, TaskService>();
            services.AddScoped<IProposalService, ProposalService>();
            services.AddScoped<ITaskExtensionService, TaskExtensionService>();
            services.AddScoped<IProposalApplicationUserService, ProposalApplicationUserService>();
            services.AddScoped<ITaskUserService, TaskUserService>();
            services.AddScoped<ITimesheetEntryService, TimesheetEntryService>();
            services.AddScoped<ITimesheetApprovedDateRangeService, TimesheetApprovedDateRangeService>();
            services.AddScoped<IInvoiceProjectService, InvoiceProjectService>();
            services.AddScoped<IPartialPaymentService, PartialPaymentService>();
            services.AddScoped<IInvoiceTimesheetEntryService, InvoiceTimesheetEntryService>();
            services.AddScoped<IProjectTaskUserAssignedService, ProjectTaskUserAssignedService>();
            services.AddScoped<UserManager<ApplicationUser>>();
            services.AddScoped<ContractNumberService>();
            services.AddScoped<IContactClubService, ContactClubService>();
            services.AddScoped<RestClient>();
            services.AddKendo();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, BioTechIdentityContext context,
              RoleManager<ApplicationRole> roleManager)
        {
            //Iron PDF license
            IronPdf.License.LicenseKey = Configuration["IronPdfSerial"];
            //Checking if IronPDF is licensed
            var isIronPdfLicensed = IronPdf.License.IsLicensed;
            // serialog setup
            Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(Configuration)
            .CreateLogger();
            Log.Information("The web application is starting up...");
            //Log.CloseAndFlush();


            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseDeveloperExceptionPage();
                //app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }                  
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseCookiePolicy();
            app.UseSession();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
