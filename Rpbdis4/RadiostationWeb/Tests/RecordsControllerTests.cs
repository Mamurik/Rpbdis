using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using RadiostationWeb.Data;
using RadiostationWeb.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Tests
{
    public class RecordsControllerTests
    {
        private readonly Mock<RadioStationDbContext> _mockContext;
        private readonly RecordsController _controller;

        public RecordsControllerTests()
        {
            _mockContext = new Mock<RadioStationDbContext>();
            _controller = new RecordsController(_mockContext.Object);
        }
      


        [Fact]
        public async Task Edit_ValidRecord_UpdatesAndRedirectsToIndex()
        {
            // Arrange
            var record = new RadiostationWeb.Models.Record { RecordId = 1, Title = "Old Record", Album = "Old Album", Year = 2022, ArtistId = 1, GenreId = 1 };
            _mockContext.Setup(m => m.Records.FindAsync(record.RecordId)).ReturnsAsync(record);
            _mockContext.Setup(m => m.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // Act
            var result = await _controller.Edit(record.RecordId, "Updated Record", "Updated Album", 2023, record.ArtistId, record.GenreId);

            // Assert
            _mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }

        [Fact]
        public async Task Edit_RecordNotFound_ReturnsNotFound()
        {
            // Arrange
            var recordId = 1;
            _mockContext.Setup(m => m.Records.FindAsync(recordId)).ReturnsAsync((RadiostationWeb.Models.Record)null);

            // Act
            var result = await _controller.Edit(recordId, "Updated Record", "Updated Album", 2023, 1, 1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }


        [Fact]
        public async Task DeleteConfirmed_ValidId_RemovesRecordAndRedirectsToIndex()
        {
            // Arrange
            var record = new RadiostationWeb.Models.Record { RecordId = 1 };
            _mockContext.Setup(m => m.Records.FindAsync(record.RecordId)).ReturnsAsync(record);
            _mockContext.Setup(m => m.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // Act
            var result = await _controller.DeleteConfirmed(record.RecordId);

            // Assert
            _mockContext.Verify(m => m.Records.Remove(record), Times.Once);
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }

        [Fact]
        public async Task DeleteConfirmed_InvalidId_RedirectsToIndex()
        {
            // Arrange
            _mockContext.Setup(m => m.Records.FindAsync(1)).ReturnsAsync((RadiostationWeb.Models.Record)null);

            // Act
            var result = await _controller.DeleteConfirmed(1);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }




        [Fact]
        public async Task Create_ValidRecord_AddsRecordAndRedirectsToIndex()
        {
            // Arrange
            var title = "New Record";
            var album = "New Album";
            var year = 2023;
            var artistId = 1;
            var genreId = 1;

            // Act
            var result = await _controller.Create(title, album, year, artistId, genreId);

            // Assert
            _mockContext.Verify(m => m.Add(It.IsAny<RadiostationWeb.Models.Record>()), Times.Once);
            _mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }

        

    }
}