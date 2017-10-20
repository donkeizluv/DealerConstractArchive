using System;
using System.Collections.Generic;

namespace DealerContractArchive.EntityModels
{
    public partial class Scan
    {
        public int ScanId { get; set; }
        public int DealerId { get; set; }
        public string FilePath { get; set; }
        public DateTime UploadDate { get; set; }
        public string Username { get; set; }

        public Dealer Dealer { get; set; }
        public Users UsernameNavigation { get; set; }
    }
}
