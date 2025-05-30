using System.Text;
using FluentAssertions;
using MeraStore.Services.Logging.Application.Features.Responses.Create;
using MeraStore.Services.Logging.Domain.Models;
using MeraStore.Services.Logging.Domain.Repositories;
using Moq;

namespace MeraStore.Service.Logging.Api.Tests.Features.Responses.Create;

public class CreateApiResponseLogHandlerTests
{
    private readonly Mock<IApiLogRepository> _mockRepo;
    private readonly CreateApiResponseLogHandler _handler;

    public CreateApiResponseLogHandlerTests()
    {
        _mockRepo = new Mock<IApiLogRepository>();
        _handler = new CreateApiResponseLogHandler(_mockRepo.Object);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Create_And_Return_LogEntry()
    {
        // Arrange
        var command = new CreateApiResponseLogCommand(200, Guid.NewGuid(),"{\"result\":true}"u8.ToArray(), 
            "application/json", Guid.NewGuid().ToString());

        // Act
        var result = await _handler.ExecuteAsync(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.StatusCode.Should().Be(command.StatusCode);
        result.RequestId.Should().Be(command.RequestId);
        result.ContentType.Should().Be(command.ContentType);
        result.CorrelationId.Should().Be(command.CorrelationId);
        result.Timestamp.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));

        _mockRepo.Verify(r => r.AddResponseLogAsync(It.IsAny<ApiResponseLog>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Throw_LoggingServiceException_On_Failure()
    {
        // Arrange
        var command =   new CreateApiResponseLogCommand(200, Guid.NewGuid(), Encoding.UTF8.GetBytes(""),
            "application/json", Guid.NewGuid().ToString());

        _mockRepo.Setup(r => r.AddResponseLogAsync(It.IsAny<ApiResponseLog>(), It.IsAny<CancellationToken>()))
                 .ThrowsAsync(new Exception("Database is on fire 🔥", new InvalidOperationException("Inferno inside")));

        // Act
        Func<Task> act = async () => await _handler.ExecuteAsync(command, CancellationToken.None);

        // Assert
        await act.Should()
            .ThrowAsync<Exception>() // If you wrap in BaseAppException later, adjust this
            .Where(ex => ex.Message.Contains("Database is on fire"));

        _mockRepo.Verify(r => r.AddResponseLogAsync(It.IsAny<ApiResponseLog>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
