namespace OrderManagementApi.Models.DTOs;

public class OrderResponseDto
{
    public int Id { get; set; }
    public string CustomerName { get; set; } = string.Empty;

    public string ProductName { get; set; } = string.Empty;

    public int Quantity { get; set; }

    public decimal Price { get; set; }

    public int Version { get; set; }
}