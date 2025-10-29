using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using NSubstitute;
using Settlement.Internal.Models;
using Settlement.JackTime.Models;
using Settlement.JackTime.Services;

namespace Settlement.JackTime.UnitTests.Services;

public class JackTimeServicesTests
{
    private readonly IOptions<AppSettingsKey> _mockAppSettingsKey;
    private readonly InvokeExternalAPIServices _mockInvokeExternalApiServices;
    private readonly GenerateServices _mockGenerateServices;
    private readonly JackTimeServices _service;

    public JackTimeServicesTests()
    {
        _mockAppSettingsKey = Substitute.For<IOptions<AppSettingsKey>>();
        _mockInvokeExternalApiServices = Substitute.For<InvokeExternalAPIServices>();
        _mockGenerateServices = Substitute.For<GenerateServices>();
        _service = new JackTimeServicesImp(_mockAppSettingsKey, _mockInvokeExternalApiServices, _mockGenerateServices);
    }

    [Fact]
    public async Task GetNotificationsAsync_WithValidOffset_ShouldReturnSuccessResult()
    {
        // Arrange
        var offset = 12345ul;
        var expectedUrl = "https://api.jacktime.com/query";
        var apiResponse = "{\"queryResult\":{\"resultCode\":\"SUCCESS\",\"notifications\":{\"offset\":12345,\"notifications\":[]}}}";
        var expectedResult = new Results<NotificationsModels>
        {
            Data = new NotificationsModels { Offset = 12345, Notifications = [] },
            ErrMsg = "SUCCESS"
        };

        var appSettings = new AppSettingsKey
        {
            ThirdPartyUri = new ThirdPartySettingKeys
            {
                JackTimeNotifications = "https://api.jacktime.com/"
            }
        };
        _mockAppSettingsKey.Value.Returns(appSettings);
        _mockInvokeExternalApiServices.PostAsync(Arg.Any<string>(), Arg.Any<string>()).Returns(apiResponse);
        _mockGenerateServices.GetNotificationsAsync(apiResponse).Returns(Task.FromResult(expectedResult));

        // Act
        var result = await _service.GetNotificationsAsync(offset);

        // Assert
        Assert.NotNull(result);
        if (!result.IsSuccessful)
        {
            Console.WriteLine($"Error message: {result.ErrMsg}");
        }
        Assert.True(result.IsSuccessful);
        Assert.Equal("SUCCESS", result.ErrMsg);
        Assert.NotNull(result.Data);
        Assert.Equal(12345ul, result.Data.Offset);

        await _mockInvokeExternalApiServices.Received(1).PostAsync(
            Arg.Is<string>(s => s.Contains("lastConsumedOffset") && s.Contains("12345")),
            Arg.Is<string>(url => url == expectedUrl)
        );
        await _mockGenerateServices.Received(1).GetNotificationsAsync(apiResponse);
    }

    [Fact]
    public async Task GetNotificationsAsync_WithZeroOffset_ShouldHandleCorrectly()
    {
        // Arrange
        var offset = 0ul;
        var expectedUrl = "https://api.jacktime.com/query";
        var apiResponse = "{\"queryResult\":{\"resultCode\":\"SUCCESS\",\"notifications\":{\"offset\":0,\"notifications\":[]}}}";
        var expectedResult = new Results<NotificationsModels>
        {
            Data = new NotificationsModels { Offset = 0, Notifications = [] },
            ErrMsg = "SUCCESS"
        };

        var appSettings = new AppSettingsKey
        {
            ThirdPartyUri = new ThirdPartySettingKeys
            {
                JackTimeNotifications = "https://api.jacktime.com/"
            }
        };
        _mockAppSettingsKey.Value.Returns(appSettings);
        _mockInvokeExternalApiServices.PostAsync(Arg.Any<string>(), Arg.Any<string>()).Returns(apiResponse);
        _mockGenerateServices.GetNotificationsAsync(apiResponse).Returns(Task.FromResult(expectedResult));

        // Act
        var result = await _service.GetNotificationsAsync(offset);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccessful);
        Assert.Equal("SUCCESS", result.ErrMsg);
        Assert.NotNull(result.Data);
        Assert.Equal(0ul, result.Data.Offset);

