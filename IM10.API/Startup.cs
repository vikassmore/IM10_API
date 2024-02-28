
using Castle.Core.Smtp;
using CorePush.Apple;
using CorePush.Google;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using IM10.API.Hubs;
using IM10.BAL.Implementaion;
using IM10.BAL.Interface;
using IM10.Entity.DataModels;
using IM10.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IM10.API
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

            services.AddOptions();
            services.AddHttpClient<FcmSender>();
            services.AddHttpClient<ApnSender>();
            services.AddControllers();
            
            FirebaseApp.Create(new AppOptions()
            {
                Credential = GoogleCredential.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "im10-f32bd-firebase-adminsdk-ea3aj-efa6ba6ad0.json")),
                 ProjectId = "im10-f32bd"
            });
            //configure SignalR
            services.AddSignalR();
            services.AddCors(options => {
                options.AddPolicy("AllowAnyOrigin", builder =>
                    builder.WithOrigins("http://localhost:4200","http://im10ui.meshbagroup.com/")
                           .AllowAnyMethod()                                          
                           .AllowAnyHeader()
                           .AllowCredentials());
            });
            services.AddDbContext<Entity.DataModels.IM10DbContext>(options => options
                                                 .UseLazyLoadingProxies()
                                                 .UseSqlServer(Configuration["ConnectionStrings:DefaultConnection"]));
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Latest);
            services.Configure<FormOptions>(o =>
            {
                o.ValueLengthLimit = int.MaxValue;
                o.MultipartBodyLengthLimit = int.MaxValue;
                o.MemoryBufferThreshold = int.MaxValue;
            });

            services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = 60000000; // Set the maximum request size in bytes (60MB in this example)
            });
            // Register all injecting interfaces with implemented class
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUserPlayerService, UserPlayerService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddTransient<IEmailSenderService, EmailSenderService>();
            services.Configure<Models.EmailSettings>(Configuration.GetSection("EmailSettings"));
            services.AddScoped<IMasterAPIsService, MasterAPIsService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IContentDetailService, ContentDetailService>();
            services.AddScoped<IPlayerDetailService,PlayerDetailService>();
            services.AddScoped<IContentUpdateService, ContentUpdateService>();
            services.AddScoped<IAdvContentDetailService,AdvContentDetailService>();
            services.AddScoped<IListingDetailService,ListingDetailService>();
            services.AddScoped<IAdvContentMappingService,AdvContentMappingService>();
            services.AddScoped<IContentCommentService,ContentCommentService>();
            services.Configure<ConfigurationModel>(Configuration.GetSection("ConfigurationModel"));
            services.AddScoped<IErrorAuditLogService, ErrorAuditLogService>();
            services.AddScoped<ICampaignDetailService,CampaignDetailService>();
            services.AddScoped<ICampaignSocialMediaDetailService,CampaignSocialMediaDetailService>();
            services.AddScoped<IEndorsmentDetailService, EndorsmentDetailService>();
            services.AddScoped<IEndorsmentTypeService, EndorsmentTypeService>();
            services.AddScoped<IMobileServices, MobileService>();
            services.AddScoped<IUserAuditLogService, UserAuditLogService>();
            services.AddScoped<IFCMNotificationService, FCMNotificationService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.Configure<FcmNotificationSetting>(Configuration.GetSection("FcmNotification"));
            services.Configure<SMSSettingModel>(Configuration.GetSection("SMSSettingTwilioModel"));
            // Adding jwtBearer Authentication  

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
             .AddJwtBearer(options =>
              {
                  options.SaveToken = true;
                  options.RequireHttpsMetadata = false;
                  options.TokenValidationParameters = new TokenValidationParameters()
                  {
                      ValidateIssuer = true,
                      ValidateAudience = true,
                      ValidAudience = Configuration["JWT:ValidAudience"],
                      ValidIssuer = Configuration["JWT:ValidIssuer"],
                      IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:Secret"]))
                  };
              });

            // Register the Swagger generator, defining 1 or more Swagger documents

            services.AddSwaggerGen(c =>
            {
                // This is to generate the Default UI of Swagger Documentation

                c.SwaggerDoc("v1", new OpenApiInfo { Title = "IM10.API", Version = "v1" });

                // To Enable authorization using Swagger (JWT)

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = Configuration["APISecurityDefinition:Description"],
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                          new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            new string[] {}
                    }
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
           
            
            if (env.IsDevelopment() || env.IsProduction())
            {
                app.UseDeveloperExceptionPage();               
            }


            app.UseCors(builder => builder.AllowAnyOrigin()
                                .AllowAnyMethod()
                                .WithHeaders("authorization", "accept", "content-type", "origin"));

            app.UseCors(builder => builder.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin()
               .WithExposedHeaders("content-disposition"));

            app.UseCors("AllowAnyOrigin");

            //Accesing Physical Files like img, pdf
            app.UseStaticFiles();
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                Path.Combine(Directory.GetCurrentDirectory(), "Resources", "ContentFile")),
                RequestPath = "/ContentFile",
                ServeUnknownFileTypes = true,
                DefaultContentType = "video/image"
            });

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                Path.Combine(Directory.GetCurrentDirectory(), "Resources", "AdvtContentFile")),
                RequestPath = "/AdvtContentFile",
                ServeUnknownFileTypes = true,
                DefaultContentType = "video/image"
            });

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
               Path.Combine(Directory.GetCurrentDirectory(), "Resources", "CompanyLogoFile")),
                RequestPath = "/CompanyLogoFile",
                ServeUnknownFileTypes = true,
                DefaultContentType = "video/image"
            });

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                Path.Combine(Directory.GetCurrentDirectory(), "Resources", "PlayerDocumentDetails")),
                RequestPath = "/PlayerDocumentDetails",
                ServeUnknownFileTypes = true,
                DefaultContentType = "image"
            });
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
               Path.Combine(Directory.GetCurrentDirectory(), "Resources", "PlayerDataDetails")),
                RequestPath = "/PlayerDataDetails",
                ServeUnknownFileTypes = true,
                DefaultContentType = "video/image"
            });

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
               Path.Combine(Directory.GetCurrentDirectory(), "Resources", "ScreenShotFile")),
                RequestPath = "/ScreenShotFile",
                ServeUnknownFileTypes = true,
                DefaultContentType = "video/image"
            });

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
              Path.Combine(Directory.GetCurrentDirectory(), "Resources", "ErrorAuditLogFile")),
                RequestPath = "/ErrorAuditLogFile",
                ServeUnknownFileTypes = true,
                
            });

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "IM10.API v1"));
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            
        }
    }
}
