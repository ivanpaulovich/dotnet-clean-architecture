using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;
using TodoList.Core.Boundaries;
using TodoList.Core.Boundaries.List;
using TodoList.Core.Boundaries.Rename;
using TodoList.Core.Boundaries.Todo;
using TodoList.Core.Entities;
using TodoList.Core.Gateways;
using TodoList.Core.UseCases;
using TodoList.Infrastructure.EntityFrameworkDataAccess;
using TodoList.WebApi.Controllers;

namespace TodoList.WebApi
{
    public class StartupProduction
    {
        public StartupProduction(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            AddSwagger(services);
            AddTodoListCore(services);
            AddSQLPersistence(services);
        }

        private void AddSQLPersistence(IServiceCollection services)
        {
            services.AddDbContext<TodoList.Infrastructure.EntityFrameworkDataAccess.TodoContext>(
                options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddScoped<IItemGateway, TodoList.Infrastructure.EntityFrameworkDataAccess.TodoItemGateway>();
        }

        private void AddSwagger(IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "My API (Production)", Version = "v1" });
            });
        }

        private void AddTodoListCore(IServiceCollection services)
        {
            services.AddScoped<IEntitiesFactory, EntitiesFactory>();

            services.AddScoped<Presenter, Presenter>();
            services.AddScoped<IResponseHandler<Core.Boundaries.Todo.Response>>(x => x.GetRequiredService<Presenter>());
            services.AddScoped<IResponseHandler<Core.Boundaries.List.Response>>(x => x.GetRequiredService<Presenter>());

            services.AddScoped<IUseCase<Core.Boundaries.Todo.Request>, Todo>();
            services.AddScoped<Core.Boundaries.Remove.IUseCase, Core.UseCases.Remove>();
            services.AddScoped<Core.Boundaries.List.IUseCase, Core.UseCases.List>();
            services.AddScoped<IUseCase<Core.Boundaries.Rename.Request>, Core.UseCases.Rename>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();

            UseSwagger(app);

            app.UseHttpsRedirection();
            app.UseMvc();
        }

        private void UseSwagger(IApplicationBuilder app)
        {
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                c.RoutePrefix = string.Empty;
            });
        }
    }
}