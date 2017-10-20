using System;
using System.Collections.Generic;

namespace DealerContractArchive.EntityModels
{
    public partial class Users
    {
        public Users()
        {
            Dealer = new HashSet<Dealer>();
            Pos = new HashSet<Pos>();
            Scan = new HashSet<Scan>();
        }

        public string Username { get; set; }
        public string Type { get; set; }
        public bool Active { get; set; }
        public string Description { get; set; }

        public AccountType TypeNavigation { get; set; }
        public ICollection<Dealer> Dealer { get; set; }
        public ICollection<Pos> Pos { get; set; }
        public ICollection<Scan> Scan { get; set; }
    }
}
