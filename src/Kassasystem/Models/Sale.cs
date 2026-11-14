namespace Kassasystem.Models;

// Ett pågående köp för en kund.
// Om samma vara läggs till två gånger ökar vi antalet på den raden som redan finns.
public class Sale
{
    public const decimal MaxQuantity = 1000;

    private List<SaleItem> items = new List<SaleItem>();

    public List<SaleItem> GetItems()
    {
        return items;
    }

    public bool IsEmpty()
    {
        return items.Count == 0;
    }

    // Lägger ihop alla rader.
    public decimal GetTotal()
    {
        decimal total = 0;
        foreach (SaleItem item in items)
        {
            total = total + item.GetLineTotal();
        }
        return total;
    }

    // Letar efter en rad med samma produkt-ID. Ger null om den inte finns.
    private SaleItem? FindItem(int productId)
    {
        foreach (SaleItem item in items)
        {
            if (item.Product.Id == productId)
            {
                return item;
            }
        }
        return null;
    }

    // Lägger till en vara. Kastar ArgumentException om antalet är fel.
    public void Add(Product product, decimal quantity)
    {
        if (quantity <= 0)
        {
            throw new ArgumentException("Antal måste vara större än 0.");
        }

        // En styckvara får inte ha decimaler (1,5 mjölk går inte).
        if (product.PriceType == PriceType.Styck && quantity % 1 != 0)
        {
            throw new ArgumentException(product.Name + " säljs per styck, ange ett heltal.");
        }

        SaleItem? existing = FindItem(product.Id);

        decimal newQuantity = quantity;
        if (existing != null)
        {
            newQuantity = existing.Quantity + quantity;
        }

        if (newQuantity > MaxQuantity)
        {
            throw new ArgumentException("Antal får inte vara större än " + MaxQuantity + ".");
        }

        if (existing != null)
        {
            existing.Quantity = newQuantity;
        }
        else
        {
            items.Add(new SaleItem(product, quantity));
        }
    }

    // Bygger kvittot som en text.
    public string ToReceipt(DateTime date)
    {
        string line = "--------------------------------------------";
        string doubleLine = "============================================";
        string receipt = "";

        receipt += doubleLine + "\n";
        receipt += "KVITTO  " + date.ToString("yyyy-MM-dd HH:mm:ss") + "\n";
        receipt += line + "\n";

        foreach (SaleItem item in items)
        {
            string unit = item.Product.GetUnit();

            // Kilo visas med tre decimaler, styck utan decimaler.
            string amount;
            if (item.Product.PriceType == PriceType.Kg)
            {
                amount = item.Quantity.ToString("0.000");
            }
            else
            {
                amount = item.Quantity.ToString("0");
            }

            string text = "  " + amount + " " + unit + " x " + item.Product.Price.ToString("0.00") + " kr/" + unit;

            receipt += item.Product.Name + "\n";
            receipt += text.PadRight(32) + item.GetLineTotal().ToString("0.00").PadLeft(12) + "\n";
        }

        receipt += line + "\n";
        receipt += "TOTALT".PadRight(32) + (GetTotal().ToString("0.00") + " kr").PadLeft(12) + "\n";
        receipt += doubleLine + "\n";

        return receipt;
    }
}
