using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using MediatR;
using System.Reflection;
using Streamline.Infrastructure.Persistence.SqlServer.DbContexts;
using Streamline.Infrastructure.Persistence.SqlServer.Repositories;
using Streamline.Application.Customers.CreateCustomer;
using Streamline.Application.Products.CreateProduct;
using Streamline.Application.Orders.CreateOrder;
using Streamline.API.Customers.Routes;
using Streamline.API.Products.Routes;
using Streamline.API.Orders.Routes;
using Streamline.Application.Interfaces.Repositories;
using Streamline.API.Customers.Mapping;
using Streamline.API.Customers.Dtos;
using Streamline.API.Products.Mapping;
using Streamline.API.Products.Dtos;
using Streamline.API.Orders.Mapping;
using Streamline.API.Orders.Dtos;
using Streamline.API.Factory;

var app = AppFactory.CreateApp(args);

app.MapGet("/", () => "API rodando!");

var api = app.MapGroup("/api/v1");

api.MapOrderRoutes();
api.MapProductRoutes();
api.MapCustomerRoutes();

app.Run();
