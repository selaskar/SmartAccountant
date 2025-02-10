namespace SmartAccountant.Services.Parser.Helpers;

internal static class RoundHelper
{
    /// <summary>
    /// Rounds away from zero to 4 decimal places.
    /// </summary>
    public static decimal Round(this decimal value) => Math.Round(value, 4, MidpointRounding.AwayFromZero);
}
