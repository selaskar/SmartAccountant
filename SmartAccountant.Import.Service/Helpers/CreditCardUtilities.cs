namespace SmartAccountant.Import.Service.Helpers;

internal class CreditCardUtilities
{
    /// <exception cref="ArgumentException"/>
    /// <exception cref="ArgumentNullException"/>
    internal static bool CompareNumbersWithMasking(string? cardNumberA, string? cardNumberB)
    {
        if (string.IsNullOrWhiteSpace(cardNumberA) || string.IsNullOrEmpty(cardNumberB))
            return false;

        // We don't compare lengths prior to this, because the number of spaces may be the sole differentiator.
        return Mask(cardNumberA) == Mask(cardNumberB);
    }

    /// <exception cref="ArgumentException"/>
    /// <exception cref="ArgumentNullException"/>
    internal static string Mask(string cardNumber)
    {
        if (string.IsNullOrWhiteSpace(cardNumber))
            return cardNumber;

        cardNumber = cardNumber.Replace(" ", string.Empty, StringComparison.Ordinal);

        const int unmaskedLength = 4;

        if (cardNumber.Length <= unmaskedLength * 2)
            return cardNumber;

        int maskedLength = cardNumber.Length - (unmaskedLength * 2);

        string firstPart = cardNumber[0..unmaskedLength];
        string lastPart = cardNumber[(cardNumber.Length - unmaskedLength)..];
        string maskedPart = new('*', maskedLength);

        return firstPart + maskedPart + lastPart;
    }
}
