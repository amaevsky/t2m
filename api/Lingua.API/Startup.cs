using Lingua.API.Realtime;
using Lingua.Data.Mongo;
using Lingua.EmailTemplates;
using Lingua.Services;
using Lingua.Shared;
using Lingua.ZoomIntegration;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
            services.AddMvc();
            services.AddControllers().AddNewtonsoftJson();
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                    .AddCookie(opts =>
                    {
                        opts.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.None;
                    });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Lingua.API", Version = "v1" });
            });

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    builder =>
                    {
                        builder.WithOrigins("http://localhost:3000", "https://staging.t2m.app", "https://t2m.app", "https://www.t2m.app");
                        builder.AllowAnyMethod();
                        builder.AllowAnyHeader();
                        builder.AllowCredentials();
                    });
            });

            services.AddSignalR();
            services.AddAutoMapper(typeof(Startup));


            services.AddSingleton<IAuthClient, AuthClient>();
            services.AddSingleton<IUserClient, UserClient>();
            services.AddSingleton<IUserRepository, UserRepository>();
            services.AddSingleton<IMeetingClient, MeetingClient>();
            services.AddSingleton<IRoomRepository, RoomRepository>();
            services.AddSingleton<IEmailService, SmtpEmailService>();
            services.AddScoped<IRoomService, RoomService>();
            services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
            services.AddScoped<ITemplateProvider, TemplateProvider>();
            services.AddScoped<IViewRenderService, ViewRenderService>();

            services.AddOptions();
            services.Configure<ZoomClientOptions>(Configuration.GetSection("ZoomClientOptions"));
            services.Configure<MongoOptions>(Configuration.GetSection("MongoOptions"));
            services.Configure<SmtpSettings>(Configuration.GetSection("SmtpSettings"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            //if (env.IsDevelopment())
            // {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Lingua.API v1"));
            // }

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseCors();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseMiddleware<LogUserNameMiddleware>();
            app.UseSerilogRequestLogging();

            app.UseMiddleware<ExceptionMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<RoomsHub>("/roomsHub");
            });
        }
    }
}


