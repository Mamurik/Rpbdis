using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.EntityFrameworkCore;
using RadiostationWeb.Data;
using RadiostationWeb.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Tests
{
    public class EmployeesControllerTests
    {
        private readonly Mock<RadioStationDbContext> _mockContext;
        private readonly EmployeesController _controller;

        public EmployeesControllerTests()
        {
            _mockContext = new Mock<RadioStationDbContext>();
            _controller = new EmployeesController(_mockContext.Object);
        }

        [Fact]
        public async Task Edit_Post_ValidEmployee_UpdatesEmployeeAndRedirectsToIndex()
        {
            // Arrange
            var employee = new Employee { EmployeeId = 1, FullName = "Сергей Петров", Education = "Высшее", Position = "Дирижер" };
            _mockContext.Setup(m => m.Employees.FindAsync(1)).ReturnsAsync(employee);
            _mockContext.Setup(m => m.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // Act
            var result = await _controller.Edit(employee.EmployeeId, "Иванов Иван", "Среднее", "Звукорежиссер");

            // Assert
            _mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }

        [Fact]
        public async Task Create_InvalidEmployee_ReturnsView()
        {
            // Arrange
            var employee = new Employee { FullName = "", Education = "Высшее", Position = "Ведущий" }; // Invalid model
            _controller.ModelState.AddModelError("FullName", "Required");

            // Act
            var result = await _controller.Create(employee);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(employee, viewResult.Model);
        }


        [Fact]
        public async Task Delete_Post_DeletesEmployeeAndRedirectsToIndex()
        {
            // Arrange
            var employee = new Employee { EmployeeId = 1, FullName = "Сергей Петров", Education = "Высшее", Position = "Дирижер" };
            _mockContext.Setup(m => m.Employees.FindAsync(1)).ReturnsAsync(employee);
            _mockContext.Setup(m => m.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // Act
            var result = await _controller.DeleteConfirmed(1);

            // Assert
            _mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }



        [Fact]
        public async Task Create_InvalidEmployee_ReturnsViewWithModelStateErrors()
        {
            // Arrange
            var employee = new Employee { FullName = "", Education = "Высшее", Position = "Ведущий" }; // Invalid model
            _controller.ModelState.AddModelError("FullName", "FullName is required");

            // Act
            var result = await _controller.Create(employee);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(employee, viewResult.Model);
            Assert.True(_controller.ModelState.ContainsKey("FullName"));
        }





        [Fact]
        public async Task Edit_Post_EmployeeNotFound_ReturnsNotFound()
        {
            // Arrange
            var employee = new Employee { EmployeeId = 1, FullName = "Сергей Петров", Education = "Высшее", Position = "Дирижер" };
            _mockContext.Setup(m => m.Employees.FindAsync(1)).ReturnsAsync((Employee)null);

            // Act
            var result = await _controller.Edit(employee.EmployeeId, "Иванов Иван", "Среднее", "Звукорежиссер");

            // Assert
            var notFoundResult = Assert.IsType<NotFoundResult>(result);
        }


        [Fact]
        public async Task Edit_Post_ValidEmployee_UpdatesAndRedirectsToIndex()
        {
            // Arrange
            var employee = new Employee { EmployeeId = 1, FullName = "Сергей Петров", Education = "Высшее", Position = "Дирижер" };
            _mockContext.Setup(m => m.Employees.FindAsync(1)).ReturnsAsync(employee);
            _mockContext.Setup(m => m.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // Act
            var result = await _controller.Edit(employee.EmployeeId, "Иванов Иван", "Среднее", "Звукорежиссер");

            // Assert
            _mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }



        [Fact]
        public async Task Delete_Post_ValidEmployee_DeletesAndRedirectsToIndex()
        {
            // Arrange
            var employee = new Employee { EmployeeId = 1, FullName = "Сергей Петров", Education = "Высшее", Position = "Дирижер" };
            _mockContext.Setup(m => m.Employees.FindAsync(1)).ReturnsAsync(employee);
            _mockContext.Setup(m => m.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // Act
            var result = await _controller.DeleteConfirmed(1);

            // Assert
            _mockContext.Verify(m => m.Employees.Remove(employee), Times.Once);
            _mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }


    }
}
