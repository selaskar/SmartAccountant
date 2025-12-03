namespace SmartAccountant.Shared;

public static class ApplicationDefinitions
{
    public static readonly DateOnly EpochStart = new(2000, 1, 1);

    public const string EpochStartString = "2000-01-01";

    public static readonly DateOnly EpochEnd = new(2050, 1, 1);

    public const string EpochEndString = "2050-01-01";

    public const double MaxTransactionAmount = 999999999999.9999;

    public const double MinTransactionAmount = -999999999999.9999;
}
