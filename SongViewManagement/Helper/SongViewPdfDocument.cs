using iText.Html2pdf;
using iText.Kernel.Pdf;
using iText.Layout;
using SongViewManagement.Helper;

public class SongViewPdfDocument
{
    private string FormatNumberWithCommas(long number)
    {
        return string.Format("{0:n0}", number);
    }
    public Stream GeneratePdf(List<SongDetails> songDetails)
    {
        using (var outputStream = new MemoryStream())
        {
            using (var pdfWriter = new PdfWriter(outputStream))
            {
                using (var pdfDoc = new PdfDocument(pdfWriter))
                {
                    var document = new Document(pdfDoc);

                    // HTML content with CSS styling
                    var htmlContent = @"
                                        <style>
                                            table {
                                                width: 100%;
                                                border-collapse: collapse;
                                            }
                                            th, td {
                                                border: 1px solid #dddddd;
                                                text-align: left;
                                                padding: 8px;
                                                max-width: 200px; /* Adjust the maximum width as needed */
                                                word-wrap: break-word;
                                            }
                                            th {
                                                background-color: #f2f2f2;
                                            }
                                        </style>
                                        <table>
                                            <thead>
                                                <tr>
                                                    <th>Song Name</th>
                                                    <th>URL</th>
                                                    <th>Views Count</th>
                                                    <th>Daily Views</th>
                                                    <th>Weekly Views</th>
                                                    <th>Monthly Views</th>
                                                </tr>
                                            </thead>
                                            <tbody>";

                    // Add table rows
                    foreach (var songDetail in songDetails)
                    {
                        var viewsCountFormatted = FormatNumberWithCommas(songDetail.ViewsCount);
                        var dailyViewsFormatted = FormatNumberWithCommas(songDetail.DailyViews);
                        var weeklyViewsFormatted = FormatNumberWithCommas(songDetail.WeeklyViews);
                        var monthlyViewsFormatted = FormatNumberWithCommas(songDetail.MonthlyViews);

                        htmlContent += $@"
                        <tr>
                            <td>{songDetail.SongName}</td>
                            <td>{songDetail.SongUrl}</td>
                            <td class='views-count'>{viewsCountFormatted}</td>
                            <td>{dailyViewsFormatted}</td>
                            <td>{weeklyViewsFormatted}</td>
                            <td>{monthlyViewsFormatted}</td>

                        </tr>";
                    }

                    htmlContent += @"
                        </tbody>
                    </table>";

                    // Convert HTML content to PDF
                    using (var htmlStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(htmlContent)))
                    {
                        HtmlConverter.ConvertToPdf(htmlStream, pdfDoc);
                    }
                }
            }

            // Create a new MemoryStream and copy the content of outputStream to it
            var copiedStream = new MemoryStream(outputStream.ToArray());

            // Reset the position of the copied stream to zero before returning
            copiedStream.Position = 0;

            return copiedStream;
        }
    }

}

