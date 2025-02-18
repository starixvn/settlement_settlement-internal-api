using Settlement.EventManagement.Constants;
using Settlement.EventManagement.Models;
using Settlement.EventManagement.Services;
using Settlement.Internal.Constants;

namespace Settlement.EventManagement.UnitTests.Services;

public class AdjustEventStatusServiceTests
{
    private readonly AdjustEventStatusServiceImp service = new AdjustEventStatusServiceImp();

    [Fact]
    public void EventStatusAbandoned_ResultDetailIsNull_ReturnRightEventStatus()
    {
        // Arrange
        const string expectedStatus = "completed";

        // Act
        var actualStatus = service.EventStatusAbandoned(EventStatus.Completed, null, SportType.BasketBall);

        // Assert
        Assert.Equal(expectedStatus, actualStatus);
    }

    [Fact]
    public void EventStatusAbandoned_BasketballAndAbandonedSDTInRound_ReturnsCorrectStatus()
    {
        // Arrange
        var resultDetails = new List<ResultDetail>
        {
            new () { ResourceId = ResourceType.Halftime, SettlementStatusId = SettlementStatus.Abandoned},
            new () { ResourceId = ResourceType.Final, SettlementStatusId = SettlementStatus.Abandoned },
            new() { ResourceId = "Q1", SettlementStatusId = SettlementStatus.Abandoned }
           };
        const string expectedStatus = "Abandoned Q1";

        // Act
        var actualStatus = service.EventStatusAbandoned(EventStatus.Completed, resultDetails, SportType.BasketBall);

        // Assert
        Assert.Equal(expectedStatus, actualStatus);
    }

    [Fact]
    public void EventStatusAbandoned_BasketballAndAbandonedSDTInFinal_ReturnsCorrectStatus()
    {
        // Arrange
        var resultDetails = new List<ResultDetail>
        {
            new () { ResourceId = ResourceType.Halftime, SettlementStatusId = SettlementStatus.Running},
            new () { ResourceId = ResourceType.Final, SettlementStatusId = SettlementStatus.Abandoned },
            new() { ResourceId = "Q1", SettlementStatusId = SettlementStatus.Running }
           };
        const string expectedStatus = "Abandoned FT";

        // Act
        var actualStatus = service.EventStatusAbandoned(EventStatus.Completed, resultDetails, SportType.BasketBall);

        // Assert
        Assert.Equal(expectedStatus, actualStatus);
    }

    [Fact]
    public void EventStatusAbandoned_BasketballAndAbandonedSDTInHalfTime_ReturnsCorrectStatus()
    {
        // Arrange
        var resultDetails = new List<ResultDetail>
        {
            new () { ResourceId = ResourceType.Halftime, SettlementStatusId = SettlementStatus.Abandoned},
            new () { ResourceId = ResourceType.Final, SettlementStatusId = SettlementStatus.Abandoned },
            new() { ResourceId = "Q1", SettlementStatusId = SettlementStatus.Running }
           };
        const string expectedStatus = "Abandoned HT";

        // Act
        var actualStatus = service.EventStatusAbandoned(EventStatus.Completed, resultDetails, SportType.BasketBall);

        // Assert
        Assert.Equal(expectedStatus, actualStatus);
    }

    [Fact]
    public void EventStatusAbandoned_BaseBallAndExtraInning_ReturnsCorrectStatus()
    {
        // Arrange
        var resultDetails = new List<ResultDetail>
        {
            new () { ResourceId = ResourceType.ExtraInning, SettlementStatusId = SettlementStatus.Abandoned},
        };
        const string expectedStatus = "Abandoned Extra Inning";

        // Act
        var actualStatus = service.EventStatusAbandoned(EventStatus.Completed, resultDetails, SportType.BaseBall);

        // Assert
        Assert.Equal(expectedStatus, actualStatus);
    }

    [Fact]
    public void EventStatusAbandoned_BaseBallAndInning1_ReturnsCorrectStatus()
    {
        // Arrange
        var resultDetails = new List<ResultDetail>
        {
            new () { ResourceId = "001", SettlementStatusId = SettlementStatus.Abandoned},
        };
        const string expectedStatus = "Abandoned 1st Inning";

        // Act
        var actualStatus = service.EventStatusAbandoned(EventStatus.Completed, resultDetails, SportType.BaseBall);

        // Assert
        Assert.Equal(expectedStatus, actualStatus);
    }

    [Fact]
    public void EventStatusAbandoned_BaseBallAndInning2_ReturnsCorrectStatus()
    {
        // Arrange
        var resultDetails = new List<ResultDetail>
        {
            new () { ResourceId = "002", SettlementStatusId = SettlementStatus.Abandoned},
        };
        const string expectedStatus = "Abandoned 2nd Inning";

        // Act
        var actualStatus = service.EventStatusAbandoned(EventStatus.Completed, resultDetails, SportType.BaseBall);

        // Assert
        Assert.Equal(expectedStatus, actualStatus);
    }

    [Fact]
    public void EventStatusAbandoned_BaseBallAndInning3_ReturnsCorrectStatus()
    {
        // Arrange
        var resultDetails = new List<ResultDetail>
        {
            new () { ResourceId = "003", SettlementStatusId = SettlementStatus.Abandoned},
        };
        const string expectedStatus = "Abandoned 3rd Inning";

        // Act
        var actualStatus = service.EventStatusAbandoned(EventStatus.Completed, resultDetails, SportType.BaseBall);

        // Assert
        Assert.Equal(expectedStatus, actualStatus);
    }

    [Fact]
    public void EventStatusAbandoned_BaseBallAndInning4_ReturnsCorrectStatus()
    {
        // Arrange
        var resultDetails = new List<ResultDetail>
        {
            new () { ResourceId = "004", SettlementStatusId = SettlementStatus.Abandoned},
        };
        const string expectedStatus = "Abandoned 4th Inning";

        // Act
        var actualStatus = service.EventStatusAbandoned(EventStatus.Completed, resultDetails, SportType.BaseBall);

        // Assert
        Assert.Equal(expectedStatus, actualStatus);
    }
}