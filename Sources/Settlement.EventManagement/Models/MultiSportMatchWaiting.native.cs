namespace Settlement.EventManagement.Models;

public class MultiSportMatchWaiting
{
    public int MatchId { get; set; }

    public int SportType { get; set; }

    public string EventStatus { get; set; }

    public string BetStatus { get; set; }

    public int? HalfTimeHomeScore { get; set; }

    public int? HalfTimeAwayScore { get; set; }

    public int? FinalHomeScore { get; set; }

    public int? FinalAwayScore { get; set; }

    public int? IsCalculatingSingle { get; set; }

    public int? IsCalculatingMP { get; set; }

    public bool IsCalculated { get; set; }

    public bool IsMultiCaled { get; set; }

    public bool IsRefund { get; set; }

    public bool IsCalRefund { get; set; }

    public bool IsHalfTimeCal { get; set; }

    public string SuspenderHalfTime { get; set; }

    public string SuspenderFullTime { get; set; }

    public string EarlySettleStatus { get; set; }

    public bool IsExistRunningTicket { get; set; }

    public bool IsExistMarkedTicket { get; set; }

    public int AutoPayoutStatus { get; set; }

    public int InstantStatus { get; set; }

    public MoreScores MoreScores { get; set; }

    public string EventStatusColor { get; set; }

    public string NewEventStatus { get; set; }

    public bool IsHavingIntervalScore { get; set; }

    public bool IsAutoPayoutForMatch { get; set; }

    public bool IsAutoPayoutForLeague { get; set; }

    public RemarkSetting Remark { get; set; }

    public string ExHalfTimeScore { get; set; }

    public string ExFullTimeScore { get; set; }

    public bool IsNotConsistentExHtScore { get; set; }

    public bool IsNotConsistentExFtScore { get; set; }

    public string ConfirmHalfTimeScore { get; set; }

    public string ConfirmFullTimeScore { get; set; }

    public AutoCalculationSetting AutoCalcSetting { get; set; }
}