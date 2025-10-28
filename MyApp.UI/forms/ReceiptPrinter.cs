using System;
using System.Collections.Generic;
using System.Globalization;
using System.Diagnostics;
using System.IO;
using System.Data;
using MyApp.Models;

namespace POSPrinting
{
    public class ReceiptPrinter
    {
        public class Sale
        {
            public List<SaleItem> Items { get; set; } = new();
            public decimal InvoiceAmount { get; set; }
            public decimal InvoiceDiscount { get; set; }
            public decimal InvoiceNet { get; set; }
        }

        public void PrintReceipt(PrintSettings settings, Sale sale, FbrResponse? fbr , bool isFbrActive, string posId, string ntn, string strn)
        {
            string date = DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss tt", CultureInfo.InvariantCulture);
            decimal fbrPosCharges = 1.00m;
            string fbrPosLogoSrc = GetFbrLogoPath();
            // Update this path

            // HTML Layout optimized for 80mm thermal printer with slightly larger fonts
            string html = $@"
                    <!DOCTYPE html>
                    <html lang='en'>
                    <head>
                        <meta charset='UTF-8'>
                        <script src='https://cdn.tailwindcss.com'></script>
                        <style>
                            @media print {{
                                body {{ width: 280px; margin:0; padding:0; }}
                                .print-button {{ display: none !important; }}
                            }}
                            @media screen {{
                                body {{ 
                                    display: flex; 
                                    justify-content: center; 
                                    align-items: center; 
                                    min-height: 100vh; 
                                    margin: 0; 
                                    padding: 20px;
                                    background-color: #f0f0f0;
                                }}
                                .receipt-container {{
                                    background: white;
                                    padding: 20px;
                                    border-radius: 8px;
                                    box-shadow: 0 4px 12px rgba(0,0,0,0.1);
                                }}
                                .print-button {{
                                    position: fixed;
                                    top: 20px;
                                    right: 20px;
                                    background: #007cba;
                                    color: white;
                                    border: none;
                                    padding: 12px 24px;
                                    border-radius: 6px;
                                    cursor: pointer;
                                    font-size: 16px;
                                    font-weight: bold;
                                    z-index: 1000;
                                    box-shadow: 0 2px 8px rgba(0,0,0,0.2);
                                }}
                                .print-button:hover {{
                                    background: #005a87;
                                }}
                            }}
                        </style>
                    </head>
                    <body>
                        
                        <div class='receipt-container'>
                            <div class='mx-auto w-full' style='width:275px;'>

                                <!-- Header -->
                                <div class='text-center mb-2'>
                                    {(settings.Name != null ? $"<h2 class='text-base font-bold m-0'>{settings.Name}</h2>" : "")}
                                    {(settings.Type != null ? $"<p class='text-sm m-0'>{settings.Type}</p>" : "")}
                                    {(settings.Address != null ? $"<p class='text-sm m-0'>{settings.Address}</p>" : "")}
                                    {(settings.Phone != null ? $"<p class='text-sm m-0'>Ph: {settings.Phone}</p>" : "")}
                                    {(isFbrActive && !string.IsNullOrEmpty(ntn) ? $"<p class='text-sm m-0'>NTN # {ntn}</p>" : "")}
                                    {(isFbrActive && !string.IsNullOrEmpty(strn) ? $"<p class='text-sm mb-2'>STRN # {strn}</p>" : "")}
                                </div>

                                <!-- Date Box -->
                                <div class='border border-black text-center mb-2 p-1 text-sm'>{date}</div>

                                <!-- Items Table -->
                                <table class='w-full table-fixed border border-black border-collapse text-[12px]'>
                                    <thead>
                                        <tr class='border border-black'>
                                            <th class='w-[45%] text-left border border-black p-1'>Name</th>
                                            <th class='w-[18%] text-right border border-black p-1'>Rate</th>
                                            <th class='w-[17%] text-right border border-black p-1'>Qty</th>
                                            <th class='w-[20%] text-right border border-black p-1'>Amount</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        {GetTableBodyTailwind(sale)}
                                    </tbody>
                                </table>

                                <!-- Summary Box -->
                                <div class='border border-black p-2 mt-2 text-[12px]'>
                                    <div class='flex justify-between mb-1'><span>Sub Total</span><span>{sale.InvoiceAmount:F2}</span></div>
                                    <div class='flex justify-between mb-1'><span>Discount</span><span>{sale.InvoiceDiscount:F2}</span></div>
                                    <div class='flex justify-between font-bold mb-1'>
                                        <span>Net Amount</span><span>{sale.InvoiceNet:F2}</span>
                                    </div>
                                    {(isFbrActive ? $@"
                                    <div class='flex justify-between mb-1'><span>Tax</span><span>{fbr.TotalTaxCharged:F2}</span></div>
                                    <div class='flex justify-between mb-1'><span>FBR POS Charges</span><span>{fbrPosCharges:F2}</span></div>
                                    <div class='flex justify-between font-bold'>
                                        <span>Total (Incl. Tax)</span><span>{sale.InvoiceNet + fbr.TotalTaxCharged + fbrPosCharges:F2}</span>
                                    </div>" : "")}
                                </div>

                                <!-- FBR Section -->
                                {(isFbrActive ? $@"
                                <div class='text-center mt-2'>
                                    <div class='text-sm mb-1'>FBR Inv # {fbr.FbrInvoiceNumber}</div>
                                    <div class='flex justify-center items-center space-x-2'>
                                        <img class='w-16 h-16' src='{fbrPosLogoSrc}' />
                                        <img class='w-28 h-28' src='{fbr.QrCodeBase64}' />
                                    </div>
                                    <div class='text-sm mt-1'>POS ID: {posId}</div>
                                </div>" : "")}

                                <!-- Footer -->
                                <div class='text-center mt-2 text-[12px]'>
                                    {(settings.Remarks != null ? $"<div class='mt-2'>{settings.Remarks}</div>" : "")}
                                    {(settings.Website != null ? $"<div class='mt-1'>{settings.Website}</div>" : "")}
                                </div>

                            </div>
                        </div>
                        <script>
                                window.onload = function() {{
                                    window.print();
                                    // Wait for print dialog to close, then close window automatically
                                    window.onafterprint = function(){{
                                        window.close();
                                    }};
                                }};
                            </script>
                    </body>
                    </html>";

            ShowPrintPreview(html);
        }

        private void ShowPrintPreview(string html)
        {
            string tempFile = Path.Combine(Path.GetTempPath(), $"receipt_preview_{Guid.NewGuid()}.html");

            File.WriteAllText(tempFile, html);

            Process.Start(new ProcessStartInfo
            {
                FileName = tempFile,
                UseShellExecute = true,
            });

            Task.Run(async () =>
            {
                await Task.Delay(30000); // 30 seconds
                try { if (File.Exists(tempFile)) File.Delete(tempFile); } catch { }
            });
        }


        private string GetTableBodyTailwind(Sale sale)
        {
            string rows = "";
            foreach (var item in sale.Items)
            {
                string productName = item.Name.Length > 20
                    ? item.Name.Substring(0, 17) + "..."
                    : item.Name;

                rows += $@"
                <tr class='border border-black'>
                    <td class='border border-black p-1 text-left truncate'>{productName}</td>
                    <td class='border border-black p-1 text-right'>{item.Rate:F2}</td>
                    <td class='border border-black p-1 text-right'>{item.Qty}</td>
                    <td class='border border-black p-1 text-right'>{item.Amount:F2}</td>
                </tr>";
            }

            return rows;
        }
        private string GetFbrLogoPath()
        {
            try
            {
                // This works on all devices - looks in the application's directory
                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string imagePath = Path.Combine(baseDirectory, "assests", "12.jpeg");

                if (File.Exists(imagePath))
                {
                    return new Uri(imagePath).AbsoluteUri;
                }
                else
                {
                    // Fallback to base64 if file not found
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }
    }
}