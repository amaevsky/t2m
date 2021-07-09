using Lingua.API.Realtime;
using Lingua.Data.Mongo;
using Lingua.Shared;
using Lingua.ZoomIntegration;
using Lingua.ZoomIntegration.Auth;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

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
            services.AddControllers();
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
                        builder.WithOrigins("http://localhost:3000", "https://lingua-web.azurewebsites.net/");
                        builder.AllowAnyMethod();
                        builder.AllowAnyHeader();
                        builder.AllowCredentials();
                    });
            });

            services.AddSignalR();


            services.AddSingleton<IAuthClient, AuthClient>();
            services.AddSingleton<IUserService, ZoomIntegration.UserService>();
            services.AddSingleton<Shared.Users.IUserService, Data.Mongo.UserService>();
            services.AddSingleton<IMeetingService, MeetingService>();
            services.AddSingleton<IRoomService, RoomService>();
            services.AddSingleton<ITokenProvider, RefreshableTokenProvider>();

            services.AddOptions();
            services.Configure<ZoomClientOptions>(Configuration.GetSection("ZoomClientOptions"));
            services.Configure<MongoOptions>(Configuration.GetSection("MongoOptions"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<RoomsHub>("/roomsHub");
            });
        }
    }
}


