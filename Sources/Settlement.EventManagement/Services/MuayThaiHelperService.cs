namespace Settlement.EventManagement.Services.MultiSports;

public interface MuayThaiHelperService
{
    (string Home, string Away) MapSboToSettle(int homeValue, int awayValue);
}

public class MuayThaiHelperServiceImp : MuayThaiHelperService
{
    public (string Home, string Away) MapSboToSettle(int homeValue, int awayValue)
    {
        return homeValue switch
        {
            >= 1 and <= 5 when awayValue == 0 => ("1", "0"),
            0 when awayValue is >= 1 and <= 5 => ("0", "1"),
            6 when awayValue == 0 => ("5", "0"),
            0 when awayValue == 6 => ("0", "5"),
            6 when awayValue == 6 => ("5", "5"),
            _ => (homeValue.ToString(), awayValue.ToString())
        };
    }
}