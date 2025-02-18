namespace Settlement.EventManagement.Models;

public class RemarkSetting
{
    private const int ConvertToBase = 2;

    private const int UnderSettlePosition = 2;

    private const int MtdHoldPosition = 4;

    private const int KeyHoldPosition = 6;

    public RemarkSetting()
    {
    }

    public RemarkSetting(byte settlementStatus)
    {
        SettlementStatus = Convert.ToString(settlementStatus, ConvertToBase).Reverse();
    }

    public string SettlementStatus { get; set; }

    public bool IsPending => !string.IsNullOrWhiteSpace(SettlementStatus) && SettlementStatus[0].Equals('1');

    public bool IsUndecided => !string.IsNullOrWhiteSpace(SettlementStatus) && SettlementStatus.Length > 1 && SettlementStatus[1].Equals('1');

    public bool IsUnderSettle => !string.IsNullOrWhiteSpace(SettlementStatus) && SettlementStatus.Length > UnderSettlePosition && SettlementStatus[UnderSettlePosition].Equals('1');

    public bool IsMtdHold => !string.IsNullOrWhiteSpace(SettlementStatus) && SettlementStatus.Length > MtdHoldPosition && SettlementStatus[MtdHoldPosition].Equals('1');

    public bool IsKeyHold => !string.IsNullOrWhiteSpace(SettlementStatus) && SettlementStatus.Length > KeyHoldPosition && SettlementStatus[KeyHoldPosition].Equals('1');
}