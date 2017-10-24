using DealerContractArchive.EntityModels;
using System.Collections.Generic;

namespace DealContractArchiver.ViewModels
{
    public class DealerListingViewModel
    {
        public List<Dealer> DealerModels { get; set; }
        public List<string> DocumentNames { get; set; }
        public bool IsFilterApplied { get; set; }
        public string FilterType { get; set; }
        public string FilterString { get; set; }
        public static int ItemPerPage { get; set; } = 15;

        //update these every time add record
        public int TotalPages { get; private set; }
        public int TotalRows { get; private set; }

        public DealerListingViewModel()
        {
            DealerModels = new List<Dealer>();
            IsFilterApplied = false;
            FilterString = string.Empty;
        }
        public void UpdatePagination(int totalRows)
        {
            TotalRows = totalRows;
            TotalPages = (TotalRows + ItemPerPage - 1) / ItemPerPage;
            if (TotalPages < 1)
                TotalPages = 1;
        }
    }
}