namespace POS.Application.Helpers;

public static class BarcodeNormalizer
{
    public static string Normalize(string? barcode)
    {
        if (string.IsNullOrWhiteSpace(barcode))
        {
            return string.Empty;
        }

        return barcode.Trim().ToUpperInvariant();
    }

    public static bool LooksLikeBarcode(string? value)
    {
        var normalized = Normalize(value);
        return normalized.Length >= 4 && normalized.All(c => char.IsLetterOrDigit(c) || c == '-' || c == '_');
    }
}
