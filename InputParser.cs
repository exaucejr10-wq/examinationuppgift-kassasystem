using System.Globalization;

namespace Kassasystem.Logic;

// Tolkar det kassören skriver, t.ex. "300 2" eller "300 1,5".
// Fungerar som int.TryParse: returnerar true om det gick, och svaren kommer ut via "out".
public static class InputParser
{
    public static bool TryParse(string? input, out int id, out decimal quantity)
    {
        id = 0;
        quantity = 0;

        if (input == null)
        {
            return false;
        }

        // RemoveEmptyEntries gör att flera mellanslag i rad inte ger tomma delar.
        string[] parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length != 2)
        {
            return false;
        }

        if (!int.TryParse(parts[0], out id))
        {
            return false;
        }

        // Både 1,5 och 1.5 ska fungera.
        string amountText = parts[1].Replace(',', '.');
        if (!decimal.TryParse(amountText, NumberStyles.Number, CultureInfo.InvariantCulture, out quantity))
        {
            return false;
        }

        return true;
    }
}
