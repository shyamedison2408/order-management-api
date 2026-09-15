using OrderManagementApi.Models.DTOs;
using OrderManagementApi.Models.Entities;

namespace OrderManagementApi.Repositories;

public interface IOrderRepository
{
    Task<Order> CreateAsync(CreateOrderRequestDto order);

    Task<Order?> GetOrderEntityByIdAsync(int id);

    Task<bool> DeleteAsync(Order order);

    Task<OrderResponseDto?> GetOrderByIdAsync(int id);
}