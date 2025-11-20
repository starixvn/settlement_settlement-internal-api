using Settlement.JackTime.Constants;

namespace Settlement.JackTime.Models;

public class Results<T>
{
    public string ErrMsg { get; set; } = string.Empty;

    public bool IsSuccessful => string.Equals(ErrMsg, Result.Success, StringComparison.CurrentCultureIgnoreCase);

    public T? Data { get; set; }
}