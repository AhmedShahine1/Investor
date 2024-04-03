using Microsoft.AspNetCore.Mvc;
using PdfSharpCore;
using PdfSharpCore.Pdf;
using TheArtOfDev.HtmlRenderer.PdfSharp;

namespace Investor.Controllers
{
    public class PdfController : BaseController
    {
        [HttpGet("generate")]
        public IActionResult GeneratePdf()
        {
            var data = new PdfDocument();
            string htmlContent = "<div style = 'margin: 20px auto; max-width: 600px; padding: 20px; border: 1px solid #ccc; background-color: #FFFFFF; font-family: Arial, sans-serif;' >";
            htmlContent += "<div style = 'margin-bottom: 20px; text-align: center;'>";
            htmlContent += "<img src = 'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcROnYPD5QO8ZJvPQt8ClnJNPXduCeX89dSOxA&usqp=CAU' alt = 'School Logo' style = 'max-width: 100px; margin-bottom: 10px;' >";
            htmlContent += "</div>";
            htmlContent += "<p style = 'margin: 0;' >Jobin School Management</p>";
            htmlContent += "<p style = 'margin: 0;' > 123 School Street, Sample Street</p>";
            htmlContent += "<p style = 'margin: 0;' > Phone: 123 - 456 - 7890 </p>";
            htmlContent += "<p style = 'margin: 0;' > Tamilnadu </p>";
            htmlContent += "<div style = 'text-align: center; margin-bottom: 20px;'>";
            htmlContent += "<h1> Fees Structure </h1>";
            htmlContent += "</div>";
            htmlContent += "<h3> StudentDetails:</h3>";
            htmlContent += "<p> Name:</p>";
            htmlContent += "<p> STD:</p>";
            htmlContent += "<table style = 'width: 100%; border-collapse: collapse;'>";
            htmlContent += "<thead>";
            htmlContent += "<tr>";
            htmlContent += "<th style = 'padding: 8px; text-align: left; border-bottom: 1px                           solid #ddd;' > Fee Description </th>";
            htmlContent += "<th style = 'padding: 8px; text-align: left; border-bottom: 1px solid #ddd;' > Amount(INR) </th>";
            htmlContent += "</tr><hr/>";
            htmlContent += "</thead>";
            htmlContent += "<tbody>";
            htmlContent += "<tr>";
            htmlContent += "<td style = 'padding: 8px; text-align: left; border-bottom: 1px solid #ddd;' > Tuition Fee </td>";
            htmlContent += "<td style = 'padding: 8px; text-align: left; border-bottom: 1px solid #ddd;' >RS500/- </td>";
            htmlContent += "</tr>";
            htmlContent += "<tr>";
            htmlContent += "<td style = 'padding: 8px; text-align: left; border-bottom: 1px solid #ddd;' > Transportation Fee </td>";
            htmlContent += "<td style = 'padding: 8px; text-align: left; border-bottom: 1px solid #ddd;' >RS100/- </td>";
            htmlContent += "</tr>";
            htmlContent += "<tr>";
            htmlContent += "<td style = 'padding: 8px; text-align: left; border-bottom: 1px solid #ddd;' > Books and Supplies</td>";
            htmlContent += "<td style = 'padding: 8px; text-align: left; border-bottom: 1px solid #ddd;' >RS50/- </td>";
            htmlContent += "</tr>";
            htmlContent += "</tbody>";
            htmlContent += "<tfoot>";
            htmlContent += "<tr>";
            htmlContent += "<td style = 'padding: 8px; text-align: right; font-weight: bold;'> Total:</td>";
            htmlContent += "<td style = 'padding: 8px; text-align: left; border-top: 1px solid #ddd;' >$650 </td>";
            htmlContent += "</tr>";
            htmlContent += "</tfoot>";
            htmlContent += "</table>";
            htmlContent += "</div>";
            PdfGenerator.AddPdfPages(data, htmlContent, PageSize.A4);
            byte[]? response = null;
            using (MemoryStream ms = new MemoryStream())
            {
                data.Save(ms);
                response = ms.ToArray();
            }
            string fileName = "FeesStructure.pdf";
            return File(response, "application/pdf", fileName);
        }
    }
}
