using System.Globalization;
using Kassasystem.Data;
using Kassasystem.Models;

namespace Kassasystem.Tests;

public class SaleTests
{
    private Product bananer = new Product(300, "Bananer", 29.90m, PriceType.Kg);
    private Product mjolk = new Product(301, "Mjölk", 15.90m, PriceType.Styck);

    [Fact]
    public void LineTotal_Styck_PrisGangerAntal()
    {
        Sale sale = new Sale();
        sale.Add(mjolk, 2);

        Assert.Equal(31.80m, sale.GetItems()[0].GetLineTotal());
    }

    [Fact]
    public void LineTotal_Kilo_MedDecimaler()
    {
        Sale sale = new Sale();
        sale.Add(bananer, 1.5m);

        Assert.Equal(44.85m, sale.GetItems()[0].GetLineTotal());
    }

    [Fact]
    public void LineTotal_AvrundasTillHelaOren()
    {
        Sale sale = new Sale();
        sale.Add(bananer, 0.333m); // 9,9567 blir 9,96

        Assert.Equal(9.96m, sale.GetItems()[0].GetLineTotal());
    }

    [Fact]
    public void Total_SummerarRader_OchSlarIhopSammaProdukt()
    {
        Sale sale = new Sale();
        sale.Add(mjolk, 2);
        sale.Add(mjolk, 1);
        sale.Add(bananer, 1.5m);

        Assert.Equal(2, sale.GetItems().Count);
        Assert.Equal(3m, sale.GetItems()[0].Quantity);
        Assert.Equal(92.55m, sale.GetTotal()); // 47,70 + 44,85
    }

    [Fact]
    public void TomtKop_TotalArNoll()
    {
        Sale sale = new Sale();

        Assert.True(sale.IsEmpty());
        Assert.Equal(0m, sale.GetTotal());
    }

    [Theory]
    [InlineData(0)]      // noll
    [InlineData(-1)]     // negativt
    [InlineData(1.5)]    // decimal på styckvara
    [InlineData(1001)]   // över maxgränsen
    public void Add_OgiltigtAntal_KastarFel(double quantity)
    {
        Sale sale = new Sale();

        Assert.Throws<ArgumentException>(() => sale.Add(mjolk, (decimal)quantity));
        Assert.True(sale.IsEmpty());
    }

    [Fact]
    public void Kvitto_InnehallerVarorOchTotal()
    {
        CultureInfo.CurrentCulture = new CultureInfo("sv-SE");
        Sale sale = new Sale();
        sale.Add(mjolk, 2);

        string receipt = sale.ToReceipt(new DateTime(2026, 9, 7, 12, 0, 0));

        Assert.Contains("Mjölk", receipt);
        Assert.Contains("31,80 kr", receipt);
        Assert.Contains("2026-09-07 12:00:00", receipt);
    }

    [Fact]
    public void KvittoFilnamn_FoljerFormatet()
    {
        string fileName = ReceiptStorage.GetFileName(new DateTime(2026, 9, 7));

        Assert.Equal("RECEIPT_20260907.txt", fileName);
    }
}
