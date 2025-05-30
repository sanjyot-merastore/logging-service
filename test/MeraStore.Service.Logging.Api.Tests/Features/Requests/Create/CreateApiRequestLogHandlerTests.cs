using System.Text;
using FluentAssertions;
using MeraStore.Services.Logging.Application.Features.Requests.Create;
using MeraStore.Services.Logging.Domain.Models;
using MeraStore.Services.Logging.Domain.Repositories;
using MeraStore.Shared.Kernel.Exceptions.Core;
using Moq;

namespace MeraStore.Service.Logging.Api.Tests.Features.Requests.Create;

public class CreateApiRequestLogHandlerTests
{
    private readonly Mock<IApiLogRepository> _mockRepo;
    private readonly CreateApiRequestLogHandler _handler;

    public CreateApiRequestLogHandlerTests()
    {
        _mockRepo = new Mock<IApiLogRepository>();
        _handler = new CreateApiRequestLogHandler(_mockRepo.Object);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Create_And_Return_LogEntry()
    {
        // Arrange
        var command = new CreateApiRequestLogCommand("POST", "/api/data", Encoding.UTF8.GetBytes("{ \"test\": true }"),
            "application/json", Guid.NewGuid().ToString());

        // Act
        var result = await _handler.ExecuteAsync(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.HttpMethod.Should().Be(command.HttpMethod);
        result.Url.Should().Be(command.Url);
        result.ContentType.Should().Be(command.ContentType);
        result.CorrelationId.Should().Be(command.CorrelationId);
        result.Timestamp.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));

        _mockRepo.Verify(r => r.AddRequestLogAsync(It.IsAny<ApiRequestLog>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Throw_LoggingServiceException_On_Failure()
    {
        // Arrange
        var command = new CreateApiRequestLogCommand("POST", "/api/data", Encoding.UTF8.GetBytes(""),
            "application/json", Guid.NewGuid().ToString());

        _mockRepo.Setup(r => r.AddRequestLogAsync(It.IsAny<ApiRequestLog>(), It.IsAny<CancellationToken>()))
                 .ThrowsAsync(new Exception("DB write failed", new InvalidOperationException("Inner boom")));

        // Act
        Func<Task> act = async () => await _handler.ExecuteAsync(command, CancellationToken.None);

        // Assert
        await act.Should()
            .ThrowAsync<BaseAppException>()
            .Where(ex => ex.Message.Contains("DB write failed") && ex.InnerException is InvalidOperationException);

        _mockRepo.Verify(r => r.AddRequestLogAsync(It.IsAny<ApiRequestLog>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
