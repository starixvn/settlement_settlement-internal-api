using MessagePack;

namespace Settlement.EventManagement.Models;

public class Match
{
    private const string Dash = "-";

    private string eventStatus = string.Empty;

    private string suspenderHalfTime = string.Empty;

    private string suspenderFullTime = string.Empty;

    public int MatchId { get; set; }

    public int? HalfTimeHomeScore { get; set; }

    public int? HalfTimeAwayScore { get; set; }

    public int? FinalHomeScore { get; set; }

    public int? FinalAwayScore { get; set; }

    public int AutoPayoutStatus { get; set; }

    public int AutoInstantStatus { get; set; }

    public int? IsCalculatingSingle { get; set; }

    public int? IsCalculatingMP { get; set; }

    public string NewEventStatus { get; set; }

    public string EventStatusColor { get; set; }

    public string EarlySettleStatus { get; set; }

    public string ConfirmHalfTimeScore { get; set; }

    public string ConfirmFullTimeScore { get; set; }

    public string ExHalfTimeScore { get; set; }

    public string ExFullTimeScore { get; set; }

    public string BetStatus { get; set; }

    public bool IsExistMarkedTicket { get; set; }

    public bool IsExistRunningTicket { get; set; }

    public bool IsHavingIntervalScore { get; set; }

    public bool IsNotConsistentExHtScore { get; set; }

    public bool IsNotConsistentExFtScore { get; set; }

    public bool IsCalRefund { get; set; }

    public bool IsCalculated { get; set; }

    public bool IsHalfTimeCal { get; set; }

    public bool IsMultiCaled { get; set; }

    public bool IsRefund { get; set; }

    public string SuspenderHalfTime

    {
        get => string.IsNullOrEmpty(suspenderHalfTime) ? Dash : suspenderHalfTime;

        set => suspenderHalfTime = value;
    }

    public string SuspenderFullTime
    {
        get => string.IsNullOrEmpty(suspenderFullTime) ? Dash : suspenderFullTime;

        set => suspenderFullTime = value;
    }

    public string EventStatus
    {
        get => eventStatus.MapToString(BetStatus, Remark.IsPending);

        set
        {
            eventStatus = value;
        }
    }

    public RemarkSetting Remark => new(SettlementStatus);

    public MoreScores MoreScores { get; set; }

    public AutoCalculationSetting AutoCalcSetting => new(AutoSetting, AutoSettingForLeague);

    [IgnoreMember]
    public List<SiteMatch> SiteMatch { get; set; } = [];

    [IgnoreMember]
    public bool IsRunningEarlyBetTypes { get; set; }

    [IgnoreMember]
    public bool IsWaitingEarlyBetTypes { get; set; }

    [IgnoreMember]
    public bool IsExistEarlyBetTypes { get; set; }

    [IgnoreMember]
    public bool IsOpenOdds { get; set; }

    [IgnoreMember]
    public int SportType { get; set; }

    [IgnoreMember]
    public byte SettlementStatus { get; set; }

    [IgnoreMember]
    public int AutoSetting { get; set; }

    [IgnoreMember]
    public int AutoSettingForLeague { get; set; }

    [IgnoreMember]
    public int BestOfMap { get; set; }

    [IgnoreMember]
    public int ChildMatchType { get; set; }

    public override bool Equals(object obj)
    {
        return obj is Match match && match.MatchId == MatchId;
    }

    public override int GetHashCode()
    {
        return MatchId;
    }
}