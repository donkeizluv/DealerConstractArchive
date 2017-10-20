using System;
using System.Collections.Generic;

namespace DealerContractArchive.EntityModels
{
    public partial class Dealer
    {
        public Dealer()
        {
            Pos = new HashSet<Pos>();
            Scan = new HashSet<Scan>();
        }

        public int DealerId { get; set; }
        public string GroupName { get; set; }
        public string BussinessId { get; set; }
        public string TaxId { get; set; }
        public string DealerName { get; set; }
        public string RegisteredName { get; set; }
        public string Representative { get; set; }
        public bool Gender { get; set; }
        public string Position { get; set; }
        public string Owner { get; set; }
        public string Hqaddress { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public DateTime StartEffective { get; set; }
        public DateTime EndEffective { get; set; }
        public string Delegate { get; set; }
        public string SubDelegate { get; set; }
        public string Username { get; set; }

        public DealerGroup GroupNameNavigation { get; set; }
        public Users UsernameNavigation { get; set; }
        public ICollection<Pos> Pos { get; set; }
        public ICollection<Scan> Scan { get; set; }
    }
}
