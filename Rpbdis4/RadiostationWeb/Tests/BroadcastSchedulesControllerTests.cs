using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using RadiostationWeb.Data;
using RadiostationWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Tests
{
    public class BroadcastSchedulesControllerTests
    {
        private readonly Mock<RadioStationDbContext> _mockContext;
        private readonly BroadcastSchedulesController _controller;

        public BroadcastSchedulesControllerTests()
        {
            _mockContext = new Mock<RadioStationDbContext>();
            _controller = new BroadcastSchedulesController(_mockContext.Object);
        }

        [Fact]
        public async Task Create_ValidBroadcastSchedule_RedirectsToIndex()
        {
            // Arrange
            var schedule = new BroadcastSchedule { EmployeeId = 1, RecordId = 1, BroadcastDate = DateTime.Now.AddDays(1) };
            _mockContext.Setup(m => m.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // Act
            var result = await _controller.Create(schedule.EmployeeId, schedule.RecordId, schedule.BroadcastDate);

            // Assert
            _mockContext.Verify(m => m.Add(It.IsAny<BroadcastSchedule>()), Times.Once);
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }


        [Fact]
        public async Task Edit_Post_ScheduleNotFound_ReturnsNotFound()
        {
            // Arrange
            var scheduleId = 1;
            _mockContext.Setup(m => m.BroadcastSchedules.FindAsync(scheduleId)).ReturnsAsync((BroadcastSchedule)null);

            // Act
            var result = await _controller.Edit(scheduleId, 1, 1, DateTime.Now);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Edit_Post_ValidSchedule_UpdatesAndRedirectsToIndex()
        {
            // Arrange
            var schedule = new BroadcastSchedule { ScheduleId = 1, EmployeeId = 1, RecordId = 1, BroadcastDate = DateTime.Now.AddDays(1) };
            _mockContext.Setup(m => m.BroadcastSchedules.FindAsync(schedule.ScheduleId)).ReturnsAsync(schedule);
            _mockContext.Setup(m => m.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // Act
            var result = await _controller.Edit(schedule.ScheduleId, 2, 2, DateTime.Now.AddDays(2));

            // Assert
            _mockContext.Verify(m => m.Update(schedule), Times.Once);
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }

      

        [Fact]
        public async Task DeleteConfirmed_ScheduleNotFound_RedirectsToIndex()
        {
            // Arrange
            _mockContext.Setup(m => m.BroadcastSchedules.FindAsync(1)).ReturnsAsync((BroadcastSchedule)null);

            // Act
            var result = await _controller.DeleteConfirmed(1);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }

        [Fact]
        public async Task DeleteConfirmed_ValidSchedule_RemovesScheduleAndRedirectsToIndex()
        {
            // Arrange
            var schedule = new BroadcastSchedule { ScheduleId = 1 };
            _mockContext.Setup(m => m.BroadcastSchedules.FindAsync(schedule.ScheduleId)).ReturnsAsync(schedule);
            _mockContext.Setup(m => m.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // Act
            var result = await _controller.DeleteConfirmed(schedule.ScheduleId);

            // Assert
            _mockContext.Verify(m => m.BroadcastSchedules.Remove(schedule), Times.Once);
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }
    }
}