using Kassasystem.Data;
using Kassasystem.Logic;
using Kassasystem.Models;

namespace Kassasystem.UI;

// All konsolkod (menyer, ReadLine, WriteLine) ligger här.
// Själva logiken finns i de andra klasserna.
public class CashRegisterApp
{
    private Dictionary<int, Product> products;
    private ReceiptStorage storage;

    public CashRegisterApp(Dictionary<int, Product> products, ReceiptStorage storage)
    {
        this.products = products;
        this.storage = storage;
    }

    // Huvudmenyn. Körs tills användaren väljer 0.
    public void Run()
    {
        bool running = true;
        while (running)
        {
            Console.WriteLine();
            Console.WriteLine("KASSASYSTEM");
            Console.WriteLine("1. Ny kund");
            Console.WriteLine("2. Admin");
            Console.WriteLine("0. Avsluta");
            Console.Write("Val: ");

            string? choice = Console.ReadLine();

            if (choice == null || choice.Trim() == "0")
            {
                Console.WriteLine("Hej då!");
                running = false;
            }
            else if (choice.Trim() == "1")
            {
                NewCustomer();
            }
            else if (choice.Trim() == "2")
            {
                Console.WriteLine("Admin finns inte i den här versionen.");
            }
            else
            {
                Console.WriteLine("Ogiltigt val, välj 1, 2 eller 0.");
            }
        }
    }

    // Ett köp för en kund. Körs tills kunden betalar eller köpet avbryts.
    private void NewCustomer()
    {
        Sale sale = new Sale();
        Console.WriteLine();
        Console.WriteLine("NY KUND - skriv <produktid> <antal>, PAY för att betala, CANCEL för att avbryta.");

        while (true)
        {
            Console.Write("> ");
            string? input = Console.ReadLine();

            if (input == null)
            {
                return;
            }

            string command = input.Trim().ToUpper();

            if (command == "CANCEL")
            {
                Console.WriteLine("Köpet avbröts.");
                return;
            }
            else if (command == "PAY")
            {
                if (sale.IsEmpty())
                {
                    Console.WriteLine("Inga varor registrerade ännu.");
                }
                else
                {
                    Pay(sale);
                    return;
                }
            }
            else
            {
                AddProduct(sale, input);
            }
        }
    }

    private void AddProduct(Sale sale, string input)
    {
        int id;
        decimal quantity;

        if (!InputParser.TryParse(input, out id, out quantity))
        {
            Console.WriteLine("Fel format. Skriv <produktid> <antal>, t.ex. 300 2");
            return;
        }

        if (!products.ContainsKey(id))
        {
            Console.WriteLine("Produkt " + id + " finns inte.");
            return;
        }

        Product product = products[id];

        try
        {
            sale.Add(product, quantity);
            Console.WriteLine(product.Name + " tillagd. Totalt hittills: " + sale.GetTotal().ToString("0.00") + " kr");
        }
        catch (ArgumentException ex)
        {
            // Fel antal, t.ex. 0 eller decimaler på en styckvara.
            Console.WriteLine(ex.Message);
        }
    }

    private void Pay(Sale sale)
    {
        DateTime now = DateTime.Now;
        string receipt = sale.ToReceipt(now);

        Console.WriteLine();
        Console.WriteLine(receipt);

        try
        {
            string path = storage.Save(receipt, now);
            Console.WriteLine("Kvittot sparades i " + path);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Kunde inte spara kvittot: " + ex.Message);
        }
    }
}
