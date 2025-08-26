namespace Settlement.EventManagement.Services;

public interface MuayThaiHelperService
{
    (string Home, string Away, string Round) MapSboToSettle(int homeValue, int awayValue);
}

public class MuayThaiHelperServiceImp : MuayThaiHelperService
{
    public (string Home, string Away, string Round) MapSboToSettle(int homeValue, int awayValue)
    {
        return homeValue switch
        {
            >= 1 and <= 5 when awayValue == 0 => ("1", "0", homeValue.ToString()),
            0 when awayValue is >= 1 and <= 5 => ("0", "1", awayValue.ToString()),
            6 when awayValue == 0 => ("5", "0", string.Empty),
            0 when awayValue == 6 => ("0", "5", string.Empty),
            6 when awayValue == 6 => ("5", "5", string.Empty),
            _ => (homeValue.ToString(), awayValue.ToString(), string.Empty)
        };
    }
}