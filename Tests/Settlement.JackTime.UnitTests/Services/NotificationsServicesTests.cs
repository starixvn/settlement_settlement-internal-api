using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf.Collections;
using Microsoft.Extensions.Options;
using NSubstitute;
using Settlement.Internal.Models;
using Settlement.JackTime.Models;
using Settlement.JackTime.Services;

namespace Settlement.JackTime.UnitTests.Services;

public class NotificationsServicesTests
{
    private readonly BetBuilderOddsNotificationServices service;
    private readonly IOptions<AppSettingsKey> mockAppSettingsKey;
    private readonly InvokeExternalAPIServices mockInvokeExternalApiServices;
    private readonly GenerateServices mockGenerateServices;
    private readonly CreateRequestServices mockCreateRequestServices;

    public NotificationsServicesTests()
    {
        mockAppSettingsKey = Substitute.For<IOptions<AppSettingsKey>>();
        mockInvokeExternalApiServices = Substitute.For<InvokeExternalAPIServices>();
        mockGenerateServices = Substitute.For<GenerateServices>();
        mockCreateRequestServices = Substitute.For<CreateRequestServices>();
        service = new BetBuilderOddsNotificationServicesImp();
    }

    [Fact]
    public void ExtractLegsFromNotifications_WithValidNotifications_ShouldReturnBetBuilderOddsNotifications()
    {
        // Arrange
        var notifications = new RepeatedField<Notification>();
        var notification1 = CreateMockNotification("match1", "bet1", 1.5);
        var notification2 = CreateMockNotification("match2", "bet2", 2.0);
        notifications.Add(notification1);
        notifications.Add(notification2);

        // Act
        var result = service.ExtractOddsFromNotifications(notifications);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        
        var firstResult = result[0];
        Assert.Equal("match1", firstResult.MatchId);
        Assert.Equal("bet1", firstResult.BetId);
        Assert.Equal(1.5m, firstResult.Odds);

        var secondResult = result[1];
        Assert.Equal("match2", secondResult.MatchId);
        Assert.Equal("bet2", secondResult.BetId);
        Assert.Equal(2.0m, secondResult.Odds);
    }

    [Fact]
    public void ExtractLegsFromNotifications_WithNullBetBuilderOdds_ShouldFilterOutNullNotifications()
    {
        // Arrange
        var notifications = new RepeatedField<Notification>();
        var notification1 = CreateMockNotification("match1", "bet1", 1.5);
        var notification2 = CreateMockNotificationWithNullBetBuilderOdds();
        var notification3 = CreateMockNotification("match3", "bet3", 3.0);
        notifications.Add(notification1);
        notifications.Add(notification2);
        notifications.Add(notification3);

        // Act
        var result = service.ExtractOddsFromNotifications(notifications);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal("match1", result[0].MatchId);
        Assert.Equal("match3", result[1].MatchId);
    }

    [Fact]
    public void ExtractLegsFromNotifications_WithEmptyNotifications_ShouldReturnEmptyList()
    {
        // Arrange
        var notifications = new RepeatedField<Notification>();

        // Act
        var result = service.ExtractOddsFromNotifications(notifications);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public void ExtractLegsFromNotifications_WithAllNullBetBuilderOdds_ShouldReturnEmptyList()
    {
        // Arrange
        var notifications = new RepeatedField<Notification>();
        var notification1 = CreateMockNotificationWithNullBetBuilderOdds();
        var notification2 = CreateMockNotificationWithNullBetBuilderOdds();
        notifications.Add(notification1);
        notifications.Add(notification2);

        // Act
        var result = service.ExtractOddsFromNotifications(notifications);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public void ExtractLegsFromNotifications_WithDecimalOdds_ShouldConvertCorrectly()
    {
        // Arrange
        var notifications = new RepeatedField<Notification>();
        var notification = CreateMockNotification("match1", "bet1", 1.234567);
        notifications.Add(notification);

        // Act
        var result = service.ExtractOddsFromNotifications(notifications);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(1.234567m, result[0].Odds);
    }

    private static Notification CreateMockNotification(string matchId, string betId, double odds)
    {
        // Create mock objects using reflection or dynamic objects
        // Since we can't directly instantiate protobuf objects, we'll use a different approach
        var notification = new Notification();
        
        // Use reflection to set the BetBuilderOdds property
        var betBuilderOddsType = typeof(Notification).GetProperty("BetBuilderOdds")?.PropertyType;
        if (betBuilderOddsType != null)
        {
            var betBuilderOdds = Activator.CreateInstance(betBuilderOddsType);
            
            // Set MatchId
            var matchIdProperty = betBuilderOddsType.GetProperty("MatchId");
            if (matchIdProperty != null)
            {
                var matchIdObj = Activator.CreateInstance(matchIdProperty.PropertyType);
                var matchIdIdProperty = matchIdProperty.PropertyType.GetProperty("Id");
                matchIdIdProperty?.SetValue(matchIdObj, matchId);
                matchIdProperty.SetValue(betBuilderOdds, matchIdObj);
            }
            
            // Set Bet
            var betProperty = betBuilderOddsType.GetProperty("Bet");
            if (betProperty != null)
            {
                var betObj = Activator.CreateInstance(betProperty.PropertyType);
                var betIdProperty = betProperty.PropertyType.GetProperty("BetId");
                if (betIdProperty != null)
                {
                    var betIdObj = Activator.CreateInstance(betIdProperty.PropertyType);
                    var betIdIdProperty = betIdProperty.PropertyType.GetProperty("Id");
                    betIdIdProperty?.SetValue(betIdObj, betId);
                    betIdProperty.SetValue(betObj, betIdObj);
                }
                betProperty.SetValue(betBuilderOdds, betObj);
            }
            
            // Set Odds
            var oddsProperty = betBuilderOddsType.GetProperty("Odds");
            oddsProperty?.SetValue(betBuilderOdds, odds);
            
            typeof(Notification).GetProperty("BetBuilderOdds")?.SetValue(notification, betBuilderOdds);
        }
        
        return notification;
    }

    private static Notification CreateMockNotificationWithNullBetBuilderOdds()
    {
        return new Notification();
    }
}