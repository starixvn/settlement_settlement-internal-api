namespace Settlement.JackTime.Models;

public class Results<T>
{
    public string ErrMsg { get; set; } = string.Empty;

    public bool IsSuccessful => string.IsNullOrEmpty(ErrMsg);

    public T? Data { get; set; }
}