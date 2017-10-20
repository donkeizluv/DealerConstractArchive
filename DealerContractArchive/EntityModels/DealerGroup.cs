using System;
using System.Collections.Generic;

namespace DealerContractArchive.EntityModels
{
    public partial class DealerGroup
    {
        public DealerGroup()
        {
            Dealer = new HashSet<Dealer>();
        }

        public string GroupName { get; set; }

        public ICollection<Dealer> Dealer { get; set; }
    }
}
