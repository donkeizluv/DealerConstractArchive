using System.Collections.Generic;

namespace DealContractArchiver.ViewModels
{
    public enum FilterColumn
    {
        None = 0,
        Index = 1,
        Name = 2,
        Phone = 3,
        Address = 4,
        TaxId = 5,
        Added = 6
    }
    public class DealerListingViewModel
    {
        //NYI: sort
        public List<DealerViewModel> DealerModels { get; set; }
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
            DealerModels = new List<DealerViewModel>();
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