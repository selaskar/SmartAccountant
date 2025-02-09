namespace SmartAccountant.Services.Parser.Helpers;

internal static class RoundHelper
{
    public static decimal Round(this decimal value) => Math.Round(value, 4, MidpointRounding.AwayFromZero);
}
