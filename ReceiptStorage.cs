namespace Kassasystem.Data;

// Sparar kvitton i filen RECEIPT_yyyyMMdd.txt.
// Kvitton från samma dag läggs sist i samma fil.
public class ReceiptStorage
{
    private string folder;

    public ReceiptStorage(string folder)
    {
        this.folder = folder;
    }

    public static string GetFileName(DateTime date)
    {
        return "RECEIPT_" + date.ToString("yyyyMMdd") + ".txt";
    }

    // Sparar kvittot och returnerar sökvägen till filen.
    public string Save(string receipt, DateTime date)
    {
        Directory.CreateDirectory(folder); // gör inget om mappen redan finns
        string path = Path.Combine(folder, GetFileName(date));
        File.AppendAllText(path, receipt + "\n");
        return path;
    }
}
