using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FileUploadLib.Data.OldContext
{
    public partial class UsersDetail
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public Guid? ApplicationId { get; set; }
        public int? KundennummerWaWi { get; set; }
        public decimal? Limit { get; set; }
        public string AccountEmail { get; set; }
        public Guid? NewsletterConfirmationCode { get; set; }
        public bool? NewsletterConfirmed { get; set; }
        public DateTime? NewsletterNewsletterConfirmedDate { get; set; }
        public string OtherPrivateEmail { get; set; }
        public string OtherBusinessEmail { get; set; }
        [Display(Name ="Anrede")]
        public string Salutation { get; set; }
        public string Titel { get; set; }
        public int? Sex { get; set; }
        [Display(Name = "Vorname")]
        public string FirstName { get; set; }
        [Display(Name = "Nachname")]
        public string LastName { get; set; }
        [Display(Name = "Firma")]
        public string Company { get; set; }
        [Display(Name = "Firmenwebseite")]
        public string CompanyWebSite { get; set; }
        [Display(Name = "Abteilung")]
        public string Department { get; set; }
        [Display(Name = "Position")]
        public string Position { get; set; }
        public bool? InternalExternal { get; set; }
        [Display(Name = "Geburtstag")]
        public DateTime? Birthday { get; set; }
        [Display(Name = "Land")]
        public string Country { get; set; }
        [Display(Name = "Nr.")]
        public string StreetNumber { get; set; }
        [Display(Name = "Straße")]
        public string Street { get; set; }
        [Display(Name = "Postleitzahl")]
        public string Zip { get; set; }
        [Display(Name = "Stadt")]
        public string City { get; set; }
        [Display(Name = "Telefonnummer Firma")]
        public string PhoneBusiness { get; set; }
        [Display(Name = "Faxnummer Firma")]
        public string FaxBusiness { get; set; }
        [Display(Name = "Telefonnummer Privat")]
        public string PhonePrivate { get; set; }
        [Display(Name = "Telefonnummer Mobil")]
        public string PhoneMobile { get; set; }
        public string Picture { get; set; }
        public string Description { get; set; }
        public string Field1String { get; set; }
        public string Field2String { get; set; }
        public string Field3String { get; set; }
        public string Field4String { get; set; }
        public string Field5String { get; set; }
        public string Field1 { get; set; }
        public string Field2 { get; set; }
        public string Field3 { get; set; }
        public string Field4 { get; set; }
        public string Field5 { get; set; }
        public bool? Field1Bit { get; set; }
        public bool? Field2Bit { get; set; }
        public bool? Field3Bit { get; set; }
        public int? SortId { get; set; }
        public string InsertedBy { get; set; }
        public DateTime? InsertedDate { get; set; }
        public string EditedBy { get; set; }
        public DateTime? EditedDate { get; set; }
        public bool? Deleted { get; set; }
        public string DeletedReason { get; set; }
        public string PreferredLanguage { get; set; }
        public bool? Editable { get; set; }
    }
}
