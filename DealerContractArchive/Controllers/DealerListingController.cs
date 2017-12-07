using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using DealContractArchiver.ViewModels;
using DealerContractArchive.EntityModels;
using DealContractArchiver.ViewModels.Helper;
using Microsoft.AspNetCore.Http;
using System.IO;
using DealerContractArchive.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Caching.Memory;
using System.Diagnostics;

namespace DealerContractArchive.Views
{
    [Route("API/ContractListing/[action]")]
    [Authorize]
    public class DealerListingController : Controller
    {
        public double CachedContentLifeMinutes
        {
            get
            {
                var value = _config.GetSection("CacheOptions").GetValue<double>("CachedContentLifeMinutes");
                if (value < 1)
                    return 5; //default if not set
                return value;
            }
        }
        public string ScanFolder
        {
            get
            {
                return _config.GetSection("FileStorage").GetValue<string>("ScanFolder");
            }
        }

        private DealerContractContext _context;
        private IConfiguration _config;
        private IMemoryCache _cache;
        public DealerListingController(DealerContractContext context, IConfiguration config, IMemoryCache memoryCache)
        {
            _context = context;
            _config = config;
            _cache = memoryCache;
        }

        [HttpGet]
        public DealerListingViewModel GetContractViewerModel([FromQuery] int page = 1, [FromQuery] bool filter = false, [FromQuery] int type = 0, [FromQuery] string contains = "", [FromQuery] string orderBy = "", [FromQuery] bool asc = true)
        {
            using (_context)
            {
                DealerListingViewModel model;
                var cacheKey = MakeCacheKey(page, filter, type, contains, orderBy, asc);
                //BUG: no total page in cached
                if (_cache.TryGetValue<DealerListingViewModel>(cacheKey, out var dealerModels))
                {
                    //cached
                    model = dealerModels;
                    Debug.Print("From cache...");
                }
                else
                {
                    //no cached, create new model
                    Debug.Print("Create new model...");
                    model = new DealerListingViewModel();
                    //set model state
                    var filterColumnName = FilterColumnTranslater(type);
                    model.FilterType = filterColumnName.ToString();
                    model.IsFilterApplied = filter;
                    model.FilterString = contains;
                    model.DocumentNames = GetDocumentNames(_context);
                    int totalRows;

                    if (!string.IsNullOrEmpty(filterColumnName) && filter && !string.IsNullOrEmpty(contains))
                    {
                        model.DealerModels = GetFilteredDealers(_context, filterColumnName, contains, page, orderBy, asc, out totalRows);
                    }
                    else
                    {
                        model.DealerModels = GetDealers(_context, out totalRows, page, orderBy, asc);
                    }
                    model.UpdatePagination(totalRows);
                    //cache the model
                    _cache.Set(cacheKey, model, GetDefaultCacheOptions());
                }
                return model;
            }
        }
        private MemoryCacheEntryOptions GetDefaultCacheOptions()
        {
            return new MemoryCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2)
            };
        }
        private string MakeCacheKey(int page, bool filter, int type, string contains, string orderBy, bool asc)
        {
            return $"{page}_{filter}_{type}_{contains}_{orderBy}_{asc}";
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

        //setting....
        private const string AcceptedUploadType = "application/pdf";
        private readonly double MinFileLength = 0;

        [HttpPost]
        [Authorize(Roles = "Admin, User")]
        public IActionResult UploadScan([FromQuery]int dealerId)
        {
            var files = Request.Form.Files;
            if (files.Count < 1 || files.Count > 1) return BadRequest("Invalid file count.");
            var file = files.First();
            if (file.Length < (int)MinFileLength) return BadRequest("File is too small");
            if (string.Compare(file.ContentType, AcceptedUploadType, true) != 0) return BadRequest("Invalid file type.");
            using (_context)
            {
                var dealer = _context.Dealer.FirstOrDefault(c => c.DealerId == dealerId);
                if (dealer == null) return BadRequest($"Dealer id: {dealerId} not found");
                //add new Scan entry
                var fileName = EnviromentHelper.ScanFilePathMaker(file.FileName, dealerId);
                var scan = _context.Scan.Add(new Scan()
                {
                    DealerId = dealer.DealerId,
                    UploadDate = DateTime.Now,
                    Username = User.Identity.Name,
                    FilePath = fileName
                });
                //save file
                if (!SaveScan(file, fileName))
                {
                    return BadRequest("File already exists!, contact admin");
                }
                _context.SaveChanges();
            }
            return Ok();
        }
        private bool SaveScan(IFormFile file, string fileName)
        {
            //do save
            //if file exists?
            var path = Path.Combine(EnviromentHelper.RootPath, ScanFolder);
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            var fullPath = Path.Combine(path, fileName);
            if ((new FileInfo(fileName)).Exists) return false;
            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                file.CopyTo(stream);
            }
            return true;
        }

        private const string DocumentListKey = "DocKey_";
        private List<string> GetDocumentNames(DealerContractContext context)
        {
            if (_cache.TryGetValue<List<string>>(DocumentListKey, out var docNames))
            {
                return docNames;
            }
            else
            {
                var list = new List<string>();
                foreach (var doc in context.Document)
                {
                    if (!doc.Effective) continue;
                    list.Add(doc.Name);
                }
                _cache.Set(DocumentListKey, list, GetDefaultCacheOptions());
                return list;
            }
        }

        private static string FilterColumnTranslater(int value)
        {
            switch (value)
            {
                case 1:
                    return "GroupName";
                case 2:
                    return "DealerName";
                case 3:
                    return "TaxId";
                case 4:
                    return "Username";
                default:
                    return string.Empty;
            }
        }

        //wow!
        //https://stackoverflow.com/questions/2728340/how-can-i-do-an-orderby-with-a-dynamic-string-parameter
        private static Func<Dealer, object> OrderTranslater(string orderBy)
        {
            switch (orderBy)
            {
                case "GroupName":
                    return i => i.GroupName;
                case "DealerName":
                    return i => i.DealerName;
                case "ResigteredName":
                    return i => i.RegisteredName;
                case "TaxId":
                    return i => i.TaxId;
                case "Effective":
                    return i => i.Effective;
                case "Phone":
                    return i => i.Phone;
                case "Username":
                    return i => i.Username;
                case "ContractNo":
                    return i => i.ContractNo;
                default:
                    return i => i.GroupName;
            }
        }

        private List<Dealer> GetFilteredDealers(DealerContractContext context, string columnName, string filterString, int page, string orderBy, bool asc, out int totalRows)
        {
            int excludedRows = (page - 1) * DealerListingViewModel.ItemPerPage;
            var query = context.Dealer.Include(d => d.Pos).Include(d => d.Scan)
                .Where(ExpressionHelper.GetContainsExpression<Dealer>(columnName, filterString));
            totalRows = query.Count();
            var ordered = asc ? query.OrderBy(OrderTranslater(orderBy)) : query.OrderByDescending(OrderTranslater(orderBy));
            //set total rows
            return ordered.Skip(excludedRows).Take(DealerListingViewModel.ItemPerPage).ToList();
        }

        private List<Dealer> GetDealers(DealerContractContext context, out int totalRows, int pageNum, string orderBy, bool asc)
        {
            int getPage = pageNum < 1 ? 1 : pageNum;
            int excludedRows = (getPage - 1) * DealerListingViewModel.ItemPerPage;
            var query = context.Dealer.Include(d => d.Pos).Include(d => d.Scan);
            totalRows = query.Count();
            var ordered = asc ? query.OrderBy(OrderTranslater(orderBy)) : query.OrderByDescending(OrderTranslater(orderBy));
            return ordered.Skip(excludedRows).Take(DealerListingViewModel.ItemPerPage).ToList();
        }

        //private List<Pos> GetPos(int dealerId)
        //{
        //    using (var context = new DealerContractContext())
        //    {
        //        //populate pos list
        //        var dealer = context.Dealer.Include(d => d.Pos).FirstOrDefault(d => d.DealerId == dealerId);
        //        //var dealer = context.Dealer.FirstOrDefault(d => d.DealerId == dealerId)
        //        return dealer.Pos.ToList();
        //    }
        //}
    }
}
