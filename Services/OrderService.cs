using Microsoft.EntityFrameworkCore;
using OrderManagementApi.Models.DTOs;
using OrderManagementApi.Models.Entities;
using OrderManagementApi.Repositories;

namespace OrderManagementApi.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _repository;

    public OrderService(IOrderRepository repository)
    {
        _repository = repository;
    }

    public async Task<Order> CreateAsync(CreateOrderRequestDto order)
    {
        if (order.Quantity <= 0)
        {
            throw new ArgumentException("Quantity must be greater than zero.");
        }

        if (order.Price <= 0)
        {
            throw new ArgumentException("Price must be greater than zero.");
        }

        return await _repository.CreateAsync(order);
    }

    public async Task<bool> DeleteAsync(int id, int version)
    {
        if (id <= 0)
        {
            throw new ArgumentException("Invalid order ID.");
        }
    
        var order = await _repository.GetOrderEntityByIdAsync(id);
    
        if (order is null)
        {
            return false;
        }
    
        if (order.Version != version)
        {
            throw new DbUpdateConcurrencyException(
                "The order has been modified by another process.");
        }
    
        return await _repository.DeleteAsync(order);
    }

    public async Task<OrderResponseDto?> GetOrderByIdAsync(int id)
    {
        if (id <= 0)
        {
            throw new ArgumentException("Invalid order ID.");
        }
        return await _repository.GetOrderByIdAsync(id);
    }
}
 