        await _mockInvokeExternalApiServices.Received(1).PostAsync(
            Arg.Is<string>(s => s.Contains("query")),
            Arg.Is<string>(url => url == expectedUrl)
        );
    }

    [Fact]
    public async Task GetNotificationsAsync_WithLargeOffset_ShouldHandleCorrectly()
    {
        // Arrange
        var offset = 999999999ul;
        var expectedUrl = "https://api.jacktime.com/query";
        var apiResponse = "{\"queryResult\":{\"resultCode\":\"SUCCESS\",\"notifications\":{\"offset\":999999999,\"notifications\":[]}}}";
        var expectedResult = new Results<NotificationsModels>
        {
            Data = new NotificationsModels { Offset = 999999999, Notifications = [] },
            ErrMsg = "SUCCESS"
        };

        var appSettings = new AppSettingsKey
        {
            ThirdPartyUri = new ThirdPartySettingKeys
            {
                JackTimeNotifications = "https://api.jacktime.com/"
            }
        };
        _mockAppSettingsKey.Value.Returns(appSettings);
        _mockInvokeExternalApiServices.PostAsync(Arg.Any<string>(), Arg.Any<string>()).Returns(apiResponse);
        _mockGenerateServices.GetNotificationsAsync(apiResponse).Returns(Task.FromResult(expectedResult));

        // Act
        var result = await _service.GetNotificationsAsync(offset);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccessful);
        Assert.Equal("SUCCESS", result.ErrMsg);
        Assert.NotNull(result.Data);
        Assert.Equal(999999999ul, result.Data.Offset);

        await _mockInvokeExternalApiServices.Received(1).PostAsync(
            Arg.Is<string>(s => s.Contains("lastConsumedOffset") && s.Contains("999999999")),
            Arg.Is<string>(url => url == expectedUrl)
        );
    }

    [Fact]
    public async Task GetNotificationsAsync_WithDifferentBaseUrl_ShouldUseCorrectUrl()
    {
        // Arrange
        var offset = 12345ul;
        var expectedUrl = "https://different-api.com/query";
        var apiResponse = "{\"queryResult\":{\"resultCode\":\"SUCCESS\",\"notifications\":{\"offset\":12345,\"notifications\":[]}}}";
        var expectedResult = new Results<NotificationsModels>
        {
            Data = new NotificationsModels { Offset = 12345, Notifications = [] },
            ErrMsg = "SUCCESS"
        };

        var appSettings = new AppSettingsKey
        {
            ThirdPartyUri = new ThirdPartySettingKeys
            {
                JackTimeNotifications = "https://different-api.com/"
            }
        };
        _mockAppSettingsKey.Value.Returns(appSettings);
        _mockInvokeExternalApiServices.PostAsync(Arg.Any<string>(), Arg.Any<string>()).Returns(apiResponse);
        _mockGenerateServices.GetNotificationsAsync(apiResponse).Returns(Task.FromResult(expectedResult));

        // Act
        var result = await _service.GetNotificationsAsync(offset);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccessful);

        await _mockInvokeExternalApiServices.Received(1).PostAsync(
            Arg.Any<string>(),
            Arg.Is<string>(url => url == expectedUrl)
        );
    }

    [Fact]
    public async Task GetNotificationsAsync_WhenGenerateServiceReturnsError_ShouldReturnErrorResult()
    {
        // Arrange
        var offset = 12345ul;
        var errorResult = new Results<NotificationsModels>
        {
            Data = null,
            ErrMsg = "API call failed"
        };

        var appSettings = new AppSettingsKey
        {
            ThirdPartyUri = new ThirdPartySettingKeys
            {
                JackTimeNotifications = "https://api.jacktime.com/"
            }
        };
        _mockAppSettingsKey.Value.Returns(appSettings);
        _mockInvokeExternalApiServices.PostAsync(Arg.Any<string>(), Arg.Any<string>()).Returns("some response");
        _mockGenerateServices.GetNotificationsAsync(Arg.Any<string>()).Returns(Task.FromResult(errorResult));

        // Act
        var result = await _service.GetNotificationsAsync(offset);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.IsSuccessful);
        Assert.Equal("API call failed", result.ErrMsg);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task GetNotificationsAsync_ShouldCreateCorrectRequestStructure()
    {
        // Arrange
        var offset = 54321ul;
        var apiResponse = "{\"queryResult\":{\"resultCode\":\"SUCCESS\",\"notifications\":{\"offset\":54321,\"notifications\":[]}}}";
        var expectedResult = new Results<NotificationsModels>
        {
            Data = new NotificationsModels { Offset = 54321, Notifications = [] },
            ErrMsg = "SUCCESS"
        };

        var appSettings = new AppSettingsKey
        {
            ThirdPartyUri = new ThirdPartySettingKeys
            {
                JackTimeNotifications = "https://api.jacktime.com/"
            }
        };
        _mockAppSettingsKey.Value.Returns(appSettings);
        _mockInvokeExternalApiServices.PostAsync(Arg.Any<string>(), Arg.Any<string>()).Returns(apiResponse);
        _mockGenerateServices.GetNotificationsAsync(apiResponse).Returns(Task.FromResult(expectedResult));

        // Act
        await _service.GetNotificationsAsync(offset);

        // Assert
        await _mockInvokeExternalApiServices.Received(1).PostAsync(
            Arg.Is<string>(s =>
                s.Contains("service") &&
                s.Contains("query") &&
                s.Contains("lastConsumedOffset")),
            Arg.Any<string>()
        );
    }
}