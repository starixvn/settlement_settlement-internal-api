namespace Settlement.EventManagement.Criteria;

public class GetResultDetailCriteria : CriteriaBase
{
    public string MatchIdList { get; set; }

    public override string GetSettingKey() => "NAP_Set_ResultDetailSel";

    public override bool IsValid() => !string.IsNullOrWhiteSpace(MatchIdList);
}