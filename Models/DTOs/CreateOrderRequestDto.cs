namespace OrderManagementApi.Models.DTOs;

public class CreateOrderRequestDto
{
    public string CustomerName { get; set; } = string.Empty;

    public string ProductName { get; set; } = string.Empty;

    public int Quantity { get; set; }

    public decimal Price { get; set; }
}