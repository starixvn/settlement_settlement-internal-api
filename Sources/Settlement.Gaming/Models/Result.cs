namespace Settlement.Gaming.Models;

public class Result
{
    public int ErrCode { get; set; }

    public string ErrMsg { get; set; }

    public bool IsSuccessful => string.IsNullOrEmpty(ErrMsg) && ErrCode == 0;
}