using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DealerContractArchive.ViewModels
{
    public class PosViewModel
    {
        public int PosId { get; set; }
        public int DealerId { get; set; }
        public string PosName { get; set; }
        public string PosCode { get; set; }
        public string Address { get; set; }
        public int Batch { get; set; }
        public string Region { get; set; }
        public string Province { get; set; }
        public string Bl { get; set; }
        public string Brand { get; set; }
        public string Status { get; set; }
        public string Username { get; set; }
    }
}
