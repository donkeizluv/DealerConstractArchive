using System.Linq;
using Microsoft.AspNetCore.Mvc;
using DealerContractArchive.EntityModels;
using DealerContractArchive.Helper;
using System.IO;
using System.Text;
using GemBox.Document;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;

namespace DealerContractArchive.Controllers
{
    [Authorize]
    public class DocumentController : Controller
    {
        private const string GemboxDocumentKey = "DTJX-2LSB-QJV3-R3XP";

        private DealerContractContext _context;
        private IConfiguration _config;
        public DocumentController(DealerContractContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }
        [HttpGet]
        public IActionResult GetDocument([FromQuery] int dealerId, [FromQuery] string docName)
        {
            using (_context)
            {
                var dealer = _context.Dealer.FirstOrDefault(c => c.DealerId == dealerId);
                if (dealer == null) return BadRequest();

                var document = _context.Document.FirstOrDefault(c => string.Compare(docName, c.Name, true) == 0);
                if (document == null) return BadRequest();
                var docFullPath = EnviromentHelper.GetDocumentFullPath(document.Filename);

                if (!(new FileInfo(docFullPath)).Exists) return NoContent();
                string docContent = System.IO.File.ReadAllText(docFullPath);
                docContent = FillContractDocument(docContent, dealer);
                //return Content(docContent);
                return Content(docContent, "text/html", Encoding.UTF8);
            }
            //return View();
        }
        [HttpGet]
        public IActionResult GetDocumentPdf() //works!
        {
            ComponentInfo.SetLicense(GemboxDocumentKey);
            var doc = new DocumentModel();
            doc.Sections.Add(new Section(doc, new Paragraph(doc, "Hello!")));

            var responseStream = new MemoryStream();
            doc.Save(responseStream, SaveOptions.PdfDefault);

            //to return file use File()
            var response = File(responseStream, "application/pdf");
            return response;
        }

        //Name: @name
        //Address: @address
        //Tax Id: @taxid

        private string FillContractDocument(string documentContent, Dealer dealer)
        {
            var content = documentContent.Replace("@name", dealer.DealerName);
            content = content.Replace("@address", dealer.Hqaddress);
            content = content.Replace("@taxid", dealer.TaxId);
            return content;
        }
    }
}
