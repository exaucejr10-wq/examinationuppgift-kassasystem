using Kassasystem.Data;
using Kassasystem.Logic;
using Kassasystem.Models;

namespace Kassasystem.Tests;

public class ParserTests
{
    [Theory]
    [InlineData("300 2", 300, 2)]
    [InlineData("300 1.5", 300, 1.5)]
    [InlineData("300 1,5", 300, 1.5)]
    [InlineData("  300    1.5  ", 300, 1.5)]
    public void Input_Giltig_GerIdOchAntal(string input, int expectedId, double expectedQuantity)
    {
        int id;
        decimal quantity;
        bool ok = InputParser.TryParse(input, out id, out quantity);

        Assert.True(ok);
        Assert.Equal(expectedId, id);
        Assert.Equal((decimal)expectedQuantity, quantity);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("300")]
    [InlineData("abc 2")]
    [InlineData("300 två")]
    [InlineData("300 2 5")]
    public void Input_Ogiltig_GerFalse(string? input)
    {
        int id;
        decimal quantity;

        Assert.False(InputParser.TryParse(input, out id, out quantity));
    }

    [Fact]
    public void Produktrad_Giltig_SkaparProdukt()
    {
        Product? product = FileProductRepository.ParseLine("300;Bananer;29.90;Kg");

        Assert.NotNull(product);
        Assert.Equal(300, product.Id);
        Assert.Equal("Bananer", product.Name);
        Assert.Equal(29.90m, product.Price);
        Assert.Equal(PriceType.Kg, product.PriceType);
    }

    [Theory]
    [InlineData("300;Bananer;29.90")]         // för få delar
    [InlineData("abc;Bananer;29.90;Kg")]      // ID är inte ett tal
    [InlineData("300;;29.90;Kg")]             // inget namn
    [InlineData("300;Bananer;gratis;Kg")]     // pris är inte ett tal
    [InlineData("300;Bananer;-5;Kg")]         // negativt pris
    [InlineData("300;Bananer;29.90;Liter")]   // okänd pristyp
    public void Produktrad_Felaktig_GerNull(string line)
    {
        Assert.Null(FileProductRepository.ParseLine(line));
    }

    [Fact]
    public void LoadProducts_HopparOverKommentarerOchFelaktigaRader()
    {
        string file = Path.GetTempFileName();
        string[] lines = { "# kommentar", "300;Bananer;29.90;Kg", "fel rad", "", "301;Mjölk;15.90;Styck" };
        File.WriteAllLines(file, lines);

        IProductRepository repository = new FileProductRepository(file);
        List<Product> products = repository.LoadProducts();

        Assert.Equal(2, products.Count);
        Assert.Equal(300, products[0].Id);
        Assert.Equal(301, products[1].Id);
        File.Delete(file);
    }
}
