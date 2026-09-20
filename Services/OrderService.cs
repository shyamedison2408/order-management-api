using Microsoft.EntityFrameworkCore;
using OrderManagementApi.Models.DTOs;
using OrderManagementApi.Models.Entities;
using OrderManagementApi.Repositories;

namespace OrderManagementApi.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _repository;
    private readonly IRedisService _redisService;


    public OrderService(IOrderRepository repository, IRedisService redisService)
    {
        _repository = repository;
        _redisService = redisService;
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

        var deleted = await _repository.DeleteAsync(order);

        if (deleted)
        {
            await _redisService.DeleteAsync($"order:{id}");
        }

        return deleted;
    }

    public async Task<OrderResponseDto?> GetOrderByIdAsync(int id)
    {
        if (id <= 0)
        {
            throw new ArgumentException("Invalid order ID.");
        }

        var cacheKey = $"order:{id}";

        var cachedOrder = await _redisService.GetAsync(cacheKey);

        if (cachedOrder is not null)
        {
            return System.Text.Json.JsonSerializer.Deserialize<OrderResponseDto>(cachedOrder);
        }

        var order = await _repository.GetOrderByIdAsync(id);

        if (order is null)
        {
            return null;
        }

        var serializedOrder = System.Text.Json.JsonSerializer.Serialize(order);

        await _redisService.SetAsync(cacheKey, serializedOrder, TimeSpan.FromMinutes(5));

        return order;
    }
}
