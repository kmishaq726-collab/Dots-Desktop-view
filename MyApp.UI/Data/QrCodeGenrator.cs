using System;
using System.IO;
using System.Threading.Tasks;
using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;

public static class QrCodeService
{
    public static async Task<string> GenerateQrCodeAsync(string fbrInvoiceNumber)
    {
        try
        {
            return await Task.Run(() =>
            {
                using var generator = new QRCodeGenerator();
                using var data = generator.CreateQrCode(fbrInvoiceNumber, QRCodeGenerator.ECCLevel.Q);
                using var qrCode = new QRCode(data);
                using var bitmap = qrCode.GetGraphic(20);

                using var ms = new MemoryStream();
                bitmap.Save(ms, ImageFormat.Png);
                var base64 = Convert.ToBase64String(ms.ToArray());
                return $"data:image/png;base64,{base64}";
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to generate QR code: {ex.Message}");
            return string.Empty;
        }
    }
}
