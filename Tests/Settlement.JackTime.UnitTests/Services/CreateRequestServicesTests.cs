using System.Collections.Generic;
using Settlement.JackTime.Requests;
using Settlement.JackTime.Services;

namespace Settlement.JackTime.UnitTests.Services;

public class CreateRequestServicesTests
{
    private readonly CreateRequestServices service;

    public CreateRequestServicesTests()
    {
        service = new CreateRequestServicesImp();
    }

    [Fact]
    public void CreateVoidCommandRequest_WithSingleLeg_ShouldCreateCorrectCommand()
    {
        // Arrange
        var voidBetInfo = new VoidBetInfo
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
        };

        // Act
        var result = service.CreateVoidCommandRequest(voidBetInfo);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Header);
        Assert.Equal("451678251421532160_20251030002219", result.Header.TransactionId.Id);
        Assert.NotNull(result.Request);
        Assert.NotNull(result.Request.VoidLegs);
        Assert.NotNull(result.Request.VoidLegs.Bet);
        Assert.Equal("T001", result.Request.VoidLegs.Bet.BetId.Id);
        Assert.Equal("1001", result.Request.VoidLegs.Bet.UserId.Id);
        Assert.Single(result.Request.VoidLegs.Legs);

        var leg = result.Request.VoidLegs.Legs[0];
        Assert.Equal("W001", leg.WayId.Id);
        Assert.Equal("71907578", leg.Market.MatchId.Id);
        Assert.Equal("71907578_8_1", leg.Market.MarketId.Id);
    }

    [Fact]
    public void CreateVoidCommandRequest_WithMultipleLegs_ShouldCreateCorrectCommand()
    {
        // Arrange
        var voidBetInfo = new VoidBetInfo
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
        };

        // Act
        var result = service.CreateVoidCommandRequest(voidBetInfo);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Request.VoidLegs.Legs.Count);

        var leg1 = result.Request.VoidLegs.Legs[0];
        Assert.Equal("W001", leg1.WayId.Id);
        Assert.Equal("71907578", leg1.Market.MatchId.Id);
        Assert.Equal("71907578_8_1", leg1.Market.MarketId.Id);

        var leg2 = result.Request.VoidLegs.Legs[1];
        Assert.Equal("W002", leg2.WayId.Id);
        Assert.Equal("71907579", leg2.Market.MatchId.Id);
        Assert.Equal("71907579_8_2", leg2.Market.MarketId.Id);
    }

    [Fact]
    public void CreateUnVoidCommandRequest_WithSingleLeg_ShouldCreateCorrectCommand()
    {
        // Arrange
        var voidBetInfo = new VoidBetInfo
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
        };

        // Act
        var result = service.CreateUnVoidCommandRequest(voidBetInfo);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Header);
        Assert.Equal("451678251421532160_20251030002219", result.Header.TransactionId.Id);
        Assert.NotNull(result.Request);
        Assert.NotNull(result.Request.UnvoidLegs);
        Assert.NotNull(result.Request.UnvoidLegs.Bet);
        Assert.Equal("T001", result.Request.UnvoidLegs.Bet.BetId.Id);
        Assert.Equal("1001", result.Request.UnvoidLegs.Bet.UserId.Id);
        Assert.Single(result.Request.UnvoidLegs.Legs);

        var leg = result.Request.UnvoidLegs.Legs[0];
        Assert.Equal("W001", leg.WayId.Id);
        Assert.Equal("71907578", leg.Market.MatchId.Id);
        Assert.Equal("71907578_8_1", leg.Market.MarketId.Id);
    }

    [Fact]
    public void CreateNotificationsRequest_WithValidOffset_ShouldCreateCorrectRequest()
    {
        // Arrange
        var offset = 12345ul;

        // Act
        var result = service.CreateNotificationsRequest(offset);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Service);
        Assert.Equal(0, (int)result.Service.Index);
        Assert.NotNull(result.Query);
        Assert.Equal(12345UL, result.Query.LastConsumedOffset);
    }

    [Fact]
    public void CreateVoidCommandRequest_WithEmptyLegs_ShouldCreateCommandWithEmptyLegs()
    {
        // Arrange
        var voidBetInfo = new VoidBetInfo
        {
            TransactionId = "451678251421532160_20251030002219",
            TransId = "T001",
            CustId = 1001,
            Legs = new List<Leg>()
        };

        // Act
        var result = service.CreateVoidCommandRequest(voidBetInfo);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Request.VoidLegs.Legs);
        Assert.Empty(result.Request.VoidLegs.Legs);
    }
}