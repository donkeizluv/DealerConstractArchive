using System.Collections.Generic;

namespace DealerContractArchive.EntityModels
{
    public partial class Dealer
    {
        //public List<Pos> GetPosList
        //{
        //    get
        //    {
                
        //    }
        //}
        public double Effective
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
        public string SignDateShortDate
        {
            get
            {
                if (SignDate == null) return string.Empty;
                return SignDate?.ToShortDateString();
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