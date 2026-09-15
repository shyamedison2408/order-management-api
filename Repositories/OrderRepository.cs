using Microsoft.EntityFrameworkCore;
using OrderManagementApi.Data;
using OrderManagementApi.Models.DTOs;
using OrderManagementApi.Models.Entities;

namespace OrderManagementApi.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly OrderDbContext _context;

    public OrderRepository(OrderDbContext context)
    {
        _context = context;
    }

    public async Task<Order> CreateAsync(CreateOrderRequestDto order)
    {
        var newOrder = new Order
        {
            CustomerName = order.CustomerName,
            ProductName = order.ProductName,
            Quantity = order.Quantity,
            Price = order.Price
        };
        _context.Orders.Add(newOrder);

        await _context.SaveChangesAsync();

        return newOrder;
    }

    public async Task<Order?> GetOrderEntityByIdAsync(int id)
    {
        return await _context.Orders.FindAsync(id);
    }

    public async Task<OrderResponseDto?> GetOrderByIdAsync(int id)
    {
        return await _context.Orders
            .AsNoTracking()
            .Where(o => o.Id == id)
            .Select(o => new OrderResponseDto
            {
                Id = o.Id,
                CustomerName = o.CustomerName,
                ProductName = o.ProductName,
                Quantity = o.Quantity,
                Price = o.Price,
                Version = o.Version,
            })
            .FirstOrDefaultAsync();
    }

    public async Task<bool> DeleteAsync(Order order)
    {
        _context.Orders.Remove(order);
        var result = await _context.SaveChangesAsync();
        return result > 0;
    }
}