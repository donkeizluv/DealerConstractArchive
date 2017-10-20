using System;
using System.Collections.Generic;

namespace DealerContractArchive.EntityModels
{
    public partial class Pos
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

        public Dealer Dealer { get; set; }
        public Users UsernameNavigation { get; set; }
    }
}
