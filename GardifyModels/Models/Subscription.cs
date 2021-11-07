using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GardifyModels.Models
{
    public class Subscription: _BaseEntity
    {
        public Guid UserId { get; set; }
        public bool IsGardifyPlusMonthly => MonthlyEndDate > DateTime.Now;
        //public bool IsGardifyPlusAnnually { get; set; }
        //public bool IsGardifyPlusAnnually => AnnualEndDate > AnnualStartDate;
        public bool IsGardifyPlusTest => MonthlyEndDate > DateTime.Now;
        public bool IsTestReceive => TestStartDate != null && TestEndDate < DateTime.Now;
        public DateTime? MonthlyStartDate { get; set; }
        public DateTime? TestStartDate { get; set; }
        public DateTime? TestEndDate { get; set; }

        public DateTime? MonthlyEndDate { get; set; }
        public DateTime? AnnualStartDate { get; set; }
        public DateTime? AnnualEndDate { get; set; }

    }

    public class SubscriptionViewModel
    {
        public Guid UserId { get; set; }
        public bool IsGardifyPlusMonthly { get; set; }
        public bool IsTestReceive { get; set; }

        public bool IsGardifyPlusTest { get; set; }



        //public bool IsGardifyPlusMonthly => MonthlyEndDate > DateTime.Now;
        ////public bool IsGardifyPlusAnnually { get; set; }
        //public bool IsGardifyPlusAnnually => AnnualEndDate > AnnualStartDate;
        //public bool IsGardifyPlusTest => MonthlyEndDate > DateTime.Now;
        //public bool IsTestReceive => TestStartDate != null && TestEndDate < DateTime.Now;
        public DateTime? MonthlyStartDate { get; set; }
        public DateTime? TestStartDate { get; set; }
        public DateTime? TestEndDate { get; set; }

        public DateTime? MonthlyEndDate { get; set; }
        public DateTime? AnnualStartDate { get; set; }
        public DateTime? AnnualEndDate { get; set; }

    }

    public class TransactionRequestModel
    {
        public string Data { get; set; }
    }

    public class IOSTransactionInputModel
    {
        public int status { get; set; }
        public List<LatestReceipt> latest_receipt_info { get; set; }
    }

    public class LatestReceipt
    {
        public string product_id { get; set; }
        public string purchase_date { get; set; }
        public string expires_date { get; set; }

    }
}