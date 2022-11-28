using AutoMapper;
using FluentValidation.AspNetCore;
using MerchantForm.Context;
using MerchantForm.Contexts.WebMarchant;
using MerchantForm.Converter;
using MerchantForm.Services;
using MerchantForm.Services.Interfaces;
using MerchantForm.Validator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MerchantForm
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
            var connectionString = Configuration["ConnectionStrings:FormDatabase"];

            services.AddDbContext<WebmerchantContext>(option => option.UseSqlServer(connectionString));
           
            services.AddCors(options => options.AddPolicy("AllowEverything", builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "MerchantForm", Version = "v1" });
            });
            services.AddFluentValidation(options =>
            {
                options.RegisterValidatorsFromAssemblyContaining<Startup>();
               // options.RunDefaultMvcValidationAfterFluentValidationExecutes = false;
                //  options.AutomaticValidationEnabled = true;
                //  options.ImplicitlyValidateChildProperties = true;
                //  options.ConfigureClientsideValidation(enabled :false);
            });
            services.AddScoped<IMerchantFormValidator,MerchantFormValidator>();
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddScoped<IDirectorConcatService,DirectorConcatService>();
            services.AddScoped<IImageAndByteArrayConverter,ImageAndByteArrayConverter>();
            services.AddSingleton<IEmailService, EmailService>();
            services.AddScoped< IAccountDetailsService,AccountDetailsService>();
            services.AddScoped<ICharacterValidator,CharacterValidator>();
            services.AddScoped<IFormService,FormService> ();
            services.AddScoped<IManagerService, ManagerService>();
            services.AddScoped < IOtpService, OtpService> ();
         
            services.AddScoped<IFinnacleService, FinnacleService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                
            }
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("./v1/swagger.json", "MerchantForm v1"));

     //     app.UseHttpsRedirection();

            app.UseRouting();
          
            app.UseCors("AllowEverything");

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
