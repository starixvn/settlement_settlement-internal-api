using System.Collections.Generic;
using System.Threading.Tasks;
using Fanex.Logging;
using Fanex.Logging.Sentry.RavenClient;
using Microsoft.Extensions.Options;
using NSubstitute;
using Settlement.Internal.Models;
using Settlement.JackTime.Models;
using Settlement.JackTime.Requests;
using Settlement.JackTime.Services;
using SharpRaven;

namespace Settlement.JackTime.UnitTests.Services;

public class VoidLegServicesTests
{
    private readonly VoidLegsServicesImp service;
    private readonly IOptions<AppSettingsKey> mockAppSettingsKey;
    private readonly InvokeExternalAPIServices mockInvokeExternalApiServices;
    private readonly GenerateServices mockGenerateServices;
    private readonly CreateRequestServices mockCreateRequestServices;

    public VoidLegServicesTests()
    {
        mockAppSettingsKey = Substitute.For<IOptions<AppSettingsKey>>();
        mockInvokeExternalApiServices = Substitute.For<InvokeExternalAPIServices>();
        mockGenerateServices = Substitute.For<GenerateServices>();
        mockCreateRequestServices = Substitute.For<CreateRequestServices>();

        var logEngine = new List<ILogEngine>();

        var sentryEngineOptions = new SentryEngineOptions
        {
            Dsn = new Dsn("http://2dfee6c5bbc844558ad3bef5f4b59ee1@sentry.nexdev.net/9")
        };
        logEngine.Add(new SentryLogging(sentryEngineOptions));

        LogManager.SetDefaultLogCategory("NAP-Settlement-LOCAL").Use(logEngine);

        service = new VoidLegsServicesImp(mockAppSettingsKey, mockInvokeExternalApiServices, mockGenerateServices, mockCreateRequestServices);
    }

