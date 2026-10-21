using System.Globalization;
using System.Text;
using Kassasystem.Data;
using Kassasystem.Models;
using Kassasystem.UI;

// Program.cs kopplar bara ihop delarna och startar kassan.
Console.OutputEncoding = Encoding.UTF8;
CultureInfo.CurrentCulture = new CultureInfo("sv-SE"); // svenska priser, t.ex. 15,90

string productFile = Path.Combine(AppContext.BaseDirectory, "products.txt");
IProductRepository repository = new FileProductRepository(productFile);

List<Product> productList;
try
{
    productList = repository.LoadProducts();
}
catch (FileNotFoundException ex)
{
    Console.WriteLine("Fel: " + ex.Message);
    Console.WriteLine("Programmet kan inte starta utan produkter.");
    return;
}

// Lägg produkterna i en Dictionary så att vi snabbt kan hitta en produkt med dess ID.
Dictionary<int, Product> products = new Dictionary<int, Product>();
foreach (Product product in productList)
{
    if (!products.ContainsKey(product.Id))
    {
        products.Add(product.Id, product);
    }
}

Console.WriteLine(products.Count + " produkter inlästa.");

string receiptFolder = Path.Combine(Directory.GetCurrentDirectory(), "Kvitton");
ReceiptStorage storage = new ReceiptStorage(receiptFolder);

CashRegisterApp app = new CashRegisterApp(products, storage);
app.Run();
