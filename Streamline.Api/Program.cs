using DotNetEnv;
using Streamline.Infrastructure.Persistence.SqlServer.DbContexts;
using Streamline.Application.Customers.CreateCustomer;
using Streamline.Application.Repositories;
using Streamline.Infrastructure.Persistence.SqlServer.Repositories;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Streamline.API.Customers.Mapping;
using Streamline.API.Customers.Dtos;
using MediatR;
using System.Reflection;
using Streamline.API.Factory;

var app = AppFactory.CreateApp(args);

app.MapGet("/", () => "API rodando!");

var api = app.MapGroup("/api/v1");

var customer = api.MapGroup("/customer");

customer.MapPost("/", async (CreateCustomerDto dto, IMediator mediator, IMapper mapper) =>
{
    var command = mapper.Map<CreateCustomerCommand>(dto);
    var result = await mediator.Send(command);
    return Results.Ok(result);
})
.WithTags("Customer")
.WithMetadata(new Swashbuckle.AspNetCore.Annotations.SwaggerOperationAttribute(
    summary: "Create a new customer",
    description: "Endpoint to create a customer"
));

app.Run();
