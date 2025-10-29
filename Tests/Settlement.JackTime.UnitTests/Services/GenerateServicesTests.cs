using System.Threading.Tasks;
using Settlement.JackTime.Services;

namespace Settlement.JackTime.UnitTests.Services;

public class GenerateServicesTests
{
    private readonly GenerateServices _service;

    public GenerateServicesTests()
    {
        // Create a real implementation instead of mocking
        var notificationsServices = new NotificationsServicesImp();
        _service = new GenerateServicesImp(notificationsServices);
    }

    [Fact]
    public async Task GetNotificationsAsync_WithValidResponse_ShouldReturnSuccessResult()
    {
        // Arrange
        var validResponse = """
        {
            "queryResult": {
                "resultCode": "SUCCESS",
                "notifications": {
                    "offset": 12345,
                    "notifications": []
                }
            }
        }
        """;

        // Act
        var result = await _service.GetNotificationsAsync(validResponse);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccessful);
        Assert.NotNull(result.Data);
        Assert.Equal(12345ul, result.Data.Offset);
        Assert.Empty(result.Data.Notifications);
    }

    [Fact]
    public async Task GetNotificationsAsync_WithValidResponseWithNotifications_ShouldReturnSuccessResult()
    {
        // Arrange
        var validResponse = """
        {
            "queryResult": {
                "resultCode": "SUCCESS",
                "notifications": {
                    "offset": 54321,
                    "notifications": [
                        {
                            "betBuilderOdds": {
                                "matchId": {"id": "match1"},
                                "bet": {"betId": {"id": "bet1"}},
                                "odds": 1.5
                            }
                        }
                    ]
                }
            }
        }
        """;

        // Act
        var result = await _service.GetNotificationsAsync(validResponse);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccessful);
        Assert.NotNull(result.Data);
        Assert.Equal(54321ul, result.Data.Offset);
        Assert.NotEmpty(result.Data.Notifications);
    }
}