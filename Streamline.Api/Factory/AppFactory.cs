using DotNetEnv;
using Streamline.Infrastructure.Persistence.SqlServer.DbContexts;
using Streamline.Application.Repositories;
using Streamline.Infrastructure.Persistence.SqlServer.Repositories;
using Streamline.Application.Customers.CreateCustomer;
using Microsoft.EntityFrameworkCore;
using MediatR;
using AutoMapper;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.OpenApi;

namespace Streamline.API.Factory
{
    public static class AppFactory
    {
        public static WebApplication CreateApp(string[] args)
        {
            Env.Load();

            var builder = WebApplication.CreateBuilder(args);

            var sqlConnection = Environment.GetEnvironmentVariable("SQLSERVER_CONNECTION");

            builder.Services.AddDbContext<SqlServerDbContext>(options =>
                options.UseSqlServer(sqlConnection));

            builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();

            builder.Services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(CreateCustomerCommandHandler).Assembly);
            });

            builder.Services.AddAutoMapper(typeof(AppFactory));

            builder.Services.AddMvc();

            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "Streamline E-commerce", Version = "v1" });
                options.EnableAnnotations();
            });

            var app = builder.Build();

            app.MapSwagger();
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("v1/swagger.json", "Streamline E-commerce");
            });

            return app;
        }
    }
}
