using System.Text;
using Lingua.API.Realtime;
using Lingua.Data.Mongo;
using Lingua.EmailTemplates;
using Lingua.Services;
using Lingua.Services.Rooms.Commands;
using Lingua.Shared;
using Lingua.ZoomIntegration;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;

namespace Lingua.API
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
            services.AddControllers().AddNewtonsoftJson();

            var jwtOptionsSection = Configuration
                .GetSection("JwtOptions");
            var jwtOptions = jwtOptionsSection.Get<JwtOptions>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(x =>
                {
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey =
                            new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtOptions.EncryptionKey)),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidIssuer = jwtOptions.Issuer
                    };
                });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "Lingua.API", Version = "v1"});
            });

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    builder =>
                    {
                        builder.AllowAnyOrigin();
                        builder.AllowAnyMethod();
                        builder.AllowAnyHeader();
                    });
            });

            services.AddSignalR();
            services.AddAutoMapper(typeof(AutoMapperProfile), typeof(Data.Mongo.AutoMapperProfile));
            services.AddMediatR(typeof(RoomUpdatesSignalRService), typeof(CreateRoomCommand));


            services.AddSingleton<IAuthClient, AuthClient>();
            services.AddSingleton<IUserClient, UserClient>();
            services.AddSingleton<IUserRepository, UserRepository>();
            services.AddSingleton<IMeetingClient, MeetingClient>();
            services.AddSingleton<IRoomRepository, RoomRepository>();
            services.AddSingleton<IEmailService, SmtpEmailService>();
            services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
            services.AddSingleton<ITemplateProvider, TemplateProvider>();
            services.AddSingleton<IViewRenderService, ViewRenderService>();

            services.AddOptions();
            services.Configure<ZoomClientOptions>(Configuration.GetSection("ZoomClientOptions"));
            services.Configure<AmplitudeOptions>(Configuration.GetSection("AmplitudeOptions"));
            services.Configure<MongoOptions>(Configuration.GetSection("MongoOptions"));
            services.Configure<SmtpSettings>(Configuration.GetSection("SmtpSettings"));
            services.Configure<JwtOptions>(jwtOptionsSection);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Lingua.API v1"));
            }
            else
            {
                app.UseHttpsRedirection();
            }

            app.UseRouting();
            app.UseCors();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseMiddleware<LogUserNameMiddleware>();
            app.UseSerilogRequestLogging();

            app.UseMiddleware<ExceptionMiddleware>();
            app.UseMiddleware<ExceptionMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<RoomsHub>("/roomsHub");
            });
        }
    }
}