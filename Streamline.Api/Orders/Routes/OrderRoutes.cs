using MediatR;
using AutoMapper;
using Streamline.API.Orders.Dtos;
using Streamline.Application.Orders.CreateOrder;
using Streamline.Application.Orders.ListOrder;
using Streamline.Application.Orders.GetOrderById;
using Streamline.Domain.Enums;

namespace Streamline.API.Orders.Routes
{
    public static class OrderRoutes
    {
        public static void MapOrderRoutes(this IEndpointRouteBuilder app)
        {
            var group = app
                .MapGroup("/order")
                .WithTags("Order");

            group.MapPost("/", async (
                CreateOrderDto dto,
                IMediator mediator,
                IMapper mapper) =>
            {
                var command = mapper.Map<CreateOrderCommand>(dto);
                var result = await mediator.Send(command);
                return Results.Ok(result);
            })
            .WithMetadata(new Swashbuckle.AspNetCore.Annotations.SwaggerOperationAttribute(
                summary: "Create a new order",
                description: "Endpoint to create an order"
            ));

            group.MapGet("/", async (
                EStatusOrder? status,
                int? customerId,
                DateTime? createdFrom,
                DateTime? createdTo,
                IMediator mediator) =>
            {
                var query = new ListOrderQuery
                {
                    Status = status,
                    CustomerId = customerId,
                    CreatedFrom = createdFrom,
                    CreatedTo = createdTo
                };

                var result = await mediator.Send(query);
                return Results.Ok(result);
            })
            .WithMetadata(new Swashbuckle.AspNetCore.Annotations.SwaggerOperationAttribute(
                summary: "List orders",
                description: "List orders filtered by status, customer and creation date"
            ));

            group.MapGet("/{id}", async (int id, IMediator mediator) =>
            {
                var query = new GetOrderByIdQuery
                {
                    Id = id
                };

                var result = await mediator.Send(query);

                if (result == null)
                    return Results.NotFound($"Order with Id {id} not found.");

                return Results.Ok(result);
            })
            .WithMetadata(new Swashbuckle.AspNetCore.Annotations.SwaggerOperationAttribute(
                summary: "Get order by ID",
                description: "Retrieves a single order along with its customer and product details based on the specified order ID."
            ));
        }
    }
}
