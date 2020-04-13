using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Mmu.IdentityClient.WebApi
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
            services.AddCors(
                options =>
                {
                    options.AddPolicy(
                        "All",
                        builder =>
                            builder
                                .WithOrigins("http://localhost:4200", "http://localhost:4201")
                                .AllowAnyHeader()
                                .AllowAnyMethod()
                                .AllowCredentials());
                });

            services.AddControllers();

            services.AddAuthorization(
                options =>
                    options.AddPolicy(
                        "WritePolicy",
                        builder =>
                        {
                            builder.RequireScope("api.write");
                        }));

            services.AddAuthentication("Bearer")
                .AddIdentityServerAuthentication(
                    options =>
                    {
                        options.Authority = "https://localhost:44339";
                        options.RequireHttpsMetadata = false;
                        options.ApiName = "IdentityClientWebApi";
                    });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCors("All");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

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
