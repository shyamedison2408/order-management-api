using OrderManagementApi.Models.DTOs;
using OrderManagementApi.Models.Entities;

namespace OrderManagementApi.Services;

public interface IOrderService
{
    Task<Order> CreateAsync(CreateOrderRequestDto order);

    Task<bool> DeleteAsync(int id, int version);

    Task<OrderResponseDto?> GetOrderByIdAsync(int id);
}