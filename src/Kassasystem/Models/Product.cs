namespace Kassasystem.Models;

// Hur en produkt prissätts: per styck eller per kilo.
public enum PriceType
{
    Styck,
    Kg
}

// En produkt i butiken, t.ex. 300 Bananer 29,90 kr/kg.
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public PriceType PriceType { get; set; }

    public Product(int id, string name, decimal price, PriceType priceType)
    {
        Id = id;
        Name = name;
        Price = price;
        PriceType = priceType;
    }

    // Enheten som visas på kvittot: "kg" eller "st".
    public string GetUnit()
    {
        if (PriceType == PriceType.Kg)
        {
            return "kg";
        }
        return "st";
    }
}

// En rad i ett köp: vilken produkt och hur många (eller hur många kilo).
public class SaleItem
{
    public Product Product { get; set; }
    public decimal Quantity { get; set; }

    public SaleItem(Product product, decimal quantity)
    {
        Product = product;
        Quantity = quantity;
    }

    // Pris för raden, avrundat till hela ören.
    public decimal GetLineTotal()
    {
        return Math.Round(Product.Price * Quantity, 2);
    }
}
