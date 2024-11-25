using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Lab6.Controllers;
using Lab6.Data;
using Lab6.Models;
using Lab6.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Tests
{
    public class BroadcastScheduleControllerTests
    {
        private Mock<RadioStationDbContext> _mockContext;
        private Mock<DbSet<BroadcastSchedule>> _mockDbSet;

        public BroadcastScheduleControllerTests()
        {
            _mockContext = new Mock<RadioStationDbContext>();
            _mockDbSet = new Mock<DbSet<BroadcastSchedule>>();
        }

        [Fact]
        public void Get_ShouldReturnBroadcastSchedules()
        {
            // Arrange
            var schedules = new List<BroadcastSchedule>
            {
                new BroadcastSchedule
                {
                    ScheduleId = 1,
                    BroadcastDate = DateTime.Now,
                    EmployeeId = 1,
                    RecordId = 1,
                    Employee = new Employee { EmployeeId = 1, FullName = "John Doe" },
                    Record = new Lab6.Models.Record { RecordId = 1, Title = "Song A" }
                }
            }.AsQueryable();

            _mockDbSet.As<IQueryable<BroadcastSchedule>>().Setup(m => m.Provider).Returns(schedules.Provider);
            _mockDbSet.As<IQueryable<BroadcastSchedule>>().Setup(m => m.Expression).Returns(schedules.Expression);
            _mockDbSet.As<IQueryable<BroadcastSchedule>>().Setup(m => m.ElementType).Returns(schedules.ElementType);
            _mockDbSet.As<IQueryable<BroadcastSchedule>>().Setup(m => m.GetEnumerator()).Returns(schedules.GetEnumerator());

            _mockContext.Setup(c => c.BroadcastSchedules).Returns(_mockDbSet.Object);
            var controller = new BroadcastScheduleController(_mockContext.Object);

            // Act
            var result = controller.Get();

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("John Doe", result.First().EmployeeName);
        }

     

        [Fact]
        public void Post_ShouldAddBroadcastSchedule()
        {
            // Arrange
            var newSchedule = new BroadcastSchedule
            {
                ScheduleId = 3,
                BroadcastDate = DateTime.Now,
                EmployeeId = 1,
                RecordId = 1
            };

            var schedules = new List<BroadcastSchedule>().AsQueryable();
            _mockDbSet.As<IQueryable<BroadcastSchedule>>().Setup(m => m.Provider).Returns(schedules.Provider);
            _mockDbSet.As<IQueryable<BroadcastSchedule>>().Setup(m => m.Expression).Returns(schedules.Expression);
            _mockDbSet.As<IQueryable<BroadcastSchedule>>().Setup(m => m.ElementType).Returns(schedules.ElementType);
            _mockDbSet.As<IQueryable<BroadcastSchedule>>().Setup(m => m.GetEnumerator()).Returns(schedules.GetEnumerator());

            _mockContext.Setup(c => c.BroadcastSchedules).Returns(_mockDbSet.Object);
            var controller = new BroadcastScheduleController(_mockContext.Object);

            // Act
            var result = controller.Post(newSchedule);

            // Assert
            _mockContext.Verify(c => c.BroadcastSchedules.Add(It.Is<BroadcastSchedule>(bs => bs.ScheduleId == 3)), Times.Once);
            _mockContext.Verify(c => c.SaveChanges(), Times.Once);
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void Put_ShouldUpdateBroadcastSchedule()
        {
            // Arrange
            var existingSchedule = new BroadcastSchedule
            {
                ScheduleId = 1,
                BroadcastDate = DateTime.Now,
                EmployeeId = 1,
                RecordId = 1
            };

            var updatedSchedule = new BroadcastSchedule
            {
                ScheduleId = 1,
                BroadcastDate = DateTime.Now.AddDays(1),
                EmployeeId = 2,
                RecordId = 2
            };

            var schedules = new List<BroadcastSchedule> { existingSchedule }.AsQueryable();

            _mockDbSet.As<IQueryable<BroadcastSchedule>>().Setup(m => m.Provider).Returns(schedules.Provider);
            _mockDbSet.As<IQueryable<BroadcastSchedule>>().Setup(m => m.Expression).Returns(schedules.Expression);
            _mockDbSet.As<IQueryable<BroadcastSchedule>>().Setup(m => m.ElementType).Returns(schedules.ElementType);
            _mockDbSet.As<IQueryable<BroadcastSchedule>>().Setup(m => m.GetEnumerator()).Returns(schedules.GetEnumerator());

            _mockContext.Setup(c => c.BroadcastSchedules).Returns(_mockDbSet.Object);
            var controller = new BroadcastScheduleController(_mockContext.Object);

            // Act
            var result = controller.Put(updatedSchedule);

            // Assert
            _mockContext.Verify(c => c.Update(It.Is<BroadcastSchedule>(bs => bs.ScheduleId == 1 && bs.EmployeeId == 2)), Times.Once);
            _mockContext.Verify(c => c.SaveChanges(), Times.Once);
            Assert.IsType<OkObjectResult>(result);
        }

        
       
        [Fact]
        public void Post_ShouldAddNewBroadcastSchedule()
        {
            // Arrange
            var newSchedule = new BroadcastSchedule { ScheduleId = 4, EmployeeId = 1, RecordId = 1 };

            _mockContext.Setup(c => c.BroadcastSchedules.Add(It.IsAny<BroadcastSchedule>())).Verifiable();
            _mockContext.Setup(c => c.SaveChanges()).Verifiable();

            var controller = new BroadcastScheduleController(_mockContext.Object);

            // Act
            var result = controller.Post(newSchedule);

            // Assert
            _mockContext.Verify(c => c.BroadcastSchedules.Add(It.IsAny<BroadcastSchedule>()), Times.Once);
            _mockContext.Verify(c => c.SaveChanges(), Times.Once);
            Assert.IsType<OkObjectResult>(result); // Ожидаем, что возвращается Ok с добавленным расписанием
        }
       

       
      

        [Fact]
        public void Delete_ShouldRemoveBroadcastSchedule()
        {
            // Arrange
            var existingSchedule = new BroadcastSchedule
            {
                ScheduleId = 1,
                BroadcastDate = DateTime.Now,
                EmployeeId = 1,
                RecordId = 1
            };

            var schedules = new List<BroadcastSchedule> { existingSchedule }.AsQueryable();

            _mockDbSet.As<IQueryable<BroadcastSchedule>>().Setup(m => m.Provider).Returns(schedules.Provider);
            _mockDbSet.As<IQueryable<BroadcastSchedule>>().Setup(m => m.Expression).Returns(schedules.Expression);
            _mockDbSet.As<IQueryable<BroadcastSchedule>>().Setup(m => m.ElementType).Returns(schedules.ElementType);
            _mockDbSet.As<IQueryable<BroadcastSchedule>>().Setup(m => m.GetEnumerator()).Returns(schedules.GetEnumerator());

            _mockContext.Setup(c => c.BroadcastSchedules).Returns(_mockDbSet.Object);
            var controller = new BroadcastScheduleController(_mockContext.Object);

            // Act
            var result = controller.Delete(1);

            // Assert
            _mockContext.Verify(c => c.BroadcastSchedules.Remove(It.Is<BroadcastSchedule>(bs => bs.ScheduleId == 1)), Times.Once);
            _mockContext.Verify(c => c.SaveChanges(), Times.Once);
            Assert.IsType<OkObjectResult>(result);
        }
    }
}
