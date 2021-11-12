using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Localization.Routing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using NaturalPersonAPI.DataContext;
using NaturalPersonAPI.Helper;
using NaturalPersonAPI.Middlewares;
using NaturalPersonAPI.Repository;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace NaturalPersonAPI
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

            services.AddLocalization(o => o.ResourcesPath = "Resources");

            services.AddControllers()
                .AddNewtonsoftJson()
                .AddFluentValidation(x => x.RegisterValidatorsFromAssemblyContaining<Startup>());


            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "NaturalPersonAPI", Version = "v1" });
                c.SchemaFilter<EnumSchemaFilter>();
            });


            services.AddSwaggerGenNewtonsoftSupport();

            services.AddAutoMapper(typeof(Startup));

            services.AddDbContext<AppDbContext>(o => o.UseSqlServer(Configuration.GetConnectionString("Default")));

            services.AddScoped<INaturalPersonService, NaturalPersonService>();
            services.AddScoped<IFileProcessingService, FileProcessingService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "NaturalPersonAPI v1"));

            app.UseRouting();

            app.UseAuthorization();

            var supportedCultures = new[] { "ka-GE", "en-US" };
            var localizationOptions = new RequestLocalizationOptions().SetDefaultCulture(supportedCultures[0])
                .AddSupportedCultures(supportedCultures)
                .AddSupportedUICultures(supportedCultures);

            app.UseRequestLocalization(localizationOptions);

            //custom middleware
            app.UseErrorLogging();

            app.UseHttpsRedirection();



            app.UseStaticFiles();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
