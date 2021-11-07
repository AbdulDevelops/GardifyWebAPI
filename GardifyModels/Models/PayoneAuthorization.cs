using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GardifyModels.Models
{
    public class PayoneAuthorization : PayoneRequest
    {
        public int RequestAID { get; set; }
        public PayoneClearingType ClearingType { get; set; }
        public int RequestReference { get; set; }
        public int RequestAmount { get; set; }
        public PayoneCurrency Currency { get; set; }
        public string RequestParam { get; set; }
        public string RequestNarrativeText { get; set; }
        public PayoneClearingSubtype RequestClearingSubtype { get; set; }
        public string RequestSettleAccount { get; set; }
        public string RequestWorkorderId { get; set; }
        public string RequestInvoiceId { get; set; }
        public PayoneInvoiceDeliveryMode RequestDeliveryMode { get; set; }
        public string RequestInvoiceAppendix { get; set; }

        #region Automatically generated elements depending on NotMapped elements
        public int RequestInvoiceDeliveryDate
        {
            get
            {
                if (RequestInvoiceDeliveryDateObj != null)
                {
                    return int.Parse(RequestInvoiceDeliveryDateObj.ToString("yyyyMMdd"));
                }
                else
                {
                    return 00010101;
                }
            }
        }

        public int RequestInvoiceDeliveryEndDate
        {
            get
            {
                if (RequestInvoiceDeliveryEndDateObj != null)
                {
                    return int.Parse(RequestInvoiceDeliveryEndDateObj.ToString("yyyyMMdd"));
                }
                else
                {
                    return 00010101;
                }
            }
        }

        public int RequestDocumentDate
        {
            get
            {
                if (RequestDocumentDateObj != null)
                {
                    return int.Parse(RequestDocumentDateObj.ToString("yyyyMMdd"));
                }
                else
                {
                    return 00010101;
                }
            }
        }

        public int RequestBookingDate
        {
            get
            {
                if (RequestBookingDateObj != null)
                {
                    return int.Parse(RequestBookingDateObj.ToString("yyyyMMdd"));
                }
                else
                {
                    return 00010101;
                }
            }
        }

        public int RequestDueTime
        {
            get
            {
                return (int)(RequestDueTimeObj.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            }
        }
        #endregion

        #region NotMapped Elements, used for formatized Database and API elements
        [NotMapped]
        public List<PayoneAuthorizationItem> Items { get; set; }
        [NotMapped]
        public DateTime RequestInvoiceDeliveryDateObj { get; set; }
        [NotMapped]
        public DateTime RequestInvoiceDeliveryEndDateObj { get; set; }
        [NotMapped]
        public DateTime RequestDocumentDateObj { get; set; }
        [NotMapped]
        public DateTime RequestBookingDateObj { get; set; }
        [NotMapped]
        public DateTime RequestDueTimeObj { get; set; }
        #endregion
    }

    public class PayoneAuthorizationPersonalData
    {
        public string CustomerId { get; set; }
        public int UserId { get; set; }
        public string Salutation { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Company { get; set; }
        public string Street { get; set; }
        public string AddressAddition { get; set; }
        public string Zip { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string Email { get; set; }
        public string TelephoneNumber { get; set; }
        public PayoneLanguage Language { get; set; }
        public string VATId { get; set; }
        public PayoneGender Gender { get; set; }
        public string Ip { get; set; }
        #region Automatically generated elements with correct format for API
        public int Birthday
        {
            get
            {
                if (BirthdayObj != null)
                {
                    return int.Parse(BirthdayObj.ToString("yyyyMMdd"));
                }
                else
                {
                    return 00010101;
                }
            }
        }
        #endregion

        #region NotMapped elements, used for formatizing into API format
        public DateTime BirthdayObj { get; set; }
        #endregion
    }

    public class PayoneCashOnDelivery
    {
        public PayoneShippingProvider ShippingProvider { get; set; }
    }

    public class PayoneCreditCard
    {
        public int CardPan { get; set; }
        public int MyProperty { get; set; }
    }

    public class PayoneEWallet
    {
        public PayoneWalletProvider WalletProvider { get; set; }
        public string SuccessUrl { get; set; }
        public string ErrorUrl { get; set; }
        public string BackUrl { get; set; }
    }

    public class PayoneOnlineTransfer
    {
        public PayoneOnlineBankTransferType OnlineTransferType { get; set; }
        public PayoneCountry BankCountry { get; set; }
        public string BankAccount { get; set; }
        public string BankCode { get; set; }
        public string BankGroupType { get; set; }
        public string IBAN { get; set; }
        public string BIC { get; set; }
        public string SuccessUrl { get; set; }
        public string ErrorUrl { get; set; }
        public string BackUrl { get; set; }
    }

    public class PayoneDebitPayment
    {
        public string IBAN { get; set; }
        public string BIC { get; set; }
        public string BankCountry { get; set; }
        public string BankAccount { get; set; }
        public string BankCode { get; set; }
        public string BankAccountHolder { get; set; }
        public string MandateIdentification { get; set; }
    }

    public class PayoneDeliveryData
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Company { get; set; }
        public string Street { get; set; }
        public string Zip { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public PayoneCountry Country { get; set; }
    }

    public class PayoneAuthorizationItem
    {
        public PayoneAuthorizationItem ItemType { get; set; }
        public string ProductNumber { get; set; }
        public int UnitPrice { get; set; }
        public int Quantity { get; set; }
        public string Description { get; set; }
        public int VATRate { get; set; }
        #region Automatically generated API elements
        public int DeliveryDate
        {
            get
            {
                if (DeliveryDateObj != null)
                {
                    return int.Parse(DeliveryDateObj.ToString("yyyyMMdd"));
                }
                else
                {
                    return 00010101;
                }
            }
        }
        public int DeliveryPeriodEndDate
        {
            get
            {
                if (DeliveryPeriodEndDateObj != null)
                {
                    return int.Parse(DeliveryPeriodEndDateObj.ToString("yyyyMMdd"));
                }
                else
                {
                    return 00010101;
                }
            }
        }
        #endregion
        #region NotMapped elements, used for formatizing Database and API formats
        public DateTime DeliveryDateObj { get; set; }
        public DateTime DeliveryPeriodEndDateObj { get; set; }
        #endregion
    }
}