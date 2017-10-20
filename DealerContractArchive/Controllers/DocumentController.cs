//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Mvc;
//using DealerContractArchive.EntityModels;
//using DealerContractArchive.Helper;
//using System.IO;
//using Microsoft.AspNetCore.Mvc.Formatters;
//using System.Net.Http.Headers;
//using System.Text;
//using GemBox.Document;
//using Microsoft.AspNetCore.Authorization;
////using GemBox.Document;

//// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

//namespace DealerContractArchive.Controllers
//{
//    [Authorize]
//    public class DocumentController : Controller
//    {
//        private const string gemboxdocumentkey = "dtjx-2lsb-qjv3-r3xp";

//        [httpget]
//        public iactionresult getdocument([fromquery] int contractid, [fromquery] string docname)
//        {
//            using (var context = new dealercontractcontext())
//            {
//                var contract = context.contracts.firstordefault(c => c.contractid == contractid);
//                if (contract == null) return badrequest();

//                var document = context.documents.firstordefault(c => string.compare(docname, c.name, true) == 0);
//                if (document == null) return badrequest();
//                var docfullpath = enviromenthelper.getdocumentfullpath(document.filename);

//                if (!(new fileinfo(docfullpath)).exists) return nocontent();
//                string doccontent = system.io.file.readalltext(docfullpath);
//                doccontent = fillcontractdocument(doccontent, contract);
//                //return content(doccontent);
//                return content(doccontent, "text/html", encoding.utf8);
//            }
//            //return view();
//        }
//        [httpget]
//        public iactionresult getdocumentpdf() //works!
//        {
//            componentinfo.setlicense(gemboxdocumentkey);
//            var doc = new documentmodel();
//            doc.sections.add(new section(doc, new paragraph(doc, "hello!")));

//            var responsestream = new memorystream();
//            doc.save(responsestream, saveoptions.pdfdefault);

//            //to return file use file()
//            var response = file(responsestream, "application/pdf");
//            return response;
//        }

//        //name: @name
//        //address: @address
//        //tax id: @taxid

//        private string fillcontractdocument(string documentcontent, contracts contract)
//        {
//            var content = documentcontent.replace("@name", contract.name);
//            content = content.replace("@address", contract.address);
//            content = content.replace("@taxid", contract.taxid);
//            return content;
//        }
//    }
//}
