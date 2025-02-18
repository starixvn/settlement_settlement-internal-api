using Fanex.Data;

namespace Settlement.EventManagement.Criteria;

public class GetWaitingMultiSportCriteria : CriteriaBase
{
    public string MatchIds { get; set; }

    [SpParam(Ignored = true)]
    public string LeagueIds { get; set; }

    [SpParam(Ignored = true)]
    public bool IsLeader { get; set; }

    [SpParam(Ignored = true)]
    public string DbServer { get; set; }

    public override string GetSettingKey() => $"NAP_Set_WaitingOtherSportSel_{DbServer}";

    public override bool IsValid() => !string.IsNullOrWhiteSpace(MatchIds);
}