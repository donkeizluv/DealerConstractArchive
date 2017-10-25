using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DealerContractArchive.EntityModels
{
    public partial class Scan
    {
        public string UploadDateShortDate
        {
            get
            {
                return UploadDate.ToShortDateString();
            }
        }
    }
}
