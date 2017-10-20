using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using DealContractArchiver.ViewModels;
using DealerContractArchive.EntityModels;
using DealContractArchiver.ViewModels.Helper;
using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using System.IO;
using DealerContractArchive.Helper;
using Microsoft.AspNetCore.Authorization;
using DealerContractArchive.ViewModels;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DealerContractArchive.Views
{
    //2 choices:
    //route defined in each controler or
    //global route in startup
    [Route("API/ContractListing/[action]")]
    [Authorize]
    public class DealerListingController : Controller
    {
        [HttpGet]
        public DealerListingViewModel GetContractViewerModel([FromQuery] int page = 1, [FromQuery] bool filter = false, [FromQuery] int type = 0, [FromQuery] string contains = "")
        {
            var model = new DealerListingViewModel();
            //set model state
            var filterColumnName = FilterColumn.None;
            if (Enum.IsDefined(typeof(FilterColumn), type))
            {
                filterColumnName = (FilterColumn)type;
            }
            model.FilterType = filterColumnName.ToString();
            model.IsFilterApplied = filter;
            model.FilterString = contains;
            model.DocumentNames = GetDocumentNames();
            int totalRows;
            if (filter && !string.IsNullOrEmpty(contains))
            {
                //model.ContractModels = GetFilteredConstractsQuery(filterColumnName, contains, page, out totalRows);
                model.DealerModels = GetDealers(out totalRows, page);
            }
            else
            {
                model.DealerModels = GetDealers(out totalRows, page);
            }
            model.UpdatePagination(totalRows);
            return model;
        }

        //        [HttpPost]
        //        [Authorize(Roles = "Admin, User")]
        //        public IActionResult AddNewContract([FromBody] ContractViewModel contract)
        //        {
        //            //NYI: validate data
        //            if (contract == null) return BadRequest();
        //            try
        //            {
        //                using (var context = new DealerContractContext())
        //                {
        //                    var newContract = new Contracts()
        //                    {
        //                        Name = contract.Name,
        //                        Address = contract.Address,
        //                        Phone = contract.Phone,
        //                        TaxId = contract.TaxId,
        //                        Commission = contract.Commission,
        //                        Effective = contract.Effective,
        //                        UserId = contract.UserId,
        //                        ScannedContractUrl = null
        //                    };
        //                    context.Contracts.Add(newContract);
        //                    int result = context.SaveChanges();
        //                    if (result > 0)
        //                    {
        //                        return Ok();
        //                    }

        //                    return BadRequest();
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //#if DEBUG
        //                throw ex;
        //#endif
        //                return BadRequest();
        //            }
        //        }

        ////setting....
        //private const string AcceptedUploadType = "application/pdf";
        //private readonly double MinFileLength = 0;

        //[HttpPost]
        //[Authorize(Roles = "Admin, User")]
        //public IActionResult UploadScan([FromQuery]int contractId)
        //{
        //    var files = Request.Form.Files;
        //    if (files.Count < 1 || files.Count > 1) return BadRequest("Invalid file count.");
        //    var file = files.First();
        //    if (file.Length < (int)MinFileLength) return BadRequest("File is too small");
        //    if (string.Compare(file.ContentType, AcceptedUploadType, true) != 0) return BadRequest("Invalid file type.");
        //    using (var context = new DealerContractContext())
        //    {
        //        var contract = context.Contracts.FirstOrDefault(c => c.ContractId == contractId);
        //        if (contract == null) return BadRequest("Contract not found");
        //        if (!string.IsNullOrEmpty(contract.ScannedContractUrl)) return BadRequest("Contract has scan uploaded already");

        //        if (!SaveScan(file, contract.ContractId))
        //        {
        //            throw new InvalidOperationException();
        //        }
        //        contract.ScannedContractUrl = EnviromentHelper.ScanFilePathMaker(file.FileName, contract.ContractId);
        //        context.SaveChanges();
        //    }
        //    return Ok();
        //}

        private List<string> GetDocumentNames()
        {
            using (var context = new DealerContractContext())
            {
                var list = new List<string>();
                foreach (var doc in context.Document)
                {
                    if (!doc.Effective) continue;
                    list.Add(doc.Name);
                }
                return list;
            }
        }

        private bool SaveScan(IFormFile file, int index)
        {
            //do save
            //if file exists?
            var path = Path.Combine(EnviromentHelper.RootPath, EnviromentHelper.ScanFolder);
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            var fileName = Path.Combine(path, EnviromentHelper.ScanFilePathMaker(file.FileName, index));
            if ((new FileInfo(fileName)).Exists) return false;
            using (var stream = new FileStream(fileName, FileMode.Create))
            {
                file.CopyTo(stream);
            }
            return true;
        }

//        private List<DealerViewModel> GetFilteredConstractsQuery(FilterColumn filterCol, string filterString, int page, out int totalRows)
//        {
//            var list = new List<DealerViewModel>();
//            using (var context = new DealerContractContext())
//            {
//                int excludedRows = (page - 1) * DealerListingViewModel.ItemPerPage;
//                string processedFilterString = filterString;
//                if (filterCol == FilterColumn.Added) //convert username to int first
//                {
//                    var user = context.Users.Where(u => u.Username == filterString);
//                    if (user == null || user.Count() < 1)
//                        processedFilterString = "-1";
//                    else
//                        processedFilterString = user.First().UserId.ToString();
//                }
//                var query = context.Contracts
//                    .OrderBy(c => c.ContractId)
//                    .Where(ExpressionHelper.GetContainsExpression<Contracts>(filterCol.ToString(), processedFilterString));
//                totalRows = query.Count();
//                query = query.Skip(excludedRows).Take(DealerListingViewModel.ItemPerPage);
//                if (query.Any())
//                {
//                    list = (from c in query
//                            select new DealerViewModel()
//                            {
//                                ContractId = c.ContractId,
//                                Name = c.Name,
//                                Address = c.Address,
//                                Commission = c.Commission,
//                                Effective = c.Effective,
//                                Phone = c.Phone,
//                                ScannedContractUrl = c.ScannedContractUrl,
//                                TaxId = c.TaxId,
//                                UserId = c.UserId,
//                                Username = c.User.Username
//                            }).ToList();

//                }
//#if DEBUG
//                Debug.Print($"filterd list count: {list.Count}");
//#endif

//                return list;
//            }
//        }
        
        private List<DealerViewModel> GetDealers(out int totalRows, int pageNum = 1)
        {
            int getPage = pageNum < 1 ? 1 : pageNum;
            using (var context = new DealerContractContext())
            {
                totalRows = context.Dealer.Count();
                int excludedRows = (getPage - 1) * DealerListingViewModel.ItemPerPage;
                var query = context.Dealer
                    .OrderBy(c => c.GroupName)
                    .Skip(excludedRows)
                    .Take(DealerListingViewModel.ItemPerPage)
                    .AsQueryable();
                //copy to model
                //for some fucking reason, direct entity to json is a paint in the ass :/
                var list = (from c in query
                            select new DealerViewModel()
                            {
                                DealerId = c.DealerId,
                                GroupName = c.GroupName,
                                BussinessId = c.BussinessId,
                                DealerName = c.DealerName,
                                RegisteredName = c.RegisteredName,
                                Delegate = c.Delegate,
                                SubDelegate = c.SubDelegate,
                                StartEffective = c.StartEffective,
                                EndEffective = c.EndEffective,
                                Fax = c.Fax,
                                Gender = c.Gender,
                                Hqaddress = c.Hqaddress,
                                Owner = c.Owner,
                                Phone = c.Phone,
                                Position = c.Position,
                                Representative = c.Representative,
                                TaxId = c.TaxId,
                                Username = c.Username,
                                Pos = GetPos(c.DealerId)
                            }).ToList();
                if (list.Any())
                {
                    return list;
                }
                return null;
            }
        }
        private List<PosViewModel> GetPos(int dealerId)
        {
            using (var context = new DealerContractContext())
            {
                //populate pos list
                var dealer = context.Dealer.Include(d => d.Pos).FirstOrDefault(d => d.DealerId == dealerId);
                //var dealer = context.Dealer.FirstOrDefault(d => d.DealerId == dealerId);
                var posList = from p in dealer.Pos
                              select new PosViewModel()
                              {
                                  Address = p.Address,
                                  Batch = p.Batch,
                                  Bl = p.Bl,
                                  Brand = p.Brand,
                                  DealerId = p.DealerId,
                                  PosCode = p.PosCode,
                                  PosId = p.PosId,
                                  PosName = p.PosName,
                                  Province = p.Province,
                                  Region = p.Region,
                                  Status = p.Status,
                                  Username = p.Username
                              };
                return posList.ToList();
            }
        }
    }
}
