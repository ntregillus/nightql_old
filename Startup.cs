using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Serialization;
using NightQL.Data;
using NightQL.Data.DbEntities;
using Swashbuckle.AspNetCore.Examples;
using Swashbuckle.AspNetCore.Swagger;

namespace NightQL
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            ConfigureAutoMapper();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().AddJsonOptions(options=>
            {
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            });

            services.AddSwaggerGen(cfg => {
                cfg.DescribeAllParametersInCamelCase();
                cfg.SwaggerDoc("v1", new Info {Title="NightQL", Version="v1"});
                cfg.OperationFilter<ExamplesOperationFilter>(); // [SwaggerRequestExample] & [SwaggerResponseExample]
                cfg.OperationFilter<DescriptionOperationFilter>(); // [Description] on Response properties
                cfg.OperationFilter<AuthorizationInputOperationFilter>(); // Adds an Authorization input box to every endpoint
            });
            services.AddDbContext<ConfigContext>(o => 
            {
                o.UseSqlServer(Configuration.GetConnectionString("ConfigConnection"));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
            app.UseSwagger();
            app.UseSwaggerUI(cfg => 
            {
                cfg.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");

            });
        }

        protected void ConfigureAutoMapper()
        {
            Mapper.Initialize(cfg => {
                cfg.CreateMap<DbSchema, Models.Schema>().ReverseMap();
                cfg.CreateMap<DbEntity, Models.Entity>().ReverseMap();
                cfg.CreateMap<DbRelationship, Models.Relationship>().ReverseMap();
            });

        }
    }
}
