using Microsoft.EntityFrameworkCore;
using Moq;
using OrderManagementApi.Models.DTOs;
using OrderManagementApi.Models.Entities;
using OrderManagementApi.Repositories;
using OrderManagementApi.Services;

namespace OrderManagementApi.Tests;

public class OrderServiceTests
{
    // ============================================================
    // GET ORDER TESTS
    // ============================================================

    [Fact]
    public async Task GetOrderByIdAsync_ValidId_ReturnsOrder()
    {
        // Arrange
        var mockRepository = new Mock<IOrderRepository>();
        var mockRedis = new Mock<IRedisService>();

        var expectedOrder = new OrderResponseDto
        {
            Id = 1,
            CustomerName = "John",
            ProductName = "Laptop",
            Quantity = 2,
            Price = 50000,
            Version = 1
        };

        mockRepository
            .Setup(r => r.GetOrderByIdAsync(1))
            .ReturnsAsync(expectedOrder);

        var service = new OrderService(mockRepository.Object, mockRedis.Object);

        // Act
        var result = await service.GetOrderByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedOrder.Id, result.Id);
        Assert.Equal(expectedOrder.CustomerName, result.CustomerName);
        Assert.Equal(expectedOrder.ProductName, result.ProductName);
        Assert.Equal(expectedOrder.Quantity, result.Quantity);
        Assert.Equal(expectedOrder.Price, result.Price);
        Assert.Equal(expectedOrder.Version, result.Version);
    }


    [Fact]
    public async Task GetOrderByIdAsync_OrderNotFound_ReturnsNull()
    {
        // Arrange
        var mockRepository = new Mock<IOrderRepository>();

        mockRepository
            .Setup(r => r.GetOrderByIdAsync(999))
            .ReturnsAsync((OrderResponseDto?)null);

        var mockRedis = new Mock<IRedisService>();
        var service = new OrderService(mockRepository.Object, mockRedis.Object);

        // Act
        var result = await service.GetOrderByIdAsync(999);

        // Assert
        Assert.Null(result);
    }


    [Fact]
    public async Task GetOrderByIdAsync_InvalidId_ThrowsArgumentException()
    {
        // Arrange
        var mockRepository = new Mock<IOrderRepository>();
        var mockRedis = new Mock<IRedisService>();
        var service = new OrderService(mockRepository.Object, mockRedis.Object);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => service.GetOrderByIdAsync(0));

        mockRepository.Verify(
            r => r.GetOrderByIdAsync(It.IsAny<int>()),
            Times.Never);
    }


    // ============================================================
    // CREATE ORDER TESTS
    // ============================================================

    [Fact]
    public async Task CreateAsync_ValidOrder_CreatesOrder()
    {
        // Arrange
        var mockRepository = new Mock<IOrderRepository>();

        var request = new CreateOrderRequestDto
        {
            CustomerName = "John",
            ProductName = "Laptop",
            Quantity = 2,
            Price = 50000
        };

        var expectedOrder = new Order
        {
            Id = 1,
            CustomerName = "John",
            ProductName = "Laptop",
            Quantity = 2,
            Price = 50000,
            Version = 1
        };

        mockRepository
            .Setup(r => r.CreateAsync(request))
            .ReturnsAsync(expectedOrder);

        var mockRedis = new Mock<IRedisService>();
        var service = new OrderService(mockRepository.Object, mockRedis.Object);

        // Act
        var result = await service.CreateAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedOrder.Id, result.Id);
        Assert.Equal(expectedOrder.CustomerName, result.CustomerName);
        Assert.Equal(expectedOrder.ProductName, result.ProductName);
        Assert.Equal(expectedOrder.Quantity, result.Quantity);
        Assert.Equal(expectedOrder.Price, result.Price);

        mockRepository.Verify(
            r => r.CreateAsync(request),
            Times.Once);
    }


    [Fact]
    public async Task CreateAsync_QuantityZero_ThrowsArgumentException()
    {
        // Arrange
        var mockRepository = new Mock<IOrderRepository>();

        var request = new CreateOrderRequestDto
        {
            CustomerName = "John",
            ProductName = "Laptop",
            Quantity = 0,
            Price = 50000
        };

        var mockRedis = new Mock<IRedisService>();
        var service = new OrderService(mockRepository.Object, mockRedis.Object);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => service.CreateAsync(request));

        Assert.Equal(
            "Quantity must be greater than zero.",
            exception.Message);

        mockRepository.Verify(
            r => r.CreateAsync(It.IsAny<CreateOrderRequestDto>()),
            Times.Never);
    }


    [Fact]
    public async Task CreateAsync_NegativeQuantity_ThrowsArgumentException()
    {
        // Arrange
        var mockRepository = new Mock<IOrderRepository>();

        var request = new CreateOrderRequestDto
        {
            CustomerName = "John",
            ProductName = "Laptop",
            Quantity = -1,
            Price = 50000
        };

        var mockRedis = new Mock<IRedisService>();
        var service = new OrderService(mockRepository.Object, mockRedis.Object);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => service.CreateAsync(request));

        Assert.Equal(
            "Quantity must be greater than zero.",
            exception.Message);

        mockRepository.Verify(
            r => r.CreateAsync(It.IsAny<CreateOrderRequestDto>()),
            Times.Never);
    }


    [Fact]
    public async Task CreateAsync_PriceZero_ThrowsArgumentException()
    {
        // Arrange
        var mockRepository = new Mock<IOrderRepository>();

        var request = new CreateOrderRequestDto
        {
            CustomerName = "John",
            ProductName = "Laptop",
            Quantity = 2,
            Price = 0
        };

        var mockRedis = new Mock<IRedisService>();
        var service = new OrderService(mockRepository.Object, mockRedis.Object);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => service.CreateAsync(request));

        Assert.Equal(
            "Price must be greater than zero.",
            exception.Message);

        mockRepository.Verify(
            r => r.CreateAsync(It.IsAny<CreateOrderRequestDto>()),
            Times.Never);
    }


    [Fact]
    public async Task CreateAsync_NegativePrice_ThrowsArgumentException()
    {
        // Arrange
        var mockRepository = new Mock<IOrderRepository>();

        var request = new CreateOrderRequestDto
        {
            CustomerName = "John",
            ProductName = "Laptop",
            Quantity = 2,
            Price = -500
        };

        var mockRedis = new Mock<IRedisService>();
        var service = new OrderService(mockRepository.Object, mockRedis.Object);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => service.CreateAsync(request));

        Assert.Equal(
            "Price must be greater than zero.",
            exception.Message);

        mockRepository.Verify(
            r => r.CreateAsync(It.IsAny<CreateOrderRequestDto>()),
            Times.Never);
    }


    // ============================================================
    // DELETE ORDER TESTS
    // ============================================================

    [Fact]
    public async Task DeleteAsync_ValidOrder_DeletesOrder()
    {
        // Arrange
        var mockRepository = new Mock<IOrderRepository>();
        var mockRedis = new Mock<IRedisService>();


        var order = new Order
        {
            Id = 1,
            CustomerName = "John",
            ProductName = "Laptop",
            Quantity = 2,
            Price = 50000,
            Version = 1
        };

        mockRepository
            .Setup(r => r.GetOrderEntityByIdAsync(1))
            .ReturnsAsync(order);

        mockRepository
            .Setup(r => r.DeleteAsync(order))
            .ReturnsAsync(true);

        var service = new OrderService(mockRepository.Object, mockRedis.Object);

        // Act
        var result = await service.DeleteAsync(1, 1);

        // Assert
        Assert.True(result);

        mockRepository.Verify(
            r => r.GetOrderEntityByIdAsync(1),
            Times.Once);

        mockRepository.Verify(
            r => r.DeleteAsync(order),
            Times.Once);
    }


    [Fact]
    public async Task DeleteAsync_OrderNotFound_ReturnsFalse()
    {
        // Arrange
        var mockRepository = new Mock<IOrderRepository>();

        mockRepository
            .Setup(r => r.GetOrderEntityByIdAsync(999))
            .ReturnsAsync((Order?)null);

        var mockRedis = new Mock<IRedisService>();
        var service = new OrderService(mockRepository.Object, mockRedis.Object);

        // Act
        var result = await service.DeleteAsync(999, 1);

        // Assert
        Assert.False(result);

        mockRepository.Verify(
            r => r.DeleteAsync(It.IsAny<Order>()),
            Times.Never);
    }


    [Fact]
    public async Task DeleteAsync_InvalidId_ThrowsArgumentException()
    {
        // Arrange
        var mockRepository = new Mock<IOrderRepository>();
        var mockRedis = new Mock<IRedisService>();

        var service = new OrderService(mockRepository.Object, mockRedis.Object);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => service.DeleteAsync(0, 1));

        Assert.Equal(
            "Invalid order ID.",
            exception.Message);

        mockRepository.Verify(
            r => r.GetOrderEntityByIdAsync(It.IsAny<int>()),
            Times.Never);

        mockRepository.Verify(
            r => r.DeleteAsync(It.IsAny<Order>()),
            Times.Never);
    }


    [Fact]
    public async Task DeleteAsync_VersionMismatch_ThrowsConcurrencyException()
    {
        // Arrange
        var mockRepository = new Mock<IOrderRepository>();
        var mockRedis = new Mock<IRedisService>();

        var order = new Order
        {
            Id = 1,
            CustomerName = "John",
            ProductName = "Laptop",
            Quantity = 2,
            Price = 50000,

            // Database has version 2
            Version = 2
        };

        mockRepository
            .Setup(r => r.GetOrderEntityByIdAsync(1))
            .ReturnsAsync(order);

        var service = new OrderService(mockRepository.Object, mockRedis.Object);

        // Act & Assert
        await Assert.ThrowsAsync<DbUpdateConcurrencyException>(
            () => service.DeleteAsync(1, 1));
    }


    [Fact]
    public async Task DeleteAsync_CorrectVersion_CallsRepositoryDelete()
    {
        // Arrange
        var mockRepository = new Mock<IOrderRepository>();
        var mockRedis = new Mock<IRedisService>();

        var order = new Order
        {
            Id = 1,
            CustomerName = "John",
            ProductName = "Laptop",
            Quantity = 2,
            Price = 50000,
            Version = 5
        };

        mockRepository
            .Setup(r => r.GetOrderEntityByIdAsync(1))
            .ReturnsAsync(order);

        mockRepository
            .Setup(r => r.DeleteAsync(order))
            .ReturnsAsync(true);

        var service = new OrderService(mockRepository.Object, mockRedis.Object);

        // Act
        await service.DeleteAsync(1, 5);

        // Assert
        mockRepository.Verify(
            r => r.DeleteAsync(order),
            Times.Once);
    }


    [Fact]
    public async Task DeleteAsync_VersionMismatch_DoesNotDeleteOrder()
    {
        // Arrange
        var mockRepository = new Mock<IOrderRepository>();

        var order = new Order
        {
            Id = 1,
            CustomerName = "John",
            ProductName = "Laptop",
            Quantity = 2,
            Price = 50000,

            // Actual version in database
            Version = 5
        };

        mockRepository
            .Setup(r => r.GetOrderEntityByIdAsync(1))
            .ReturnsAsync(order);

        var mockRedis = new Mock<IRedisService>();
        var service = new OrderService(mockRepository.Object, mockRedis.Object);

        // Act & Assert
        await Assert.ThrowsAsync<DbUpdateConcurrencyException>(
            () => service.DeleteAsync(1, 4));

        // Delete must NOT happen
        mockRepository.Verify(
            r => r.DeleteAsync(It.IsAny<Order>()),
            Times.Never);
    }


    // ============================================================
    // REPOSITORY INTERACTION TESTS
    // ============================================================

    [Fact]
    public async Task CreateAsync_ValidOrder_CallsRepositoryOnce()
    {
        // Arrange
        var mockRepository = new Mock<IOrderRepository>();

        var request = new CreateOrderRequestDto
        {
            CustomerName = "John",
            ProductName = "Laptop",
            Quantity = 2,
            Price = 50000
        };

        mockRepository
            .Setup(r => r.CreateAsync(request))
            .ReturnsAsync(new Order
            {
                Id = 1,
                CustomerName = "John",
                ProductName = "Laptop",
                Quantity = 2,
                Price = 50000,
                Version = 1
            });

        var mockRedis = new Mock<IRedisService>();
        var service = new OrderService(mockRepository.Object, mockRedis.Object);

        // Act
        await service.CreateAsync(request);

        // Assert
        mockRepository.Verify(
            r => r.CreateAsync(request),
            Times.Once);
    }


    [Fact]
    public async Task GetOrderByIdAsync_ValidId_CallsRepositoryOnce()
    {
        // Arrange
        var mockRepository = new Mock<IOrderRepository>();

        mockRepository
            .Setup(r => r.GetOrderByIdAsync(1))
            .ReturnsAsync(new OrderResponseDto
            {
                Id = 1,
                CustomerName = "John",
                ProductName = "Laptop",
                Quantity = 2,
                Price = 50000,
                Version = 1
            });

        var mockRedis = new Mock<IRedisService>();
        var service = new OrderService(mockRepository.Object, mockRedis.Object);

        // Act
        await service.GetOrderByIdAsync(1);

        // Assert
        mockRepository.Verify(
            r => r.GetOrderByIdAsync(1),
            Times.Once);
    }


    [Fact]
    public async Task DeleteAsync_VersionMismatch_DeleteRepositoryNeverCalled()
    {
        // Arrange
        var mockRepository = new Mock<IOrderRepository>();
        var mockRedis = new Mock<IRedisService>();

        var order = new Order
        {
            Id = 1,
            CustomerName = "John",
            ProductName = "Laptop",
            Quantity = 2,
            Price = 50000,
            Version = 10
        };

        mockRepository
            .Setup(r => r.GetOrderEntityByIdAsync(1))
            .ReturnsAsync(order);

        var service = new OrderService(mockRepository.Object, mockRedis.Object);

        // Act & Assert
        await Assert.ThrowsAsync<DbUpdateConcurrencyException>(
            () => service.DeleteAsync(1, 9));

        // Repository DeleteAsync must never be called
        mockRepository.Verify(
            r => r.DeleteAsync(It.IsAny<Order>()),
            Times.Never);
    }
}