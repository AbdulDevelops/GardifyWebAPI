using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GardifyModels.Models
{
    public class PayoneEnums
    {

    }

    public enum PayoneWalletProvider
    {
        [Display(Name = "PayPal Express")]
        PPE,
        [Display(Name = "paysafecard")]
        PSC
    }

    public enum PayoneCardType
    {
        [Display(Name = "Visa")]
        V,
        [Display(Name = "MasterCard")]
        M,
        [Display(Name = "American Express")]
        A,
        [Display(Name = "Diners")]
        D,
        [Display(Name = "JCB")]
        J,
        [Display(Name = "Maestro International")]
        O,
        [Display(Name = "Discover")]
        C,
        [Display(Name = "Carte Bleue")]
        B,
        [Display(Name = "China Union Pay / CUP")]
        P
    }

    public enum PayoneShippingProvider
    {
        [Display(Name = "DHL, Germany")]
        DHL,
        [Display(Name = "Bartolini, Italy")]
        BRT
    }

    public enum PayoneLanguage
    {
        [Display(Name = "German")]
        de
    }

    public enum PayoneOnlineBankTransferType
    {
        [Display(Name = "Sofortbanking (DE, AT, CH, NL")]
        PNT,
        [Display(Name = "giropay (DE)")]
        GPY,
        [Display(Name = "eps - online transfer (AT)")]
        EPS,
        [Display(Name = "PostFinance E-Finance (CH)")]
        PFF,
        [Display(Name = "PostFinance Card (CH)")]
        PFC,
        [Display(Name = "iDEAL (NL)")]
        IDL,
        [Display(Name = "Przelewvy24 (PL)")]
        P24,
        [Display(Name = "Bancontact")]
        BCT
    }

    public enum PayoneGender
    {
        [Display(Name = "Male")]
        m,
        [Display(Name = "Female")]
        f
    }

    public enum PayoneCountry
    {
        [Display(Name = "Germany")]
        DE,
        [Display(Name = "Switzerland")]
        CH,
        [Display(Name = "Österreich")]
        AT
    }

    public enum PayoneClearingType
    {
        [Display(Name = "Debit Payment")]
        elv,
        [Display(Name = "Credit card")]
        cc,
        [Display(Name = "Invoice")]
        rec,
        [Display(Name = "Cash on delivery")]
        cod,
        [Display(Name = "Online Bank Transfer")]
        sb,
        [Display(Name = "e-wallet")]
        wlt,
        [Display(Name = "Financig")]
        fnc
    }

    public enum PayoneCurrency
    {
        [Display(Name = "Euro")]
        EUR,
        [Display(Name = "Schweizer Franken")]
        CHF
    }

    public enum PayoneClearingSubtype
    {
        [Display(Name = "PAYONE secure purchase on invoice")]
        POV
    }

    public enum PayoneFinancingType
    {
        [Display(Name = "BillSAFE invoice")]
        BSV,
        [Display(Name = "Klarna invoice")]
        KLV,
        [Display(Name = "Klarna Installment")]
        KLS,
        [Display(Name = "Payolution-Invoicing")]
        PYV,
        [Display(Name = "Payolution-Installment")]
        PYS,
        [Display(Name = "Payolution-Debit")]
        PYD,
        [Display(Name = "Ratepay Debit")]
        RPD,
        [Display(Name = "Ratepay Prepayment")]
        RPP,
        [Display(Name = "Ratepay Installment")]
        RPS,
        [Display(Name = "Ratepay Invoicing")]
        RPV,

    }

    public enum PayoneInvoiceDeliveryMode
    {
        [Display(Name = "Post")]
        M,
        [Display(Name = "PDF (via email)")]
        P,
        [Display(Name = "No Delivery")]
        N
    }

    public enum PayoneItemType
    {
        [Display(Name = "Goods")]
        goods,
        [Display(Name = "Shipping charges")]
        shipment,
        [Display(Name = "Handling fee")]
        handling,
        [Display(Name = "Voucher/discount")]
        voucher
    }
}