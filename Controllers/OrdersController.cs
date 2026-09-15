using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderManagementApi.Models.DTOs;
using OrderManagementApi.Models.Entities;
using OrderManagementApi.Services;

namespace OrderManagementApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _service;

    public OrdersController(IOrderService service)
    {
        _service = service;
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<Order>> CreateOrder(CreateOrderRequestDto order)
    {
        var createdOrder = await _service.CreateAsync(order);

        return CreatedAtAction(
            nameof(GetOrder),
            new { id = createdOrder.Id },
            createdOrder);
    }
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteOrder(int id,DeleteOrderRequestDto request)
    {
        var deleted = await _service.DeleteAsync(id, request.Version);
        if (!deleted)
        {
            return NotFound();
        }
        return Ok("Order deleted successfully.");
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<OrderResponseDto?>> GetOrder(int id)
    {
        var order = await _service.GetOrderByIdAsync(id);
        return Ok(order);
    }
}