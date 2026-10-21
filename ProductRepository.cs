using System.Globalization;
using Kassasystem.Models;

namespace Kassasystem.Data;

// Interface: resten av programmet vet bara ATT produkter kan hämtas,
// inte VARIFRÅN. Då kan vi byta till t.ex. en databas utan att ändra något annat.
public interface IProductRepository
{
    List<Product> LoadProducts();
}

// Läser produkter från en textfil med rader som "300;Bananer;29.90;Kg".
public class FileProductRepository : IProductRepository
{
    private string filePath;

    public FileProductRepository(string filePath)
    {
        this.filePath = filePath;
    }

    public List<Product> LoadProducts()
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("Hittar inte produktfilen: " + filePath);
        }

        List<Product> products = new List<Product>();
        string[] lines = File.ReadAllLines(filePath);

        foreach (string rawLine in lines)
        {
            string line = rawLine.Trim();

            // Hoppa över tomma rader och kommentarer.
            if (line == "" || line.StartsWith("#"))
            {
                continue;
            }

            Product? product = ParseLine(line);
            if (product != null)
            {
                products.Add(product);
            }
        }

        return products;
    }

    // Gör om en rad till en Product. Ger null om raden är felaktig i stället för att krascha.
    public static Product? ParseLine(string line)
    {
        string[] parts = line.Split(';');
        if (parts.Length != 4)
        {
            return null;
        }

        string idText = parts[0].Trim();
        string name = parts[1].Trim();
        string priceText = parts[2].Trim().Replace(',', '.');
        string typeText = parts[3].Trim().ToLower();

        int id;
        if (!int.TryParse(idText, out id) || id <= 0)
        {
            return null;
        }

        if (name == "")
        {
            return null;
        }

        // InvariantCulture gör att punkt alltid räknas som decimaltecken.
        decimal price;
        if (!decimal.TryParse(priceText, NumberStyles.Number, CultureInfo.InvariantCulture, out price) || price < 0)
        {
            return null;
        }

        PriceType priceType;
        if (typeText == "kg")
        {
            priceType = PriceType.Kg;
        }
        else if (typeText == "styck")
        {
            priceType = PriceType.Styck;
        }
        else
        {
            return null;
        }

        return new Product(id, name, price, priceType);
    }
}
