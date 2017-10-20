using DealerContractArchive.EntityModels;
using DealerContractArchive.ViewModels;
using System;
using System.Collections.Generic;

namespace DealContractArchiver.ViewModels
{
    public class DealerViewModel
    {

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


        public List<PosViewModel> Pos { get; set; }

        public double EffectiveDayRemain
        {
            get
            {
                return (EndEffective - StartEffective).TotalDays;
            }
        }
        public string StartEffectiveShortDate
        {
            get
            {
                return StartEffective.ToShortDateString();
            }
        }
        public string EndEffectiveShortDate
        {
            get
            {
                return EndEffective.ToShortDateString();
            }
        }

    }
}