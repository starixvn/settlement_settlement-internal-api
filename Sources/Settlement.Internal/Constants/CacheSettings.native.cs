namespace Settlement.Internal.Constants;

public static class CacheSettings
{
    public static class Keys
    {
        public const string MemoryCacheMultiSportCacheKey = "MultiSport";
    }

    public static class Expiration
    {
        public const int ShortCallRefreshTimerSeconds = 3;
        public const int LongCallRefreshTimerMinutes = 10;
    }
}