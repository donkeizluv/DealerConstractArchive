using Microsoft.AspNetCore.Mvc;
using DealerContractArchive.Models;
using System.Diagnostics;
using System.IO;
using DealerContractArchive.Helper;
using DealerContractArchive.EntityModels;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DealerContractArchive.Controllers
{
    [Authorize]
    public class ScanController : Controller
    {
        private DealerContractContext _context;
        private IConfiguration _config;
        public ScanController(DealerContractContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        // GET: /<controller>/
        //https://docs.microsoft.com/en-us/aspnet/core/mvc/models/formatting
        [HttpGet]
        public IActionResult GetScan([FromQuery] int? scanId)
        {
            if (scanId == null) return BadRequest();
            if ((scanId ?? -1) < 0) return BadRequest(); //auto increment index cant be negative
            int id = scanId ?? -1;
            int index;
            string dbFilename = string.Empty;
            using (_context)
            {
                var scan = _context.Scan.FirstOrDefault(c => c.ScanId == id);
                if (scan == null) return BadRequest();
                index = scan.ScanId;
                dbFilename = scan.FilePath;
                if (string.IsNullOrEmpty(dbFilename)) return BadRequest("This contract has not uploaded scan yet.");
            }
            string fullPath = EnviromentHelper.GetScanfileFullPath(dbFilename);
            if (!(new FileInfo(fullPath)).Exists) return NoContent();

            //https://stackoverflow.com/questions/42460198/return-file-in-asp-net-core-web-api
            var stream = new FileStreamResult(new FileStream(fullPath, FileMode.Open, FileAccess.Read), "application/pdf");
            //to return file use File()
            var response = File(stream.FileStream, "application/pdf");
            return response;
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
