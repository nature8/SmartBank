//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace CustomerService.Tests.Controllers
//{
//    internal class CustomerControllerTests
//    {
//    }
//}


using CustomerService.Controllers;
using CustomerService.Data;
using CustomerService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CustomerService.Tests.Controllers
{
    public class CustomerControllerTests
    {
        private AppDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new AppDbContext(options);
        }

        [Fact]
        public async Task CreateCustomer_ShouldCreateCustomer()
        {
            // Arrange
            var context = GetDbContext();
            var controller = new CustomerController(context);

            var customer = new Customer
            {
                UserId = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "john@test.com",
                PhoneNumber = "9999999999",
                Address = "Indore"
            };

            // Act
            var result = await controller.CreateCustomer(customer);

            // Assert
            var createdResult =
                Assert.IsType<CreatedAtActionResult>(result.Result);

            var createdCustomer =
                Assert.IsType<Customer>(createdResult.Value);

            Assert.Equal("John", createdCustomer.FirstName);
            Assert.Equal("Doe", createdCustomer.LastName);
        }

        [Fact]
        public async Task GetCustomer_ShouldReturnCustomer()
        {
            // Arrange
            var context = GetDbContext();

            context.Customers.Add(new Customer
            {
                CustomerId = 1,
                UserId = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "john@test.com"
            });

            await context.SaveChangesAsync();

            var controller = new CustomerController(context);

            // Act
            var result = await controller.GetCustomer(1);

            // Assert
            var customer = Assert.IsType<Customer>(result.Value);

            Assert.Equal("John", customer.FirstName);
        }

        [Fact]
        public async Task GetCustomer_ShouldReturnNotFound()
        {
            // Arrange
            var context = GetDbContext();
            var controller = new CustomerController(context);

            // Act
            var result = await controller.GetCustomer(999);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetCustomers_ShouldReturnAllCustomers()
        {
            // Arrange
            var context = GetDbContext();

            context.Customers.AddRange(
                new Customer
                {
                    UserId = 1,
                    FirstName = "John"
                },
                new Customer
                {
                    UserId = 2,
                    FirstName = "Jane"
                });

            await context.SaveChangesAsync();

            var controller = new CustomerController(context);

            // Act
            var result = await controller.GetCustomers();

            // Assert
            var customers = Assert.IsAssignableFrom<IEnumerable<Customer>>(result.Value);

            Assert.Equal(2, customers.Count());
        }

        [Fact]
        public async Task UpdateCustomer_ShouldUpdateCustomer()
        {
            // Arrange
            var context = GetDbContext();

            context.Customers.Add(new Customer
            {
                CustomerId = 1,
                UserId = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "old@test.com"
            });

            await context.SaveChangesAsync();

            var controller = new CustomerController(context);

            var updatedCustomer = new Customer
            {
                UserId = 1,
                FirstName = "Johnny",
                LastName = "Smith",
                Email = "new@test.com",
                PhoneNumber = "9999999999",
                Address = "Bhopal"
            };

            // Act
            var result =
                await controller.UpdateCustomer(1, updatedCustomer);

            // Assert
            Assert.IsType<NoContentResult>(result);

            var customer =
                await context.Customers.FindAsync(1);

            Assert.NotNull(customer);
            Assert.Equal("Johnny", customer.FirstName);
            Assert.Equal("Smith", customer.LastName);
            Assert.Equal("new@test.com", customer.Email);
        }

        [Fact]
        public async Task UpdateCustomer_ShouldReturnNotFound()
        {
            // Arrange
            var context = GetDbContext();

            var controller = new CustomerController(context);

            var updatedCustomer = new Customer
            {
                FirstName = "Johnny"
            };

            // Act
            var result =
                await controller.UpdateCustomer(999, updatedCustomer);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteCustomer_ShouldDeleteCustomer()
        {
            // Arrange
            var context = GetDbContext();

            context.Customers.Add(new Customer
            {
                CustomerId = 1,
                UserId = 1,
                FirstName = "John"
            });

            await context.SaveChangesAsync();

            var controller = new CustomerController(context);

            // Act
            var result =
                await controller.DeleteCustomer(1);

            // Assert
            Assert.IsType<NoContentResult>(result);

            var customer =
                await context.Customers.FindAsync(1);

            Assert.Null(customer);
        }

        [Fact]
        public async Task DeleteCustomer_ShouldReturnNotFound()
        {
            // Arrange
            var context = GetDbContext();

            var controller = new CustomerController(context);

            // Act
            var result =
                await controller.DeleteCustomer(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}