    [Fact]
    public async Task VoidLegsAsync_WithSingleVoidBetInfo_ShouldReturnSuccessResult()
    {
        // Arrange
        var voidLegRequest = new VoidLegRequest
        {
            VoidBetInfos = new List<VoidBetInfo>
            {
                new()
                {
                    TransactionId = "451678251421532160_20251030002219",
                    TransId = "T001",
                    CustId = 1001,
                    Legs = new List<Leg>
                    {
                        new()
                        {
                            WayId = "W001",
                            MatchId = 71907578,
                            MarketId = "71907578_8_1"
                        }
                    }
                }
            }
        };

        var appSettings = new AppSettingsKey
        {
            ThirdPartyUri = new ThirdPartySettingKeys
            {
                JackTimeCommand = "https://api.jacktime.com/command"
            }
        };
        mockAppSettingsKey.Value.Returns(appSettings);
        mockInvokeExternalApiServices.PostAsync(Arg.Any<string>(), Arg.Any<string>()).Returns("success response");
        mockGenerateServices.CommandReplyMessageAsync(Arg.Any<string>()).Returns(new Results<string> { ErrMsg = "Success" });

        // Mock CreateRequestServices to return a valid CommandRequestMessage
        var mockCommandRequest = new CommandRequestMessage();
        mockCreateRequestServices.CreateVoidCommandRequest(Arg.Any<VoidBetInfo>()).Returns(mockCommandRequest);

        // Act
        var result = await service.VoidLegsAsync(voidLegRequest);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccessful);
        Assert.Equal("Success", result.ErrMsg);
    }

    [Fact]
    public async Task VoidLegsAsync_WithMultipleVoidBetInfos_ShouldReturnSuccessResult()
    {
        // Arrange
        var voidLegRequest = new VoidLegRequest
        {
            VoidBetInfos = new List<VoidBetInfo>
            {
                new()
                {
                    TransactionId = "451678251421532160_20251030002219",
                    TransId = "T001",
                    CustId = 1001,
                    Legs = new List<Leg>
                    {
                        new()
                        {
                            WayId = "W001",
                            MatchId = 71907578,
                            MarketId = "71907578_8_1"
                        },
                        new()
                        {
                            WayId = "W002",
                            MatchId = 71907579,
                            MarketId = "71907579_8_2"
                        }
                    }
                },
                new()
                {
                    TransactionId = "451678251421532160_20251030002219",
                    TransId = "T002",
                    CustId = 1002,
                    Legs = new List<Leg>
                    {
                        new()
                        {
                            WayId = "W003",
                            MatchId = 71907580,
                            MarketId = "71907580_8_3"
                        }
                    }
                }
            }
        };

        var appSettings = new AppSettingsKey
        {
            ThirdPartyUri = new ThirdPartySettingKeys
            {
                JackTimeCommand = "https://api.jacktime.com/command"
            }
        };
        mockAppSettingsKey.Value.Returns(appSettings);
        mockInvokeExternalApiServices.PostAsync(Arg.Any<string>(), Arg.Any<string>()).Returns("success response");
        mockGenerateServices.CommandReplyMessageAsync(Arg.Any<string>()).Returns(new Results<string> { ErrMsg = "Success" });

        // Mock CreateRequestServices to return a valid CommandRequestMessage
        var mockCommandRequest = new CommandRequestMessage();
        mockCreateRequestServices.CreateVoidCommandRequest(Arg.Any<VoidBetInfo>()).Returns(mockCommandRequest);

        // Act
        var result = await service.VoidLegsAsync(voidLegRequest);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccessful);
        Assert.Equal("Success", result.ErrMsg);
    }

    [Fact]
    public async Task VoidLegsAsync_WithEmptyVoidBetInfos_ShouldReturnSuccessResult()
    {
        // Arrange
        var voidLegRequest = new VoidLegRequest
        {
            VoidBetInfos = new List<VoidBetInfo>()
        };

        // Act
        var result = await service.VoidLegsAsync(voidLegRequest);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccessful);
        Assert.Equal("Success", result.ErrMsg);
    }

    [Fact]
    public async Task VoidLegsAsync_WithErrorResponse_ShouldReturnErrorResult()
    {
        // Arrange
        var voidLegRequest = new VoidLegRequest
        {
            VoidBetInfos = new List<VoidBetInfo>
            {
                new()
                {
                    TransactionId = "451678251421532160_20251030002219",
                    TransId = "T001",
                    CustId = 1001,
                    Legs = new List<Leg>
                    {
                        new()
                        {
                            WayId = "W001",
                            MatchId = 71907578,
                            MarketId = "71907578_8_1"
                        }
                    }
                }
            }
        };

        var appSettings = new AppSettingsKey
        {
            ThirdPartyUri = new ThirdPartySettingKeys
            {
                JackTimeCommand = "https://api.jacktime.com/command"
            }
        };
        mockAppSettingsKey.Value.Returns(appSettings);
        mockInvokeExternalApiServices.PostAsync(Arg.Any<string>(), Arg.Any<string>()).Returns("error response");
        mockGenerateServices.CommandReplyMessageAsync(Arg.Any<string>()).Returns(new Results<string> { ErrMsg = "API_ERROR" });

        // Mock CreateRequestServices to return a valid CommandRequestMessage
        var mockCommandRequest = new CommandRequestMessage();
        mockCreateRequestServices.CreateVoidCommandRequest(Arg.Any<VoidBetInfo>()).Returns(mockCommandRequest);

        // Act
        var result = await service.VoidLegsAsync(voidLegRequest);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.IsSuccessful);
        Assert.Equal("API_ERROR", result.ErrMsg);
    }

    [Fact]
    public async Task UnVoidLegsAsync_WithSingleVoidBetInfo_ShouldReturnSuccessResult()
    {
        // Arrange
        var voidLegRequest = new VoidLegRequest
        {
            VoidBetInfos = new List<VoidBetInfo>
            {
                new()
                {
                    TransactionId = "451678251421532160_20251030002219",
                    TransId = "T001",
                    CustId = 1001,
                    Legs = new List<Leg>
                    {
                        new()
                        {
                            WayId = "W001",
                            MatchId = 71907578,
                            MarketId = "71907578_8_1"
                        }
                    }
                }
            }
        };

        var appSettings = new AppSettingsKey
        {
            ThirdPartyUri = new ThirdPartySettingKeys
            {
                JackTimeCommand = "https://api.jacktime.com/command"
            }
        };
        mockAppSettingsKey.Value.Returns(appSettings);
        mockInvokeExternalApiServices.PostAsync(Arg.Any<string>(), Arg.Any<string>()).Returns("success response");
        mockGenerateServices.CommandReplyMessageAsync(Arg.Any<string>()).Returns(new Results<string> { ErrMsg = "Success" });

        // Mock CreateRequestServices to return a valid CommandRequestMessage
        var mockCommandRequest = new CommandRequestMessage();
        mockCreateRequestServices.CreateUnVoidCommandRequest(Arg.Any<VoidBetInfo>()).Returns(mockCommandRequest);

        // Act
        var result = await service.UnVoidLegsAsync(voidLegRequest);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccessful);
        Assert.Equal("Success", result.ErrMsg);
    }

    [Fact]
    public async Task UnVoidLegsAsync_WithMultipleVoidBetInfos_ShouldReturnSuccessResult()
    {
        // Arrange
        var voidLegRequest = new VoidLegRequest
        {
            VoidBetInfos = new List<VoidBetInfo>
            {
                new()
                {
                    TransactionId = "451678251421532160_20251030002219",
                    TransId = "T001",
                    CustId = 1001,
                    Legs = new List<Leg>
                    {
                        new()
                        {
                            WayId = "W001",
                            MatchId = 71907578,
                            MarketId = "71907578_8_1"
                        }
                    }
                },
                new()
                {
                    TransactionId = "451678251421532160_20251030002219",
                    TransId = "T002",
                    CustId = 1002,
                    Legs = new List<Leg>
                    {
                        new()
                        {
                            WayId = "W003",
                            MatchId = 71907580,
                            MarketId = "71907580_8_3"
                        }
                    }
                }
            }
        };

        var appSettings = new AppSettingsKey
        {
            ThirdPartyUri = new ThirdPartySettingKeys
            {
                JackTimeCommand = "https://api.jacktime.com/command"
            }
        };
        mockAppSettingsKey.Value.Returns(appSettings);
        mockInvokeExternalApiServices.PostAsync(Arg.Any<string>(), Arg.Any<string>()).Returns("success response");
        mockGenerateServices.CommandReplyMessageAsync(Arg.Any<string>()).Returns(new Results<string> { ErrMsg = "Success" });

        // Mock CreateRequestServices to return a valid CommandRequestMessage
        var mockCommandRequest = new CommandRequestMessage();
        mockCreateRequestServices.CreateUnVoidCommandRequest(Arg.Any<VoidBetInfo>()).Returns(mockCommandRequest);

        // Act
        var result = await service.UnVoidLegsAsync(voidLegRequest);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccessful);
        Assert.Equal("Success", result.ErrMsg);
    }

    [Fact]
    public async Task UnVoidLegsAsync_WithErrorResponse_ShouldReturnErrorResult()
    {
        // Arrange
        var voidLegRequest = new VoidLegRequest
        {
            VoidBetInfos = new List<VoidBetInfo>
            {
                new()
                {
                    TransactionId = "451678251421532160_20251030002219",
                    TransId = "T001",
                    CustId = 1001,
                    Legs = new List<Leg>
                    {
                        new()
                        {
                            WayId = "W001",
                            MatchId = 71907578,
                            MarketId = "71907578_8_1"
                        }
                    }
                }
            }
        };

        var appSettings = new AppSettingsKey
        {
            ThirdPartyUri = new ThirdPartySettingKeys
            {
                JackTimeCommand = "https://api.jacktime.com/command"
            }
        };
        mockAppSettingsKey.Value.Returns(appSettings);
        mockInvokeExternalApiServices.PostAsync(Arg.Any<string>(), Arg.Any<string>()).Returns("error response");
        mockGenerateServices.CommandReplyMessageAsync(Arg.Any<string>()).Returns(new Results<string> { ErrMsg = "API_ERROR" });

        // Mock CreateRequestServices to return a valid CommandRequestMessage
        var mockCommandRequest = new CommandRequestMessage();
        mockCreateRequestServices.CreateUnVoidCommandRequest(Arg.Any<VoidBetInfo>()).Returns(mockCommandRequest);

        // Act
        var result = await service.UnVoidLegsAsync(voidLegRequest);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.IsSuccessful);
        Assert.Equal("API_ERROR", result.ErrMsg);
    }

    [Fact]
    public async Task UnVoidLegsAsync_WithEmptyVoidBetInfos_ShouldReturnSuccessResult()
    {
        // Arrange
        var voidLegRequest = new VoidLegRequest
        {
            VoidBetInfos = new List<VoidBetInfo>()
        };

        // Act
        var result = await service.UnVoidLegsAsync(voidLegRequest);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccessful);
        Assert.Equal("Success", result.ErrMsg);
    }
}